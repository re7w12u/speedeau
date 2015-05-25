using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPEEDEAU.ADMIN.Model
{
    [Serializable]
    public class SuiviEntity
    {
        public const string DELIMITER = ";#";

        public SuiviEntity()
        {

        }

        public SuiviEntity(string input)
        {
            if (String.IsNullOrWhiteSpace(input)) throw new ArgumentOutOfRangeException("SuiviEntity constructor : input parameter is null or whitespace...");
            string[] data = input.Split(new string[] { DELIMITER }, StringSplitOptions.RemoveEmptyEntries);
            if (data.Length == 5)
            {
                SuiviItemID = Convert.ToInt32(data[0]);
                SuiviListName = data[1];
                DocID = Convert.ToInt32(data[2]);
                DocLibName = data[3];
                DocLibGUID = Guid.Parse(data[4]);
            }
            else
            {
                throw new ArgumentOutOfRangeException("SuiviEntity constructor : input does not meet the required length (4)");
            }
        }
        /// <summary>
        /// ID for item in liste de suivi
        /// </summary>
        public int SuiviItemID { get; set; }

        /// <summary>
        /// list name for liste de suivi
        /// </summary>
        public string SuiviListName { get; set; }

        /// <summary>
        /// ID for doc matching item in liste de suivi
        /// </summary>
        public int DocID { get; set; }

        /// <summary>
        /// list name for library hosting file
        /// </summary>
        public string DocLibName { get; set; }

        /// <summary>
        /// ID for library hosting file
        /// </summary>
        public Guid DocLibGUID { get; set; }

        public override string ToString()
        {
            return String.Join(DELIMITER, SuiviItemID, SuiviListName, DocID, DocLibName, DocLibGUID);
        }

        public string ToJson()
        {
            return String.Format("{{\"suiviItemId\":\"{0}\",\"suiviListName\":\"{1}\",\"docID\":\"{2}\",\"docLibName\":\"{3}\",\"docLibGuid\":\"{4}\"}}", SuiviItemID, SuiviListName, DocID, DocLibName, DocLibGUID);
        }

        
    }
}
