using Microsoft.SharePoint;
using SPEEDEAU.ADMIN.Model;
using SPEEDEAU.ADMIN.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Taxonomy;

namespace SPEEDEAU.ADMIN.Services
{
    public class DeploiementService : DocServiceBase, IDeploiementService
    {
        #region IDeploiementService implementation

        public Deploiement GetDeploiementFromSuiviItem(int id, SPWeb currentWeb, bool checkForExistingItemInDep)
        {
            // get item from liste de suivi
            Deploiement result = new Deploiement();
            //string listSuiviName = Localization.GetResource(ResourceListKeys.SUIVI_LISTNAME, ResourceFiles.CORE);
            //SPList list = web.Lists[listSuiviName];

            SPList listSuivi = MSHHelper.ListeDeSuivi;
            SPListItem item = listSuivi.GetItemById(id);

            // check if any deploiement for codificationsystem
            string listDepName = Localization.GetResource(ResourceListKeys.DEPLOIEMENT_LISTNAME, ResourceFiles.CORE);
            SPList depList = currentWeb.Lists[listDepName];
            object codificationSystem = item[CodificationHelper.GetCodeSystemeFieldName];

            if (codificationSystem == null || String.IsNullOrWhiteSpace(codificationSystem.ToString())) return null;

            if (checkForExistingItemInDep)
            {
                SPListItem depItem = CodificationHelper.GetItemForCodification(depList, codificationSystem.ToString());
                if (depItem != null)
                {
                    // there's already something in the deploiement library, so we load that to get latest data
                    HydrateIH1600FromDocLibraryItem(depItem, result);
                }
            }
            else
            {
                //first upload, so we load data from liste de suivi and enable process ih1600
                HydrateIH1600FromListSuiviItem(item, result);
                result.ProcessIH1600 = true;
            }
            return result;
        }

        public Deploiement GetDeploiementFromDepLibItem(int id, SPWeb web)
        {
            string listName = Localization.GetResource(ResourceListKeys.DEPLOIEMENT_LISTNAME, ResourceFiles.CORE);
            SPList list = web.Lists[listName];
            SPListItem item = list.GetItemById(id);

            Deploiement result = new Deploiement();
            HydrateIH1600FromDocLibraryItem(item, result);
            return result;
        }

        public SPListItem SaveDeploiementToList(Deploiement dep, string filename, SPWeb web)
        {
            string listName = Localization.GetResource(ResourceListKeys.DEPLOIEMENT_LISTNAME, ResourceFiles.CORE);
            SPDocumentLibrary docLib = web.Lists[listName] as SPDocumentLibrary;

            SPFile file;
            bool createNewVersion = false;
            if (dep.File != null && dep.File.Count() > 0)
            {
                if (String.IsNullOrWhiteSpace(filename))
                {
                    // new file
                    file = docLib.RootFolder.Files.Add(dep.FileName, dep.File, true);
                }
                else
                {
                    createNewVersion = true;
                    // existing file - first use previous name to keep version tracking
                    file = docLib.RootFolder.Files.Add(filename, dep.File, true);
                    // then rename file with new name
                    file.MoveTo(file.ParentFolder.Url + "/" + dep.FileName);
                }
            }
            else
            {
                file = docLib.RootFolder.Files[filename];
            }

            SPListItem item = file.Item;
            SetItemValues(item, dep);

            if (createNewVersion) item.Update();
            else item.SystemUpdate(false);

            return item;
        }


        /// <summary>
        /// Fetch IH1600 Info (Indice, Revision and Doc Status (PREL, BPE, None)) for a given codification
        /// </summary>
        /// <param name="web"></param>
        /// <param name="codification"></param>
        /// <returns></returns>
        public InfoIH1600 GetLatestInfoIH1600ForCodification(SPWeb web, string codification)
        {
            InfoIH1600 result = null;
            string depListName = Localization.GetResource(ResourceListKeys.DEPLOIEMENT_LISTNAME, ResourceFiles.CORE);
            SPList depList = web.Lists[depListName];
            IEnumerable<SPListItem> items = CodificationHelper.GetItemsForCodification(depList, codification);

            string indiceFieldName = Localization.GetResource(ResourceFieldsKeys.INDICE, ResourceFiles.FIELDS);

            InfoIH1600Builder builder = new InfoIH1600Builder();

            List<InfoIH1600> q = (from i in items select builder.Build(i)).ToList();

            if (q.Any())
            {
                q.Sort();
                result = q.Last();
            }

            return result;
        }

