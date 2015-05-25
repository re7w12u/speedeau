using Microsoft.SharePoint;
using SPEEDEAU.ADMIN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPEEDEAU.ADMIN.Util
{
    public static class ListHelper
    {

        public static SPList Deploiement
        {
            get
            {
                string listName = Localization.GetResource(ResourceListKeys.DEPLOIEMENT_LISTNAME, ResourceFiles.CORE);
                return SPContext.Current.Web.Lists[listName];
            }
        }

        public static SPList Referentiel
        {
            get
            {
                string listName = Localization.GetResource(ResourceListKeys.REFERENTIEL_LISTNAME, ResourceFiles.CORE);
                return SPContext.Current.Web.Lists[listName];
            }
        }

        public static SPList Suivi
        {
            get
            {
                string listName = Localization.GetResource(ResourceListKeys.SUIVI_LISTNAME, ResourceFiles.CORE);
                return SPContext.Current.Web.Lists[listName];

            }
        }

    }
}
