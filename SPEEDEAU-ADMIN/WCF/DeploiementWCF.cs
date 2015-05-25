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
    public interface IDeploiementWCF
    {

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "getDeploiementForSuiviItem/{suivi_id}")]
        Deploiement GetDeploiementForSuiviItem(string suivi_id);
    }


    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class DeploiementWCF : IDeploiementWCF
    {


        public Deploiement GetDeploiementForSuiviItem(string suivi_id)
        {
            ISuiviService suiviService = SharePointServiceLocator.GetCurrent().GetInstance<ISuiviService>();
            SuiviEntity suiviEntity = suiviService.GetDocLinkedInfo(Convert.ToInt32(suivi_id), SPContext.Current.Web);
            
            if (suiviEntity == null) return null;

            IDeploiementService depService = SharePointServiceLocator.GetCurrent().GetInstance<IDeploiementService>();
            Deploiement dep = depService.GetDeploiementFromDepLibItem(suiviEntity.DocID, SPContext.Current.Web);
            return dep;
        }
    }
}
