using Microsoft.SharePoint;
using SPEEDEAU.ADMIN;
using SPEEDEAU.ADMIN.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPEEDEAU.Alerte
{
    public abstract class IH1600Receiver : SPItemEventReceiver
    {
        /// <summary>
        /// update codification system according to codification to reflect any potential changes
        /// </summary>
        /// <param name="properties"></param>
        internal void UpdateCodification(SPItemEventProperties properties)
        {
            SPListItem item = properties.ListItem;
            string codifFieldName = Localization.GetResource(ResourceFieldsKeys.CODIFICATION, ResourceFiles.FIELDS);
            string codif = item.EnsureValue<string>(codifFieldName);
            string codifSystemfieldName = CodificationHelper.GetCodeSystemeFieldName;
            
            // skip if codif is null
            if (codif == null) return;
            // if codification is already set to new value, skip to avoid looping event receiver
            if (item[codifSystemfieldName] == CodificationHelper.CleanUpCodification(codif)) return;

            if (!String.IsNullOrEmpty(codif))
            {
                item[codifSystemfieldName] = CodificationHelper.CleanUpCodification(codif);
            }
            else
            {
                item[codifSystemfieldName] = String.Empty;
            }
            item.SystemUpdate(false);
        }

    }
}
