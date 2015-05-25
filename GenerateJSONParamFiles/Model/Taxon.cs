using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GenerateJSONParamFiles.Model
{
    [DataContract]
    class MMS
    {
        public MMS()
        {
            Groups = new List<TaxonGroup>();
        }
        [DataMember]
        public Guid ID { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public List<TaxonGroup> Groups { get; set; }
    }

    [DataContract]
    class TaxonGroup
    {
        public TaxonGroup()
        {
            TaxonSets = new List<TaxonSet>();
        }
        [DataMember]
        public Guid ID { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public List<TaxonSet> TaxonSets { get; set; }
    }

    [DataContract]
    class TaxonSet
    {
        public TaxonSet()
        {
            Taxons = new List<Taxon>();
        }
        [DataMember]
        public Guid ID { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public List<Taxon> Taxons { get; set; }
    }

    [DataContract]
    class Taxon
    {
        public Taxon()
        {
            Taxons = new List<Taxon>();
        }
        [DataMember]
        public Guid ID { get; set; }
        [DataMember]
        public string Label { get; set; }
        [DataMember]
        public List<Taxon> Taxons { get; set; }
        [DataMember]
        public bool HasChild { get; set; }
    }
}
