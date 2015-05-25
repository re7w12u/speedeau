using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Reflection;
using SPEEDEAU.ADMIN.Services;
using SPEEDEAU.ADMIN.Util;
using SPEEDEAU.ADMIN.Model;


namespace SPEEDEAU.ADMIN.Model
{
    [Serializable]
    [DataContract]
    public class Deploiement : IH1600DOC, IComparable
    {
        public Deploiement()
        {

        }
        
        public byte[] File { get; set; }

        [DataMember]
        [SpeedeauField(IsTaxon = false, IsProperty = true, PropertyName = "ID")]
        public int ID { get; set; }

        [DataMember]
        [SpeedeauField(IsTaxon = false, IsProperty =true, PropertyName = "Name")]
        public string FileName { get; set; }
        
        [DataMember]
        [SpeedeauField(ResourceFieldsKeys.TITLE, ResourceFiles.SPCORE, IsTaxon = false, IsComparable = true)]
        public string Title { get; set; }

        private string _codificationSystem;
        [DataMember]
        [SpeedeauField(ResourceFieldsKeys.CODIFICATION_SYSTEM, ResourceFiles.FIELDS, IsTaxon = false, IsComparable = true)]
        public string CodificationSystem
        {
            get
            {
                return _codificationSystem;
            }
            set
            {
                _codificationSystem = CodificationHelper.CleanUpCodification(value);
            }
        }

        [DataMember]
        [SpeedeauField(ResourceFieldsKeys.CODIFICATION, ResourceFiles.FIELDS, IsTaxon = false)]
        public string Codification { get; set; }

        [DataMember]
        [SpeedeauField(ResourceFieldsKeys.INDICE, ResourceFiles.FIELDS, IsTaxon = false)]
        public string Indice { get; set; }

        [DataMember]
        [SpeedeauField(ResourceFieldsKeys.REVISION, ResourceFiles.FIELDS, IsTaxon = false, AllowStringEmpty = true)]
        public string Revision { get; set; }

        [DataMember]
        [SpeedeauField(ResourceFieldsKeys.STATUS_DOC, ResourceFiles.FIELDS, IsTaxon = false)]
        public StatusIH1600 Status { get; set; }

        [DataMember]
        [SpeedeauField(ResourceFieldsKeys.FAMILLE_DOC, ResourceFiles.FIELDS, IsTaxon = true, IsComparable = true)]
        public TaxonomyValue FamilleDoc { get; set; }

        [DataMember]
        [SpeedeauField(ResourceFieldsKeys.NATURE_DOC, ResourceFiles.FIELDS, IsTaxon = true, IsComparable = true)]
        public TaxonomyValue NatureDoc { get; set; }

        [DataMember]
        [SpeedeauField(ResourceFieldsKeys.PROJECT_TAXON, ResourceFiles.FIELDS, IsTaxon = true, IsTaxonMulti = true, IsComparable = true)]
        public TaxonomyValue Projet { get; set; }

        [DataMember]
        [SpeedeauField(ResourceFieldsKeys.THEME_TAXON, ResourceFiles.FIELDS, IsTaxon = true)]
        public TaxonomyValue Theme { get; set; }

        [DataMember]
        [SpeedeauField(ResourceFieldsKeys.SITE_TAXON, ResourceFiles.FIELDS, IsTaxon = true, IsComparable = true)]
        public TaxonomyValue Site { get; set; }
        
        [DataMember]
        [SpeedeauField(ResourceFieldsKeys.VERIFICATION, ResourceFiles.FIELDS, IsTaxon = false)]
        public  ValidationStatus ValidationStatus { get; set; }

        [DataMember]
        [SpeedeauField(ResourceFieldsKeys.PROCESSIH1600, ResourceFiles.FIELDS, IsTaxon = false)]
        public bool ProcessIH1600 { get; set; }

        public int CompareTo(object obj)
        {
            if(obj == null) return 1;

            Deploiement other = obj as Deploiement;
            if(other == null) throw new ArgumentException("obj is not a Deploiement");

            int result = Title.CompareTo(other.Title);
            if (result != 0) return result;

            result = FamilleDoc.CompareTo(other.FamilleDoc);
            if (result != 0) return result;

            result = NatureDoc.CompareTo(other.NatureDoc);
            if (result != 0) return result;

            result = Projet.CompareTo(other.Projet);
            if (result != 0) return result;

            result = Site.CompareTo(other.Site);
            
            return result;
        }
    }

    

}
