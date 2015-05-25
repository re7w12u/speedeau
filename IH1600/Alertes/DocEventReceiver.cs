using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IH1600.Alertes
{
    public abstract class DocEventReceiver : SPItemEventReceiver
    {
        /// <summary>
        /// the issue with event receiver on document library is that they are fired twice.
        /// * once when the document is uploaded
        /// * once when the metadata form is saved
        /// In this case, I want my code to run only once and more specifically when the metadata form is saved.
        /// The trick here is to insert a unique key in the property of the current web. If the key if not there, it means 
        /// that's the first event receiver round. So I create the property bag entry and return false. If the key is there, 
        /// it means that the second run. I remove the key and return true.
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public bool UpdateNow(SPItemEventProperties properties)
        {
            bool result;
            SPWeb web = properties.Web;

            string key = this.GetType().Name + properties.ListTitle + properties.ListItemId;

            if (properties.Web.AllProperties.ContainsKey(key))
            {
                // that's the second update, triggered when saving metadata form
                web.DeleteProperty(key);
                result = true;
            }
            else
            {
                // that's the first update, triggered upon added or upload form
                web.AddProperty(key, "do not fire");
                result = false;
            }
            web.Update();
            return result;
        }
    }
}
