using Microsoft.SharePoint;
using SPEEDEAU.ADMIN.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPEEDEAU.ADMIN.Model
{
    public class InfoIH1600 : IComparable
    {
        public StatusIH1600 Status { get; set; }
        public string Indice { get; set; }
        public string Revision { get; set; }
        public ValidationStatus ValidationStatus { get; set; }

        public int CompareTo(object obj)
        {            
            if (obj == null) return 1;

            InfoIH1600 otherInfo = obj as InfoIH1600;
            if (otherInfo == null) throw new ArgumentNullException("obj is not InfoIH1600 object");

            int result = this.Indice.CompareTo(otherInfo.Indice);
            if (result != 0) return result;

            return this.Revision.CompareTo(otherInfo.Revision);            
        }
    }


    public class InfoIH1600Builder
    {

        public InfoIH1600 Build(SPListItem item)
        {
            InfoIH1600 info = new InfoIH1600();
            
            string statusFieldName = Localization.GetResource(ResourceFieldsKeys.STATUS_DOC, ResourceFiles.FIELDS);
            string statusValue = item.EnsureValue<string>(statusFieldName);            
            info.Status = EnumHelper.GetValue<StatusIH1600>(statusValue);

            string validationFieldName = Localization.GetResource(ResourceFieldsKeys.VERIFICATION, ResourceFiles.FIELDS);
            string validationValue = item.EnsureValue<string>(validationFieldName);
            info.ValidationStatus = EnumHelper.GetValue<ValidationStatus>(validationValue);
            
            string indiceFieldName = Localization.GetResource(ResourceFieldsKeys.INDICE, ResourceFiles.FIELDS);
            info.Indice = item.EnsureValue<string>(indiceFieldName);

            string revisionFieldName = Localization.GetResource(ResourceFieldsKeys.REVISION, ResourceFiles.FIELDS);
            info.Revision = item.EnsureValue<string>(revisionFieldName);
            
            return info;
        }
    }

}
