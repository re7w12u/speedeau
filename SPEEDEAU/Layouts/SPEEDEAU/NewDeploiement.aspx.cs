using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Taxonomy;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.SharePoint.Common.ServiceLocation;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Text;

using SPEEDEAU.ADMIN;
using SPEEDEAU.ADMIN.Services;
using SPEEDEAU.ADMIN.Util;
using SPEEDEAU.ADMIN.Model;
using System.IO;

namespace SPEEDEAU.Layouts
{
    public partial class NewDeploiement : LayoutsPageBase
    {
        private const string DEP_VIEWSTATE_KEY = "DepViewStateObject";
        private const string FILEUPLOAD_VIEWSTATE_KEY = "DepFileuploadPathValue";
        private Deploiement DepNew { get; set; }
        private Deploiement DepOld { get; set; }
        private bool UpdateMode { get; set; }
        private bool ForceUpload { get; set; }

        #region events handler
        protected void Page_Load(object sender, EventArgs e)
        {
            SetUpValidators();
            SetUpTaxonmyFields();
            SetWebWideProperties();
            //HandleFileCtrl();

            #region Fill in form if ID is provided
            if (!IsPostBack && !String.IsNullOrWhiteSpace(this.Request.QueryString["ID"]))
            {
                // we are updating an existing file here, so need to check file consistency
                // meaning we get the data to fill in the form from the ID
                // but when saving process is based on the file...
                // this mean that if you select (in the FileUpload control) a file that already exists but match another ID
                // you will end up updating that item...
                NewDocumentHiddenField.Value = false.ToString();
                UpdateOnlyHiddenField.Value = false.ToString();
                int id = Convert.ToInt32(this.Request.QueryString.Get("ID"));
                IDeploiementService depSvc = SharePointServiceLocator.GetCurrent().GetInstance<IDeploiementService>();
                Deploiement dep = depSvc.GetDeploiementFromDepLibItem(id, SPContext.Current.Web);
                FillInFormAllFields(dep);
            }
            #endregion

            #region fill in from liste de suivi
            if (!IsPostBack && !String.IsNullOrWhiteSpace(this.Request.QueryString["FID"]))
            {
                // get info from 'liste de suivi' item property bag.
                // no info means it's a new file
                NewDocumentHiddenField.Value = true.ToString();
                int id = Convert.ToInt32(this.Request.QueryString.Get("FID"));
                FIDHiddenField.Value = id.ToString();
                UpdateOnlyHiddenField.Value = false.ToString();
                // first upload for this 'suivi' entry. Get data from 'list de suivi' and fill in form 
                IDeploiementService depSvc = SharePointServiceLocator.GetCurrent().GetInstance<IDeploiementService>();
                Deploiement dep = depSvc.GetDeploiementFromSuiviItem(id, SPContext.Current.Web, false);
                FillInFormAllFields(dep);
            }
            #endregion

            #region set codification from url parameter
            if (!IsPostBack && !String.IsNullOrWhiteSpace(this.Request.QueryString["codification"]))
            {
                string codification = this.Request.QueryString["codification"];
                Codification.Text = codification;
                UpdateOnlyHiddenField.Value = false.ToString();
            }
            #endregion

            #region update metadata but no upload
            if (!IsPostBack && !String.IsNullOrWhiteSpace(this.Request.QueryString["UID"]))
            {
                SetUpdateMode();
                int id = Convert.ToInt32(this.Request.QueryString.Get("UID"));
                IDeploiementService depSvc = SharePointServiceLocator.GetCurrent().GetInstance<IDeploiementService>();
                Deploiement dep = depSvc.GetDeploiementFromDepLibItem(id, SPContext.Current.Web);
                FillInFormAllFields(dep);
            }
            #endregion

            #region set as New Document Mode (and not new version)
            if (!IsPostBack && !String.IsNullOrWhiteSpace(this.Request.QueryString["blank"]))
            {
                NewDocumentHiddenField.Value = true.ToString();
            }
            #endregion

            #region fill in from field value on post back
            if (IsPostBack)
            {
                #region fill in from ViewState

                if (ViewState[DEP_VIEWSTATE_KEY] != null && ViewState[DEP_VIEWSTATE_KEY] is Deploiement)
                {
                    DepOld = ViewState[DEP_VIEWSTATE_KEY] as Deploiement;
                }

                #endregion

                UpdateMode = String.IsNullOrWhiteSpace(UpdateOnlyHiddenField.Value) ? false : Convert.ToBoolean(UpdateOnlyHiddenField.Value);

                DepNew = new Deploiement();

                if (UpdateMode) SetUpdateMode();
                else
                {
                    DepNew.File = FileUpload.FileBytes;
                    DepNew.FileName = FileUpload.FileName;
                }

                DepNew.Title = Titre.Text;
                DepNew.ProcessIH1600 = GestionIH1600CheckBox.Checked;

                if (GestionIH1600CheckBox.Checked)
                {
                    DepNew.Codification = Codification.Text;
                    DepNew.CodificationSystem = Codification.Text;
                    DepNew.Status = EnumHelper.GetValue<StatusIH1600>(StatusDoc.SelectedValue, StatusIH1600.NONE);
                    DepNew.Indice = Indice.Text.ToUpper();
                    DepNew.Revision = DepNew.Status == StatusIH1600.BPE ? String.Empty : Revision.Text;
                    DepNew.ValidationStatus = ValidationStatus.None;
                }

                TaxonomyValueBuilder builder = new TaxonomyValueBuilder();
                DepNew.NatureDoc = builder.Build(TaxonomyNatureDoc);
                DepNew.FamilleDoc = builder.Build(TaxonomyFamilleDoc);
                DepNew.Projet = builder.Build(TaxonomyProjet);
                DepNew.Theme = builder.Build(TaxonomyTheme);
                DepNew.Site = builder.Build(TaxonomySiteDPIH);

            }
            #endregion
        }



