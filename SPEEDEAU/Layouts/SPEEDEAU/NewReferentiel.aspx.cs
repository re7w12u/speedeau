using System;
using System.Linq;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using SPEEDEAU.ADMIN.Model;
using SPEEDEAU.ADMIN.Util;
using SPEEDEAU.ADMIN.Services;
using SPEEDEAU.ADMIN;
using Microsoft.Practices.SharePoint.Common.ServiceLocation;
using Microsoft.SharePoint.Taxonomy;
using System.Web.UI.WebControls;
using System.IO;
using System.Reflection;

namespace SPEEDEAU.Layouts.SPEEDEAU
{
    public partial class NewReferentiel : LayoutsPageBase
    {

        private const string DEP_VIEWSTATE_KEY = "DepViewStateObject";
        private const string FILEUPLOAD_VIEWSTATE_KEY = "DepFileuploadPathValue";
        private Referentiel RefNew { get; set; }
        private Referentiel RefOld { get; set; }
        private bool UpdateMode { get; set; }

        #region event handlers
        protected void Page_Load(object sender, EventArgs e)
        {
            SetUpValidators();
            SetUpTaxonmyFields();
            SetWebWideProperties();

            #region Fill in form if ID is provided
            if (!IsPostBack && !String.IsNullOrWhiteSpace(this.Request.QueryString["ID"]))
            {
                // we are updating an existing file here, so need to check file consistency
                // meaning we get the data to fill in the form from the ID
                // but when saving process is based on the file...
                // this mean that if you select (in the FileUpload control) a file that already exists but match another ID
                // you will end up updating that item...

                UpdateOnlyHiddenField.Value = false.ToString();
                int id = Convert.ToInt32(this.Request.QueryString.Get("ID"));
                IReferentielService depSvc = SharePointServiceLocator.GetCurrent().GetInstance<IReferentielService>();
                Referentiel referentiel = depSvc.GetReferentielFromRefLibItem(id, SPContext.Current.Web);
                FillInFormAllFields(referentiel);
            }
            #endregion

            #region update metadata but no upload
            else if (!IsPostBack && !String.IsNullOrWhiteSpace(this.Request.QueryString["UID"]))
            {
                SetUpdateMode();
                int id = Convert.ToInt32(this.Request.QueryString.Get("UID"));
                IReferentielService depSvc = SharePointServiceLocator.GetCurrent().GetInstance<IReferentielService>();
                Referentiel referentiel = depSvc.GetReferentielFromRefLibItem(id, SPContext.Current.Web);
                FillInFormAllFields(referentiel);
            }
            #endregion

            #region blanck form
            else if (!IsPostBack)
            {
                // set BPE by default
                ListItem li = StatusDoc.Items.Cast<ListItem>().FirstOrDefault(l => (StatusIH1600)Enum.Parse(typeof(StatusIH1600), l.Value, true) == StatusIH1600.BPE);
                if (li != null) li.Selected = true;

                Indice.Text = "A";
            }
            #endregion

            #region fill in from field value on post back
            if (IsPostBack)
            {
                UpdateMode = String.IsNullOrWhiteSpace(UpdateOnlyHiddenField.Value) ? false : Convert.ToBoolean(UpdateOnlyHiddenField.Value);

                #region fill in from ViewState
                if (ViewState[DEP_VIEWSTATE_KEY] != null && ViewState[DEP_VIEWSTATE_KEY] is Referentiel)
                {
                    RefOld = ViewState[DEP_VIEWSTATE_KEY] as Referentiel;
                }
                #endregion

                #region create new Referentiel from posted data
                RefNew = new Referentiel();

                if (UpdateMode) SetUpdateMode();
                else
                {
                    RefNew.File = FileUpload.FileBytes;
                    RefNew.FileName = FileUpload.FileName;
                }

                RefNew.Title = Titre.Text;
                RefNew.ProcessIH1600 = GestionIH1600CheckBox.Checked;

                if (GestionIH1600CheckBox.Checked)
                {
                    RefNew.Codification = Codification.Text;
                    RefNew.CodificationSystem = Codification.Text;
                    RefNew.Status = EnumHelper.GetValue<StatusIH1600>(StatusDoc.SelectedValue, StatusIH1600.NONE);
                    RefNew.Indice = Indice.Text.ToUpper();
                    RefNew.Revision = RefNew.Status == StatusIH1600.BPE ? String.Empty : Revision.Text;
                    RefNew.ValidationStatus = ValidationStatus.None;
                }

                TaxonomyValueBuilder builder = new TaxonomyValueBuilder();
                RefNew.NatureDoc = builder.Build(TaxonomyNatureDoc);
                RefNew.FamilleDoc = builder.Build(TaxonomyFamilleDoc);
                RefNew.Projet = builder.Build(TaxonomyProjet);
                RefNew.Theme = builder.Build(TaxonomyTheme);
                RefNew.EC = builder.Build(TaxonomyEC);
                #endregion
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
                IReferentielService refSvc = SharePointServiceLocator.GetCurrent().GetInstance<IReferentielService>();
                SPListItem item = refSvc.SaveReferentielToList(RefNew, RefOld == null ? String.Empty : RefOld.FileName, SPContext.Current.Web);
                SetCloseDialog();
            }
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
        private void FillInFormCoreFields(Referentiel referentiel)
        {
            try
            {
                // add to viewstate to get it back when submitting
                ViewState.Add(DEP_VIEWSTATE_KEY, referentiel);

                Titre.Text = referentiel.Title;
                Codification.Text = referentiel.Codification;
                TaxonomyFamilleDoc.Text = referentiel.FamilleDoc.EnsureValue();
                TaxonomyNatureDoc.Text = referentiel.NatureDoc.EnsureValue();
                TaxonomyTheme.Text = referentiel.Theme.EnsureValue();
                TaxonomyEC.Text = referentiel.EC.EnsureValue();

                // commented out as per ticket https://www.speedeau.fr/sites/projet/Lists/tickets/DispForm.aspx?ID=163
                //string[] s = new string[] { TaxonomyProjet.Text };
                //TaxonomyProjet.Text = String.Join(";", s.Union(referentiel.Projet.EnsureValue().Split(new char[] { ';' })));

                string projet = referentiel.Projet.EnsureValue();
                if (!String.IsNullOrWhiteSpace(projet)) TaxonomyProjet.Text = projet;

            }
            catch (Exception err)
            {
                LoggerManager.Error(LoggerCategory.Referentiel, "Error while fetching data for Referentiel form: ID={0}", this.Request.QueryString["ID"]);
                if (err is OverflowException || err is OverflowException) LoggerManager.Error(LoggerCategory.Referentiel, "{0}", "Probably occured while converting ID to int");
                LoggerManager.Error(LoggerCategory.Referentiel, err);
            }
        }

        /// <summary>
        /// fill in matching form extra fields with value from Deploiement
        /// Extra field means all field that are not bind to liste de suivi and that 
        /// do not need to be consistent with data in the liste de suivi
        /// </summary>
        /// <param name="id">id of the SPListItem in Deploiement doc library</param>
        private void FillInFormAllFields(Referentiel referentiel)
        {
            try
            {
                FillInFormCoreFields(referentiel);

                if (referentiel.ProcessIH1600)
                {
                    if (UpdateMode)
                    {
                        // in update mode - set values as saved for current version. 
                        ListItem li = StatusDoc.Items.Cast<ListItem>().FirstOrDefault(l => referentiel.Status == (StatusIH1600)Enum.Parse(typeof(StatusIH1600), l.Value, true));
                        if (li != null) li.Selected = true;
                        Indice.Text = referentiel.Indice;
                        Revision.Text = referentiel.Revision;
                    }
                    else
                    {
                        // in upload mode we expect a new version - set values as for new version
                        #region set Status Documentaire (BPE, PREL)
                        // we expect status should be BPE
                        ListItem li = StatusDoc.Items.Cast<ListItem>().FirstOrDefault(l => StatusIH1600.BPE == (StatusIH1600)Enum.Parse(typeof(StatusIH1600), l.Value, true));
                        if (li != null) li.Selected = true;
                        #endregion

                        #region set Indice
                        if (String.IsNullOrWhiteSpace(referentiel.Indice)) Indice.Text = "A";
                        else Indice.Text = ((char)((int)Char.Parse(referentiel.Indice) + 1)).ToString().ToUpper();
                        #endregion

                        #region set Revision
                        Revision.Text = String.Empty;
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
                LoggerManager.Error(LoggerCategory.Referentiel, "Error while fetching data for Referentiel form: ID={0}", this.Request.QueryString["ID"]);
                if (err is OverflowException || err is OverflowException) LoggerManager.Error(LoggerCategory.Referentiel, "{0}", "Probably occured while converting ID to int");
                LoggerManager.Error(LoggerCategory.Referentiel, err);
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

                string fDocPropertyName = Localization.GetResource(ResourcePropertyBag.WEB_PROPERTYBAG_REFERENTIEL_FAMILLEDOC, ResourceFiles.CORE);
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

            string siteTermSet = Localization.GetResource(ResourceMMS.EC, ResourceFiles.MMS);
            Guid siteId = taxonomySession.TermStores[metadataService].Groups[group].TermSets[siteTermSet].Id;
            TaxonomyEC.SspId.Add(termStore.Id);
            TaxonomyEC.SSPList = termStore.Id.ToString();
            TaxonomyEC.TermSetId.Add(siteId);
            TaxonomyEC.TermSetList = siteId.ToString();
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


        #region validation
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
        /// Indice is mandatory with BPE, otherwise fine
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void ValidateIndice_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            if (GestionIH1600CheckBox.Checked)
            {
                StatusIH1600 selection = EnumHelper.GetValue<StatusIH1600>(StatusDoc.SelectedValue, StatusIH1600.NONE);
                args.IsValid = (selection == StatusIH1600.PREL || (selection == StatusIH1600.BPE && !String.IsNullOrWhiteSpace(Indice.Text)));
            }
        }

        /// <summary>
        /// letter cannot decrease
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void ValidateIndiceIncrement_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            if (GestionIH1600CheckBox.Checked)
            {
                if (RefOld != null && RefNew != null && !String.IsNullOrWhiteSpace(RefOld.Indice) && !String.IsNullOrWhiteSpace(RefNew.Indice))
                {
                    if (RefOld.Status == StatusIH1600.PREL && Char.Parse(RefNew.Indice) < Char.Parse(RefOld.Indice))
                    {
                        args.IsValid = false;
                    }
                    else if (RefOld.Status == StatusIH1600.BPE)
                    {
                        if (UpdateMode)
                        {
                            if (Char.Parse(RefNew.Indice) < Char.Parse(RefOld.Indice)) args.IsValid = false;
                        }
                        else if (Char.Parse(RefNew.Indice) <= Char.Parse(RefOld.Indice)) args.IsValid = false;
                    }
                }
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
                StatusIH1600 selection = EnumHelper.GetValue<StatusIH1600>(StatusDoc.SelectedValue, StatusIH1600.NONE);
                //if (selection == DocStatus.BPE && !String.IsNullOrWhiteSpace(Revision.Text))
                //{
                //    args.IsValid = false;
                //    (source as CustomValidator).ErrorMessage = "Merci de renseigner l'Indice pour un document BPE";
                //}
                //else
                if (selection == StatusIH1600.PREL && !String.IsNullOrWhiteSpace(Indice.Text) && String.IsNullOrWhiteSpace(Revision.Text))
                {
                    args.IsValid = false;
                    (source as CustomValidator).ErrorMessage = "Vous ne pouvez pas saisir un indice sans préciser le numéro de révision.";
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

            if (String.IsNullOrWhiteSpace(StatusDoc.SelectedValue) || RefOld == null || !GestionIH1600CheckBox.Checked) return;

            CustomValidator validator = source as CustomValidator;

            //string indice = Indice.Text;
            string revision = Revision.Text;
            //string validationStatus = ValidationStatusHiddenField.Value;
            StatusIH1600 docStatus = EnumHelper.GetValue<StatusIH1600>(StatusDoc.SelectedValue, StatusIH1600.NONE);
            //ValidationStatus status;

            if (docStatus == StatusIH1600.PREL)
            {
                if (String.IsNullOrWhiteSpace(revision))
                {
                    if (!String.IsNullOrWhiteSpace(RefOld.Revision))
                    {
                        args.IsValid = false;
                        validator.ErrorMessage = "Merci de converser ou incrémenter la révision";
                    }
                }
                else
                {
                    if (!String.IsNullOrWhiteSpace(RefOld.Revision) && Convert.ToInt32(RefOld.Revision) > Convert.ToInt32(revision))
                    {
                        // does not apply if indice is different
                        if (!String.IsNullOrWhiteSpace(RefOld.Indice) && Char.Parse(RefOld.Indice) == Char.Parse(RefNew.Indice))
                        {
                            // revision can only increase
                            args.IsValid = false;
                            validator.ErrorMessage = "Le numéro de révision ne peut pas être inférieur à la version actuelle.";
                        }
                    }
                }
            }
            else if (docStatus == StatusIH1600.BPE && !String.IsNullOrWhiteSpace(revision))
            {
                // revision must be empty for BPE
                args.IsValid = false;
                validator.ErrorMessage = "Un document BPE ne peut pas avoir numéro de de révision";
            }
        }

        /// <summary>
        /// check file format is the same as previous one
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void ValidateFileUpload2_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (RefOld == null || String.IsNullOrWhiteSpace(RefOld.FileName) || Path.GetExtension(FileUpload.FileName) == Path.GetExtension(RefOld.FileName))
            {
                args.IsValid = true;
            }
            else
            {
                args.IsValid = false;
                (source as CustomValidator).ErrorMessage = "Le nom du fichier que vous avez sélectionné doit rester au même format que le précédent, à savoir : " + Path.GetExtension(RefOld.FileName);
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

            SPList list = ListHelper.Referentiel;
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
                string fieldName = Localization.GetResource(ResourceFieldsKeys.CODIFICATION_SYSTEM, ResourceFiles.FIELDS);
                string existingFileCodification = items[0].EnsureValue<string>(fieldName);
                if (!String.IsNullOrWhiteSpace(existingFileCodification) && RefNew.CodificationSystem != existingFileCodification)
                {
                    args.IsValid = false;
                }
            }
            else
            {
                throw new AmbiguousMatchException("SPQuery found 2 files with same name in same library !! Weird !! check Query : " + sQuery);
            }
        }

        /// <summary>
        /// Ensure that there's no file with same format (file extension) and same codification and same indice
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        protected void ValidateFileUpload4_ServerValidate(object source, ServerValidateEventArgs args)
        {
            // get file extension
            string extension = new FileInfo(FileUpload.FileName).Extension;
            // get indice 
            string indice = Indice.Text.Trim().ToUpper();
            // get codification 
            string codification = CodificationHelper.CleanUpCodification(Codification.Text);

            // get file with same codification and same indice
            SPQuery qry = new SPQuery();
            qry.Query = String.Format(@"<Where>
                                        <And>
                                            <Eq>
                                                <FieldRef Name='CodificationSystem' />
                                                <Value Type='Text'>{0}</Value>
                                            </Eq>
                                            <Eq>
                                               <FieldRef Name='EDFVersion' />
                                               <Value Type='Text'>{1}</Value>
                                            </Eq>
                                         </And>
                                     </Where>", codification, indice);
            qry.ViewFields = @"<FieldRef Name=""FileLeafRef"" /><FieldRef Name=""ID"" />";

            SPList list = ListHelper.Referentiel;
            SPListItemCollection items = list.GetItems(qry);

            
            if (items.Count > 0)
            {
                //there are one or more files with same codification and same indice
                // check if match has same file format

                bool sameFormat = items.Cast<SPListItem>().Any(f => f[SPBuiltInFieldId.FileLeafRef] != null && new FileInfo(f[SPBuiltInFieldId.FileLeafRef].ToString()).Extension.Equals(extension));
                if (sameFormat)
                {
                    args.IsValid = false;
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
                    (source as CustomValidator).ErrorMessage = "Merci de renseigner le statut documentaire du document";
                    return;
                }

                // do not allow uploading new file with BPE status if current validation == VAO or if any observations
                if (RefOld != null)
                {
                    StatusIH1600 selection = EnumHelper.GetValue<StatusIH1600>(StatusDoc.SelectedValue, StatusIH1600.NONE);
                    if (selection == StatusIH1600.BPE)
                    {
                        if (RefOld.ValidationStatus == ValidationStatus.VAO) args.IsValid = false;
                        //else
                        //{
                        //    IObservationService obsService = SharePointServiceLocator.GetCurrent().GetInstance<IObservationService>();
                        //    bool hasObs = obsService.HasValidObservationsForDocID(Convert.ToInt32(RefOld.ID));
                        //    if (hasObs) args.IsValid = false;
                        //}
                    }

                }
            }
        }
        #endregion
    }
}
