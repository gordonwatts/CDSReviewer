
using System;
using System.IO;
using System.Net;
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
        public IObservable<IDocumentMetadata> GetDocumentMetadata(int docID)
        {
            // Create the web request to get this item.

            var reqUri = new Uri(string.Format("https://cds.cern.ch/record/{0}/export/xm?ln=en", docID));
            var wr = WebRequest.CreateHttp(reqUri);

            var s = Observable
                    .StartAsync(tnk => Task.Factory.FromAsync<WebResponse>(wr.BeginGetResponse, wr.EndGetResponse, null))
                    .Select(resp => resp.GetResponseStream())
                    .SelectMany(resp => Observable.Using(() => new StreamReader(resp), strm => Observable.StartAsync(tkn => strm.ReadToEndAsync())))
                    .Select(ParseToMD);
            return s;
        }

        /// <summary>
        /// Given a string, run the MARC21 parser on it to convert it to an actual
        /// item.
        /// </summary>
        /// <param name="marc21XML"></param>
        /// <returns></returns>
        private IDocumentMetadata ParseToMD(string marc21XML)
        {
            return MARC21Parser.ParseForMetadata(marc21XML);
        }
    }
}
