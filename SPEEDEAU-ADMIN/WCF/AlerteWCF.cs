using Microsoft.Practices.SharePoint.Common.ServiceLocation;
using Microsoft.SharePoint;
using SPEEDEAU.ADMIN.Model;
using SPEEDEAU.ADMIN.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace SPEEDEAU.ADMIN.WCF
{

    [ServiceContract]
    public interface IAlerteWCF
    {
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "notifyauthor/{listname}/{itemID}/{newStatus}")]
        bool NotifyAuthor(string listname, string itemID, string newStatus);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "notifysitemembers/{listname}/{itemID}")]
        bool NotifySiteMembers(string listname, string itemID);
    }


    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class AlerteWCF : IAlerteWCF
    {

        public bool NotifyAuthor(string listname, string itemID, string newStatus)
        {
            IAlerteService alertService = SharePointServiceLocator.GetCurrent().GetInstance<IAlerteService>();
            string webUrl = SPContext.Current.Web.Url;
            return alertService.NotifyAuthor(webUrl, listname, Convert.ToInt32(itemID), newStatus);
        }

        public bool NotifySiteMembers(string listname, string itemID)
        {
            IAlerteService alertService = SharePointServiceLocator.GetCurrent().GetInstance<IAlerteService>();
            string webUrl = SPContext.Current.Web.Url;
            return alertService.NotifySiteMembers(webUrl, listname, Convert.ToInt32(itemID));
        }
    }
}
