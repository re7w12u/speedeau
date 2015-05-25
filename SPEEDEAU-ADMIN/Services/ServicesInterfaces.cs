using Microsoft.SharePoint;
using SPEEDEAU.ADMIN.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPEEDEAU.ADMIN.Services
{
    public interface IH1600DOC{}

    public interface IPermissionsService
    {
        bool CanUploadToDeploiement();
        bool CanViewMenu();

        bool CanChangeListeDeSuivi();
    }

    public interface IReferentielService : IH1600DOC
    {        
        Referentiel GetReferentielFromRefLibItem(int id, SPWeb web);        
        SPListItem SaveReferentielToList(Referentiel dep, string filename, SPWeb web);
    }

    public interface IDeploiementService : IH1600DOC
    {
        Deploiement GetDeploiementFromDepLibItem(int id, SPWeb web);
        Deploiement GetDeploiementFromSuiviItem(int id, SPWeb web, bool checkForExistingItemInDep);
        SPListItem SaveDeploiementToList(Deploiement dep, string filename, SPWeb web);
        InfoIH1600 GetLatestInfoIH1600ForCodification(SPWeb web, string codification);

    }

    public interface ISuiviService
    {
        Deploiement GetDeploiement(string codificationsystem);
        //bool HasPropertyBag(int suivi_id);
        //bool HasPropertyBag(int suivi_id, SPWeb web);
        SuiviEntity GetDocLinkedInfo(int suivi_id);
        SuiviEntity GetDocLinkedInfo(int suivi_id, SPWeb web);
        SPListItem GetItemForCodificationSystem(string codificationsystem);
        //void SetPropertyBag(SPListItem item, int suivi_id, SPWeb web);
        //void SetPropertyBag(SPListItem item, int suivi_id);

        string CodificationSystem(int suivi_id);

        Suivi GetSuiviByID(int suivi_id, SPWeb web);

        SPListItem UpdateSuivi(Suivi suivi, SPWeb web);
        SPListItem NewSuivi(Suivi suivi, SPWeb web);

    }

    public interface IObservationService
    {
        List<Observation> GetObservationsForDocID(int doc_id);
        List<Observation> GetObservationsForSuiviID(int suivi_id);
        bool HasObservationForDocID(int doc_id);
        bool HasObservationForSuiviID(int suivi_id);
        bool HasValidObservationsForDocID(int doc_id);
        bool HasValidObservationsForSuiviID(int suivi_id);
    }

    public interface IWebProperties
    {
        void Set(string key, string value, SPWeb targetWeb);
        void Set(string key, string value);
        string Get(string key, SPWeb targetWeb);
        string Get(string key);
    }

    public interface IAlerteService
    {
        bool NotifyAuthor(string webUrl, string listname, int itemID, string newStatus);
        bool NotifySiteMembers(string webUrl, string listname, int itemID);
    }
}
