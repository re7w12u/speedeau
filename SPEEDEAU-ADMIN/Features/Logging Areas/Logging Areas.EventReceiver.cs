using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.Practices.SharePoint.Common.Logging;
using SPEEDEAU;
using Microsoft.SharePoint.Administration;
using Microsoft.Practices.SharePoint.Common.ServiceLocation;
using Microsoft.Practices.SharePoint.Common.Configuration;

namespace SPEEDEAU.ADMIN.Features.Logging_Areas
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("9e4a76f7-3e4b-456f-b44e-f46670af3935")]
    public class Logging_AreasEventReceiver : SPFeatureReceiver
    {
        private DiagnosticsAreaCollection _speedeauAreas = null;
        public System.Collections.Generic.IEnumerable<DiagnosticsArea> SpeedeauAreas
        {
            get
            {
                if (_speedeauAreas == null)
                {
                    _speedeauAreas = new DiagnosticsAreaCollection();

                    DiagnosticsArea area = new DiagnosticsArea(LoggerManager.AreaName);
                    foreach (LoggerCategory category in Enum.GetValues(typeof(LoggerCategory)))
                    {
                        area.DiagnosticsCategories.Add(new DiagnosticsCategory(LoggerManager.CategoryName(category),
                                                        EventSeverity.ErrorCritical,
                                                        TraceSeverity.High));
                    }
                    _speedeauAreas.Add(area);
                }
                return _speedeauAreas;
            }
        }

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            IConfigManager configMgr = SharePointServiceLocator.GetCurrent().GetInstance<IConfigManager>();
            DiagnosticsAreaCollection configuredAreas = new DiagnosticsAreaCollection(configMgr);

            foreach (DiagnosticsArea newArea in SpeedeauAreas)
            {
                var existingArea = configuredAreas[newArea.Name];

                if (existingArea == null)
                {
                    configuredAreas.Add(newArea);
                }
                else
                {
                    foreach (DiagnosticsCategory c in newArea.DiagnosticsCategories)
                    {
                        var existingCategory = existingArea.DiagnosticsCategories[c.Name];
                        if (existingCategory == null)
                        {
                            existingArea.DiagnosticsCategories.Add(c);
                        }
                    }
                }
            }
            configuredAreas.SaveConfiguration();
        }


        // Uncomment the method below to handle the event raised before a feature is deactivated.

        //public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        //{
        //}


        // Uncomment the method below to handle the event raised after a feature has been installed.

        //public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        //{
        //}


        // Uncomment the method below to handle the event raised before a feature is uninstalled.

        //public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        //{
        //}

        // Uncomment the method below to handle the event raised when a feature is upgrading.

        //public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
        //{
        //}
    }
}
