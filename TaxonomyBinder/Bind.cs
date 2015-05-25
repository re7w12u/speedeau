using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TaxonomyBinder
{
    public class Bind
    {
        public string Name { get; set; }
        public Guid ID { get; set; }
        public string TermSetName { get; set; }
        public Guid TermSetID { get; set; }
    }

    //public class BindFieldAttribute : Attribute
    //{
    //    public string FieldName { get; set; }

    //    public BindFieldAttribute(string fieldName)
    //    {
    //        FieldName = fieldName;
    //    }
    //}

    public class BindBuilder
    {
        public Bind Bind { get; set; }
        public Bind Build(XElement xml)
        {
            Bind = new Bind();

            Bind.Name = xml.Attribute("name").Value;
            Bind.ID = Guid.Parse(xml.Attribute("id").Value);
            //Bind.TermSetID = Guid.Parse(xml.Attribute("termsetid").Value);
            Bind.TermSetName = xml.Attribute("termsetname").Value;

            return Bind;
        }
    }
}
