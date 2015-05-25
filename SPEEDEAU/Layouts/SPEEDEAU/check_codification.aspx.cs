using System;
using System.Linq;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Utilities;
using System.Web.UI.WebControls;
using SPEEDEAU.ADMIN;
using SPEEDEAU.ADMIN.Util;
using SPEEDEAU.ADMIN.Services;
using Microsoft.Practices.SharePoint.Common.ServiceLocation;
using System.Collections.Generic;

namespace SPEEDEAU.Layouts.SPEEDEAU
{
    public partial class check_codification : LayoutsPageBase
    {
       
        protected void Page_Load(object sender, EventArgs e)
        {
            SetUpValidators();
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

        private SPListItem _item = null;
        private SPListItem Item
        {
            get
            {
                if (_item == null)
                {
                    string value = CodificationHelper.CleanUpCodification(Codification.Text);
                    ISuiviService suiviService = SharePointServiceLocator.GetCurrent().GetInstance<ISuiviService>();
                    _item = suiviService.GetItemForCodificationSystem(value);
                }
                return _item;
            }
        }

        /// <summary>
        /// triggered when the user click on the save button.
        /// It create a new Deploiement entity and saves it to the list using IDeploiementService
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SaveBtn_Click(object sender, EventArgs e)
        {
            // redirect to form with ID as parameter to load data from liste de suivi
            if (IsValid)
            {
               string queryString = "FID=" + Item.ID;
               SPUtility.Redirect("/speedeau/newdeploiement.aspx", SPRedirectFlags.RelativeToLayoutsPage, this.Context, queryString);
            }
        }

        protected void IgnoreBtn_Click(object sender, EventArgs e)
        {
            SPUtility.Redirect("/speedeau/newdeploiement.aspx", SPRedirectFlags.RelativeToLayoutsPage, this.Context, "blank=true");
        }

        protected void ForceCode_Click(object sender, EventArgs e)
        {
            string value = Codification.Text;
            string queryString = "blank=true&codification=" + value;
            SPUtility.Redirect("/speedeau/newdeploiement.aspx", SPRedirectFlags.RelativeToLayoutsPage, this.Context, queryString);
        }


        protected void CustomValidatorCodification_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            // do not validate is codification is forced
            if (this.Page.Request.Params.Get("__EVENTTARGET") != "ForceBtn")
            {
                #region look for codification match in list de suivi
                //                string listName = Localization.GetResource(ResourceListKeys.SUIVI_LISTNAME, ResourceFiles.CORE);
                //                SPList list = SPContext.Current.Web.Lists[listName];
                //                string value = Codification.Text;

                //                SPQuery q = new SPQuery();
                //                q.Query = String.Format(@"   <Where>
                //                                    <Eq>
                //                                       <FieldRef Name='Codification' />
                //                                       <Value Type='Text'>{0}</Value>
                //                                    </Eq>
                //                           </Where>", value);

                //                SPListItemCollection coll = list.GetItems(q);


                #endregion

                if (Item == null)
                {
                    args.IsValid = false;
                    (source as CustomValidator).ErrorMessage = "Nous n'avons trouvé aucune codification correspondante.";
                    ForceCodeRow.Visible = true;
                }

                //if (coll.Count == 1)
                //{
                //    // one perfect match - everything's fine
                //    item = coll[0];
                //    args.IsValid = true;
                //}
                //else if (coll.Count == 0)
                //{
                //    // no match : mistake or non existent code - enable force button to continue with current codification
                //    args.IsValid = false;
                //    (source as CustomValidator).ErrorMessage = "Nous n'avons trouvé aucune codification correspondante.";
                //    ForceCodeRow.Visible = true;
                //}
                //else
                //{
                //    // more than one item founds - sounds weiiiiird !! - force ignore
                //    args.IsValid = false;
                //    (source as CustomValidator).ErrorMessage = "Il semble qu'il y ait un problème car nous avons trouvé plusieurs codifications.";
                //    SaveBtn.Enabled = false;
                //}
            }
        }

    }
}
