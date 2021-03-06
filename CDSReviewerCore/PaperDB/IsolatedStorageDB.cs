﻿using CDSReviewerCore.Data;
using PCLStorage;
using Polenter.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CDSReviewerCore.PaperDB
{
    /// <summary>
    /// The device local database, implemented in isolated storage.
    /// </summary>
    public class IsolatedStorageDB : IInternalPaperDB
    {
        /// <summary>
        /// Add or update a data item in our local storage.
        /// </summary>
        /// <param name="stub"></param>
        /// <param name="full"></param>
        /// <returns></returns>
        public async Task Add(Data.PaperStub stub, Data.PaperFullInfo full)
        {
            // Full info is just saved under the object ID.
            await SaveFullInfo(stub.ID, full);

            await UpdateStubList(stub);

            // Create the file where we will be storing any papers we manage to download.
            var rootFolder = await _paperDBFolder.Value;
            var paperFolderName = PaperFolderName(stub);
            await rootFolder.CreateFolderAsync(paperFolderName, CreationCollisionOption.OpenIfExists);
        }

        /// <summary>
        /// Add or replace some stub info in the stub list.
        /// </summary>
        /// <param name="stub"></param>
        /// <returns></returns>
        private async Task UpdateStubList(PaperStub stub)
        {
            var list = await GetStubInformation();
            var finalList = list
                .Where(l => l.ID != stub.ID)
                .Concat(stub.AsSingleList());
            await SaveStubInformation(finalList);
        }

        /// <summary>
        /// Save the full info for us...
        /// </summary>
        /// <param name="paperID"></param>
        /// <param name="full"></param>
        /// <returns></returns>
        private async Task SaveFullInfo(string paperID, Data.PaperFullInfo full)
        {
            await SaveObject(paperID, full);
        }

        /// <summary>
        /// Replace the file info.
        /// </summary>
        /// <param name="paperID"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public async Task Merge(string paperID, PaperFile[] files)
        {
            var obj = await LoadObject(paperID) as PaperFullInfo;
            if (obj == null)
                throw new ArgumentException("paperID");
            obj.Files = files;
            await SaveObject(paperID, obj);
        }

        /// <summary>
        /// Cache the serialize so we don't have to keep creating it and destroying it.
        /// </summary>
        private Lazy<SharpSerializer> _binarySerlizer = new Lazy<SharpSerializer>(() => new SharpSerializer(true));

        /// <summary>
        /// Write a linearizable object out.
        /// </summary>
        /// <param name="fileID"></param>
        /// <param name="full"></param>
        /// <returns></returns>
        private async Task SaveObject(string fileID, object full)
        {
            using (var wtr = await CreateFile(await _paperDBFolder.Value, fileID))
            {
                _binarySerlizer.Value.Serialize(full, wtr);
            }
        }

        /// <summary>
        /// Open a stream for writing in a folder with a given filename.
        /// </summary>
        /// <param name="f">The folder in which to place the file.</param>
        /// <param name="fileName">The filename to write</param>
        /// <returns>A stream where the file can be written. Must be Disposed.</returns>
        private async Task<Stream> CreateFile(IFolder f, string fileName)
        {
            var folder = await _paperDBFolder.Value;
            var outfile = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            return await outfile.OpenAsync(FileAccess.ReadAndWrite);
        }

        /// <summary>
        /// Load the object from some settings file.
        /// </summary>
        /// <param name="fileID">The file we should load an object from</param>
        /// <returns>The object we've read back, or a null object if the file isn't there</returns>
        private async Task<object> LoadObject(string fileID)
        {
            var folder = await _paperDBFolder.Value;
            try
            {
                var infile = await folder.GetFileAsync(fileID);
                if (infile == null)
                    return null;

                using (var rdr = await infile.OpenAsync(FileAccess.Read))
                {
                    return _binarySerlizer.Value.Deserialize(rdr);
                }
            }
            catch (PCLStorage.Exceptions.FileNotFoundException)
            {
                // If the file isn't there, we return null.
                return null;
            }
        }

        /// <summary>
        /// Cache the settings root.
        /// </summary>
        private static Lazy<Task<IFolder>> _paperDBFolder = new Lazy<Task<IFolder>>(BuildPaperFolderDBTask);

        /// <summary>
        /// Do not call. To init and re-init the lazy task.
        /// </summary>
        /// <returns></returns>
        private static Task<IFolder> BuildPaperFolderDBTask()
        {
            var rootFolder = FileSystem.Current.LocalStorage;
            return rootFolder.CreateFolderAsync("paperDB", CreationCollisionOption.OpenIfExists);
        }

        /// <summary>
        /// Delete everything in the database. Used for testing.
        /// WARNING: behavior is undefined if an instance of IsolatedStorgeDB is still around
        /// when this is called. We may be caching things in order to speed up performance,
        /// and the clear can't know about all instances around!
        /// </summary>
        /// <returns></returns>
        internal static async Task Clear()
        {
            var rootFolder = await _paperDBFolder.Value;
            await rootFolder.DeleteAsync();
            _paperDBFolder = new Lazy<Task<IFolder>>(BuildPaperFolderDBTask);
        }

        /// <summary>
        /// Remove an item from the internal list.
        /// </summary>
        /// <param name="paperID"></param>
        /// <returns></returns>
        public async Task Remove(string paperID)
        {
            var list = await GetStubInformation();
            var newlist = list
                .Where(l => l.ID != paperID);
            await SaveStubInformation(newlist);
        }

        /// <summary>
        /// Return the full information for all the list. This is expensive!
        /// </summary>
        /// <returns>the complete list of items in one go.</returns>
        public async Task<IEnumerable<Tuple<Data.PaperStub, Data.PaperFullInfo>>> GetFullInformation()
        {
            // It would be cool if we could do this with await's, but I can't see how to
            // do this in a LINQ expression. The Task always propagates to the outside.

            var list = await GetStubInformation();
            List<Tuple<PaperStub, PaperFullInfo>> l = new List<Tuple<PaperStub, PaperFullInfo>>();
            foreach (var item in list)
            {
                l.Add(Tuple.Create(item, await GetFullInfoForID(item.ID)));
            }
            return l;
        }

        /// <summary>
        /// A single empty stub list. Just helpful, and, I hope, efficient.
        /// </summary>
        private PaperStub[] gEmptyStubList = new PaperStub[0];

        /// <summary>
        /// Return the complete list of stubs. This should be optimized to run pretty
        /// fast, all things given.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<PaperStub>> GetStubInformation()
        {
            var r = (await LoadObject("paperlist")) as PaperStub[];
            if (r == null)
                return gEmptyStubList;
            return r;
        }

        /// <summary>
        /// Save the list of paper stubs back into the system.
        /// </summary>
        /// <param name="finalList"></param>
        /// <returns></returns>
        private Task SaveStubInformation(IEnumerable<PaperStub> finalList)
        {
            return SaveObject("paperlist", finalList.ToArray());
        }

        /// <summary>
        /// Given a paper ID, see if we can't find it.
        /// </summary>
        /// <param name="paperID"></param>
        /// <returns></returns>
        public Task<Data.PaperFullInfo> GetFullInfoForID(string paperID)
        {
            return ReadFullInfo(paperID);
        }

        /// <summary>
        /// Return the info for a particular paper ID
        /// </summary>
        /// <param name="paperID"></param>
        /// <returns></returns>
        public async Task<Tuple<PaperStub, PaperFullInfo>> GetPaperInfoForID(string paperID)
        {
            return Tuple.Create((await GetStubInformation()).Where(ps => ps.ID == paperID).First(), await GetFullInfoForID(paperID));
        }

        /// <summary>
        /// Return the full info for a paper.
        /// </summary>
        /// <param name="paperID"></param>
        /// <returns></returns>
        private async Task<PaperFullInfo> ReadFullInfo(string paperID)
        {
            return (await LoadObject(paperID)) as PaperFullInfo;
        }

        /// <summary>
        /// Return true if the file has been downloaded
        /// </summary>
        /// <param name="id"></param>
        /// <param name="file"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public async Task<bool> IsFileDownloaded(PaperStub id, PaperFile file, PaperFileVersion version)
        {
            var info = await GetFolderAndNameForPaperFile(id, file, version);
            return (await info.Item1.CheckExistsAsync(info.Item2)).HasFlag(ExistenceCheckResult.FileExists);
        }


        /// <summary>
        /// Create a stream that will write to a file we have internally opened.
        /// </summary>
        /// <param name="id">The paper ID that we need to cache</param>
        /// <param name="file"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public async Task<Stream> CreatePaperFile(PaperStub id, PaperFile file, PaperFileVersion version)
        {
            // Build up the file and the location where we can store it.

            var info = await GetFolderAndNameForPaperFile(id, file, version);
            var f = await info.Item1.CreateFileAsync(info.Item2, CreationCollisionOption.ReplaceExisting);
            return await f.OpenAsync(FileAccess.ReadAndWrite);
        }

        /// <summary>
        /// Return the info needed to location a particular paper file in our system. Don't check for its existance, but do throw
        /// if the directory isn't there!
        /// </summary>
        /// <param name="id"></param>
        /// <param name="file"></param>
        /// <param name="vers"></param>
        /// <returns></returns>
        private async Task<Tuple<IFolder, string>> GetFolderAndNameForPaperFile(PaperStub id, PaperFile file, PaperFileVersion vers)
        {
            var rootFolder = await _paperDBFolder.Value;
            var paperFolderName = PaperFolderName(id);
            if (!(await rootFolder.CheckExistsAsync(paperFolderName)).HasFlag(ExistenceCheckResult.FolderExists))
            {
                throw new FileNotFoundException(string.Format("Internal error: paper storage for {0} not created yet.", id.ID));
            }
            var folder = await rootFolder.GetFolderAsync(paperFolderName);

            var filename = string.Format("{0}-{1}", vers.VersionNumber.ToString("D2"), file.FileName);

            return Tuple.Create(folder, filename);
        }

        /// <summary>
        /// Given the stub for a paper, return the folder where all files for it will be stored.
        /// </summary>
        /// <param name="id">Paper id that we will be accessing.</param>
        /// <returns>String with the file name</returns>
        private static string PaperFolderName(PaperStub id)
        {
            var paperFolderName = string.Format("papers_{0}", id.ID);
            return paperFolderName;
        }
    }
}
