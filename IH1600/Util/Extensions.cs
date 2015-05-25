using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IH1600.Util
{
    public static class Extensions
    {
        public static T EnsureValue<T>(this SPItemEventDataCollection data, string fieldName)
        {
            if (data[fieldName] != null)
            {
                return (T)data[fieldName];
            }
            return default(T);
        }

        public static T EnsureValue<T>(this SPListItem item, string fieldName)
        {
            if (item.Fields.ContainsField(fieldName) && item[fieldName] != null)
            {
                return (T)item[fieldName];
            }
            return default(T);
        }
    }
}
