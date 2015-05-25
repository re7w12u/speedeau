using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IH1600.Util;

namespace IH1600.Alertes
{
    class Alerte
    {



        public static void CreateAlert(SPItemEventProperties properties)
        {
            //SPItemEventDataCollection data = properties.AfterProperties;
            SPWeb web = properties.Web;
            //SPListItem item = web.Lists.TryGetList(properties.ListTitle).GetItemById(properties.ListItemId);
            SPListItem item = properties.ListItem;
            SPFile file = item.File;
            SPList alerte = web.Lists["Alertes"];
            SPListItem newAlerte = alerte.AddItem();

            string title = properties.AfterProperties.EnsureValue<string>("Title");
            string version = properties.AfterProperties.EnsureValue<string>("EDFVersion");
            string revision = properties.AfterProperties.EnsureValue<string>("EDFRevision");
            string status = properties.AfterProperties.EnsureValue<string>("Status");
            //newAlerte["Title"] = String.Format("{0} {1} {2}",
            //                                item.EnsureValue<string>("Title"),
            //                                item.EnsureValue<string>("EDFVersion"),
            //                                item.EnsureValue<string>("EDFRevision"));

            newAlerte["Title"] = String.Format("{0} {1} {2}", title, version, revision);

            newAlerte["Body"] = String.Format("Téléchargement du document <a href='{0}'>{1}</a> en version {2}{3} et Status {4}",
                                            file.Url,
                                            file.Name,
                                            title,
                                            version,
                                            status);
            newAlerte.Update();
        }
    }
}
