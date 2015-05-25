using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxonomyBinder
{
    class Program
    {
        static void Main(string[] args)
        {
            Program p = new Program();
            p.Run();

            Console.WriteLine("Done");
            Console.ReadLine();

        }

        private void Run()
        {
            string siteUrl = Conf.Instance.SiteUrl;
            Console.WriteLine("Opening site {0} ", siteUrl); 
            using (SPSite site = new SPSite(siteUrl))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    TaxonomySession session = new TaxonomySession(site);
                    TermStore store = session.TermStores.First(t => t.Id == Guid.Parse(Conf.Instance.MMS_ID));
                    Console.WriteLine("TermStore OK - {0} ", store.Name);

                    Group group = store.Groups.First(g => g.Id == Guid.Parse(Conf.Instance.Group_ID));
                    Console.WriteLine("Group OK - {0} ", group.Name);

                    List<Bind> binds = Conf.Instance.Binds;
                    Console.WriteLine("Bind OK - count = {0} ", binds.Count);  
                    foreach (Bind b in binds)
                    {
                        if (web.Fields.Contains(b.ID))
                        {
                            SPField field = web.Fields[b.ID];
                            TaxonomyField tField = field as TaxonomyField;

                            if (tField.SspId != Guid.Empty && tField.TermSetId != Guid.Empty && tField.SspId == store.Id)
                            {
                                Console.WriteLine("SKIP {0} ", b.Name); 
                                continue;
                            }
                            TermSet set = group.TermSets.First(s => s.Name == b.TermSetName);
                            tField.SspId = store.Id;
                            tField.TermSetId = set.Id;
                            tField.AnchorId = Guid.Empty;
                            tField.TargetTemplate = String.Empty;
                            field.Update(true);
                            Console.WriteLine("OK {0} bound to {1}", b.Name, b.TermSetName);
                        }
                    }
                }
            }


        }
    }
}
