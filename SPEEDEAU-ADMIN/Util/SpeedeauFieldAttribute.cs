using SPEEDEAU.ADMIN.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPEEDEAU.ADMIN.Util
{
    public class SpeedeauFieldAttribute : Attribute
    {
        /// <summary>
        /// Resource Key for field name
        /// </summary>
        public string ResxKey { get; set; }
        /// <summary>
        /// Resource file for field name
        /// </summary>
        public string ResxFile { get; set; }
        /// <summary>
        /// indicates that the field has to be proceed as TaxonomyValue
        /// </summary>
        public bool IsTaxon { get; set; }
        public bool IsTaxonMulti { get; set; }
        /// <summary>
        /// indicates that the field store a value of a nativ SP Field (i.e. Title or ID)
        /// </summary>
        public bool IsProperty { get; set; }
        /// <summary>
        /// related to IsProperty. Indicates the name of the property
        /// </summary>
        public string PropertyName { get; set; }
        public bool IsComparable { get; set; }
        public bool AllowStringEmpty { get; set; }
        /// <summary>
        /// indicates whether the field has to be proceed as a ComplexType (i.e. List<PickerEntity>)
        /// </summary>
        public bool IsComplexType { get; set; }
        /// <summary>
        /// Class to instantiate to handle field
        /// </summary>
        public Type ComplexTypeHandler { get; set; }
        public SpeedeauFieldAttribute()
        {

        }
        public SpeedeauFieldAttribute(string fieldName, string resxFile)
        {
            ResxKey = fieldName;
            ResxFile = resxFile;
        }
    }
}
