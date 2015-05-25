using Microsoft.Practices.SharePoint.Common.ServiceLocation;
using Microsoft.SharePoint;
using SPEEDEAU.ADMIN.Model;
using SPEEDEAU.ADMIN.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPEEDEAU.ADMIN.Services
{
    public class ObservationsService : ServiceBase, IObservationService
    {
        private string ObsListName;
        private string DepListName;
        private string IndiceFieldName;
        private string RevisionFieldName;

        public ObservationsService()
        {
            ObsListName = Localization.GetResource(ResourceListKeys.OBSERVATIONS_DEP_LISTNAME, ResourceFiles.CORE);
            DepListName = Localization.GetResource(ResourceListKeys.DEPLOIEMENT_LISTNAME, ResourceFiles.CORE);
            IndiceFieldName = Localization.GetResource(ResourceFieldsKeys.INDICE, ResourceFiles.FIELDS);
            RevisionFieldName = Localization.GetResource(ResourceFieldsKeys.REVISION, ResourceFiles.FIELDS);
        }

        public List<Observation> GetObservationsForDocID(int doc_id)
        {
            List<Observation> result = new List<Observation>();

            SPWeb web = SPContext.Current.Web;
            SPList depList = web.Lists.TryGetList(DepListName);
            SPListItem dep = depList.GetItemById(doc_id);

            string title = dep.EnsureValue<string>(SPBuiltInFieldId.Title);
            string indice = dep.EnsureValue<string>(IndiceFieldName);
            string revision = dep.EnsureValue<string>(RevisionFieldName);

            if (!String.IsNullOrWhiteSpace(title) || !String.IsNullOrWhiteSpace(indice) || !String.IsNullOrWhiteSpace(revision))
            {                
                SPListItemCollection items = GetObservations(web, title, indice, revision);
                if (items.Count > 0)
                {
                    ObservationBuilder builder = new ObservationBuilder();
                    foreach (SPListItem item in items)
                    {
                        result.Add(builder.Build(item));
                    }
                }
            }
            return result;
        }

        public List<Observation> GetObservationsForSuiviID(int suivi_id)
        {            
            ISuiviService suiviService = SharePointServiceLocator.GetCurrent().GetInstance<ISuiviService>();
            SuiviEntity suivi = suiviService.GetDocLinkedInfo(suivi_id, SPContext.Current.Web);
            return GetObservationsForDocID(suivi.DocID);
        }

        public bool HasObservationForDocID(int doc_id)
        {
            SPWeb web = SPContext.Current.Web;
            SPList depList = web.Lists.TryGetList(DepListName);
            SPListItem dep = depList.GetItemById(doc_id);

            string title = dep.EnsureValue<string>(SPBuiltInFieldId.Title);
            string indice = dep.EnsureValue<string>(IndiceFieldName);
            string revision = dep.EnsureValue<string>(RevisionFieldName);

            if (String.IsNullOrWhiteSpace(title) || String.IsNullOrWhiteSpace(indice) || String.IsNullOrWhiteSpace(revision))
            {
                return false;
            }
            else
            {
                SPListItemCollection items = GetObservations(web, title, indice, revision);
                return items.Count > 0;                
            }     
        }

        public bool HasObservationForSuiviID(int suivi_id)
        {
            ISuiviService suiviService = SharePointServiceLocator.GetCurrent().GetInstance<ISuiviService>();
            SuiviEntity suivi = suiviService.GetDocLinkedInfo(suivi_id, SPContext.Current.Web);
            return HasObservationForDocID(suivi.DocID);
        }

        public bool HasValidObservationsForDocID(int doc_id)
        {
            List<Observation> obs = GetObservationsForDocID(doc_id);
            return obs.Any(o => o.Validated);
        }

        public bool HasValidObservationsForSuiviID(int suivi_id)
        {
            List<Observation> obs = GetObservationsForSuiviID(suivi_id);
            return obs.Any(o => o.Validated);
        }

        #region private methods
        /// <summary>
        ///  get Observations items
        /// </summary>
        /// <param name="web"></param>
        /// <param name="title"></param>
        /// <param name="indice"></param>
        /// <param name="revision"></param>
        /// <returns></returns>
        private SPListItemCollection GetObservations(SPWeb web, string title, string indice, string revision)
        {
            SPList obsList = web.Lists.TryGetList(ObsListName);
            SPQuery q = new SPQuery();
            q.Query = String.Format(@"   <Where>
                              <And>
                                 <And>
                                    <Eq>
                                       <FieldRef Name='Title' />
                                       <Value Type='Text'>{0}</Value>
                                    </Eq>
                                    <Eq>
                                       <FieldRef Name='EDFVersion' />
                                       <Value Type='Text'>{1}</Value>
                                    </Eq>
                                 </And>
                                 <Eq>
                                    <FieldRef Name='EDFRevision' />
                                    <Value Type='Text'>{2}</Value>
                                 </Eq>
                              </And>
                           </Where>", title, indice, revision); 
            SPListItemCollection coll = obsList.GetItems(q);
            return coll;
        }
        #endregion
    }
}
