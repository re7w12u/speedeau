using System;
using System.Linq;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.Practices.SharePoint.Common.ServiceLocation;
using SPEEDEAU.ADMIN.Services;
using SPEEDEAU.ADMIN;
using SPEEDEAU.ADMIN.Util;
using Microsoft.SharePoint.Taxonomy;
using SPEEDEAU.ADMIN.Model;

namespace SPEEDEAU.Layouts.SPEEDEAU
{

    enum SuiviEditMode
    {
        NEW,
        EDIT,
        DUPLICATE
    }

    public partial class SuiviEdit : LayoutsPageBase
    {
        public const string MODE = "mode";
        public const string SID = "SID";
        private Suivi suiviItem { get; set; }
        private SuiviEditMode currentMode;
        private string currentID;


        #region event handlers
        protected void Page_Load(object sender, EventArgs e)
        {
            SetUpValidators();
            SetUpTaxonmyFields();

            #region Get Request
            // checking request mode (new, edit or duplicate) get ID and then display form accordingly
            if (!IsPostBack && !String.IsNullOrWhiteSpace(this.Request.QueryString[MODE]) && !String.IsNullOrWhiteSpace(this.Request.QueryString[SID]))
            {
                // find out mode parameter
                string mode = this.Request.QueryString.Get(MODE);
                Enum.TryParse<SuiviEditMode>(mode, true, out currentMode);
                CurrentModeHiddenField.Value = currentMode.ToString();

                // need to get data from selected item identified by SID parameter
                currentID = this.Request.QueryString.Get(SID);
                ISuiviService suiviSVC = SharePointServiceLocator.GetCurrent().GetInstance<ISuiviService>();
                suiviItem = suiviSVC.GetSuiviByID(Convert.ToInt32(currentID), SPContext.Current.Web);

                FillInFormFields(suiviItem);
            }
            #endregion

            #region PostBack
            else if (IsPostBack && !String.IsNullOrWhiteSpace(CurrentModeHiddenField.Value))
            {
                // get current mode (new, edit or duplicate)
                Enum.TryParse<SuiviEditMode>(CurrentModeHiddenField.Value, out currentMode);

                #region fill in with submitted data

                TaxonomyValueBuilder taxBuilder = new TaxonomyValueBuilder();

                suiviItem = new Suivi();

                // if edit mode, we need the ID to get to know which item to update
                if (currentMode == SuiviEditMode.EDIT) suiviItem.ID = Convert.ToInt32(IDHiddenField.Value);

                suiviItem.Operation = OperationTextBox.Text;
                suiviItem.FamilleDoc = taxBuilder.Build(TaxonomyFamilleDoc);
                suiviItem.Title = Titre.Text;
                suiviItem.NatureDoc = taxBuilder.Build(TaxonomyNatureDoc);
                suiviItem.Requis = RequisCheckBox.Checked;
                suiviItem.Codification = Codification.Text;
                suiviItem.CodificationSystem = CodificationHelper.CleanUpCodification(Codification.Text); ;
                suiviItem.Indice = Indice.Text;
                suiviItem.ObservationsRNVO = ObservationsTextBox.Text;
                suiviItem.Fourniture = FournitureTextBox.Text;
                suiviItem.DateCible = DateCibleTextBox.Text.EnsureValue<DateTime>();
                suiviItem.FormatDemande = FormatTextBox.Text;
                suiviItem.TempsEstime = TempsEstimeTextBox.Text.EnsureValue<int>();
                suiviItem.ResteAFaire = ResteAFaireTextBox.Text.EnsureValue<int>();
                suiviItem.Redaction = taxBuilder.Build(TaxonomyRedaction);
                suiviItem.Verificateur = VerificateurPicker.ResolvedEntities.Cast<PickerEntity>().ToList();
                suiviItem.Approbateur = ApprobateurPicker.ResolvedEntities.Cast<PickerEntity>().ToList();
                suiviItem.Projet = taxBuilder.Build(ProjectHiddenField.Value);
                suiviItem.Site = taxBuilder.Build(SiteHiddenField.Value);

                suiviItem.Livraison_dtg = DTGCheckBox.Checked;
                suiviItem.Livraison_date_dtg = SetLivraisonValue(DTGCheckBox.Checked, DTGDateTextBox.Text);
                suiviItem.Livraison_format_dtg = SetLivraisonValue(DTGCheckBox.Checked, DTGFormatTextBox.Text);
                suiviItem.Livraison_stockage_dtg = SetLivraisonValue(DTGCheckBox.Checked, DTGStcokageTextBox.Text);

                suiviItem.Livraison_exploitant = ExploitantCheckBox.Checked;
                suiviItem.Livraison_date_exploitant = SetLivraisonValue(ExploitantCheckBox.Checked, ExploitantDateTextBox.Text);
                suiviItem.Livraison_format_exploitant = SetLivraisonValue(ExploitantCheckBox.Checked, ExploitantFormatTextBox.Text);
                suiviItem.Livraison_stockage_exploitant = SetLivraisonValue(ExploitantCheckBox.Checked, ExploitantStockageTextBox.Text);

                suiviItem.Livraison_integrateur = IntegrateurCheckBox.Checked;
                suiviItem.Livraison_date_integrateur = SetLivraisonValue(IntegrateurCheckBox.Checked, IntegrateurDateTextBox.Text);
                suiviItem.Livraison_format_integrateur = SetLivraisonValue(IntegrateurCheckBox.Checked, IntegrateurFormatTextBox.Text);
                suiviItem.Livraison_stockage_integrateur = SetLivraisonValue(IntegrateurCheckBox.Checked, IntegrateurStockageTextBox.Text);

                suiviItem.Livraison_mco = MCOCheckBox.Checked;
                suiviItem.Livraison_date_mco = SetLivraisonValue(MCOCheckBox.Checked, MCODateTextBox.Text);
                suiviItem.Livraison_format_mco = SetLivraisonValue(MCOCheckBox.Checked, MCOFormatTextBox.Text);
                suiviItem.Livraison_stockage_mco = SetLivraisonValue(MCOCheckBox.Checked, MCOStockageTextBox.Text);

                suiviItem.Livraison_tableautier = TableautierCheckBox.Checked;
                suiviItem.Livraison_date_tableautier = SetLivraisonValue(TableautierCheckBox.Checked, TableautierDateTextBox.Text);
                suiviItem.Livraison_format_tableautier = SetLivraisonValue(TableautierCheckBox.Checked, TableautierFormatTextBox.Text);
                suiviItem.Livraison_stockage_tableautier = SetLivraisonValue(TableautierCheckBox.Checked, TableautierStockageTextBox.Text);


                #endregion
            }
            #endregion

        }