        protected void CancelBtn_Click(object sender, EventArgs e)
        {
            Server.TransferRequest(Page.Request.UrlReferrer.AbsolutePath);
        }

        /// <summary>
        /// triggered when the user click on the save button.
        /// It create a new Deploiement entity and saves it to the list using IDeploiementService
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SaveBtn_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                IDeploiementService depSvc = SharePointServiceLocator.GetCurrent().GetInstance<IDeploiementService>();
                SPListItem item = depSvc.SaveDeploiementToList(DepNew, DepOld == null ? String.Empty : DepOld.FileName, SPContext.Current.Web);
                SetCloseDialog();

            }
        }

        /// <summary>
        /// reload field with data from liste de suivi
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ReloadSuiviData_Click(object sender, EventArgs e)
        {
            ReloadFormTable.Visible = false;

            string code = CodificationHelper.CleanUpCodification(Codification.Text);

            // get dep from suivi
            ISuiviService suiviService = SharePointServiceLocator.GetCurrent().GetInstance<ISuiviService>();
            Deploiement suiviData = suiviService.GetDeploiement(code);

            if (suiviData == null)
            {
                //.ErrorMessage = "Votre codification ne correspond à aucune entrée dans la liste de suivi.";
                return;
            }

            suiviData.FileName = DepOld.FileName;
            suiviData.File = DepOld.File;

            FillInFormCoreFields(suiviData);
        }

        protected void ForceUpload_Click(object sender, EventArgs e)
        {
            ForceUpload = true;
            SameFileNameFormTable.Visible = false;
            Validate();
            SaveBtn_Click(sender, e);
        }
        #endregion

        #region private methods
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
        /// use session to keep fileupload control with value
        /// </summary>
        private void HandleFileCtrl()
        {
            const string sessionKey = "SPDO_FileUpload";
            //If first time page is submitted and we have file in FileUpload control but not in session 
            if (Session[sessionKey] == null && FileUpload.HasFile) Session[sessionKey] = FileUpload;
            // if postback occurs, use Session to put back FileUpload value
            else if (Session[sessionKey] != null && !FileUpload.HasFile) FileUpload = Session[sessionKey] as FileUpload;
            // Now there could be another situation when Session has File but user want to change the file 
            // In this case we have to change the file in session object 
            else if (FileUpload.HasFile) Session[sessionKey] = FileUpload;
        }

        /// <summary>
        /// hide file upload control and disabled attached validators
        /// </summary>
        private void SetUpdateMode()
        {
            UpdateOnlyHiddenField.Value = true.ToString();
            UpdateMode = true;
            ValidateFileUpload.Enabled = false;
            ValidateFileUpload2.Enabled = false;
            FormTable.Controls.Remove(FileUploadRow);
        }

        /// <summary>
        /// fill in matching form core fields with value from Deploiement
        /// core fields means 
        /// </summary>
        /// <param name="id">id of the SPListItem in Deploiement doc library</param>
        private void FillInFormCoreFields(Deploiement dep)
        {
            try
            {
                // add to viewstate to get it back when submitting
                ViewState.Add(DEP_VIEWSTATE_KEY, dep);

                Titre.Text = dep.Title;
                Codification.Text = dep.Codification;
                TaxonomyFamilleDoc.Text = dep.FamilleDoc.EnsureValue();
                TaxonomyNatureDoc.Text = dep.NatureDoc.EnsureValue();
                TaxonomyTheme.Text = dep.Theme.EnsureValue();
                TaxonomySiteDPIH.Text = dep.Site.EnsureValue();

                string projet = dep.Projet.EnsureValue();
                if (!String.IsNullOrWhiteSpace(projet)) TaxonomyProjet.Text = projet;

                //ValidationStatusHiddenField.Value = Enum.GetName(typeof(ValidationStatus), dep.ValidationStatus);
            }
            catch (Exception err)
            {
                LoggerManager.Error(LoggerCategory.Deploiement, "Error while fetching data for Deploiement form: ID={0}", this.Request.QueryString["ID"]);
                if (err is OverflowException || err is OverflowException) LoggerManager.Error(LoggerCategory.Deploiement, "{0}", "Probably occured while converting ID to int");
                LoggerManager.Error(LoggerCategory.Deploiement, err);
            }
        }

        /// <summary>
        /// fill in matching form extra fields with value from Deploiement
        /// Extra field means all field that are not bind to liste de suivi and that 
        /// do not need to be consistent with data in the liste de suivi
        /// </summary>
        /// <param name="id">id of the SPListItem in Deploiement doc library</param>
        private void FillInFormAllFields(Deploiement dep)
        {
            if (dep != null)
            {
                try
                {
                    FillInFormCoreFields(dep);

                    if (dep.ProcessIH1600)
                    {
                        if (UpdateMode)
                        {
                            // in update mode - set values as saved for current version. 
                            ListItem li = StatusDoc.Items.Cast<ListItem>().FirstOrDefault(l => dep.Status == (StatusIH1600)Enum.Parse(typeof(StatusIH1600), l.Value, true));
                            if (li != null) li.Selected = true;
                            Indice.Text = dep.Indice;
                            Revision.Text = dep.Revision;
                        }
                        else
                        {
                            // in upload mode we expect a new version - set values as for new version
                            #region set Status Documentaire (BPE, PREL)
                            ListItem li = null;
                            if (dep.Status == StatusIH1600.BPE)
                            {
                                // if current status if BPE, next expected status should be PREL
                                li = StatusDoc.Items.Cast<ListItem>().FirstOrDefault(l => StatusIH1600.PREL == (StatusIH1600)Enum.Parse(typeof(StatusIH1600), l.Value, true));
                            }
                            else if (dep.ValidationStatus == ValidationStatus.VSO || dep.ValidationStatus == ValidationStatus.VSOSC || dep.ValidationStatus == ValidationStatus.VSOSV)
                            {
                                // if current validation is VSO, VSOSV, VSOSC, next expected status should be BPE
                                li = StatusDoc.Items.Cast<ListItem>().FirstOrDefault(l => StatusIH1600.BPE == (StatusIH1600)Enum.Parse(typeof(StatusIH1600), l.Value, true));
                            }
                            else
                            {
                                li = StatusDoc.Items.Cast<ListItem>().FirstOrDefault(l => StatusIH1600.PREL == (StatusIH1600)Enum.Parse(typeof(StatusIH1600), l.Value, true));
                            }
                            if (li != null) li.Selected = true;
                            #endregion

                            #region set Indice

                            InfoIH1600 info = null;
                            if (!String.IsNullOrWhiteSpace(dep.Codification))
                            {
                                // check is other document with same codification exists
                                IDeploiementService depSvc = SharePointServiceLocator.GetCurrent().GetInstance<IDeploiementService>();
                                info = depSvc.GetLatestInfoIH1600ForCodification(SPContext.Current.Web, dep.CodificationSystem);
                            }

                            if (info != null)
                            {
                                if (!String.IsNullOrWhiteSpace(info.Indice))
                                {
                                    if (info.Status == StatusIH1600.BPE) Indice.Text = ((char)((int)Char.Parse(info.Indice) + 1)).ToString().ToUpper();
                                    else Indice.Text = info.Indice.ToUpper();
                                }
                                else Indice.Text = "A";
                            }
                            else Indice.Text = "A";
                            //else if (dep.Status == StatusIH1600.BPE) Indice.Text = ((char)((int)Char.Parse(dep.Indice) + 1)).ToString().ToUpper();
                            //else Indice.Text = dep.Indice.ToUpper();

                            #endregion

                            #region set Revision
                            if (info != null)
                            {
                                if (String.IsNullOrWhiteSpace(info.Revision) || info.Status == StatusIH1600.BPE)
                                {
                                    // it's a new version, either first uplaod ever or new version after BPE, so starting from 0
                                    Revision.Text = "0";
                                }
                                else if (info.ValidationStatus == ValidationStatus.VSO || info.ValidationStatus == ValidationStatus.VSOSV || info.ValidationStatus == ValidationStatus.VSOSC)
                                {
                                    // it's a BPE Version, so only letter. Revision is blank
                                    Revision.Text = String.Empty;
                                }
                                else
                                {
                                    // it's a regular new PREL version, just increment current revision
                                    Revision.Text = (Convert.ToInt32(info.Revision) + 1).ToString();
                                }
                            }
                            else Revision.Text = "0";
                            //else if (String.IsNullOrWhiteSpace(dep.Revision) || dep.Status == StatusIH1600.BPE)
                            //{
                            //    // it's a new version, either first uplaod ever or new version after BPE, so starting from 0
                            //    Revision.Text = "0";
                            //}
                            //else if (dep.ValidationStatus == ValidationStatus.VSO || dep.ValidationStatus == ValidationStatus.VSOSV || dep.ValidationStatus == ValidationStatus.VSOSC)
                            //{
                            //    // it's a BPE Version, so only letter. Revision is blank
                            //    Revision.Text = String.Empty;
                            //}
                            //else
                            //{
                            //    // it's a regular new PREL version, just increment current revision
                            //    Revision.Text = (Convert.ToInt32(dep.Revision) + 1).ToString();
                            //}
                            #endregion
                        }
                    }
                    else
                    {
                        DisableGestionIH1600();
                    }
                }
                catch (Exception err)
                {
                    LoggerManager.Error(LoggerCategory.Deploiement, "Error while fetching data for Deploiement form: ID={0}", this.Request.QueryString["ID"]);
                    if (err is OverflowException || err is OverflowException) LoggerManager.Error(LoggerCategory.Deploiement, "{0}", "Probably occured while converting ID to int");
                    LoggerManager.Error(LoggerCategory.Deploiement, err);
                }
            }
        }

        /// <summary>
        /// some settings such projet are web wide
        /// </summary>
        private void SetWebWideProperties()
        {
            if (!IsPostBack)
            {
                // web wide value
                TaxonomyValueBuilder builder = new TaxonomyValueBuilder();
                IWebProperties webProp = SharePointServiceLocator.GetCurrent().GetInstance<IWebProperties>();

                string propertyName = Localization.GetResource(ResourcePropertyBag.WEB_PROPERTYBAG_PROJECT, ResourceFiles.CORE);
                string project = webProp.Get(propertyName);
                if (!String.IsNullOrWhiteSpace(project))
                {
                    TaxonomyValue value = builder.Build(project);
                    TaxonomyProjet.Text = value.EnsureValue();
                }

                string fDocPropertyName = Localization.GetResource(ResourcePropertyBag.WEB_PROPERTYBAG_DEPLOIEMENT_FAMILLEDOC, ResourceFiles.CORE);
                string familleDocValue = webProp.Get(fDocPropertyName);
                if (!String.IsNullOrWhiteSpace(familleDocValue))
                {
                    TaxonomyValue familleDoc = builder.Build(familleDocValue);
                    TaxonomyFamilleDoc.Text = familleDoc.EnsureValue();
                }
            }
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

            string projetTermSet = Localization.GetResource(ResourceMMS.PROJETS, ResourceFiles.MMS);
            Guid ProjetId = taxonomySession.TermStores[metadataService].Groups[group].TermSets[projetTermSet].Id;
            TaxonomyProjet.SspId.Add(termStore.Id);
            TaxonomyProjet.SSPList = termStore.Id.ToString();
            TaxonomyProjet.TermSetId.Add(ProjetId);
            TaxonomyProjet.TermSetList = ProjetId.ToString();

            string themeTermSet = Localization.GetResource(ResourceMMS.THEME, ResourceFiles.MMS);
            Guid ThemeId = taxonomySession.TermStores[metadataService].Groups[group].TermSets[themeTermSet].Id;
            TaxonomyTheme.SspId.Add(termStore.Id);
            TaxonomyTheme.SSPList = termStore.Id.ToString();
            TaxonomyTheme.TermSetId.Add(ThemeId);
            TaxonomyTheme.TermSetList = ThemeId.ToString();

            string siteTermSet = Localization.GetResource(ResourceMMS.SITE, ResourceFiles.MMS);
            Guid siteId = taxonomySession.TermStores[metadataService].Groups[group].TermSets[siteTermSet].Id;
            TaxonomySiteDPIH.SspId.Add(termStore.Id);
            TaxonomySiteDPIH.SSPList = termStore.Id.ToString();
            TaxonomySiteDPIH.TermSetId.Add(siteId);
            TaxonomySiteDPIH.TermSetList = siteId.ToString();
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
        /// inset js to close the Gestion IH1600 section
        /// </summary>
        private void DisableGestionIH1600()
        {
            string cssClass = "spdeau-hide";
            RowCodification.CssClass = RowCodification.CssClass + " " + cssClass;
            RowIndice.CssClass = RowIndice.CssClass + " " + cssClass;
            RowRevision.CssClass = RowRevision.CssClass + " " + cssClass;
            RowStatusDoc.CssClass = RowStatusDoc.CssClass + " " + cssClass;
            GestionIH1600CheckBox.Checked = false;
        }
        #endregion

        #region  validation
        /// <summary>
        /// check that codification is specified for BPE
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void ValidateCodification_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            if (GestionIH1600CheckBox.Checked)
            {
                StatusIH1600 selection = EnumHelper.GetValue<StatusIH1600>(StatusDoc.SelectedValue, StatusIH1600.NONE);
                args.IsValid = (selection == StatusIH1600.PREL || (selection == StatusIH1600.BPE && !String.IsNullOrWhiteSpace(Codification.Text)));
            }
        }

        /// <summary>
        /// if codification is not empty, validate that data in the form actually match data in 'liste de suivi'
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void ValidateSuiviConsitency_ServerValidate(object source, ServerValidateEventArgs args)
        {
            // no need to check until codification is filled in
            if (String.IsNullOrWhiteSpace(Codification.Text) || !GestionIH1600CheckBox.Checked)
            {
                args.IsValid = true;
                return;
            }

            // remove all character except letters and digits
            string code = CodificationHelper.CleanUpCodification(Codification.Text);

            // get dep from suivi
            ISuiviService suiviService = SharePointServiceLocator.GetCurrent().GetInstance<ISuiviService>();
            Deploiement suiviData = suiviService.GetDeploiement(code);

            if (suiviData == null)
            {
                // La codification ne correspond à aucune entrée dans la liste de suivi.";
                args.IsValid = true;
                return;
            }

            string msg = Localization.GetResource(ResourceValidatorKeys.VALIDATE_SUIVI_CONSISTENCY, ResourceFiles.VALIDATOR);
            StringBuilder sb = new StringBuilder(msg + "<ul>");
            // compare dep with current value
            if (!String.IsNullOrWhiteSpace(suiviData.Title) && suiviData.Title.CompareTo(Titre.Text) != 0)
            {
                args.IsValid = false;
                sb.Append("<li>Titre</li>");
            }

            if (suiviData.Projet != null && suiviData.Projet.Terms.Count > 0 && suiviData.Projet.CompareTo(DepNew.Projet) != 0)
            {
                args.IsValid = false;
                sb.Append("<li>Projet</li>");
            }

            if (suiviData.FamilleDoc != null && suiviData.FamilleDoc.Terms.Count > 0 && suiviData.FamilleDoc.CompareTo(DepNew.FamilleDoc) != 0)
            {
                args.IsValid = false;
                sb.Append("<li>Famille Documentaire</li>");
            }

            if (suiviData.NatureDoc != null && suiviData.NatureDoc.Terms.Count > 0 && suiviData.NatureDoc.CompareTo(DepNew.NatureDoc) != 0)
            {
                args.IsValid = false;
                sb.Append("<li>Nature Documentaire</li>");
            }

            if (suiviData.Site != null && suiviData.Site.Terms.Count > 0 && suiviData.Site.CompareTo(DepNew.Site) != 0)
            {
                args.IsValid = false;
                sb.Append("<li>Site</li>");
            }

            if (!args.IsValid)
            {
                sb.Append("</ul>");
                (source as CustomValidator).ErrorMessage = sb.ToString();
                ReloadFormTable.Visible = true;
                ViewState.Add(DEP_VIEWSTATE_KEY, DepOld);
            }

        }

        /// <summary>
        /// Indice is mandatory with BPE, or with PREL if revision is set, otherwise fine
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void ValidateIndice_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            if (GestionIH1600CheckBox.Checked)
            {
                if (DepNew.Status == StatusIH1600.BPE && String.IsNullOrWhiteSpace(Indice.Text))
                {
                    args.IsValid = false;
                    (source as CustomValidator).ErrorMessage = Localization.GetResource(ResourceValidatorKeys.VALIDATE_INDICE_1, ResourceFiles.VALIDATOR);
                }
                else if (DepNew.Status == StatusIH1600.PREL && String.IsNullOrWhiteSpace(Indice.Text) && !String.IsNullOrWhiteSpace(Revision.Text))
                {
                    args.IsValid = false;
                    (source as CustomValidator).ErrorMessage = Localization.GetResource(ResourceValidatorKeys.VALIDATE_INDICE_2, ResourceFiles.VALIDATOR);
                }
            }
        }

        /// <summary>
        /// letter cannot decrease - it gets the latest ih1600 info (indice, revision and status) for all documents with same codification
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void ValidateIndiceIncrement_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            if (GestionIH1600CheckBox.Checked)
            {
                if (DepNew != null && !String.IsNullOrWhiteSpace(DepNew.Indice))
                {
                    string codificationSys = CodificationHelper.CleanUpCodification(Codification.Text);

                    IDeploiementService depSvc = SharePointServiceLocator.GetCurrent().GetInstance<IDeploiementService>();
                    // query all documents with same codification to get the latest indice and revision info
                    InfoIH1600 currentStatus = depSvc.GetLatestInfoIH1600ForCodification(SPContext.Current.Web, codificationSys);

                    if (currentStatus != null && DepNew != null && !String.IsNullOrWhiteSpace(currentStatus.Indice) && !String.IsNullOrWhiteSpace(DepNew.Indice))
                    {
                        if (currentStatus.Status == StatusIH1600.PREL && Char.Parse(DepNew.Indice) < Char.Parse(currentStatus.Indice))
                        {
                            args.IsValid = false;
                        }
                        else if (currentStatus.Status == StatusIH1600.BPE)
                        {
                            if (UpdateMode)
                            {
                                if (Char.Parse(DepNew.Indice) < Char.Parse(currentStatus.Indice)) args.IsValid = false;
                            }
                            else if (Char.Parse(DepNew.Indice) <= Char.Parse(currentStatus.Indice)) args.IsValid = false;
                        }
                    }

                }


                //if (DepOld != null && DepNew != null && !String.IsNullOrWhiteSpace(DepOld.Indice) && !String.IsNullOrWhiteSpace(DepNew.Indice))
                //{
                //    if (DepOld.Status == DocStatus.PREL && Char.Parse(DepNew.Indice) < Char.Parse(DepOld.Indice))
                //    {
                //        args.IsValid = false;
                //    }
                //    else if (DepOld.Status == DocStatus.BPE)
                //    {
                //        if (UpdateMode)
                //        {
                //            if (Char.Parse(DepNew.Indice) < Char.Parse(DepOld.Indice)) args.IsValid = false;
                //        }
                //        else if (Char.Parse(DepNew.Indice) <= Char.Parse(DepOld.Indice)) args.IsValid = false;
                //    }
                //}
            }
        }

        /// <summary>
        /// check that indice and revision are set according to PREL or BPE requirements, meaning :
        /// - BPE : Indice mandatory, no revision
        /// - PREL : Indice optional, revision mandatory if indice specified
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void ValidateRevision_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            if (GestionIH1600CheckBox.Checked)
            {
                //DocStatus selection = EnumHelper.GetValue<DocStatus>(StatusDoc.SelectedValue, DocStatus.NONE);
                //if (selection == DocStatus.BPE && !String.IsNullOrWhiteSpace(Revision.Text))
                //{
                //    args.IsValid = false;
                //    (source as CustomValidator).ErrorMessage = "Merci de renseigner l'Indice pour un document BPE";
                //}
                //else 
                if (DepNew.Status == StatusIH1600.PREL && !String.IsNullOrWhiteSpace(Indice.Text) && String.IsNullOrWhiteSpace(Revision.Text))
                {
                    args.IsValid = false;
                    (source as CustomValidator).ErrorMessage = Localization.GetResource(ResourceValidatorKeys.VALIDATE_REVISION, ResourceFiles.VALIDATOR);
                }
                //args.IsValid = ((StatusDoc.SelectedValue == DocStatus.PREL && !String.IsNullOrWhiteSpace(Revision.Text)) || (StatusDoc.SelectedValue == DocStatus.BPE && )); 
            }
        }

        /// <summary>
        /// check that revision cannot decrease within the same indice
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void ValidateRevisionIncrement_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            string codificationSys = CodificationHelper.CleanUpCodification(Codification.Text);
            IDeploiementService depSvc = SharePointServiceLocator.GetCurrent().GetInstance<IDeploiementService>();
            // query all documents with same codification to get the latest indice and revision info
            InfoIH1600 currentStatus = depSvc.GetLatestInfoIH1600ForCodification(SPContext.Current.Web, codificationSys);


            if (String.IsNullOrWhiteSpace(StatusDoc.SelectedValue) || currentStatus == null || !GestionIH1600CheckBox.Checked) return;

            CustomValidator validator = source as CustomValidator;

            //string indice = Indice.Text;
            string revision = DepNew.Revision;
            //string validationStatus = ValidationStatusHiddenField.Value;
            StatusIH1600 docStatus = DepNew.Status;
            //ValidationStatus status;

            if (docStatus == StatusIH1600.PREL)
            {
                if (String.IsNullOrWhiteSpace(revision))
                {
                    if (!String.IsNullOrWhiteSpace(currentStatus.Revision))
                    {
                        args.IsValid = false;
                        validator.ErrorMessage = Localization.GetResource(ResourceValidatorKeys.VALIDATE_REVISION_INCREMENT_1, ResourceFiles.VALIDATOR);
                    }
                }
                else
                {
                    if (!String.IsNullOrWhiteSpace(currentStatus.Revision) && Convert.ToInt32(currentStatus.Revision) >= Convert.ToInt32(revision))
                    {
                        // does not apply if indice is different
                        if (!String.IsNullOrWhiteSpace(currentStatus.Indice) && Char.Parse(currentStatus.Indice) == Char.Parse(DepNew.Indice))
                        {
                            // revision can only increase
                            args.IsValid = false;
                            validator.ErrorMessage = String.Format(Localization.GetResource(ResourceValidatorKeys.VALIDATE_REVISION_INCREMENT_2, ResourceFiles.VALIDATOR), currentStatus.Revision);
                        }
                    }
                }
            }
            else if (docStatus == StatusIH1600.BPE && !String.IsNullOrWhiteSpace(revision))
            {
                // revision must be empty for BPE
                args.IsValid = false;
                validator.ErrorMessage = Localization.GetResource(ResourceValidatorKeys.VALIDATE_REVISION_INCREMENT_3, ResourceFiles.VALIDATOR);
            }



            //if (String.IsNullOrWhiteSpace(StatusDoc.SelectedValue) || DepOld == null || !GestionIH1600CheckBox.Checked) return;

            //CustomValidator validator = source as CustomValidator;

            ////string indice = Indice.Text;
            //string revision = DepNew.Revision;
            ////string validationStatus = ValidationStatusHiddenField.Value;
            //StatusIH1600 docStatus = DepNew.Status;
            ////ValidationStatus status;

            //if (docStatus == StatusIH1600.PREL)
            //{
            //    if (String.IsNullOrWhiteSpace(revision))
            //    {
            //        if (!String.IsNullOrWhiteSpace(DepOld.Revision))
            //        {
            //            args.IsValid = false;
            //            validator.ErrorMessage = Localization.GetResource(ResourceValidatorKeys.VALIDATE_REVISION_INCREMENT_1, ResourceFiles.VALIDATOR);
            //        }
            //    }
            //    else
            //    {
            //        if (!String.IsNullOrWhiteSpace(DepOld.Revision) && Convert.ToInt32(DepOld.Revision) > Convert.ToInt32(revision))
            //        {
            //            // does not apply if indice is different
            //            if (!String.IsNullOrWhiteSpace(DepOld.Indice) && Char.Parse(DepOld.Indice) == Char.Parse(DepNew.Indice))
            //            {
            //                // revision can only increase
            //                args.IsValid = false;
            //                validator.ErrorMessage = Localization.GetResource(ResourceValidatorKeys.VALIDATE_REVISION_INCREMENT_2, ResourceFiles.VALIDATOR);
            //            }
            //        }
            //    }
            //}
            //else if (docStatus == StatusIH1600.BPE && !String.IsNullOrWhiteSpace(revision))
            //{
            //    // revision must be empty for BPE
            //    args.IsValid = false;
            //    validator.ErrorMessage = Localization.GetResource(ResourceValidatorKeys.VALIDATE_REVISION_INCREMENT_3, ResourceFiles.VALIDATOR);
            //}
        }

        /// <summary>
        /// check file format is the same as previous one
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void ValidateFileUpload2_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (DepOld == null || String.IsNullOrWhiteSpace(DepOld.FileName) || Path.GetExtension(FileUpload.FileName) == Path.GetExtension(DepOld.FileName))
            {
                args.IsValid = true;
            }
            else
            {
                args.IsValid = false;
                (source as CustomValidator).ErrorMessage = Localization.GetResource(ResourceValidatorKeys.VALIDATE_FILE_UPLOAD2, ResourceFiles.VALIDATOR) + Path.GetExtension(DepOld.FileName);
            }
        }

        /// <summary>
        /// check that file name does not exist with another codification
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void ValidateFileUpload3_ServerValidate(object source, ServerValidateEventArgs args)
        {
            // get file with same name

            string sQuery = String.Format(@"<Where><Eq><FieldRef Name=""FileLeafRef"" />  <Value Type=""Text"">{0}</Value></Eq></Where>", FileUpload.FileName);
            string sViewFields = @"<FieldRef Name=""Codification"" /><FieldRef Name=""CodificationSystem"" /><FieldRef Name=""ID"" />";
            string sViewAttrs = @"Scope=""Recursive""";
            uint iRowLimit = 1;

            var oQuery = new SPQuery();
            oQuery.Query = sQuery;
            oQuery.ViewFields = sViewFields;
            oQuery.ViewAttributes = sViewAttrs;
            oQuery.RowLimit = iRowLimit;

            SPList list = ListHelper.Deploiement;
            SPListItemCollection items = list.GetItems(oQuery);

            // if no file - fine, it's a first upload
            if (items.Count == 0)
            {
                args.IsValid = true;
                return;
            }

            // if file exists, need to check that codification of existing file match the codification entered in the form
            else if (items.Count == 1)
            {
                if (String.IsNullOrWhiteSpace(DepNew.Codification))
                {
                    if (Boolean.Parse(NewDocumentHiddenField.Value))
                    {
                        // cannot upload a new document if file name exist                        
                        args.IsValid = false;
                        (source as CustomValidator).ErrorMessage = Localization.GetResource(ResourceValidatorKeys.VALIDATE_FILE_UPLOAD3_2, ResourceFiles.VALIDATOR);
                    }
                }
                else
                {
                    string fieldName = Localization.GetResource(ResourceFieldsKeys.CODIFICATION_SYSTEM, ResourceFiles.FIELDS);
                    string existingFileCodification = items[0].EnsureValue<string>(fieldName);
                    if (!String.IsNullOrWhiteSpace(existingFileCodification) && DepNew.CodificationSystem != existingFileCodification)
                    {
                        args.IsValid = false;
                        (source as CustomValidator).ErrorMessage = Localization.GetResource(ResourceValidatorKeys.VALIDATE_FILE_UPLOAD3_1, ResourceFiles.VALIDATOR);
                    }
                }
            }
            else
            {
                throw new AmbiguousMatchException("SPEEDEAU : SPQuery found 2 files with same name in same library !! Weird !! check Query : " + sQuery);
            }
        }


        /// <summary>
        /// Ensure that there's no file with same format (file extension) and same codification
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void ValidateFileUpload4_ServerValidate(object source, ServerValidateEventArgs args)
        {
            // get file extension
            string extension = new FileInfo(FileUpload.FileName).Extension;
            // get codification 
            string codification = CodificationHelper.CleanUpCodification(Codification.Text);

            // get file with same codification and same indice
            SPQuery qry = new SPQuery();
            qry.Query = String.Format(@"<Where>
                                            <Eq>
                                                <FieldRef Name='CodificationSystem' />
                                                <Value Type='Text'>{0}</Value>
                                            </Eq>
                                     </Where>", codification);
            qry.ViewFields = @"<FieldRef Name=""FileLeafRef"" /><FieldRef Name=""ID"" />";

            SPList list = ListHelper.Deploiement;
            SPListItemCollection items = list.GetItems(qry);

            if (items.Count != 0)
            {
                // there are one or more files with same codification
                // check if any matches has same file format
                bool sameFormat = items.Cast<SPListItem>().Any(f => f["FileLeafRef"] != null && new FileInfo(f["FileLeafRef"].ToString()).Extension.Equals(extension));

                if (sameFormat)
                {
                    args.IsValid = false;
                    (source as CustomValidator).ErrorMessage = Localization.GetResource(ResourceValidatorKeys.VALIDATE_FILE_UPLOAD4, ResourceFiles.VALIDATOR);
                }
            }
        }


        /// <summary>
        /// check status : do not allow BPE with VAO
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void ValidateStatusDoc_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (GestionIH1600CheckBox.Checked)
            {
                if (String.IsNullOrWhiteSpace(StatusDoc.SelectedValue))
                {
                    args.IsValid = false;
                    (source as CustomValidator).ErrorMessage = Localization.GetResource(ResourceValidatorKeys.VALIDATE_STATUS_DOC, ResourceFiles.VALIDATOR);
                    return;
                }

                // do not allow uploading new file with BPE status if current validation == VAO or if any observations

                string codificationSys = CodificationHelper.CleanUpCodification(Codification.Text);
                IDeploiementService depSvc = SharePointServiceLocator.GetCurrent().GetInstance<IDeploiementService>();
                // query all documents with same codification to get the latest indice and revision info
                InfoIH1600 currentStatus = depSvc.GetLatestInfoIH1600ForCodification(SPContext.Current.Web, codificationSys);
                if (currentStatus != null)
                {
                    StatusIH1600 selection = EnumHelper.GetValue<StatusIH1600>(StatusDoc.SelectedValue, StatusIH1600.NONE);
                    if (selection == StatusIH1600.BPE)
                    {
                        if (currentStatus.ValidationStatus == ValidationStatus.VAO) args.IsValid = false;
                    }
                }


                //if (DepOld != null)
                //{
                //    StatusIH1600 selection = EnumHelper.GetValue<StatusIH1600>(StatusDoc.SelectedValue, StatusIH1600.NONE);
                //    if (selection == StatusIH1600.BPE)
                //    {
                //        if (DepOld.ValidationStatus == ValidationStatus.VAO) args.IsValid = false;
                //        //else
                //        //{
                //        //    IObservationService obsService = SharePointServiceLocator.GetCurrent().GetInstance<IObservationService>();
                //        //    bool hasObs = obsService.HasValidObservationsForDocID(Convert.ToInt32(DepOld.ID));
                //        //    if (hasObs) args.IsValid = false;
                //        //}
                //    }

                //}
            }
        }
        #endregion

    }
}
