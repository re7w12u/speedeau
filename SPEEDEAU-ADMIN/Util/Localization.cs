using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPEEDEAU.ADMIN
{
    public class Localization
    {
        /// <summary>
        /// Default resource file (core is the default WSS resource file)
        /// </summary>
        private const string DEFAULTRESOURCESFILE = "speedeau.core";

        /// <summary>
        /// Resource internal prefix
        /// </summary>
        private const string RESOURCESPREFIX = "$Resources:";

        /// <summary>
        /// Resource file
        /// </summary>
        private readonly string _ResFile = DEFAULTRESOURCESFILE;

        /// <summary>
        /// Default LCID
        /// </summary>
        private readonly uint LCID = 0;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the Localization class
        /// Use current website's language and "core" as default resource file 
        /// </summary>
        public Localization()
        {
            LCID = SPContext.Current.Web.Language;
        }

        /// <summary>
        /// Initializes a new instance of the Localization class
        /// Use current website's language and a specific resource file 
        /// </summary>
        /// <param name="defaultResourceFile">Default Resource filename</param>
        public Localization(string defaultResourceFile)
        {
            _ResFile = defaultResourceFile;
            LCID = SPContext.Current.Web.Language;
        }

        /// <summary>
        /// Initializes a new instance of the Localization class
        /// Use a specific language and the default "core" resource file 
        /// </summary>
        /// <param name="lcid">Culture ID</param>
        public Localization(uint lcid)
        {
            this.LCID = lcid;
        }

        /// <summary>
        /// Initializes a new instance of the Localization class
        /// Use specific language and resource file 
        /// </summary>
        /// <param name="defaultResourceFile">Default Resource filename</param>
        /// <param name="lcid">Culture ID</param>
        public Localization(string defaultResourceFile, uint lcid)
        {
            _ResFile = defaultResourceFile;
            this.LCID = lcid;
        }
        #endregion

        /// <summary>
        /// Get language of the current SPWeb
        /// </summary>
        /// <returns>Language of the current SPWeb</returns>
        public static uint GetLcid()
        {
            return (uint)System.Threading.Thread.CurrentThread.CurrentUICulture.LCID;

            // if(SPContext.Current == null)
            //     return 1033;
            // if (SPContext.Current.Web != null)
            //     return SPContext.Current.Web.Language;
            // return 1033;
        }

        /// <summary>
        /// Get language of the current Culture Info
        /// </summary>
        /// <returns>Language of the current Culture Info</returns>
        public static CultureInfo GetCultureInfo()
        {
            return new CultureInfo((int)GetLcid());
        }

        /// <summary>
        /// Get string from key, resource file and language
        /// </summary>
        /// <param name="resKey">Resource key</param>
        /// <param name="resFile">Resource file ("core" for core.resx, core.en-US.resx, etc)</param>
        /// <returns>String from key, resource file and language</returns>
        public static string GetResource(string resKey, string resFile)
        {
            uint lcid = GetLcid();
            string resValue = SPUtility.GetLocalizedString(RESOURCESPREFIX + resKey, resFile, lcid);
            if (resValue == RESOURCESPREFIX + resKey && lcid != 0)
            {
                resValue = SPUtility.GetLocalizedString(RESOURCESPREFIX + resKey, resFile, 0);
            }

            return resValue;
        }

        /// <summary>
        /// Get string from key, resource file and language
        /// </summary>
        /// <param name="resKey">Resource key</param>
        /// <param name="resFile">Resource file ("core" for core.resx, core.en-US.resx, etc)</param>
        /// <param name="lcid">Resource LCID</param>
        /// <returns>String from key, resource file and language</returns>
        public static string GetResource(string resKey, string resFile, uint lcid)
        {
            string resValue = SPUtility.GetLocalizedString(RESOURCESPREFIX + resKey, resFile, lcid);
            if (resValue == RESOURCESPREFIX + resKey && lcid != 0)
            {
                resValue = SPUtility.GetLocalizedString(RESOURCESPREFIX + resKey, resFile, 0);
            }

            return resValue;
        }

        /// <summary>
        /// Get string from resource key
        /// </summary>
        /// <param name="resKey">Resource key</param>
        /// <returns>String from resource key</returns>
        public string GetResource(string resKey)
        {
            if (_ResFile != null)
            {
                return GetResource(resKey, _ResFile, LCID);
            }
            return resKey;
        }
    }

}
