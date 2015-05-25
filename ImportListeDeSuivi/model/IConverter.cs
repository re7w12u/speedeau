using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportListeDeSuivi.model
{
    public interface IConverter
    {
        object Convert(string value);
    }


    public class BoolConverter : IConverter
    {
        public object Convert(string value)
        {
            return System.Convert.ToBoolean(value);
        }
    }

    public class IntConverter : IConverter
    {
        public object Convert(string value)
        {
            return System.Convert.ToInt32(value);
        }
    }

    public class GenericConverter : IConverter
    {
        public object Convert<T>(string value)
        {
            return System.Convert.ChangeType(value, typeof(T));
        }
    }


}
