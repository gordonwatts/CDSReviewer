
using System;
using System.Collections.Generic;
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

                // If this is a deleted document...
                if (ExtractDataFieldFirst(col.record[0], MARC21Spec.MARC21Identifiers.DFCDSStatus, "c") == "DELETED")
                    throw new CDSException("Document has been deleted from the CDS archive");

                // Parse out the fields we need for everything.
                string title = ExtractDataFieldFirst(col.record[0], MARC21Spec.MARC21Identifiers.DFTitleStatement, "a");
                string abs = ExtractDataFieldFirst(col.record[0], MARC21Spec.MARC21Identifiers.DFAbstractStatement, "a");
                var authors = ExtractDataFieldList(col.record[0], MARC21Spec.MARC21Identifiers.DFAuthorList, "a").ToArray();
                return new DocMetaData() { Title = title, Abstract = abs, Authors = authors };
            }
        }

        /// <summary>
        /// Thrown when we get an exception during parsing.
        /// </summary>
        public class CDSException : Exception
        {
            public CDSException() { }
            public CDSException(string message) : base(message) { }
            public CDSException(string message, System.Exception inner) : base(message, inner) { }
        }

        /// <summary>
        /// Extract a string from a data field.
        /// </summary>
        /// <param name="col">The collection for a single document</param>
        /// <param name="fieldID">The data field number</param>
        /// <returns>null if it can't be found, other wise the string contents of the title</returns>
        private static string ExtractDataFieldFirst(MARC21Spec.recordType document, string fieldID, string subfieldCode)
        {
            var df = document.datafield.Where(r => r.tag == fieldID).FirstOrDefault();
            if (df == null)
                return null;

            var data = df.subfield.Where(s => s.code == subfieldCode).FirstOrDefault();
            if (data == null)
                return null;
            return data.Value;
        }

        /// <summary>
        /// Returns all the data fields that satisfy the requirements.
        /// </summary>
        /// <param name="document">The collection for the single document</param>
        /// <param name="fieldID">The field number</param>
        /// <param name="subfieldCode">The author</param>
        /// <returns></returns>
        private static IEnumerable<string> ExtractDataFieldList(MARC21Spec.recordType document, string fieldID, string subfieldCode)
        {
            var df = document.datafield
                .Where(r => r.tag == fieldID)
                .SelectMany(r => r.subfield.Where(s => s.code == subfieldCode));
            foreach (var item in df)
            {
                yield return item.Value;
            }
        }
    }
}
