using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPEEDEAU.ADMIN.Util
{
    public static class EnumHelper
    {
        public static T GetValue<T>(string enumValue)
        {
            if (!typeof(T).IsEnum) throw new ArgumentException("T must be an enumerated type");
            return (T)Enum.Parse(typeof(T), enumValue, true);
        }

        public static T GetValue<T>(string enumValue, T defaultValue)
        {
            if (String.IsNullOrWhiteSpace(enumValue)) return defaultValue;
            if (!typeof(T).IsEnum) return defaultValue;
            return (T)Enum.Parse(typeof(T), enumValue, true);
        }




    }
}
