using Microsoft.Practices.SharePoint.Common.ServiceLocation;
using Microsoft.SharePoint;
using SPEEDEAU.ADMIN.Model;
using SPEEDEAU.ADMIN.Services;
using SPEEDEAU.ADMIN.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SPEEDEAU.ADMIN.WCF
{

    [ServiceContract]
    public interface ISuiviWCF
    {
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "getPropertyBag/{suivi_id}")]
        string GetPropertyBag(string suivi_id);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "GetListSuiviGUID")]
        string GetListSuiviGUID();

        //[OperationContract]
        //[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "hasDoc/{suivi_id}")]
        //bool hasDoc(string suivi_id);

        //[OperationContract]
        //[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "getDocID/{suivi_id}")]
        //int getDocID(string suivi_id);

        //[OperationContract]
        //[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "getDocRestUrl/{suivi_id}")]
        //string getDocRestUrl(string suivi_id);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "GetSuiviItem/{suivi_id}")]
        string GetSuiviItem(string suivi_id);

        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "GetAllDocInfo/{listname}/{codification}")]
        IEnumerable<SuiviInfoEntity> GetAllDocInfo(string listname, string codification);
    }


    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class SuiviWCF : ISuiviWCF
    {
        public string GetPropertyBag(string suivi_id)
        {
            ISuiviService suivi = SharePointServiceLocator.GetCurrent().GetInstance<ISuiviService>();
            SuiviEntity entity = suivi.GetDocLinkedInfo(Convert.ToInt32(suivi_id));
            return entity == null ? String.Empty : entity.ToJson();
        }

        public string GetListSuiviGUID()
        {
            return MSHHelper.ListeDeSuivi.ID.ToString();
        }


        public string GetSuiviItem(string suivi_id)
        {
            Suivi result = null;
            // get Msh context
            using (SPSite site = MSHHelper.Site)
            {
                using (SPWeb web = site.OpenWeb())
                {
                    ISuiviService suivi = SharePointServiceLocator.GetCurrent().GetInstance<ISuiviService>();
                    //SuiviEntity info = suivi.GetDocLinkedInfo(Convert.ToInt32(suivi_id), SPContext.Current.Web);
                    result = suivi.GetSuiviByID(Convert.ToInt32(suivi_id), web);

                }
            }

            var x = new
            {
                ID = result.ID,
                Term = String.Join("#;", result.FamilleDoc.Terms.Select(t => t.Term).ToArray()),
                CodificationSystem = result.CodificationSystem
            };

            return new JavaScriptSerializer().Serialize(x);
        }

        public IEnumerable<SuiviInfoEntity> GetAllDocInfo(string listname, string codification)
        {
            SPWeb web = SPContext.Current.Web;
            string codifSystem = CodificationHelper.CleanUpCodification(codification);

            List<SuiviInfoEntity> info = new List<SuiviInfoEntity>();
            if (listname == "Déploiement") GetAllDocInfo_Deploiement(codifSystem, ref info);
            else if (listname == "Référentiel") GetAllDocInfo_Referentiel(codifSystem, ref info);

            return info;


        }

        private void GetAllDocInfo_Referentiel(string codifSystem, ref List<SuiviInfoEntity> info)
        {
            SPWeb web = SPContext.Current.Web;

            // get item from list de suivi - which tells us the criteria (Indice) 
            //string listSuiviName = Localization.GetResource(ResourceListKeys.SUIVI_LISTNAME, ResourceFiles.CORE);
            //SPList suiviList = web.Lists[listSuiviName];
            SPList suiviList = MSHHelper.ListeDeSuivi;
            IEnumerable<SPListItem> suiviItems = CodificationHelper.GetItemsForCodification(suiviList, codifSystem);

            // we are supposed to get exactly one match from liste de suivi. 
            if (suiviItems.Count() == 1)
            {
                string revisionFieldName = Localization.GetResource(ResourceFieldsKeys.REVISION, ResourceFiles.FIELDS);
                string indiceFieldName = Localization.GetResource(ResourceFieldsKeys.INDICE, ResourceFiles.FIELDS);
                string indice = suiviItems.First().EnsureValue<string>(indiceFieldName);

                // get all files for codif and search if any file match Indice
                string listRefName = Localization.GetResource(ResourceListKeys.REFERENTIEL_LISTNAME, ResourceFiles.CORE);
                SPList refList = web.Lists[listRefName];
                IEnumerable<SPListItem> items = CodificationHelper.GetItemsForCodification(refList, codifSystem);

                foreach (SPListItem item in items)
                {
                    IEnumerable<SPListItemVersion> q;
                    IEnumerable<SPListItemVersion> versions = item.Versions.Cast<SPListItemVersion>();

                    if (!String.IsNullOrWhiteSpace(indice))
                    {
                        // check if indice value match the value specified in the liste de suivi
                        q = from i in versions
                            where !String.IsNullOrWhiteSpace(i.EnsureValue<string>(indiceFieldName))
                                && i.EnsureValue<string>(indiceFieldName).Equals(indice)
                                && (String.IsNullOrWhiteSpace(i.EnsureValue<string>(revisionFieldName)))
                            select i;
                    }
                    else
                    {
                        q = from i in versions
                            where i.IsCurrentVersion
                            select i;
                    }

                    if (q.Any())
                    {
                        SPListItemVersion version = q.First();
                        SuiviInfoEntity infoSuivi = new Model.SuiviInfoEntity();
                        infoSuivi.Auteur = version.EnsureValue<string>(SPBuiltInFieldId.Editor).Split(new string[] { ";#" }, StringSplitOptions.RemoveEmptyEntries)[1];
                        // handle name formatted as:  Joël Delphin,#i:0#.w|speedeau\joel-externe.delphin,#joel-externe.delphin@edf.fr,#,#Joël Delphin
                        if (infoSuivi.Auteur.Contains(",#")) infoSuivi.Auteur = infoSuivi.Auteur.Split(new string[] { ",#" }, StringSplitOptions.RemoveEmptyEntries).First();
                        infoSuivi.DateModif = version.EnsureValue<string>(SPBuiltInFieldId.Modified);
                        infoSuivi.Indice = version.EnsureValue<string>(Localization.GetResource(ResourceFieldsKeys.INDICE, ResourceFiles.FIELDS));
                        infoSuivi.Revision = version.EnsureValue<string>(Localization.GetResource(ResourceFieldsKeys.REVISION, ResourceFiles.FIELDS));
                        infoSuivi.StatutDoc = version.EnsureValue<string>(Localization.GetResource(ResourceFieldsKeys.STATUS_DOC, ResourceFiles.FIELDS));
                        infoSuivi.Verification = version.EnsureValue<string>(Localization.GetResource(ResourceFieldsKeys.VERIFICATION, ResourceFiles.FIELDS));

                        if (version.IsCurrentVersion)
                        {
                            infoSuivi.DocName = version.ListItem.File.Name;
                            infoSuivi.DocUrl = Uri.EscapeUriString(version.ListItem.File.ServerRelativeUrl);
                        }
                        else
                        {
                            infoSuivi.DocName = version.FileVersion.File.Name;
                            infoSuivi.DocUrl = Uri.EscapeUriString(web.ServerRelativeUrl + "/" + version.FileVersion.Url);
                        }



                        info.Add(infoSuivi);
                    }
                }
            }
        }


        private void GetAllDocInfo_Deploiement(string codifSystem, ref List<SuiviInfoEntity> info)
        {
            SPWeb web = SPContext.Current.Web;
            string listDepName = Localization.GetResource(ResourceListKeys.DEPLOIEMENT_LISTNAME, ResourceFiles.CORE);
            SPList depList = web.Lists[listDepName];

            IEnumerable<SPListItem> items = CodificationHelper.GetItemsForCodification(depList, codifSystem);
            foreach (SPListItem item in items)
            {
                SuiviInfoEntity infoSuivi = new Model.SuiviInfoEntity();
                infoSuivi.Auteur = item.EnsureValue<string>(SPBuiltInFieldId.Editor).Split(new string[] { ";#" }, StringSplitOptions.RemoveEmptyEntries)[1];
                infoSuivi.DateModif = item.EnsureValue<string>(SPBuiltInFieldId.Modified);
                infoSuivi.DocName = item.File.Name;
                infoSuivi.DocUrl = Uri.EscapeUriString(item.File.ServerRelativeUrl);
                infoSuivi.Indice = item.EnsureValue<string>(Localization.GetResource(ResourceFieldsKeys.INDICE, ResourceFiles.FIELDS));
                infoSuivi.Revision = item.EnsureValue<string>(Localization.GetResource(ResourceFieldsKeys.REVISION, ResourceFiles.FIELDS));
                infoSuivi.StatutDoc = item.EnsureValue<string>(Localization.GetResource(ResourceFieldsKeys.STATUS_DOC, ResourceFiles.FIELDS));
                infoSuivi.Verification = item.EnsureValue<string>(Localization.GetResource(ResourceFieldsKeys.VERIFICATION, ResourceFiles.FIELDS));
                info.Add(infoSuivi);
            }
        }
    }
}
