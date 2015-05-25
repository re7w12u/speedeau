using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.Practices.SharePoint.Common.Configuration;
using Microsoft.Practices.SharePoint.Common.Logging;
using System.Web.UI.WebControls;
using Microsoft.Practices.SharePoint.Common.ServiceLocation;
using SPEEDEAU;
using Microsoft.SharePoint.Administration;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using SPEEDEAU.ADMIN.Services;
using SPEEDEAU.ADMIN.Util;

namespace SPEEDEAU.ADMIN.Layouts
{
    public partial class TestServices : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void GetLoggingCategories(object sender, EventArgs e)
        {
            IConfigManager configMgr = SharePointServiceLocator.GetCurrent().GetInstance<IConfigManager>();
            DiagnosticsAreaCollection configuredAreas = new DiagnosticsAreaCollection(configMgr);

            foreach (DiagnosticsArea area in configuredAreas)
            {
                TreeNode node = new TreeNode(area.Name);

                foreach (DiagnosticsCategory c in area.DiagnosticsCategories)
                {
                    node.ChildNodes.Add(new TreeNode(c.Name));
                }

                LogCategoriesList1.Nodes.Add(node);
            }


        }

        protected void GetEventID(object sender, EventArgs e)
        {
            try
            {
                EventIdLabel.Text = LoggerManager.EventId.ToString();
            }
            catch (Exception err)
            {
                EventIdLabel.Text = err.Message;
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SPEEDEAU", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, "TestService.aspx - could not get event id : ");
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SPEEDEAU", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, err.Message);
            }
        }
        protected void LoggingButton_Click(object sender, EventArgs e)
        {
            try
            {
                string text = LoggingText.Text;
                LoggerManager.Debug(LoggerCategory.Deploiement, "Just a test from central Admin : {0}", text);
                LoggingLabel.Text = "OK :)";
            }
            catch (Exception err)
            {
                LoggingLabel.Text = err.Message;
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SPEEDEAU", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, "Logging Test from TestService.aspx Failed : ");
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SPEEDEAU", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, err.Message);                
            }
        }

        protected void GetCustomServices_Click(object sender, EventArgs e)
        {
            IServiceLocator serviceLocator = SharePointServiceLocator.GetCurrent();
            IServiceLocatorConfig config = serviceLocator.GetInstance<IServiceLocatorConfig>();
            List<TypeMapping> types = config.GetTypeMappings();
            TypeMappingGV.DataSource = types;
            TypeMappingGV.DataBind();
        }

        protected void SetWebProperties_Click(object sender, EventArgs e)
        {
            string url = WebPropertiesUrl.Text;
            if (!String.IsNullOrWhiteSpace(url))
            {
                using (SPSite site = new SPSite(url))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        IServiceLocator serviceLocator = SharePointServiceLocator.GetCurrent();
                        IWebProperties webProp = serviceLocator.GetInstance<IWebProperties>();
                        string key = Localization.GetResource(ResourcePropertyBag.WEB_PROPERTYBAG_PROJECT, ResourceFiles.CORE);

                        string value = WebPropertiesValue.Text;
                        if (!String.IsNullOrWhiteSpace(value))
                        {
                            webProp.Set(key, value, web);
                        }
                    }
                }
            }


        }

        protected void GetWebProperties_Click(object sender, EventArgs e)
        {
            string url = WebPropertiesUrl.Text;
            if (!String.IsNullOrWhiteSpace(url))
            {
                using (SPSite site = new SPSite(url))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        IServiceLocator serviceLocator = SharePointServiceLocator.GetCurrent();
                        IWebProperties webProp = serviceLocator.GetInstance<IWebProperties>();
                        string key = Localization.GetResource(ResourcePropertyBag.WEB_PROPERTYBAG_PROJECT, ResourceFiles.CORE);
                        WebPropertiesLabel.Text = webProp.Get(key, web);
                        
                    }
                }
            }

            
        }
    }
}
