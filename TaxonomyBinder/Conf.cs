using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TaxonomyBinder
{
    public class Conf
    {
        
        private static Conf _conf;

        public static Conf Instance
        {
            get
            {
                if (_conf == null)
                {
                    _conf = new Conf();
                    _conf.Load();
                }

                return _conf;
            }
        }

        public List<Bind> Binds { get; set; }
        public string MMS_Name { get; set; }
        public string MMS_ID { get; set; }
        public string Group_Name { get; set; }
        public string Group_ID { get; set; }
        public string SiteUrl { get; set; }

        private void Load()
        {            
            string path = "conf.xml";
            XElement xml = XElement.Load(path);

            MMS_Name = xml.Element("mms_name").Attribute("name").Value;
            MMS_ID = xml.Element("mms_name").Attribute("id").Value;
            Group_Name = xml.Element("group_name").Attribute("name").Value;
            Group_ID = xml.Element("group_name").Attribute("id").Value;
            SiteUrl = xml.Element("siteurl").Value;

            Binds = new List<Bind>();
            BindBuilder builder = new BindBuilder();
            foreach (XElement x in xml.Descendants("field"))
            {
                Binds.Add(builder.Build(x));
            }
        }

    }
}
