
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace CDSReviewerCore.Raw
{
    /// <summary>
    /// Code to help with parsing a MARC21 document
    /// </summary>
    internal class MARC21Parser
    {
        /// <summary>
        /// Given an xml document as a string, parse out the info for
        /// our metadata header.
        /// </summary>
        /// <param name="marc21XML"></param>
        /// <returns></returns>
        public static IDocumentMetadata ParseForMetadata(string marc21XML)
        {
            // Load up the document

            var s = new XmlSerializer(typeof(MARC21Spec.collectionType));
            using (var reader = new StringReader(marc21XML))
            {
                var col = s.Deserialize(new StringReader(marc21XML)) as MARC21Spec.collectionType;

                // If this contains more records than we are expecting...
                if (col == null || col.record == null || col.record.Length != 1)
                    throw new InvalidDataException("The MARC21 XML from CDS has no records in it.");

                // Parse out the fields we need for everything.
                string title = ExtractDataField(col.record[0], MARC21Spec.MARC21Identifiers.DFTitleStatement, "a");

                return new DocMetaData() { Title = title };
            }
        }

        /// <summary>
        /// Extract a string from a data field.
        /// </summary>
        /// <param name="col">The collection for a single document</param>
        /// <param name="fieldID">The data field number</param>
        /// <returns>null if it can't be found, other wise the string contents of the title</returns>
        private static string ExtractDataField(MARC21Spec.recordType document, string fieldID, string subfieldCode)
        {
            var df = document.datafield.Where(r => r.tag == fieldID).FirstOrDefault();
            if (df == null)
                return null;

            var data = df.subfield.Where(s => s.code == subfieldCode).FirstOrDefault();
            if (data == null)
                return null;
            return data.Value;
        }
    }
}
