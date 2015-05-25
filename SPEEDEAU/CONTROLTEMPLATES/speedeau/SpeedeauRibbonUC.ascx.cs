using Microsoft.Practices.SharePoint.Common.ServiceLocation;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using SPEEDEAU.ADMIN.Services;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace SPEEDEAU.CONTROLTEMPLATES.speedeau
{
    public partial class SpeedeauRibbonUC : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SPList list = SPContext.Current.List;
            if (list != null)
            {
                int template = (int)list.BaseTemplate;
                if (template == 10100 || template == 10101)
                {
                    HideTabs("Ribbon.Library", "Ribbon.Document");
                }
                else if (template == 20300)
                {
                    HideTabs("Ribbon.List", "Ribbon.ListItem");
                }
            }
        }

        private void HideTabs(params string[] ids)
        {
            IPermissionsService permSvc = SharePointServiceLocator.GetCurrent().GetInstance<IPermissionsService>();
            bool showMenu = permSvc.CanViewMenu();
            if (!showMenu)
            {
                SiteActions siteActions = SiteActions.GetCurrent(this.Page);
                if (siteActions != null) siteActions.Visible = false;

                SPRibbon ribbon = SPRibbon.GetCurrent(this.Page);
                if (ribbon != null)
                {
                    foreach (string id in ids)
                    {
                        ribbon.TrimById(id);
                    }
                }
            }
        }
    }
}
