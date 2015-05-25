using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;
using System.Net;

namespace IH1600.Alertes.Referentiel_EventReceiver
{
    /// <summary>
    /// List Item Events
    /// </summary>
    public class Referentiel_EventReceiver : SPItemEventReceiver
    {
        /// <summary>
        /// An item was added.
        /// </summary>
        public override void ItemAdded(SPItemEventProperties properties)
        {

            string listName = properties.List.Title;
            int itemID = properties.ListItemId;
            string url = properties.Web.Url + "/_vti_bin/speedeau/Alerte.svc/NotifySiteMembers/" + listName + "/" + itemID;

            SPListItem item = properties.ListItem;
            item["Title"] = "[$$UPDATED$$]" + url;
            item.Update();
            
            //WebClient client = new WebClient();
            //client.UseDefaultCredentials = true;
            //client.DownloadString(url);

        }


    }
}