using Microsoft.Practices.SharePoint.Common.ServiceLocation;
using Microsoft.SharePoint;
using SPEEDEAU.ADMIN.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPEEDEAU.ADMIN.Services
{
    class PermissionsService : ServiceBase, IPermissionsService
    {
        public bool CanUploadToDeploiement()
        {
            SPWeb web = SPContext.Current.Web;
            string listName = Localization.GetResource(ResourceListKeys.DEPLOIEMENT_LISTNAME, ResourceFiles.CORE);
            SPList list = web.Lists[listName];
            return list.DoesUserHavePermissions(SPBasePermissions.AddListItems);
        }

        public bool CanViewMenu()
        {
            SPWeb web = SPContext.Current.Web;
            SPList list = SPContext.Current.List;
            return list.DoesUserHavePermissions(SPBasePermissions.ManageLists);
        }


        public bool CanChangeListeDeSuivi()
        {
            //using (SPSite site = MSHHelper.Site)
            //{
            //    using (SPWeb web = site.OpenWeb())
            //    {
            //        string listName = Localization.GetResource(ResourceListKeys.SUIVI_LISTNAME, ResourceFiles.CORE);
            //        SPList list = web.Lists[listName];
            //        return list.DoesUserHavePermissions(SPBasePermissions.EditListItems);
            //    }
            //}

            SPList list = MSHHelper.ListeDeSuivi;
            return list.DoesUserHavePermissions(SPBasePermissions.EditListItems);
        }
    }
}
