using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPEEDEAU.ADMIN.Model;
using Microsoft.SharePoint.Utilities;
using System.Globalization;
using System.Web;

namespace SPEEDEAU.ADMIN.Util
{
    public static class SpeedeauExtensions
    {

        #region SPListItemVersion Get Value
        public static T EnsureValue<T>(this SPListItemVersion item, string fieldName)
        {
            if (item.Fields.ContainsField(fieldName) && item[fieldName] != null)
            {
                return (T)Convert.ChangeType(item[fieldName], typeof(T));
            }
            else
            {
                return default(T);
            }
        }

        public static T EnsureValue<T>(this SPListItemVersion item, Guid fieldGuid)
        {
            SPField f = item.ListItem.Fields[fieldGuid];
            if (item.Fields.Contains(fieldGuid) && item[f.Title] != null)
            {
                return (T)Convert.ChangeType(item[f.Title], typeof(T));
            }
            return default(T);
        }

        #endregion

        #region SPListItem Get Value
        public static T EnsureValue<T>(this SPListItem item, string fieldName)
        {
            if (item.Fields.ContainsField(fieldName) && item[fieldName] != null)
            {
                return (T)Convert.ChangeType(item[fieldName], typeof(T));
            }
            else
            {
                return default(T);
            }
        }


        public static string EnsureValue(this TaxonomyValue value)
        {
            if (value == null) return String.Empty;
            else return value.ToString();
        }

        public static T EnsureValue<T>(this SPListItem item, Guid fieldGuid)
        {
            if (item.Fields.Contains(fieldGuid) && item[fieldGuid] != null)
            {
                return (T)Convert.ChangeType(item[fieldGuid], typeof(T));
            }
            return default(T);
        }
        #endregion

        #region SPListItem Set Value
        public static void EnsureValue<T>(this SPListItem item, string fieldName, T value, bool AllowEmpty)
        {
            if (item.Fields.ContainsField(fieldName) && value != null)
            {
                if (AllowEmpty || !String.IsNullOrWhiteSpace(value.ToString()))
                {
                    //date value like “01/01/0001 00:00:00” is incorrect for SharePoint - throw “SPException : invalid date/time…”
                    if (value is DateTime) item.EnsureValue(fieldName, Convert.ToDateTime(value));
                    else item[fieldName] = value;
                }
            }
        }

        public static void EnsureValue<T>(this SPListItem item, Guid fieldGuid, T value, bool AllowEmpty)
        {
            if (item.Fields.Contains(fieldGuid) && value != null)
            {
                if (AllowEmpty || !String.IsNullOrWhiteSpace(value.ToString()))
                {
                    item[fieldGuid] = value;
                }
            }
        }

        public static void EnsureValue(this SPListItem item, string fieldName, DateTime dateTime)
        {
            if (dateTime > DateTime.MinValue)
            {
                item[fieldName] = SPUtility.CreateISO8601DateTimeFromSystemDateTime(dateTime);
            }
        }

        #endregion

        #region Generic
        public static T EnsureValue<T>(this string input)
        {
            //Type targetType = typeof(T);
            //if (targetType.FullName == typeof(DateTime).FullName)
            //{
            //    if (String.IsNullOrWhiteSpace(input)) return (T)DateTime.MinValue;
            //    //else Convert.ToDateTime
            //}

            //else
            
            
            if (String.IsNullOrWhiteSpace(input)) return default(T);           
            else return (T)Convert.ChangeType(input, typeof(T), new ValueFormatProvider());            
        }
        #endregion
    }


    public class ValueFormatProvider : IFormatProvider
    {
        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(DateTimeFormatInfo)) return new CultureInfo("fr-FR").DateTimeFormat;
            else return null;            
        }
    }


}
