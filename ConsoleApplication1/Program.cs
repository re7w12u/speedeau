using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Taxonomy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {

            Program p = new Program();

            string choice = String.Empty;
            while (choice != "7")
            {
                choice = p.Usage();
                switch (choice)
                {
                    case "1":
                        p.DeleteSite();
                        p.RecreateSite(); break;
                    case "2":
                        p.RecreateSite();
                        break;
                    case "9":
                        p.DeleteSite();
                        break;
                    case "7":
                        choice = "7";
                        break;
                    default:
                        break;
                }
            }
            Console.WriteLine("bye bye");

        }

        private string Usage()
        {
            Console.WriteLine("**********************");
            Console.WriteLine("1 - delete and create");
            Console.WriteLine("2 - create only");
            Console.WriteLine("9 - delete only");
            Console.WriteLine("7 - exit");
            Console.WriteLine("**********************");

            string choice = Console.ReadLine();
            return choice;
        }

        private string hostname  ="http://rdits-sp13-dev2";

        private void RecreateSite()
        {
            uint locale = 1036;

            string siteCollRelativeUrl = "/sites/rnvo";

            SPWebApplication webapp = SPWebApplication.Lookup(new Uri(hostname));
            SPSite newSite;
            Console.WriteLine("WebApp ok");
            if (!webapp.Sites.Any(s => s.Url == hostname + siteCollRelativeUrl))
            {
                newSite = webapp.Sites.Add(siteCollRelativeUrl, "RNVO", "", locale, "STS#0", @"eur\sesa260501", "Julien", "julien.bessiere@non.schneider-electric.com");
                Console.WriteLine("New site created");
            }
            else
            {
                newSite = new SPSite(hostname + siteCollRelativeUrl);
            }



            if (!newSite.AllWebs.Any(w => w.ServerRelativeUrl == "/sites/rnvo/ext1"))
            {
                newSite.AllWebs.Add("/sites/rnvo/ext1", "EXT1", "", locale, "STS#0", false, false);
                Console.WriteLine("EXT1 created");
            }
            if (!newSite.AllWebs.Any(w => w.ServerRelativeUrl == "/sites/rnvo/ext2"))
            {
                newSite.AllWebs.Add("/sites/rnvo/ext2", "EXT2", "", locale, "STS#0", false, false);
                Console.WriteLine("EXT2 created");
            }
            if (!newSite.AllWebs.Any(w => w.ServerRelativeUrl == "/sites/rnvo/ext3"))
            {
                newSite.AllWebs.Add("/sites/rnvo/ext3", "EXT3", "", locale, "STS#0", false, false);
                Console.WriteLine("EXT3 created");
            }


            newSite.Dispose();
        }

        private void DeleteSite()
        {
            string url = hostname + "/sites/rnvo";

            try
            {
                using (SPSite site = new SPSite(url))
                {
                    if (site != null)
                    {
                        site.Delete();
                        Console.WriteLine("Site Deleted");
                    }
                }
            }
            catch (Exception) { Console.WriteLine("could not delete " + url); }
        }

        private void CleanUpSite()
        {
            string url = hostname + "/sites/rnvo";
            using (SPSite site = new SPSite(url))
            {
                SPWeb web = site.OpenWeb();

                // delete Dep list
                SPList depList = web.Lists.TryGetList("Déploiement");
                if (depList != null) depList.Delete();

                // delete obs list
                SPList obsList = web.Lists.TryGetList("Observations");
                if (obsList != null) obsList.Delete();

                // empty trash
                web.RecycleBin.DeleteAll();
                web.Update();
                site.RecycleBin.DeleteAll();


                // delete dep ct ;
                SPContentType depct = web.ContentTypes[new SPContentTypeId("0x010100836ACCAE0B98461C8535E4CF511155E2")];
                if (depct == null) Console.WriteLine("cannot delete DEP content type");
                else depct.Delete();

                // delete obs ct
                SPContentType obsct = web.ContentTypes["0x0100D781A5E7B4864164AD41FD260BF491F0"];
                if (obsct == null) Console.WriteLine("cannot delete OBS content type");
                else obsct.Delete();
                web.Update();

                // remove fields
                web.Fields.Delete("observation");
                web.Fields.Delete("document_CodeIH1600");
                web.Fields.Delete("document_x003a_Copy_x0020_Source");
                web.Fields.Delete("document");
                web.Fields.Delete("CodeIH1600");
                web.Fields.Delete("EDFRevision");
                web.Fields.Delete("EDFVersion");
                web.Fields.Delete("observation");
                web.Fields.Delete("observation");
                web.Fields.Delete("observation");
                web.Fields.Delete("observation");
                web.Fields.Delete("observation");

                web.Fields.Delete("observation");
                web.Fields.Delete("observation");

            }
        }
    }
}
