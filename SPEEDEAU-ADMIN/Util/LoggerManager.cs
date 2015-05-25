using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.SharePoint.Common.Logging;
using Microsoft.Practices.SharePoint.Common.ServiceLocation;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPEEDEAU.ADMIN
{

    public enum LoggerCategory { ApplicationPage, Features, Fields, Event, IIS, Job, Security, Services, WebPart, Deploiement, ListeTypeB5, Referentiel, Observations, Alertes, Other }

    /// <summary>
    /// Class to manage Logger
    /// </summary>
    public static class LoggerManager
    {
        private const string CORE_RESX = "speedeau.core";
        private const string DIAGNOSTICSAREA = "DiagnosticsArea";

        private const string DIAGNOSTICSCATEGORY_APPLICATIONPAGE = "DiagnosticsArea_ApplicationPage";
        private const string DIAGNOSTICSCATEGORY_FEATURES = "DiagnosticsArea_Features";
        private const string DIAGNOSTICSCATEGORY_FIELDS = "DiagnosticsArea_Fields";
        private const string DIAGNOSTICSCATEGORY_EVENTS = "DiagnosticsArea_Events";
        private const string DIAGNOSTICSCATEGORY_IIS = "DiagnosticsArea_IIS";
        private const string DIAGNOSTICSCATEGORY_JOB = "DiagnosticsArea_Job";
        private const string DIAGNOSTICSCATEGORY_SECURITY = "DiagnosticsArea_Security";
        private const string DIAGNOSTICSCATEGORY_SERVICES = "DiagnosticsArea_Services";
        private const string DIAGNOSTICSCATEGORY_WEBPART = "DiagnosticsArea_WebPart";
        private const string DIAGNOSTICSCATEGORY_OTHER = "DiagnosticsArea_Other";
        private const string DIAGNOSTICSCATEGORY_DEPLOIEMENT = "DiagnosticsArea_Deploiement";
        private const string DIAGNOSTICSCATEGORY_LISTETYPEB5 = "DiagnosticsArea_ListeTypeB5";
        private const string DIAGNOSTICSCATEGORY_REFERENTIEL = "DiagnosticsArea_Referentiel";
        private const string DIAGNOSTICSCATEGORY_OBSERVATIONS = "DiagnosticsArea_Observations";
        private const string DIAGNOSTICSCATEGORY_ALERTES = "DiagnosticsArea_Alertes";
        private const string ERROR_EVENTID = "Error_EventId";


        private static ILogger _logger = null;

        public static int EventId
        {
            get
            {
                return int.Parse(Localization.GetResource(LoggerManager.ERROR_EVENTID, LoggerManager.CORE_RESX));
            }
        }

        public static string AreaFullName(LoggerCategory category)
        {
            return string.Format(@"{0}/{1}", AreaName, CategoryName(category));
        }

        public static string AreaName
        {
            get
            {
                return Localization.GetResource(LoggerManager.DIAGNOSTICSAREA, LoggerManager.CORE_RESX);
            }
        }

        public static string CategoryName(LoggerCategory category)
        {
            string categoryName = string.Empty;
            switch (category)
            {
                case LoggerCategory.ApplicationPage:
                    categoryName = Localization.GetResource(LoggerManager.DIAGNOSTICSCATEGORY_APPLICATIONPAGE, LoggerManager.CORE_RESX);
                    break;
                case LoggerCategory.Features:
                    categoryName = Localization.GetResource(LoggerManager.DIAGNOSTICSCATEGORY_FEATURES, LoggerManager.CORE_RESX);
                    break;
                case LoggerCategory.Fields:
                    categoryName = Localization.GetResource(LoggerManager.DIAGNOSTICSCATEGORY_FIELDS, LoggerManager.CORE_RESX);
                    break;
                case LoggerCategory.Event:
                    categoryName = Localization.GetResource(LoggerManager.DIAGNOSTICSCATEGORY_EVENTS, LoggerManager.CORE_RESX);
                    break;
                case LoggerCategory.IIS:
                    categoryName = Localization.GetResource(LoggerManager.DIAGNOSTICSCATEGORY_IIS, LoggerManager.CORE_RESX);
                    break;
                case LoggerCategory.Job:
                    categoryName = Localization.GetResource(LoggerManager.DIAGNOSTICSCATEGORY_JOB, LoggerManager.CORE_RESX);
                    break;
                case LoggerCategory.Security:
                    categoryName = Localization.GetResource(LoggerManager.DIAGNOSTICSCATEGORY_SECURITY, LoggerManager.CORE_RESX);
                    break;
                case LoggerCategory.Services:
                    categoryName = Localization.GetResource(LoggerManager.DIAGNOSTICSCATEGORY_SERVICES, LoggerManager.CORE_RESX);
                    break;
                case LoggerCategory.WebPart:
                    categoryName = Localization.GetResource(LoggerManager.DIAGNOSTICSCATEGORY_WEBPART, LoggerManager.CORE_RESX);
                    break;
                case LoggerCategory.Other:
                    categoryName = Localization.GetResource(LoggerManager.DIAGNOSTICSCATEGORY_OTHER, LoggerManager.CORE_RESX);
                    break;
                case LoggerCategory.Deploiement:
                    categoryName = Localization.GetResource(LoggerManager.DIAGNOSTICSCATEGORY_DEPLOIEMENT, LoggerManager.CORE_RESX);
                    break;
                case LoggerCategory.ListeTypeB5:
                    categoryName = Localization.GetResource(LoggerManager.DIAGNOSTICSCATEGORY_LISTETYPEB5, LoggerManager.CORE_RESX);
                    break;
                case LoggerCategory.Referentiel:
                    categoryName = Localization.GetResource(LoggerManager.DIAGNOSTICSCATEGORY_REFERENTIEL, LoggerManager.CORE_RESX);
                    break;
                case LoggerCategory.Observations:
                    categoryName = Localization.GetResource(LoggerManager.DIAGNOSTICSCATEGORY_OBSERVATIONS, LoggerManager.CORE_RESX);
                    break;
                case LoggerCategory.Alertes:
                    categoryName = Localization.GetResource(LoggerManager.DIAGNOSTICSCATEGORY_ALERTES, LoggerManager.CORE_RESX);
                    break;
            }
            return categoryName;
        }

        public static void RegisterLogger(SPSite site)
        {
            IServiceLocatorConfig typeMappings = SharePointServiceLocator.GetCurrent().GetInstance<IServiceLocatorConfig>();
            typeMappings.Site = site;
            typeMappings.RegisterTypeMapping<ILogger, SharePointLogger>();
            SharePointServiceLocator.Reset();
        }

        public static void UnRegisterLogger(SPSite site)
        {
            IServiceLocatorConfig typeMappings = SharePointServiceLocator.GetCurrent().GetInstance<IServiceLocatorConfig>();
            typeMappings.Site = site;
            typeMappings.RemoveTypeMapping<ILogger>(null);
        }

        public static ILogger Logger
        {

            get
            {
                if (_logger == null)
                {
                    IServiceLocator sl = SharePointServiceLocator.GetCurrent();
                    _logger = sl.GetInstance<ILogger>();
                }
                return _logger;
            }
        }


        public static void Debug(LoggerCategory category, string pattern, params object[] param)
        {
            Logger.TraceToDeveloper(String.Format(pattern, param), EventId, TraceSeverity.Verbose, AreaFullName(category));
        }

        public static void Warn(LoggerCategory category, string pattern, params object[] param)
        {
            Logger.TraceToDeveloper(String.Format(pattern, param), EventId, TraceSeverity.High, AreaFullName(category));
        }


        public static void Error(LoggerCategory category, string pattern, params object[] param)
        {
            Logger.TraceToDeveloper(String.Format(pattern, param), EventId, TraceSeverity.Unexpected, AreaFullName(category));
        }

        public static void Error(LoggerCategory category, Exception exception)
        {
            Logger.TraceToDeveloper(exception, EventId, TraceSeverity.Unexpected, AreaFullName(category));
        }

    }

}
