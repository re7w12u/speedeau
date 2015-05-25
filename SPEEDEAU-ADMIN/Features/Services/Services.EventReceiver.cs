using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using SPEEDEAU;
using Microsoft.Practices.SharePoint.Common.ServiceLocation;
using Microsoft.SharePoint.Administration;
using Microsoft.Practices.ServiceLocation;
using SPEEDEAU.ADMIN.Services;

namespace SPEEDEAU.ADMIN.Features.Services
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("e01bbd01-59b8-4d69-83e4-ac4f4a45077d")]
    public class ServicesEventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            try
            {
                FeatureDeactivating(properties);

                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SPEEDEAU", TraceSeverity.Medium, EventSeverity.Information), TraceSeverity.Medium, "SPEEDEAU Custom service Activation start");
                //SPSite site = properties.Feature.Parent as SPSite;
                IServiceLocator serviceLocator = SharePointServiceLocator.GetCurrent();
                IServiceLocatorConfig config = serviceLocator.GetInstance<IServiceLocatorConfig>();
                
                config.RegisterTypeMapping<IDeploiementService, DeploiementService>();
                config.RegisterTypeMapping<ISuiviService, SuiviService>();
                config.RegisterTypeMapping<IObservationService, ObservationsService>();
                config.RegisterTypeMapping<IWebProperties, WebPropertiesService>();
                config.RegisterTypeMapping<IAlerteService, AlerteService>();
                config.RegisterTypeMapping<IReferentielService, ReferentielService>();
                config.RegisterTypeMapping<IPermissionsService, PermissionsService>();
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SPEEDEAU", TraceSeverity.Medium, EventSeverity.Information), TraceSeverity.Medium, "SPEEDEAU Custom service Activation OK");
            }
            catch (Exception err)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SPEEDEAU", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, "SPEEDEAU Custom service Activation failed", err.Message, err.StackTrace);
            }
        }


        // Uncomment the method below to handle the event raised before a feature is deactivated.


        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            try
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SPEEDEAU", TraceSeverity.Medium, EventSeverity.Information), TraceSeverity.Medium, "SPEEDEAU Custom service uninstalling");
                //SPSite site = properties.Feature.Parent as SPSite;
                IServiceLocator serviceLocator = SharePointServiceLocator.GetCurrent();
                IServiceLocatorConfig config = serviceLocator.GetInstance<IServiceLocatorConfig>();

                config.RemoveTypeMappings<DeploiementService>();
                config.RemoveTypeMappings<SuiviService>();
                config.RemoveTypeMappings<ObservationsService>();
                config.RemoveTypeMappings<WebPropertiesService>();
                config.RemoveTypeMappings<AlerteService>();
                config.RemoveTypeMappings<ReferentielService>();
                config.RemoveTypeMappings<PermissionsService>();
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SPEEDEAU", TraceSeverity.Medium, EventSeverity.Information), TraceSeverity.Medium, "SPEEDEAU Custom service uninstalled");
            }
            catch (Exception err)
            {
                SPDiagnosticsService.Local.WriteTrace(0, new SPDiagnosticsCategory("SPEEDEAU", TraceSeverity.Unexpected, EventSeverity.Error), TraceSeverity.Unexpected, "SPEEDEAU Custom service uninstalling failed", err.Message, err.StackTrace);
            }
        }

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
