using Microsoft.SharePoint;
using SPEEDEAU;
using SPEEDEAU.ADMIN.Model;
using SPEEDEAU.ADMIN.Services;
using SPEEDEAU.ADMIN.Util;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPEEDEAU.ADMIN.Services
{
    /// <summary>
    /// makes the link between liste de suivi and library with the actual documents
    /// </summary>
    public class SuiviService : DocServiceBase, ISuiviService
    {
        public const string PROPERTY_BAG_NAME = "SPEEDEAU_SUIVI_PROPERTIES";

        /// <summary>
        /// make deploiement entity from data in liste de suivi matching codificationsystem
        /// </summary>
        /// <param name="codificationsystem"></param>
        /// <returns></returns>
        public Deploiement GetDeploiement(string codificationsystem)
        {
            try
            {
                Deploiement dep = new Deploiement();
                SPListItem suiviItem = GetItemForCodificationSystem(codificationsystem);
                dep.Codification = suiviItem.EnsureValue<string>(Localization.GetResource(ResourceFieldsKeys.CODIFICATION, ResourceFiles.FIELDS));
                dep.CodificationSystem = suiviItem.EnsureValue<string>(Localization.GetResource(ResourceFieldsKeys.CODIFICATION_SYSTEM, ResourceFiles.FIELDS));
                dep.Title = suiviItem.EnsureValue<string>(SPBuiltInFieldId.Title);
                dep.ID = suiviItem.EnsureValue<int>(SPBuiltInFieldId.ID);

                TaxonomyValueBuilder builder = new TaxonomyValueBuilder();

                dep.NatureDoc = builder.Build(suiviItem, Localization.GetResource(ResourceFieldsKeys.NATURE_DOC, ResourceFiles.FIELDS));
                dep.Projet = builder.Build(suiviItem, Localization.GetResource(ResourceFieldsKeys.PROJECT_TAXON, ResourceFiles.FIELDS));
                dep.Site = builder.Build(suiviItem, Localization.GetResource(ResourceFieldsKeys.SITE_TAXON, ResourceFiles.FIELDS));
                dep.FamilleDoc = builder.Build(suiviItem, Localization.GetResource(ResourceFieldsKeys.FAMILLE_DOC, ResourceFiles.FIELDS));

                return dep;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// retrieve Item from Liste de suivi based on the CodificationSystem field
        /// </summary>
        /// <param name="codificationsystem"></param>
        /// <returns></returns>
        public SPListItem GetItemForCodificationSystem(string codificationsystem)
        {
            //SPWeb web = SPContext.Current.Web;
            //string listName = Localization.GetResource(ResourceListKeys.SUIVI_LISTNAME, ResourceFiles.CORE);
            //SPList list = web.Lists[listName];            
            return CodificationHelper.GetItemForCodification(MSHHelper.ListeDeSuivi, codificationsystem);
        }

        /// <summary>
        /// get data stored in the property bag of the item in the 'liste de suivi'
        /// </summary>
        /// <param name="id">id of the item in 'liste de suivi'</param>
        /// <returns></returns>
        public SuiviEntity GetDocLinkedInfo(int id, SPWeb web)
        {
            //if (web == null) throw new NullReferenceException("We need a proper SPWeb object here");
            //string listName = Localization.GetResource(ResourceListKeys.SUIVI_LISTNAME, ResourceFiles.CORE);

            //SPList list = web.Lists[listName];
            SPList list = MSHHelper.ListeDeSuivi;
            SPListItem item = list.GetItemById(id);
            if (!item.Properties.Contains(PROPERTY_BAG_NAME)) return null;

            Object pValue = item.Properties[PROPERTY_BAG_NAME];
            return new SuiviEntity(pValue.ToString());
        }

        public SuiviEntity GetDocLinkedInfo(int id)
        {
            return GetDocLinkedInfo(id, SPContext.Current.Web);
        }

               
        public string CodificationSystem(int suivi_id)
        {
            //SPWeb web = SPContext.Current.Web;
            //string listName = Localization.GetResource(ResourceListKeys.SUIVI_LISTNAME, ResourceFiles.CORE);
            //SPList list = web.Lists[listName];
            SPList list = MSHHelper.ListeDeSuivi;
            SPListItem item = list.GetItemById(suivi_id);
            string fieldName = Localization.GetResource(ResourceFieldsKeys.CODIFICATION_SYSTEM, ResourceFiles.FIELDS);
            return item.EnsureValue<string>(fieldName);
        }


        #region handle creation / modification / duplication feature

        public Suivi GetSuiviByID(int suivi_id, SPWeb web)
        {
            Suivi result = new Suivi();
            string listSuiviName = Localization.GetResource(ResourceListKeys.SUIVI_LISTNAME, ResourceFiles.CORE);
            //SPList list = web.Lists[listSuiviName];
            SPList list = MSHHelper.ListeDeSuivi;
            SPListItem item = list.GetItemById(suivi_id);

            HydrateIH1600FromDocLibraryItem(item, result);

            return result;

            // set values
            //TaxonomyValueBuilder taxBuilder = new TaxonomyValueBuilder();
            //foreach (PropertyInfo p in result.GetType().GetProperties())
            //{
            //    SpeedeauFieldAttribute attr = p.GetCustomAttribute<SpeedeauFieldAttribute>();
            //    if (attr != null)
            //    {
            //        string fieldName = Localization.GetResource(attr.ResxKey, attr.ResxFile);

            //        if (attr.IsTaxon)
            //        {
            //            TaxonomyHelper.SetIH1600TaxonValue(item, result, taxBuilder, p, attr, fieldName);
            //        }
            //        else if (attr.IsComplexType && attr.ComplexTypeHandler.GetInterface(typeof(IStrategyHanlder).Name) != null)
            //        {
            //            IStrategyHanlder handler = Activator.CreateInstance(attr.ComplexTypeHandler) as IStrategyHanlder;
            //            if (handler != null) handler.Get(item, p, result as IH1600DOC);
            //        }
            //        else if (attr.IsProperty && !String.IsNullOrWhiteSpace(attr.PropertyName))
            //        {
            //            // we need this because some info are accessed using direct property and not indexed array like [""]
            //            // i.e item.Name for the file name, or item.Title, item.ID, etc.
            //            PropertyInfo prop = item.GetType().GetProperty(attr.PropertyName);
            //            object value = prop.GetValue(item);
            //            if (value != null) p.SetValue(result, value);
            //        }
            //        else
            //        {
            //            object value = item.EnsureValue<object>(fieldName);

            //            if (value != null)
            //            {
            //                if (value.GetType().FullName != p.PropertyType.FullName) value = Convert.ChangeType(value, p.PropertyType);
            //                p.SetValue(result, value);
            //            }
            //        }
            //    }
            //}

            //return result;
        }

        public SPListItem UpdateSuivi(Suivi suivi, SPWeb web)
        {
            //string listName = Localization.GetResource(ResourceListKeys.SUIVI_LISTNAME, ResourceFiles.CORE);
            //SPList listSuivi = web.Lists[listName];
            SPList listSuivi = MSHHelper.ListeDeSuivi;

            SPListItem item = listSuivi.GetItemById(suivi.ID);
            SetItemValues(item, suivi);
            item.Update();

            return item;
        }

        public SPListItem NewSuivi(Suivi suivi, SPWeb web)
        {
            //string listName = Localization.GetResource(ResourceListKeys.SUIVI_LISTNAME, ResourceFiles.CORE);
            //SPList listSuivi = web.Lists[listName];
            SPList listSuivi = MSHHelper.ListeDeSuivi;

            SPListItem item = listSuivi.Items.Add();
            SetItemValues(item, suivi);
            item.Update();
            return item;
        }

        #endregion
    }
}