        protected void SaveBtn_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                ISuiviService suiviSVC = SharePointServiceLocator.GetCurrent().GetInstance<ISuiviService>();
                if (currentMode == SuiviEditMode.EDIT) suiviSVC.UpdateSuivi(suiviItem, SPContext.Current.Web);
                else if (currentMode == SuiviEditMode.DUPLICATE || currentMode == SuiviEditMode.NEW) suiviSVC.NewSuivi(suiviItem, SPContext.Current.Web);

                SetCloseDialog();
            }
        }

        #endregion

        #region private


        private string SetLivraisonValue(bool isChecked, string value)
        {
            return isChecked ? value : String.Empty;
        }

        /// <summary>
        /// insert javascript to close dialog window
        /// </summary>
        private void SetCloseDialog()
        {
            //Page.Request.Params.Get("__EVENTTARGET");
            string script = "\n<script type=\"text/javascript\" language=\"Javascript\" id=\"EventScriptBlock\">\n";
            script += "SP.SOD.executeOrDelayUntilScriptLoaded(function () { SP.UI.ModalDialog.commonModalDialogClose(SP.UI.DialogResult.OK, '1'); }, 'SP.js');";
            script += "\n\n </script>";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "DeploiementcloseDialog", script, false);
        }


        /// <summary>
        /// set fields value using Suivi object based on currentMode
        /// </summary>
        /// <param name="suiviItem"></param>
        private void FillInFormFields(Suivi suiviItem)
        {
            if (suiviItem != null)
            {
                // common for all modes
                OperationTextBox.Text = suiviItem.Operation;

                if (currentMode == SuiviEditMode.NEW)
                {
                    Titre.Text = String.Empty;
                }
                else if (currentMode == SuiviEditMode.EDIT)
                {
                    Titre.Text = suiviItem.Title;
                    Codification.Text = suiviItem.Codification;
                }
                else if (currentMode == SuiviEditMode.DUPLICATE)
                {
                    Titre.Text = String.Empty;
                    Codification.Text = String.Empty;
                }

                // other fields remains blank for NEW
                if (currentMode == SuiviEditMode.DUPLICATE || currentMode == SuiviEditMode.EDIT)
                {
                    IDHiddenField.Value = suiviItem.ID.ToString();
                    TaxonomyFamilleDoc.Text = suiviItem.FamilleDoc.EnsureValue();

                    TaxonomyNatureDoc.Text = suiviItem.NatureDoc.EnsureValue();
                    RequisCheckBox.Checked = suiviItem.Requis;
                    Indice.Text = suiviItem.Indice;
                    ObservationsTextBox.Text = suiviItem.ObservationsRNVO;
                    FournitureTextBox.Text = suiviItem.Fourniture;
                    DateCibleTextBox.Text = suiviItem.DateCible.ToShortDateString();
                    FormatTextBox.Text = suiviItem.FormatDemande;
                    TempsEstimeTextBox.Text = suiviItem.TempsEstime.ToString();
                    ResteAFaireTextBox.Text = suiviItem.ResteAFaire.ToString();
                    TaxonomyRedaction.Text = suiviItem.Redaction.EnsureValue();
                    VerificateurPicker.AddEntities(suiviItem.Verificateur);
                    ApprobateurPicker.AddEntities(suiviItem.Approbateur);
                    ProjectHiddenField.Value = suiviItem.Projet.ToFullString();
                    SiteHiddenField.Value = suiviItem.Site.ToFullString();

                    DTGCheckBox.Checked = suiviItem.Livraison_dtg;
                    DTGDateTextBox.Text = suiviItem.Livraison_date_dtg;
                    DTGFormatTextBox.Text = suiviItem.Livraison_format_dtg;
                    DTGStcokageTextBox.Text = suiviItem.Livraison_stockage_dtg;

                    ExploitantCheckBox.Checked = suiviItem.Livraison_exploitant;
                    ExploitantDateTextBox.Text = suiviItem.Livraison_date_exploitant;
                    ExploitantFormatTextBox.Text = suiviItem.Livraison_format_exploitant;
                    ExploitantStockageTextBox.Text = suiviItem.Livraison_stockage_exploitant;

                    IntegrateurCheckBox.Checked = suiviItem.Livraison_integrateur;
                    IntegrateurDateTextBox.Text = suiviItem.Livraison_date_integrateur;
                    IntegrateurFormatTextBox.Text = suiviItem.Livraison_format_integrateur;
                    IntegrateurStockageTextBox.Text = suiviItem.Livraison_stockage_integrateur;

                    MCOCheckBox.Checked = suiviItem.Livraison_mco;
                    MCODateTextBox.Text = suiviItem.Livraison_date_mco;
                    MCOFormatTextBox.Text = suiviItem.Livraison_format_mco;
                    MCOStockageTextBox.Text = suiviItem.Livraison_stockage_mco;

                    TableautierCheckBox.Checked = suiviItem.Livraison_tableautier;
                    TableautierDateTextBox.Text = suiviItem.Livraison_date_tableautier;
                    TableautierFormatTextBox.Text = suiviItem.Livraison_format_tableautier;
                    TableautierStockageTextBox.Text = suiviItem.Livraison_stockage_tableautier;
                }
            }
        }

        /// <summary>
        /// set parameters for validators
        /// </summary>
        private void SetUpValidators()
        {
            IWebProperties pBag = SharePointServiceLocator.GetCurrent().GetInstance<IWebProperties>();
            string regexPropName = Localization.GetResource(ResourcePropertyBag.WEB_PROPERTYBAG_REGEX_CODIFICATION, ResourceFiles.CORE);
            string regex = pBag.Get(regexPropName);
            ValidateCodification.ValidationExpression = regex;
        }


        /// <summary>
        /// since taxonomy fields are not bound to list, they are not bound to the managed metadata store either
        /// SO for each control we wire to the correct termset
        /// TODO : use configuration file
        /// </summary>
        private void SetUpTaxonmyFields()
        {
            string metadataService = Localization.GetResource(ResourceMMS.MMS_NAME, ResourceFiles.MMS);
            string group = Localization.GetResource(ResourceMMS.GROUP_NAME, ResourceFiles.MMS);
            TaxonomySession taxonomySession = new TaxonomySession(SPContext.Current.Site);
            TermStore termStore = taxonomySession.TermStores[metadataService];

            string familleDocSet = Localization.GetResource(ResourceMMS.FAMILLE_DOC, ResourceFiles.MMS);
            Guid SiteId = taxonomySession.TermStores[metadataService].Groups[group].TermSets[familleDocSet].Id;
            TaxonomyFamilleDoc.SspId.Add(termStore.Id);
            TaxonomyFamilleDoc.SSPList = termStore.Id.ToString();
            TaxonomyFamilleDoc.TermSetId.Add(SiteId);
            TaxonomyFamilleDoc.TermSetList = SiteId.ToString();

            string natureTermSet = Localization.GetResource(ResourceMMS.NATURE_DOC, ResourceFiles.MMS);
            Guid NatureId = taxonomySession.TermStores[metadataService].Groups[group].TermSets[natureTermSet].Id;
            TaxonomyNatureDoc.SspId.Add(termStore.Id);
            TaxonomyNatureDoc.SSPList = termStore.Id.ToString();
            TaxonomyNatureDoc.TermSetId.Add(NatureId);
            TaxonomyNatureDoc.TermSetList = NatureId.ToString();

            string redactionTermSet = Localization.GetResource(ResourceMMS.REDACTION, ResourceFiles.MMS);
            Guid redactionId = taxonomySession.TermStores[metadataService].Groups[group].TermSets[redactionTermSet].Id;
            TaxonomyRedaction.SspId.Add(termStore.Id);
            TaxonomyRedaction.SSPList = termStore.Id.ToString();
            TaxonomyRedaction.TermSetId.Add(redactionId);
            TaxonomyRedaction.TermSetList = redactionId.ToString();
        }

        /// <summary>
        /// check if taxonomy field has been set with at least one value
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private bool CheckTaxonomyValue(TaxonomyWebTaggingControl taxField)
        {
            return taxField.GetValidatedTerms().Count() > 0;
        }

        #endregion

        #region validators
        /// <summary>
        /// check that Famille Doc has a value
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void TaxonomyFamilleDocValidator_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            args.IsValid = CheckTaxonomyValue(TaxonomyFamilleDoc);
        }

        /// <summary>
        /// check that Nature Doc has a value
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void TaxonomyNatureDocValidator_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            args.IsValid = CheckTaxonomyValue(TaxonomyNatureDoc);
        }

        /// <summary>
        /// check that Redaction has a value
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void TaxonomyRedactionValidator_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            args.IsValid = CheckTaxonomyValue(TaxonomyRedaction);
        }

        /// <summary>
        /// business rules says that document of type 'référentiel' must have and 'Indice'
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void ValidateIncide2_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            IWebProperties props = SharePointServiceLocator.GetCurrent().GetInstance<IWebProperties>();
            string refPropName = Localization.GetResource(ResourcePropertyBag.WEB_PROPERTYBAG_REFERENTIEL_FAMILLEDOC, ResourceFiles.CORE);
            string refLabel = props.Get(refPropName);

            var q = from t in TaxonomyFamilleDoc.GetValidatedTerms()
                    from l in t.Labels
                    where l.Value == refLabel
                    select t;

            if (q.Any())
            {
                // looks like we're dealing with a référentiel, so we need to check if indice is set
                args.IsValid = !String.IsNullOrWhiteSpace(Indice.Text);
            }

        }

        /// <summary>
        /// check that codification does not already exists
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void ValidateionCodification3_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            string listName = Localization.GetResource(ResourceListKeys.SUIVI_LISTNAME, ResourceFiles.CORE);
            SPList suiviList = SPContext.Current.Web.Lists[listName];

            int match = CodificationHelper.GetItemsForCodification(suiviList, CodificationHelper.CleanUpCodification(Codification.Text)).Count();
            // duplicate and new mode are both creating new items, so we need a non existing codification
            if ((currentMode == SuiviEditMode.NEW || currentMode == SuiviEditMode.DUPLICATE) && match > 0) args.IsValid = false;
            // edit mode allow to create new codification or use an existing one
            if (currentMode == SuiviEditMode.EDIT && match > 1 ) args.IsValid = false;                
        }

        /// <summary>
        /// make sure the date is correct and is in the range 2010 < X < 2050
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void DateCibleValidator_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            DateTime minDate = DateTime.Parse("2010/01/01");
            DateTime maxDate = DateTime.Parse("2050/01/01");
            DateTime inputDate;

            args.IsValid = DateTime.TryParse(DateCibleTextBox.Text, out inputDate) && minDate <= inputDate && inputDate <= maxDate;
        }


        #endregion

        


    }

}
