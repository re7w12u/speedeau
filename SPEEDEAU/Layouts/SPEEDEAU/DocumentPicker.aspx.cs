using System;
using System.Linq;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using SPEEDEAU.ADMIN;
using SPEEDEAU.ADMIN.Util;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.Utilities;
using System.Web;

namespace SPEEDEAU.Layouts.SPEEDEAU
{
    public partial class DocumentPicker : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && this.Page.Request.QueryString["CID"] != null)
            {
                string codifSystem = this.Page.Request.QueryString["CID"];
                if (String.IsNullOrEmpty(codifSystem))
                {
                    MsgLiteral.Text = Localization.GetResource(ResourceMessage.DOCUMENT_PICKER_NO_CODIFICATION, ResourceFiles.MESSAGE);
                    pageStatusBar.CssClass = "ms-status-red";
                    pageStatusBar.Visible = true;
                    return;
                }

                string refListName = Localization.GetResource(ResourceListKeys.DEPLOIEMENT_LISTNAME, ResourceFiles.CORE);
                SPList dep = SPContext.Current.Web.Lists[refListName];
                IEnumerable<SPListItem> items = CodificationHelper.GetItemsForCodification(dep, codifSystem);

                if (items.Count() == 0)
                {
                    MsgLiteral.Text = Localization.GetResource(ResourceMessage.DOCUMENT_PICKER_NO_FILE_FOUND, ResourceFiles.MESSAGE);
                    pageStatusBar.CssClass = "ms-status-red";
                    pageStatusBar.Visible = true;
                    return;
                }

                if (items.Count() == 1)
                {
                   // redirect to form
                    RedirectToUrl(items.First().ID.ToString());
                }

                ListView1.DataSource = items;
                ListView1.DataBind();
            }
        }

        protected string GetFileName(object item)
        {
            SPListItem i = item as SPListItem;
            if(i != null && i.File != null) return i.File.Name;
            return String.Empty;
        }

        protected string GetUserName(object login)
        {
            return SPContext.Current.Web.EnsureUser(login.ToString()).Name;
        }

        protected void SelectBtn_Click(object sender, EventArgs e)
        {
            RedirectToUrl((sender as Button).ToolTip);
        }
        protected void RedirectToUrl(string depItemID)
        {
            string url = SPContext.Current.Web.ServerRelativeUrl + "/_layouts/15/speedeau/newdeploiement.aspx";
            string queryString = "ID=" + depItemID;
            SPUtility.Redirect(url, SPRedirectFlags.Static, HttpContext.Current, queryString);
        }


    }
}
