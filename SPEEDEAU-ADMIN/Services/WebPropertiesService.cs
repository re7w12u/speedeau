using Microsoft.Practices.SharePoint.Common.ServiceLocation;
using Microsoft.SharePoint;
using SPEEDEAU.ADMIN.Model;
using SPEEDEAU.ADMIN.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPEEDEAU.ADMIN.Services
{
    public class WebPropertiesService : ServiceBase, IWebProperties
    {
        public void Set(string key, string value)
        {
            Set(key, value, SPContext.Current.Web);
        }

        public void Set(string key, string value, SPWeb web)
        {
            if (value != null)
            {
                if (web.AllProperties.ContainsKey(key)) web.SetProperty(key, value);
                else web.AddProperty(key, value);
                web.Update();
            }
        }

        public string Get(string key)
        {
            return Get(key, SPContext.Current.Web);
        }


        public string Get(string key, SPWeb web)
        {
            if (web.AllProperties.ContainsKey(key))
            {
                object o = web.GetProperty(key);
                if (o != null) return o.ToString();
            }
            return String.Empty;
        }
    }
}
