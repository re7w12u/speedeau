using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.SharePoint;
using SPEEDEAU.Services;
using Microsoft.Practices.SharePoint.Common.ServiceLocation;
using SPEEDEAU.Model;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        public string SiteUrl { get; set; }
        public string DeploiementListName { get; set; }
        public int DepItemID { get; set; }
        [TestInitialize]
        public void Init()
        {
            SiteUrl = "http://rdits-sp13-dev/sites/rnvo/ext1";
            DeploiementListName = "Déploiement";
            DepItemID = 1;
        }

        [TestMethod]
        public void Test_HydrateDeploiementFromDocLibraryItem()
        {
            using (SPSite site = new SPSite(SiteUrl))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    SPList list = web.Lists[DeploiementListName];
                    SPListItem item = list.GetItemById(DepItemID);

                    //IDeploiementService depService = SharePointServiceLocator.GetCurrent().GetInstance<IDeploiementService>();
                    //Deploiement dep = depService.GetDeploiementFromDepLibItem(DepItemID, web);
                    //Assert.IsNotNull(dep);
                    //Assert.AreEqual(dep.Title, item.Title);
                }
            }
        }
    }
}
