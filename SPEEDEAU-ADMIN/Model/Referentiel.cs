using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPEEDEAU.ADMIN.Util;
using SPEEDEAU.ADMIN.Model;
using System.Runtime.Serialization;
using SPEEDEAU.ADMIN.Util;
using System.Reflection;
using SPEEDEAU.ADMIN.Services;


namespace SPEEDEAU.ADMIN.Model
{
    [Serializable]
    [DataContract]
    public class Referentiel : IH1600DOC, IComparable
    {
        public Referentiel()
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
        [SpeedeauField(ResourceFieldsKeys.ENSEMBLE_COHERENT, ResourceFiles.FIELDS, IsTaxon = true, IsComparable = true)]
        public TaxonomyValue EC { get; set; }
        
        [DataMember]
        [SpeedeauField(ResourceFieldsKeys.VERIFICATION, ResourceFiles.FIELDS, IsTaxon = false)]
        public  ValidationStatus ValidationStatus { get; set; }

        [DataMember]
        [SpeedeauField(ResourceFieldsKeys.PROCESSIH1600, ResourceFiles.FIELDS, IsTaxon = false)]
        public bool ProcessIH1600 { get; set; }

        public int CompareTo(object obj)
        {
            if(obj == null) return 1;

            Referentiel other = obj as Referentiel;
            if (other == null) throw new ArgumentException("obj is not a Referentiel");

            int result = Title.CompareTo(other.Title);
            if (result != 0) return result;

            result = FamilleDoc.CompareTo(other.FamilleDoc);
            if (result != 0) return result;

            result = NatureDoc.CompareTo(other.NatureDoc);
            if (result != 0) return result;

            result = Projet.CompareTo(other.Projet);
            if (result != 0) return result;

            result = EC.CompareTo(other.EC);
            
            return result;
        }
    }

    

}
