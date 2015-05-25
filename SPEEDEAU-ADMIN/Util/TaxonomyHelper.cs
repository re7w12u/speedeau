using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;
using SPEEDEAU;
using SPEEDEAU.ADMIN.Model;
using SPEEDEAU.ADMIN.Util;
using SPEEDEAU.ADMIN.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SPEEDEAU.ADMIN.Util
{

    public class TaxonomyValueBuilder
    {

        public TaxonomyValue Build(TaxonomyWebTaggingControl inputCtrl)
        {

            TaxonomyValue result = new TaxonomyValue();
            try
            {
                result.TermStoreID = inputCtrl.SspId.First();
                result.TermSetID = inputCtrl.TermSetId.First();
                string[] terms = inputCtrl.Text.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string term in terms)
                {
                    string[] t = term.Split(new char[] { TaxonomyField.TaxonomyGuidLabelDelimiter }, StringSplitOptions.RemoveEmptyEntries);
                    result.Terms.Add(new TaxonomyTerm { Term = t[0], TermID = t[1] });
                }

            }
            catch (Exception err)
            {
                LoggerManager.Error(LoggerCategory.ApplicationPage, err);
            }
            return result;
        }

        internal TaxonomyValue Build(TaxonomyFieldValue vfield, TaxonomyField tField)
        {
            // new TaxonomyValue { TermSetID = tfield.TermSetId, TermStoreID = tfield.SspId, Term = vfield.Label, TermID = vfield.TermGuid });
            TaxonomyValue result = new TaxonomyValue();
            try
            {
                result.TermStoreID = tField.SspId;
                result.TermSetID = tField.TermSetId;
                string[] terms = vfield.ToString().Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string term in terms)
                {
                    string[] t = term.Split(new char[] { TaxonomyField.TaxonomyGuidLabelDelimiter }, StringSplitOptions.RemoveEmptyEntries);
                    result.Terms.Add(new TaxonomyTerm { Term = t[0], TermID = t[1] });
                }
            }
            catch (Exception err)
            {
                LoggerManager.Error(LoggerCategory.ApplicationPage, err);
            }
            return result;
        }

        internal TaxonomyValue Build(TaxonomyFieldValueCollection vfield, TaxonomyField tField)
        {
            // new TaxonomyValue { TermSetID = tfield.TermSetId, TermStoreID = tfield.SspId, Term = vfield.Label, TermID = vfield.TermGuid });
            TaxonomyValue result = new TaxonomyValue();
            try
            {
                result.TermStoreID = tField.SspId;
                result.TermSetID = tField.TermSetId;
                string[] terms = vfield.ToString().Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string term in terms)
                {
                    string[] t = term.Split(new char[] { TaxonomyField.TaxonomyGuidLabelDelimiter }, StringSplitOptions.RemoveEmptyEntries);
                    result.Terms.Add(new TaxonomyTerm { Term = t[0], TermID = t[1] });
                }
            }
            catch (Exception err)
            {
                LoggerManager.Error(LoggerCategory.ApplicationPage, err);
            }
            return result;
        }

        internal TaxonomyValue Build(SPListItem item, string fieldName)
        {
            TaxonomyField tfield = item.Fields[fieldName] as TaxonomyField;
            object vfield = item[fieldName]; // do not cast TaxonomyFieldValue or TaxonomyFieldValueCollection here because it depends on the property AllowMultipleValues

            TaxonomyValue result = null;
            if (vfield != null)
            {
                if (tfield.AllowMultipleValues) result = Build(vfield as TaxonomyFieldValueCollection, tfield);
                else result = Build(vfield as TaxonomyFieldValue, tfield);
            }
            return result;
        }

        public TaxonomyValue Build(string value)
        {
            string SEP_1 = "#;";
            string SEP_2 = ";";
            TaxonomyValue result = new TaxonomyValue();

            if (value.Contains(SEP_1))
            {
                string[] data = value.Split(new string[] { SEP_1 }, StringSplitOptions.RemoveEmptyEntries);
                result.TermStoreID = Guid.Parse(data[0]);
                result.TermSetID = Guid.Parse(data[1]);

                if (data.Length > 2 && !String.IsNullOrWhiteSpace(data[2]))
                {
                    string[] termsInfo = data[2].Split(new string[] { SEP_2 }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string t in termsInfo)
                    {
                        string[] info = t.Split(new char[] { TaxonomyField.TaxonomyGuidLabelDelimiter });
                        TaxonomyTerm term = new TaxonomyTerm();
                        term.Term = info[0];
                        term.TermID = info[1];
                        result.Terms.Add(term);
                    }
                }
            }

            return result;
        }
    }

    public static class TaxonomyHelper
    {
        /// <summary>
        /// sets the SPListItem colum value from Deploiement
        /// </summary>
        /// <param name="item"></param>
        /// <param name="dep"></param>
        /// <param name="p"></param>
        /// <param name="attr"></param>
        /// <param name="fieldName"></param>
        public static void SetSPListItemTaxonValue(SPListItem item, IH1600DOC dep, PropertyInfo p, SpeedeauFieldAttribute attr, string fieldName)
        {
            try
            {
                TaxonomyValue value = p.GetValue(dep) as TaxonomyValue;
                TaxonomyField tfield = item.Fields[fieldName] as TaxonomyField;

                //if (value.Terms.Count == 1 && !tfield.AllowMultipleValues)
                if (!tfield.AllowMultipleValues)
                {
                    if (value.Terms.Count > 0)
                    {
                        TaxonomyTerm t = value.Terms.First();
                        if (!String.IsNullOrWhiteSpace(t.TermID) && !String.IsNullOrWhiteSpace(t.Term))
                        {
                            TaxonomyFieldValue vfield = new TaxonomyFieldValue(tfield);
                            vfield.PopulateFromLabelGuidPair(t.ToString());
                            tfield.SetFieldValue(item, vfield);
                        }
                    }
                    else
                    {
                        TaxonomyFieldValue vfield = new TaxonomyFieldValue(tfield);
                        tfield.SetFieldValue(item, vfield);                        
                    }
                }
                //else if (value.Terms.Count > 1 && tfield.AllowMultipleValues)
                else if (tfield.AllowMultipleValues)
                {
                    TaxonomyFieldValueCollection vFieldColl = new TaxonomyFieldValueCollection(tfield);
                    vFieldColl.PopulateFromLabelGuidPairs(value.ToString());
                    tfield.SetFieldValue(item, vFieldColl);
                }
            }
            catch (Exception err)
            {
                LoggerManager.Error(LoggerCategory.Deploiement, "Error while processing fieldName={0}, propertyInfo={1}, attr is null = {2}, Term=", fieldName, p.Name, attr == null);
                LoggerManager.Error(LoggerCategory.Deploiement, err);
            }
        }

        /// <summary>
        /// set the Deploiement property value from SPListitem
        /// </summary>
        /// <param name="item"></param>
        /// <param name="obj"></param>
        /// <param name="taxBuilder"></param>
        /// <param name="p"></param>
        /// <param name="attr"></param>
        /// <param name="fieldName"></param>
        public static void SetIH1600TaxonValue(SPListItem item, IH1600DOC obj, TaxonomyValueBuilder taxBuilder, PropertyInfo p, SpeedeauFieldAttribute attr, string fieldName)
        {
            try
            {
                TaxonomyField tfield = item.Fields[fieldName] as TaxonomyField;
                object vfield = item[fieldName]; // do not cast TaxonomyFieldValue or TaxonomyFieldValueCollection here because it depends on the property AllowMultipleValues
                TaxonomyValue value = null;
                if (vfield != null)
                {
                    if (tfield.AllowMultipleValues) value = taxBuilder.Build(vfield as TaxonomyFieldValueCollection, tfield);
                    else value = taxBuilder.Build(vfield as TaxonomyFieldValue, tfield);
                }
                else
                {
                    value = new TaxonomyValue();
                }
                p.SetValue(obj, value);
            }
            catch (Exception err)
            {
                LoggerManager.Error(LoggerCategory.Deploiement, "Error while processing fieldName={0}, propertyInfo={1}, attr is null = {2}, Term=", fieldName, p.Name, attr == null);
                LoggerManager.Error(LoggerCategory.Deploiement, err);
            }
        }

    }
}