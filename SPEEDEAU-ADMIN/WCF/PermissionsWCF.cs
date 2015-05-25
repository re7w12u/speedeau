using Microsoft.Practices.SharePoint.Common.ServiceLocation;
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
    public interface IPermissionsWCF
    {
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "CanUploadToDeploiement")]
        bool CanUploadToDeploiement();


        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "CanChangeListeDeSuivi")]
        bool CanChangeListeDeSuivi();
    }


    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class PermissionsWCF : IPermissionsWCF
    {
        public bool CanUploadToDeploiement()
        {
            IPermissionsService permSvc = SharePointServiceLocator.GetCurrent().GetInstance<IPermissionsService>();
            return permSvc.CanUploadToDeploiement();
        }

        public bool CanChangeListeDeSuivi()
        {
            IPermissionsService permSvc = SharePointServiceLocator.GetCurrent().GetInstance<IPermissionsService>();
            return permSvc.CanChangeListeDeSuivi();
        }
    }
}
