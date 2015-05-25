using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using SPEEDEAU.ADMIN;
using SPEEDEAU.ADMIN.Util;
using SPEEDEAU.ADMIN.Model;
using SPEEDEAU.ADMIN.Services;
using Microsoft.SharePoint.Taxonomy;
using Microsoft.Practices.SharePoint.Common.ServiceLocation;
using Microsoft.SharePoint.Utilities;

namespace SPEEDEAU.Layouts
{
    public partial class Settings : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BindTaxonomy();
            if (!IsPostBack) SetFieldValue();
        }

        /// <summary>
        /// set field value from property bag
        /// </summary>
        private void SetFieldValue()
        {
            IWebProperties webProp = SharePointServiceLocator.GetCurrent().GetInstance<IWebProperties>();
            TaxonomyValueBuilder builder = new TaxonomyValueBuilder();

            // set value for project taxon field
            string ProjectPropertyName = Localization.GetResource(ResourcePropertyBag.WEB_PROPERTYBAG_PROJECT, ResourceFiles.CORE);            
            string projectPropertyValue = webProp.Get(ProjectPropertyName);
            if (!String.IsNullOrWhiteSpace(projectPropertyValue))
            {
                TaxonomyValue value = builder.Build(projectPropertyValue);
                TaxonomyProjet.Text = value.EnsureValue();
            }

            // set value for famille documentaire taxon field for deploieemnt
            string fDocPropertyName = Localization.GetResource(ResourcePropertyBag.WEB_PROPERTYBAG_DEPLOIEMENT_FAMILLEDOC, ResourceFiles.CORE);
            string familleDocValue = webProp.Get(fDocPropertyName);
            if (!String.IsNullOrWhiteSpace(familleDocValue))
            {
                TaxonomyValue familleDoc = builder.Build(familleDocValue);
                TaxonomyFamilleDocDep.Text = familleDoc.EnsureValue();
            }

            // set value for famille documentaire taxon field for référentiel
            string refFamilleDocPropertyName = Localization.GetResource(ResourcePropertyBag.WEB_PROPERTYBAG_REFERENTIEL_FAMILLEDOC, ResourceFiles.CORE);
            string refFamilleDocValue = webProp.Get(refFamilleDocPropertyName);
            if (!String.IsNullOrWhiteSpace(refFamilleDocValue))
            {
                TaxonomyValue refFamilleDoc = builder.Build(refFamilleDocValue);
                TaxonomyFamilleDocRef.Text = refFamilleDoc.EnsureValue();
            }

            // set value for url of MSH site
            string MSHSiteUrlPropertyName = Localization.GetResource(ResourcePropertyBag.WEB_PROPERTYBAG_SITE_MSH_URL, ResourceFiles.CORE);
            string mshSiteUrlValue = webProp.Get(MSHSiteUrlPropertyName);
            if (!String.IsNullOrWhiteSpace(mshSiteUrlValue)) SiteMSHTextBox.Text = mshSiteUrlValue;
            
            // set value for email field
            string EmailPropertyName = Localization.GetResource(ResourcePropertyBag.WEB_PROPERTYBAG_ALERTE_EMAIL, ResourceFiles.CORE);
            string emailValue = webProp.Get(EmailPropertyName);
            if (!String.IsNullOrWhiteSpace(emailValue))
            {
                EmailTextBox.Text = emailValue;
            }

            // set value for alerte check box
            string checkBoxPropertyName = Localization.GetResource(ResourcePropertyBag.WEB_PROPERTYBAG_ALERTE_CHECKBOX, ResourceFiles.CORE);
            string checkBoxValue = webProp.Get(checkBoxPropertyName);
            if (!String.IsNullOrWhiteSpace(checkBoxValue))
            {
                AlerteCheckBox.Checked = bool.Parse(checkBoxValue);
            }

            string regexPropertyName = Localization.GetResource(ResourcePropertyBag.WEB_PROPERTYBAG_REGEX_CODIFICATION, ResourceFiles.CORE);
            string regexValue = webProp.Get(regexPropertyName);
            if (!String.IsNullOrWhiteSpace(regexValue))
            {
                RegexCodificationTextBox.Text = regexValue;
            }

        }

        /// <summary>
        ///  bind taxonomy field with managed data store term set
        /// </summary>
        /// <returns></returns>
        private void BindTaxonomy()
        {
            string metadataService = Localization.GetResource(ResourceMMS.MMS_NAME, ResourceFiles.MMS);
            string group = Localization.GetResource(ResourceMMS.GROUP_NAME, ResourceFiles.MMS);
            TaxonomySession taxonomySession = new TaxonomySession(SPContext.Current.Site);
            TermStore termStore = taxonomySession.TermStores[metadataService];

            string projetTermSet = Localization.GetResource(ResourceMMS.PROJETS, ResourceFiles.MMS);
            Guid ProjetId = taxonomySession.TermStores[metadataService].Groups[group].TermSets[projetTermSet].Id;

            TaxonomyProjet.SspId.Add(termStore.Id);
            TaxonomyProjet.SSPList = termStore.Id.ToString();
            TaxonomyProjet.TermSetId.Add(ProjetId);
            TaxonomyProjet.TermSetList = ProjetId.ToString();

            string familleDocTermSet = Localization.GetResource(ResourceMMS.FAMILLE_DOC, ResourceFiles.MMS);
            Guid FammilleDocId = taxonomySession.TermStores[metadataService].Groups[group].TermSets[familleDocTermSet].Id;

            TaxonomyFamilleDocDep.SspId.Add(termStore.Id);
            TaxonomyFamilleDocDep.SSPList = termStore.Id.ToString();
            TaxonomyFamilleDocDep.TermSetId.Add(FammilleDocId);
            TaxonomyFamilleDocDep.TermSetList = FammilleDocId.ToString();

            TaxonomyFamilleDocRef.SspId.Add(termStore.Id);
            TaxonomyFamilleDocRef.SSPList = termStore.Id.ToString();
            TaxonomyFamilleDocRef.TermSetId.Add(FammilleDocId);
            TaxonomyFamilleDocRef.TermSetList = FammilleDocId.ToString();
        }


        /// <summary>
        /// handle save value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SaveBtn_Click(object sender, EventArgs e)
        {
            TaxonomyValueBuilder builder = new TaxonomyValueBuilder();           
            IWebProperties webProp = SharePointServiceLocator.GetCurrent().GetInstance<IWebProperties>();

            string ProjectPropertyName = Localization.GetResource(ResourcePropertyBag.WEB_PROPERTYBAG_PROJECT, ResourceFiles.CORE);
            TaxonomyValue projectValue = builder.Build(TaxonomyProjet);
            webProp.Set(ProjectPropertyName, projectValue.ToFullString());

            string depFamilleDocPropertyName = Localization.GetResource(ResourcePropertyBag.WEB_PROPERTYBAG_DEPLOIEMENT_FAMILLEDOC, ResourceFiles.CORE);
            TaxonomyValue depFamilleDocValue = builder.Build(TaxonomyFamilleDocDep);
            webProp.Set(depFamilleDocPropertyName, depFamilleDocValue.ToFullString());

            string refFamilleDocPropertyName = Localization.GetResource(ResourcePropertyBag.WEB_PROPERTYBAG_REFERENTIEL_FAMILLEDOC, ResourceFiles.CORE);
            TaxonomyValue refFamilleDocValue = builder.Build(TaxonomyFamilleDocRef);
            webProp.Set(refFamilleDocPropertyName, refFamilleDocValue.ToFullString());

            string SiteMshUrlName = Localization.GetResource(ResourcePropertyBag.WEB_PROPERTYBAG_SITE_MSH_URL, ResourceFiles.CORE);
            webProp.Set(SiteMshUrlName, SiteMSHTextBox.Text);

            string EmailPropertyName = Localization.GetResource(ResourcePropertyBag.WEB_PROPERTYBAG_ALERTE_EMAIL, ResourceFiles.CORE);            
            webProp.Set(EmailPropertyName, EmailTextBox.Text);

            string checkBoxPropertyName = Localization.GetResource(ResourcePropertyBag.WEB_PROPERTYBAG_ALERTE_CHECKBOX, ResourceFiles.CORE);
            webProp.Set(checkBoxPropertyName, AlerteCheckBox.Checked.ToString());

            string regexPropertyName = Localization.GetResource(ResourcePropertyBag.WEB_PROPERTYBAG_REGEX_CODIFICATION, ResourceFiles.CORE);
            webProp.Set(regexPropertyName, RegexCodificationTextBox.Text.Trim());
        }

        /// <summary>
        /// redirect to referrer url 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CancelBtn_Click(object sender, EventArgs e)
        {
            SPUtility.Redirect(Request.UrlReferrer.AbsolutePath, SPRedirectFlags.UseSource, Context);
        }
    }
}
