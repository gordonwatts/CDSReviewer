
using CERNSSOPCL;
using System;
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
                    .StartAsync(tnk => CERNWebAccess.GetWebResponse(reqUri))
                    .SelectMany(resp => Observable.StartAsync(tkn => resp.Content.ReadAsStringAsync()))
                    .SelectMany(xml => Observable.Start(() => ParseToMD(xml)));
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
        /// Gets the main document via a direct HTTP download.
        /// </summary>
        /// <param name="doc">The document metatdata, the main document will be pulled from there.</param>
        /// <param name="writeto">A writable stream. It will be closed and disposed of when this returns.</param>
        /// <returns>A unit sequence with a single item is returned when the request has completed.</returns>
        internal static IObservable<Unit> GetMainDocumentHttp(IDocumentMetadata doc, Stream writeto)
        {
            var wr = WebRequest.CreateHttp(doc.MainDocument);

            var s = Observable
                .StartAsync(tnk => Task.Factory.FromAsync<WebResponse>(wr.BeginGetResponse, wr.EndGetResponse, null))
                .Select(resp => resp.GetResponseStream())
                .SelectMany(resp => Observable.Using(
                    () => new CompositeDisposable(writeto, resp),
                    strm => Observable.StartAsync(tkn => resp.CopyToAsync(writeto))))
                .Select(r => Unit.Default);
            return s;
        }
    }
}
