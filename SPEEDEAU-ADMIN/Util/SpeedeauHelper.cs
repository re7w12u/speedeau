using Microsoft.Practices.SharePoint.Common.ServiceLocation;
using Microsoft.SharePoint;
using SPEEDEAU.ADMIN.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPEEDEAU.ADMIN.Util
{
    public static class CodificationHelper
    {

        public static string GetCodeSystemeFieldName
        {
            get
            {
                return Localization.GetResource(ResourceFieldsKeys.CODIFICATION_SYSTEM, ResourceFiles.FIELDS);
            }
        }

        public static string CleanUpCodification(string code)
        {
            return System.Text.RegularExpressions.Regex.Replace(code, "[^0-9a-zA-Z]", String.Empty);
        }

        public static SPListItem GetItemForCodification(SPList list, string codificationSystem)
        {
            SPQuery q = new SPQuery();
            q.Query = String.Format(@"<Where>
                            <Eq>
                                <FieldRef Name='CodificationSystem' />      
                                <Value Type='Text'>{0}</Value>
                            </Eq>  
                        </Where>", codificationSystem);
//            q.ViewFields = @"<FieldRef Name='Title' />
//                            <FieldRef Name='Codification' />
//                            <FieldRef Name='CodificationSystem' />
//                            <FieldRef Name='ID' />
//                            <FieldRef Name='Nature_documentaire' />
//                            <FieldRef Name='EDF_Projet' />
//                            <FieldRef Name='EDF_Site' />
//                            <FieldRef Name='EDF_Typologie' />";
            q.RowLimit = 1;

            SPListItemCollection itemColl = list.GetItems(q);
            if (itemColl.Count == 1) return itemColl[0];
            else if (itemColl.Count == 0) return null;
            else throw new InvalidOperationException(String.Format("SPEEDEAU - SuiviService - GetItemFromCode : one item only expected on SPQuery for codification '{0}', returned {1} instead", codificationSystem, itemColl.Count));
        }

        public static IEnumerable<SPListItem> GetItemsForCodification(SPList list, string codificationSystem)
        {
            SPQuery q = new SPQuery();
            q.Query = String.Format(@"<Where>
                            <Eq>
                                <FieldRef Name='CodificationSystem' />      
                                <Value Type='Text'>{0}</Value>
                            </Eq>  
                        </Where>", codificationSystem);
        
            SPListItemCollection itemColl = list.GetItems(q);
            return itemColl.Cast<SPListItem>();
        }

        
    }

    public static class SpeedeauHelper
    {
        
    }

    public static class MSHHelper
    {
        public static SPSite Site
        {
            get
            {
                IWebProperties webProp = SharePointServiceLocator.GetCurrent().GetInstance<IWebProperties>();
                string mshUrl = webProp.Get(Localization.GetResource(ResourcePropertyBag.WEB_PROPERTYBAG_SITE_MSH_URL, ResourceFiles.CORE));
                return new SPSite(mshUrl);            
            }
        }

        public static SPList ListeDeSuivi
        {
            get
            {
                using (SPSite site = Site)
                {
                    using(SPWeb web = site.OpenWeb()){
                        string listName = Localization.GetResource(ResourceListKeys.SUIVI_LISTNAME, ResourceFiles.CORE);
                        return web.Lists[listName];
                    }
                }
            }
        }
    }
}
