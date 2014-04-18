using CDSReviewerCore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CDSReviewerCore.Raw
{
    /// <summary>
    /// Parse HTML that comes back from the web server for a list of files.
    /// </summary>
    /// <remarks>This code is quite fragile. It relies on certain HTML tags, and as soon
    /// as CDS decideds to change things and their formatting, it is bound to break
    /// all fo this. Really, we need to find out if there is a better way to get this
    /// sort of information from them.</remarks>
    public static class HTMLFileVersionListParser
    {
        /// <summary>
        /// Parse the html into the structs that will contain file and file version info
        /// for this returned paper.
        /// </summary>
        /// <param name="htmltext">the html that should be parsed</param>
        /// <returns>All files associated with this particle paper</returns>
        internal static PaperFile[] ParseToFileList(string htmltext)
        {
            // Load up the html and parse it.
            var h = new HtmlAgilityPack.HtmlDocument();
            h.LoadHtml(htmltext);

            // Get the dates.
            var dates = from l in h.DocumentNode.Descendants("em")
                        let dt = ParseAsDate(l.InnerText)
                        where dt != null && dt.HasValue
                        select dt.Value;

            // Look for all tables, and find the one that lists the files.
            var alllinks = from l in h.DocumentNode.Descendants("a")
                        let href = l.GetAttributeValue("href", "")
                        where href.Contains("?version=")
                        select Tuple.Create(l.InnerText, href);

            var links = from p in alllinks.Zip(dates, (pr, dt) => Tuple.Create(pr.Item1, pr.Item2, dt))
                        group Tuple.Create(p.Item2, p.Item3) by p.Item1;

            var results = links
                .Select(k => ConvertToPaperFile(k.Key, k));
            return results.ToArray();
        }

        /// <summary>
        /// Parse for a date. If we fail the parsing, we return a null.
        /// </summary>
        /// <param name="dateString"></param>
        /// <returns>Null if the string isn't a date. Otherwise, a datetime struct.</returns>
        private static DateTime? ParseAsDate(string dateString)
        {
            DateTime result;
            if (DateTime.TryParse(dateString, out result))
            {
                return result;
            }
            return null;
        }

        /// <summary>
        /// For a particular paper, where the list of items is already found, create
        /// the file and version list structures.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="urlList"></param>
        /// <returns></returns>
        private static PaperFile ConvertToPaperFile(string fileName, IEnumerable<Tuple<string, DateTime>> urlList)
        {
            return new PaperFile()
            {
                FileName = fileName,
                Versions = urlList.Select(ConverToPaperVersion).Where(v => v!=null).ToArray()
            };
        }

        /// <summary>
        /// Find the version number in the URL
        /// </summary>
        private static Regex _versionFinder = new Regex(@"\?version=(?<n>[0-9]+)");

        /// <summary>
        /// Create a paper version to reference the one version we are looking at.
        /// </summary>
        /// <param name="versInfo"></param>
        /// <returns></returns>
        private static PaperFileVersion ConverToPaperVersion(Tuple<string, DateTime> versInfo)
        {
            var m = _versionFinder.Match(versInfo.Item1);
            if (!m.Success)
                return null;

            return new PaperFileVersion() {
                 VersionNumber = int.Parse(m.Groups["n"].Value),
                 VersionDate = versInfo.Item2
            };
        }
    }
}
