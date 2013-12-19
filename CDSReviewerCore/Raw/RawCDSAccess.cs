
using System;
using System.IO;
using System.Net;
using System.Reactive.Linq;
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
                    .FromAsyncPattern<WebResponse>(
                        wr.BeginGetResponse,
                        wr.EndGetResponse)
                    .Invoke()
                    .Catch(Observable.Return<WebResponse>(null))
                    .Select(ExtractString)
                    .Select(ParseToMD);

            return s;
        }
#if false
                    .SelectMany(r => Observable.Using(() => r, resp => Observable.Return(resp.)))

        var o = Observable.Return(HttpWebRequest.Create("http://www.google.com"))
                  .SelectMany(r => Observable.FromAsyncPattern<WebResponse>(
                      r.BeginGetResponse,
                      r.EndGetResponse)())
                  .SelectMany(r =>
                  {
                      return Observable.Using( () => r, (resp) => Observable.Return(resp.ContentLength));
                  });
#endif
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

        /// <summary>
        /// Convert a web response into a string.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private string ExtractString(WebResponse arg)
        {
            var u = arg.GetResponseStream();
            var rdr = new StreamReader(u);
            return rdr.ReadToEndAsync().Result;
        }
    }
}
