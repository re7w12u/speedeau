using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;
using SPEEDEAU.ADMIN.Util;
using SPEEDEAU.ADMIN;

namespace SPEEDEAU.Alerte.Suivi_Receiver
{
    /// <summary>
    /// List Item Events
    /// </summary>
    public class Suivi_Receiver : IH1600Receiver
    {
        /// <summary>
        /// An item was updated.
        /// </summary>
        public override void ItemUpdated(SPItemEventProperties properties)
        {
            base.ItemUpdated(properties);

            UpdateCodification(properties);
        }

        


    }
}