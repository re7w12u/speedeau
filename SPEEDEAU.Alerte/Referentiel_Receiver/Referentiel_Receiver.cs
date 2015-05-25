using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;
using SPEEDEAU.ADMIN;
using Microsoft.Practices.SharePoint.Common.ServiceLocation;
using SPEEDEAU.ADMIN.Services;
using SPEEDEAU.ADMIN.Util;

namespace SPEEDEAU.Alerte.Referentiel_Receiver
{
    /// <summary>
    /// List Item Events
    /// </summary>
    public class Referentiel_Receiver : IH1600Receiver
    {
        /// <summary>
        /// An item was added.
        /// </summary>
        public override void ItemAdded(SPItemEventProperties properties)
        {
            // Alerte(properties);      
        }

        public override void ItemUpdated(SPItemEventProperties properties)
        {
                SPWeb web = properties.Web;
            SPListItem item = properties.ListItem;
            
            string key = ResourcePropertyBag.WEB_PROPERTYBAG_REF_ITEMUPDATED + properties.ListId + properties.ListItemId;
            IWebProperties propBag = SharePointServiceLocator.GetCurrent().GetInstance<IWebProperties>();
            string ticks = propBag.Get(key, web);
            if(String.IsNullOrWhiteSpace(ticks)  || new TimeSpan(DateTime.Now.Ticks - Convert.ToInt64(ticks)).Minutes > 0)
            {
                DateTime modified = DateTime.Parse(item[SPBuiltInFieldId.Modified].ToString());
                propBag.Set(key, modified.Ticks.ToString(), web);
                Alerte(properties);
                UpdateCodification(properties);
            }
        }

        private static void Alerte(SPItemEventProperties properties)
        {
            LoggerManager.Logger.TraceToDeveloper("ENTERING REFERENTIEL ITEM ADDED EVENT RECEIVER", LoggerManager.EventId, LoggerManager.AreaFullName(LoggerCategory.Alertes));

            // sending email
            string listName = properties.List.Title;
            int itemID = properties.ListItemId;
            string webUrl = properties.WebUrl;

            IAlerteService alerteService = SharePointServiceLocator.GetCurrent().GetInstance<IAlerteService>();
            bool result = alerteService.NotifySiteMembers(webUrl, listName, itemID);

            LoggerManager.Logger.TraceToDeveloper(String.Format("LEAVING REFERENTIEL EVENT RECEIVER - email sent = {0}", result), LoggerManager.EventId, LoggerManager.AreaFullName(LoggerCategory.Alertes));
        }


    }
}