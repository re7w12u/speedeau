using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPEEDEAU.ADMIN.Util;

namespace SPEEDEAU.ADMIN.Model
{
    public class Observation
    {
        public int ObsID { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public int DocID { get; set; }
        public string DocTitle { get; set; }
        public string Indice { get; set; }
        public string Revision { get; set; }
        public bool Validated { get; set; }
    }

    public class ObservationBuilder
    {
        private const string SEP = ";#";

        public Observation Build(SPListItem item)
        {
            Observation result = new Observation();

            result.ObsID = item.ID;
            result.Title = item.EnsureValue<string>(SPBuiltInFieldId.Title);
            result.Body = item.EnsureValue<string>(Localization.GetResource(ResourceFieldsKeys.OBSERVATION, ResourceFiles.FIELDS));
            result.Indice = item.EnsureValue<string>(Localization.GetResource(ResourceFieldsKeys.INDICE, ResourceFiles.FIELDS));
            result.Revision = item.EnsureValue<string>(Localization.GetResource(ResourceFieldsKeys.REVISION, ResourceFiles.FIELDS));
            result.Validated = item.EnsureValue<bool>("");

            string document = item.EnsureValue<string>(Localization.GetResource(ResourceFieldsKeys.DOCUMENT, ResourceFiles.FIELDS));
            if (!String.IsNullOrWhiteSpace(document) && document.Contains(SEP))
            {
                string[] data = document.Split(new string[] { SEP }, StringSplitOptions.RemoveEmptyEntries);
                result.DocTitle = data[1];
                result.DocID = Convert.ToInt32(data[0]);
            }

            return result;
        }

    }
}
