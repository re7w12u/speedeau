using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;
using System.Net;

namespace IH1600.Alertes
{
    /// <summary>
    /// List Item Events
    /// </summary>
    public class RefEventReceiver : DocEventReceiver
    {
        /// <summary>
        /// An item was added.
        /// </summary>
        public override void ItemAdded(SPItemEventProperties properties)
        {
            bool result = UpdateNow(properties);

            SPListItem item = properties.ListItem;
            item["Title"] = "[--UPDATED--]" + item.Title;
            item.Update();

            string listName = properties.List.Title;
            int itemID = properties.ListItemId;
            WebClient client = new WebClient();
            client.UseDefaultCredentials = true;
            client.DownloadString(properties.Web.Url + "/_vti_bin/speedeau/Alerte.svc/NotifySiteMembers/" + listName + "/" + itemID);

        }

        ///// <summary>
        ///// An item was updated.
        ///// </summary>
        //public override void ItemUpdated(SPItemEventProperties properties)
        //{
        //    if (UpdateNow(properties))
        //    {
        //        Alerte.CreateAlert(properties);
        //    }
        //}

        ///// <summary>
        ///// An item is being added
        ///// </summary>
        //public override void ItemAdding(SPItemEventProperties properties)
        //{
        //    base.ItemAdding(properties);
        //}


    }
}