using Microsoft.Practices.SharePoint.Common.Logging;
using Microsoft.Practices.SharePoint.Common.ServiceLocation;
using Microsoft.SharePoint;
using SPEEDEAU.ADMIN.Model;
using SPEEDEAU.ADMIN.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SPEEDEAU.ADMIN.Services
{
    public abstract class ServiceBase
    {
        internal ILogger logger;

        public ServiceBase()
        {
            logger = SharePointServiceLocator.GetCurrent().GetInstance<ILogger>();
        }
    }
    
    public abstract class DocServiceBase : ServiceBase
    {

        /// <summary>
        /// Fill in Deploiement object using data from an SPListItem from 'Liste de suivi' list
        /// </summary>
        /// <param name="item"></param>
        /// <param name="dep"></param>
        protected static void HydrateIH1600FromListSuiviItem(SPListItem item, Deploiement dep)
        {
            TaxonomyValueBuilder taxBuilder = new TaxonomyValueBuilder();
            foreach (PropertyInfo p in dep.GetType().GetProperties())
            {
                SpeedeauFieldAttribute attr = p.GetCustomAttribute<SpeedeauFieldAttribute>();
                if (attr != null)
                {
                    string fieldName = Localization.GetResource(attr.ResxKey, attr.ResxFile);

                    if (attr.IsTaxon)
                    {
                        TaxonomyHelper.SetIH1600TaxonValue(item, dep, taxBuilder, p, attr, fieldName);
                    }
                    else
                    {
                        object value = item.EnsureValue<object>(fieldName);
                        if (value != null) p.SetValue(dep, value);
                    }
                }
            }
        }

        /// <summary>
        /// Fill in Deploiement object using data in an SPListItem from Déploiement doc library
        /// </summary>
        /// <param name="item"></param>
        /// <param name="obj"></param>
        protected static void HydrateIH1600FromDocLibraryItem(SPListItem item, IH1600DOC obj)
        {
            TaxonomyValueBuilder taxBuilder = new TaxonomyValueBuilder();
            foreach (PropertyInfo p in obj.GetType().GetProperties())
            {
                SpeedeauFieldAttribute attr = p.GetCustomAttribute<SpeedeauFieldAttribute>();
                if (attr != null)
                {
                    string fieldName = Localization.GetResource(attr.ResxKey, attr.ResxFile);

                    if (attr.IsTaxon)
                    {
                        TaxonomyHelper.SetIH1600TaxonValue(item, obj, taxBuilder, p, attr, fieldName);
                    }
                    else if (attr.IsProperty && !String.IsNullOrWhiteSpace(attr.PropertyName))
                    {
                        // we need this because some info are accessed using direct property and not [""]
                        // i.e item.Name for the file name
                        PropertyInfo prop = item.GetType().GetProperty(attr.PropertyName);
                        object value = prop.GetValue(item);
                        if (value != null) p.SetValue(obj, value);
                    }
                    else if (attr.IsComplexType && attr.ComplexTypeHandler.GetInterface(typeof(IStrategyHanlder).Name) != null)
                    {
                        IStrategyHanlder handler = Activator.CreateInstance(attr.ComplexTypeHandler) as IStrategyHanlder;
                        if (handler != null) handler.Get(item, p, obj);
                    }
                    else if (p.PropertyType.BaseType.FullName == typeof(Enum).FullName)
                    {
                        // handle enum such as ValidationStatus - actually the only one at
                        // that time, so I'm not dealing with potential other type for now
                        object value = item.EnsureValue<object>(fieldName);
                        if (value != null)
                        {
                            if (p.PropertyType.FullName == typeof(ValidationStatus).FullName)
                            {
                                ValidationStatus status;
                                if (Enum.TryParse(value.ToString(), true, out status)) p.SetValue(obj, status);
                            }
                            else if (p.PropertyType.FullName == typeof(StatusIH1600).FullName)
                            {
                                StatusIH1600 status;
                                if (Enum.TryParse(value.ToString(), true, out status)) p.SetValue(obj, status);
                            }
                        }
                    }
                    else
                    {
                        object value = item.EnsureValue<object>(fieldName);
                        if (value != null)
                        {
                            if (value.GetType().FullName != p.PropertyType.FullName) value = Convert.ChangeType(value, p.PropertyType);
                            p.SetValue(obj, value);
                        }
                    }
                }
            }
        }



        /// <summary>
        /// Fill in SPListItem using data from a Deploiement Object
        /// </summary>
        /// <param name="item"></param>
        /// <param name="obj"></param>
        protected static void SetItemValues(SPListItem item, IH1600DOC obj)
        {
            foreach (PropertyInfo p in obj.GetType().GetProperties())
            {
                SpeedeauFieldAttribute attr = p.GetCustomAttribute<SpeedeauFieldAttribute>();
                if (attr != null)
                {
                    string fieldName = Localization.GetResource(attr.ResxKey, attr.ResxFile);

                    if (attr.IsTaxon)
                    {
                        TaxonomyHelper.SetSPListItemTaxonValue(item, obj, p, attr, fieldName);
                    }
                    else if (attr.IsComplexType)
                    {
                        IStrategyHanlder handler = Activator.CreateInstance(attr.ComplexTypeHandler) as IStrategyHanlder;
                        if (handler != null) handler.Set(p, obj as IH1600DOC,ref item);
                    }
                    else
                    {
                        item.EnsureValue<object>(fieldName, p.GetValue(obj), attr.AllowStringEmpty);
                    }
                }
            }
        }


    }
}
