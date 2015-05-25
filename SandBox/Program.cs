using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using SPEEDEAU.Util;
using SPEEDEAU.Model;

namespace SandBox
{
    class Program
    {
        static void Main(string[] args)
        {
            Program p = new Program();

            string s = String.Format("stest");

            p.run();
           // p.test();

            Console.WriteLine("Done");
            Console.ReadLine();

        }

        private void test()
        {
            

        }

        private void run()
        {
            string url = "http://rdits-sp13-dev/sites/rnvo/ext3/";
            using (SPSite site = new SPSite(url))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    string fieldName ="Famille documentaire";
                    SPList list = web.Lists["Déploiement"];
                    SPListItem item = list.GetItemById(3);
                    SPField field = item.Fields[fieldName];

                    object o = item[fieldName];
                    Console.WriteLine(o.GetType().Name);


                    TaxonomyFieldValue vField = o as TaxonomyFieldValue;                    
                    TaxonomyField taxField = item.Fields[fieldName] as TaxonomyField;

                    TaxonomyValue tax = new TaxonomyValue { Term = vField.Label, TermID = vField.TermGuid, TermSetID = taxField.TermSetId, TermStoreID = taxField.SspId };

                    
                    
                    //TaxonomyField taxField = field as TaxonomyField;
                    //TaxonomyFieldValue value = new TaxonomyFieldValue(taxField);
                    //value.TermGuid = "f5f140ba-6c44-404f-9b1a-0c7d6ecd85a8";
                    //value.Label = "Bathie (La) [34C]";
                    //item["Famille documentaire"] = value;
                    //item.Update();
                }
            }
        }
    }
}
