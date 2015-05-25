using ImportListeDeSuivi.util;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ImportListeDeSuivi.model
{
    class ItemBuilder
    {
        private TaxonProxy taxonProxy;
        private bool _makeRandomCodification;

        public ItemBuilder(SPSite site, bool makeRandomCodification)
        {
            taxonProxy = new TaxonProxy(site);
            _makeRandomCodification = makeRandomCodification;
        }

        internal void Build(Entry e, SPListItem item)
        {

            var q = from p in e.GetType().GetProperties()
                    where p.GetCustomAttributes(typeof(EntryFieldAttribute), true).Any()
                    select p;

            foreach (PropertyInfo p in q)
            {
                EntryFieldAttribute attr = p.GetCustomAttribute<EntryFieldAttribute>();
                string xlsFieldName = attr.XLSFieldName;
                string spFieldName = attr.SPFieldName;

                if (item.Fields.ContainsFieldWithStaticName(spFieldName))
                {
                    #region taxon
                    if (attr.IsTaxon)
                    {
                        if (p.GetValue(e) != null && !String.IsNullOrWhiteSpace(p.GetValue(e).ToString()))
                        {
                            TermSet set = taxonProxy.GetTermSet(attr.TermSetId);
                            string inputValue = p.GetValue(e).ToString();
                            Term value = null;

                            // check if it's a guid
                            Guid termId = Guid.Empty;
                            if (Guid.TryParse(inputValue, out termId))
                            {                                
                                value = set.GetTerm(termId);
                            }
                            else
                            {
                                // input is a string
                                if (inputValue.Contains(";#")) inputValue = inputValue.Split(new string[] { ";#" }, StringSplitOptions.None)[1].Replace("\"", "");
                                if (inputValue.Contains(':')) inputValue = inputValue.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries).Last();
                                var result = set.GetAllTerms().Where(t => t.Name == inputValue);

                                if (result.Count() == 1)
                                {
                                    value = result.First();
                                }
                                else if (result.Count() == 0)
                                {
                                    Logger.Err("no term found for {0} (term set = {3}) - property {1} - item ID = {2}", inputValue, p.Name, item.ID, set.Name);
                                }
                                else if (result.Count() > 1)
                                {
                                    Logger.Err("More than 1 term found for {0} (term set = {3}) - property {1} - item ID = {2}", inputValue, p.Name, item.ID, set.Name);
                                }
                            }


                            if (value != null)
                            {
                                TaxonomyField tfield = item.Fields.GetFieldByInternalName(spFieldName) as TaxonomyField;
                                TaxonomyFieldValue vfield = new TaxonomyFieldValue(tfield);
                                vfield.Label = value.Name;
                                vfield.TermGuid = value.Id.ToString();
                                tfield.SetFieldValue(item, vfield);
                            }
                        }
                    }
                    #endregion
                    #region SPUser
                    else if (attr.IsPeople)
                    {
                        object login = p.GetValue(e);
                        if (login != null && !String.IsNullOrWhiteSpace(login.ToString())) { 
                            SPUser user = item.Web.EnsureUser(login.ToString());

                            SPField f = item.Fields.GetFieldByInternalName(spFieldName);
                            item[f.Title] = user;
                        }
                    }
                    #endregion
                    #region Other
                    else
                    {
                        SPField f = item.Fields.GetFieldByInternalName(spFieldName);

                        object o = p.GetValue(e);

                        if (f.Title.ToLower() == "codification")
                        {
                            // handle codification specifically to set codificationsystem as well
                            string code = String.Empty;

                            if (o != null && !String.IsNullOrWhiteSpace(o.ToString())) code = o.ToString();
                            else if (_makeRandomCodification) code = GetRandomCode();

                            f.ParseAndSetValue(item, code);
                            item["CodificationSystem"] = System.Text.RegularExpressions.Regex.Replace(code, @"[^0-9A-Z]", String.Empty);
                        }

                        else if (o != null && !String.IsNullOrWhiteSpace(o.ToString()))
                        {
                            f.ParseAndSetValue(item, o.ToString());
                        }
                    }
                    #endregion

                }
            }
            item.Update();
        }

        /// <summary>
        /// generate random IH1600 code
        /// </summary>
        /// <returns></returns>
        private string GetRandomCode()
        {
            Random r = new Random();

            Func<int, int, int, string> getRandomString = (min, max, length) =>
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < length; i++) sb.Append(Convert.ToChar(r.Next(min, max)));
                return sb.ToString().ToUpper();
            };

            string prefix = "IH";
            string part1 = getRandomString(65, 86, r.Next(2, 10));
            string part2 = getRandomString(65, 86, r.Next(2, 10));
            string suffix = getRandomString(48, 57, r.Next(3, 6));

            return String.Format("{0}-{1} {2}_{3}", prefix, part1, part2, suffix);

        }
    }

    class TaxonProxy
    {
        private TaxonomySession Session { get; set; }
        private TermStore Store { get; set; }
        private Dictionary<Guid, TermSet> TermSets { get; set; }

        public TaxonProxy(SPSite site)
        {
            Session = new TaxonomySession(site);
            Store = Session.DefaultSiteCollectionTermStore;
            TermSets = new Dictionary<Guid, TermSet>();
        }

        public TermSet GetTermSet(Guid Id)
        {
            TermSet set = null;
            if (TermSets.Keys.Contains(Id)) set = TermSets[Id];
            else
            {
                set = Store.GetTermSet(Id);
                TermSets.Add(Id, set);
            }
            return set;
        }
    }
}
