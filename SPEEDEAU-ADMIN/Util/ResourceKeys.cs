using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPEEDEAU.ADMIN.Util
{

    public class ResourceValidatorKeys
    {
        public const string VALIDATE_REVISION = "ValidateRevision";
        public const string VALIDATE_SUIVI_CONSISTENCY = "ValidateSuiviConsitency";
        public const string VALIDATE_INDICE_1 = "ValidateIndice_1";
        public const string VALIDATE_INDICE_2 = "ValidateIndice_2";
        public const string VALIDATE_REVISION_INCREMENT_1 = "ValidateRevisionIncrement_1";
        public const string VALIDATE_REVISION_INCREMENT_2 = "ValidateRevisionIncrement_2";
        public const string VALIDATE_REVISION_INCREMENT_3 = "ValidateRevisionIncrement_3";
        public const string VALIDATE_FILE_UPLOAD2 = "ValidateFileUpload2";
        public const string VALIDATE_STATUS_DOC = "ValidateStatusDoc";
        public const string VALIDATE_FILE_UPLOAD3_1 = "ValidateFileUpload3_1";
        public const string VALIDATE_FILE_UPLOAD3_2 = "ValidateFileUpload3_2";
        public const string VALIDATE_FILE_UPLOAD4 = "ValidateFileUpload4";
    }

    public class ResourceFieldsKeys
    {
        // native columns
        public const string TITLE = "Title";

        // custom columns
        public const string FILENAME = "FileName";
        public const string CATEGORIE_DOC = "CategorieDoc";
        public const string CODIFICATION = "Codification";
        public const string CODIFICATION_SYSTEM = "CodificationSystem";
        public const string DOCUMENT = "Document";
        public const string INDICE = "Indice";
        public const string OBSERVATION = "Observation";
        public const string REVISION = "Revision";
        public const string STATUS_DOC = "Status";
        public const string FAMILLE_DOC = "Taxon_FamilleDoc";
        public const string NATURE_DOC = "Taxon_NatureDoc";
        public const string PROJECT_TAXON = "Taxon_Projet";
        public const string SITE_TAXON = "Taxon_Site";
        public const string THEME_TAXON = "Taxon_Theme";
        public const string VERIFICATION = "Verification";
        public const string PROCESSIH1600 = "ProcessIH1600";
        public const string ENSEMBLE_COHERENT = "Taxon_EC";
        public const string OPERATION = "Operation";
        public const string REQUIS = "Requis";

        public const string OBSERVATION_RNVO = "Observation_rnvo";
        public const string FOURNITURE = "Fourniture";
        public const string DATECIBLE = "Date_cible";
        public const string FORMAT_DEMANDE = "Format_demande";
        public const string TEMPS_ESTIME = "temps_estime";
        public const string RESTE_A_FAIRE = "reste_a_faire";
        public const string REDACTION = "redaction";
        public const string VERIFICATEUR = "verificateur";
        public const string APPROBATEUR = "approbateur";

        public const string LIVRAISON_DTG = "LIVRAISON_DTG";
        public const string LIVRAISON_EXPLOITANT = "LIVRAISON_EXPLOITANT";
        public const string LIVRAISON_INTEGRATEUR = "LIVRAISON_INTEGRATEUR";
        public const string LIVRAISON_MCO = "LIVRAISON_MCO";
        public const string LIVRAISON_TABLEAUTIER = "LIVRAISON_TABLEAUTIER";

        public const string LIVRAISON_DATE_DTG = "LIVRAISON_DATE_DTG";
        public const string LIVRAISON_DATE_EXPLOITANT = "LIVRAISON_DATE_EXPLOITANT";
        public const string LIVRAISON_DATE_INTEGRATEUR = "LIVRAISON_DATE_INTEGRATEUR";
        public const string LIVRAISON_DATE_MCO = "LIVRAISON_DATE_MCO";
        public const string LIVRAISON_DATE_TABLEAUTIER = "LIVRAISON_DATE_TABLEAUTIER";

        public const string LIVRAISON_FORMAT_DTG = "LIVRAISON_FORMAT_DTG";
        public const string LIVRAISON_FORMAT_EXPLOITANT = "LIVRAISON_FORMAT_EXPLOITANT";
        public const string LIVRAISON_FORMAT_INTEGRATEUR = "LIVRAISON_FORMAT_INTEGRATEUR";
        public const string LIVRAISON_FORMAT_MCO = "LIVRAISON_FORMAT_MCO";
        public const string LIVRAISON_FORMAT_TABLEAUTIER = "LIVRAISON_FORMAT_TABLEAUTIER";

        public const string LIVRAISON_STOCKAGE_DTG = "LIVRAISON_STOCKAGE_DTG";
        public const string LIVRAISON_STOCKAGE_EXPLOITANT = "LIVRAISON_STOCKAGE_EXPLOITANT";
        public const string LIVRAISON_STOCKAGE_INTEGRATEUR = "LIVRAISON_STOCKAGE_INTEGRATEUR";
        public const string LIVRAISON_STOCKAGE_MCO = "LIVRAISON_STOCKAGE_MCO";
        public const string LIVRAISON_STOCKAGE_TABLEAUTIER = "LIVRAISON_STOCKAGE_TABLEAUTIER";

        public const string VSO = "VSO";
        public const string VAO = "VAO";
        public const string VSOSC = "VSOSC";
        public const string VSOSV = "VSOSC";
    }

    public class ResourceListKeys
    {
        public const string DEPLOIEMENT_LISTNAME = "Deploiement_ListName";
        public const string ALERTE_LISTNAME = "Alertes_ListName";
        public const string SUIVI_LISTNAME = "ListeDeSuivi_ListName";
        public const string OBSERVATIONS_DEP_LISTNAME = "Observation_Deploiement_ListName";
        public const string OBSERVATIONS_REF_LISTNAME = "Observation_Referentiel_ListName";
        public const string REFERENTIEL_LISTNAME = "Referentiel_ListName";
        public const string ALERTE_VALIDATION_LISTNAME = "Alertes_Validation_ListName";
    }

    public class ResourceFiles
    {
        public const string CORE = "speedeau.core";
        public const string VALIDATOR = "speedeau.validator";
        public const string FIELDS = "speedeau.fields";
        public const string MMS = "speedeau.mms";
        public const string SPCORE = "core";
        public const string MESSAGE = "speedeau.msg";
    }

    public class ResourceMMS
    {
        public const string MMS_NAME = "mms_name";
        public const string GROUP_NAME = "group_name";
        public const string FAMILLE_DOC = "Famille_Doc";
        public const string NATURE_DOC = "Nature_Doc";
        public const string PROJETS = "Projets";
        public const string THEME = "Theme";
        public const string SITE = "Site";
        public const string EC = "EC";
        public const string OPERATION = "Operation";
        public const string REDACTION = "Redaction";
    }

    public class ResourcePropertyBag
    {
        public const string WEB_PROPERTYBAG_PROJECT = "WebPropertyBag_Project";
        public const string WEB_PROPERTYBAG_DEPLOIEMENT_FAMILLEDOC = "WebPropertyBag_Deploiement_FamilleDoc";
        public const string WEB_PROPERTYBAG_REFERENTIEL_FAMILLEDOC = "WebPropertyBag_Referentiel_FamilleDoc";
        public const string WEB_PROPERTYBAG_ALERTE_EMAIL = "WebPropertyBag_Alerte_Email";
        public const string WEB_PROPERTYBAG_ALERTE_CHECKBOX = "WebPropertyBag_Alerte_CheckBox";
        public const string WEB_PROPERTYBAG_REF_ITEMUPDATED = "WebPropertyBag_Ref_EventReceiver_";
        public const string WEB_PROPERTYBAG_REGEX_CODIFICATION = "WebPropertyBag_Regex_Codification";
        public static string WEB_PROPERTYBAG_SITE_MSH_URL = "WebPropertyBag_Site_Msh_Url";
    }

    public class ResourceMessage
    {
        public const string DOCUMENT_PICKER_NO_FILE_FOUND = "DocumentPicker_NoFileFound";
        public const string DOCUMENT_PICKER_NO_CODIFICATION = "DocumentPicker_NoCodification";
    }
}