        #endregion

        //#region private methods
        ///// <summary>
        ///// Fill in Deploiement object using data from an SPListItem from 'Liste de suivi' list
        ///// </summary>
        ///// <param name="item"></param>
        ///// <param name="dep"></param>
        //private static void HydrateDeploiementFromListSuiviItem(SPListItem item, Deploiement dep)
        //{
        //    TaxonomyValueBuilder taxBuilder = new TaxonomyValueBuilder();
        //    foreach (PropertyInfo p in dep.GetType().GetProperties())
        //    {
        //        SpeedeauFieldAttribute attr = p.GetCustomAttribute<SpeedeauFieldAttribute>();
        //        if (attr != null)
        //        {
        //            string fieldName = Localization.GetResource(attr.ResxKey, attr.ResxFile);

        //            if (attr.IsTaxon)
        //            {
        //                TaxonomyHelper.SetIH1600TaxonValue(item, dep, taxBuilder, p, attr, fieldName);                       
        //            }
        //            else
        //            {
        //                object value = item.EnsureValue<object>(fieldName);
        //                if (value != null) p.SetValue(dep, value);
        //            }
        //        }
        //    }
        //}


        ///// <summary>
        ///// Fill in Deploiement object using data in an SPListItem from Déploiement doc library
        ///// </summary>
        ///// <param name="item"></param>
        ///// <param name="dep"></param>
        //private static void HydrateDeploiementFromDocLibraryItem(SPListItem item, Deploiement dep)
        //{
        //    TaxonomyValueBuilder taxBuilder = new TaxonomyValueBuilder();
        //    foreach (PropertyInfo p in dep.GetType().GetProperties())
        //    {
        //        SpeedeauFieldAttribute attr = p.GetCustomAttribute<SpeedeauFieldAttribute>();
        //        if (attr != null)
        //        {
        //            string fieldName = Localization.GetResource(attr.ResxKey, attr.ResxFile);

        //            if (attr.IsTaxon)
        //            {
        //                TaxonomyHelper.SetIH1600TaxonValue(item, dep, taxBuilder, p, attr, fieldName);
        //            }
        //            else if (attr.IsProperty && !String.IsNullOrWhiteSpace(attr.PropertyName))
        //            {
        //                // we need this because some info are accessed using direct property and not [""]
        //                // i.e item.Name for the file name
        //                PropertyInfo prop = item.GetType().GetProperty(attr.PropertyName);
        //                object value = prop.GetValue(item);
        //                if (value != null) p.SetValue(dep, value);
        //            }
        //            else if (p.PropertyType.BaseType.FullName == typeof(Enum).FullName)
        //            {
        //                // handle enum such as ValidationStatus - actually the only one at
        //                // that time, so I'm not dealing with potential other type for now
        //                object value = item.EnsureValue<object>(fieldName);
        //                if (value != null)
        //                {
        //                    if (p.PropertyType.FullName == typeof(ValidationStatus).FullName)
        //                    {
        //                        ValidationStatus status;
        //                        if (Enum.TryParse(value.ToString(), true, out status)) p.SetValue(dep, status);
        //                    }
        //                    else if (p.PropertyType.FullName == typeof(DocStatus).FullName)
        //                    {
        //                        DocStatus status;
        //                        if (Enum.TryParse(value.ToString(), true, out status)) p.SetValue(dep, status);
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                object value = item.EnsureValue<object>(fieldName);
        //                if (value != null) p.SetValue(dep, value);
        //            }
        //        }
        //    }
        //}



        ///// <summary>
        ///// Fill in SPListItem using data from a Deploiement Object
        ///// </summary>
        ///// <param name="item"></param>
        ///// <param name="dep"></param>
        //private static void SetItemValues(SPListItem item, Deploiement dep)
        //{
        //    foreach (PropertyInfo p in dep.GetType().GetProperties())
        //    {
        //        SpeedeauFieldAttribute attr = p.GetCustomAttribute<SpeedeauFieldAttribute>();
        //        if (attr != null)
        //        {
        //            string fieldName = Localization.GetResource(attr.ResxKey, attr.ResxFile);

        //            if (attr.IsTaxon)
        //            {
        //                TaxonomyHelper.SetSPListItemTaxonValue(item, dep, p, attr, fieldName);
        //            }
        //            else
        //            {
        //                item.EnsureValue<object>(fieldName, p.GetValue(dep));
        //            }
        //        }
        //    }
        //}


        //#endregion


        
    }
}
