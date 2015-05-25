using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenerateJSONParamFiles.Model;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Configuration;


namespace GenerateJSONParamFiles
{
    class Program
    {
        static void Main(string[] args)
        {
            Program p = new Program();
            p.TXM = new MMS();
            p.Run();
            p.SaveAsSJON();

        }

        private void SaveAsSJON()
        {
            Console.WriteLine("Serializing as JSON");
            string path = ConfigurationManager.AppSettings.Get("filePath");
            using (FileStream stream1 = File.Create(path))
            {
                string head = "Type.registerNamespace(\"SPDO\");SPDO.taxonomy = function(){};SPDO.taxonomy.data=";
                byte[] bytes = Encoding.ASCII.GetBytes(head);
                stream1.Write(bytes, 0, bytes.Length);
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(MMS));
                ser.WriteObject(stream1, TXM);
            }
        }

        public MMS TXM { get; set; }

        private void Run()
        {
            Console.WriteLine("Reading Taxonomy...");
            string siteurl = ConfigurationManager.AppSettings.Get("siteUrl");
            using (SPSite site = new SPSite(siteurl))
            {
                TaxonomySession session = new TaxonomySession(site);
                TermStore store = session.DefaultSiteCollectionTermStore;
                TXM.ID = store.Id;
                TXM.Name = store.Name;

                Group g = (from x in store.Groups
                           where x.Name == "SPEEDEAU"
                           select x).Single();

                var q = (from s in g.TermSets
                         select s.GetAllTerms().Count()).Sum();

                Progress.Total = q;

                TaxonGroup tg = new TaxonGroup { ID = g.Id, Name = g.Name };
                foreach (TermSet set in g.TermSets)
                {
                    TaxonSet tSet = new TaxonSet { ID = set.Id, Name = set.Name };
                    foreach (Term t in set.Terms)
                    {
                        Taxon taxon = GetTaxon(t);
                        tSet.Taxons.Add(taxon);
                    }
                    tg.TaxonSets.Add(tSet);
                }
                TXM.Groups.Add(tg);

            }
            Console.WriteLine("Data Loaded");
        }

        private Taxon GetTaxon(Term t)
        {
            Progress.PrintPercent();
            Taxon newTaxon = new Taxon { ID = t.Id, Label = t.Name };
            if (t.TermsCount > 0)
            {
                newTaxon.HasChild = true;
                foreach (Term child in t.Terms)
                {
                    Taxon taxon = GetTaxon(child);
                    newTaxon.Taxons.Add(taxon);
                }
            }
            else
            {
                newTaxon.HasChild = false;
            }
            return newTaxon;
        }
    }
}
