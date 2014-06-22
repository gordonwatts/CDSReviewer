
using CDSReviewerCore.Data;
using CERNSSOPCL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace CDSReviewerCore.Raw
{
    /// <summary>
    /// Access to the raw CDS website.
    /// </summary>
    internal class RawCDSAccess
    {
        /// <summary>
        /// Get the document meta-data.
        /// </summary>
        /// <param name="docID">Document number in CDS</param>
        /// <returns></returns>
        public static IObservable<IDocumentMetadata> GetDocumentMetadata(int docID)
        {
            // Create the web request to get this item.

            var reqUri = new Uri(string.Format("https://cds.cern.ch/record/{0}/export/xm?ln=en", docID));

            var s = Observable
                    .FromAsync(tnk => CERNWebAccess.GetWebResponse(reqUri))
                    .SelectMany(resp => Observable.FromAsync(tkn => resp.Content.ReadAsStringAsync()))
                    .Select(ParseToMD);
            return s;
        }

        /// <summary>
        /// Return a list of all files that CDS thinks are assoicated with a particular record.
        /// </summary>
        /// <param name="docID"></param>
        /// <returns></returns>
        public static IObservable<IEnumerable<PaperFile>> GetDocumentFiles(int docID)
        {
            // Call the paper file parser here
            var reqUri = new Uri(string.Format("https://cds.cern.ch/record/{0}/files/", docID));

            var s = Observable
                    .FromAsync(tnk => CERNWebAccess.GetWebResponse(reqUri))
                    .SelectMany(resp => Observable.FromAsync(tkn => resp.Content.ReadAsStringAsync()))
                    .Select(HTMLFileVersionListParser.ParseToFileList);
            return s;
        }

        /// <summary>
        /// Given a string, run the MARC21 parser on it to convert it to an actual
        /// item.
        /// </summary>
        /// <param name="marc21XML"></param>
        /// <returns></returns>
        private static IDocumentMetadata ParseToMD(string marc21XML)
        {
            return MARC21Parser.ParseForMetadata(marc21XML);
        }

        /// <summary>
        /// Read the contents of a file to a stream for local use.
        /// </summary>
        /// <param name="id">The id of the paper</param>
        /// <param name="fileName">The file we are after</param>
        /// <param name="version">The file verison we want to pick up</param>
        /// <param name="writeto">The stream to output the data on</param>
        /// <returns></returns>
        public static IObservable<Unit> SaveDocumentLocally(string id, string fileName, int version, Stream writeto)
        {
            // Build the URI from the file information we have.
            var fURI = new Uri(string.Format("http://cds.cern.ch/record/{0}/files/{1}?version={2}", id, fileName, version));

            // Read it!
            return ReadFromCDSToStream(writeto, fURI);
        }

        /// <summary>
        /// Run the download. Return an observable that when done the read is done.
        /// </summary>
        /// <param name="writeto"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        private static IObservable<Unit> ReadFromCDSToStream(Stream writeto, Uri uri)
        {
            var wr = WebRequest.CreateHttp(uri);

            var s = Observable
                .FromAsync(tnk => Task.Factory.FromAsync<WebResponse>(wr.BeginGetResponse, wr.EndGetResponse, null))
                .Select(resp => resp.GetResponseStream())
                .SelectMany(resp => Observable.Using(
                    () => new CompositeDisposable(writeto, resp),
                    strm => Observable.FromAsync(tkn => resp.CopyToAsync(writeto))))
                .Select(r => Unit.Default);
            return s;
        }
    }
}
