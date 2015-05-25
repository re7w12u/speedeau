using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using SPEEDEAU.ADMIN.Model;
using SPEEDEAU.ADMIN.Services;
using SPEEDEAU.ADMIN.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPEEDEAU.ADMIN.Model
{
    /// <summary>
    /// entity used for the create / modification / duplication of items in liste de suivi
    /// </summary>
    public class Suivi : IH1600DOC
    {

        public Suivi()
        {
            Verificateur = new List<PickerEntity>();
            Approbateur = new List<PickerEntity>();
        }

        [SpeedeauField(IsTaxon = false, IsProperty = true, PropertyName = "ID")]
        public int ID { get; set; }

        [SpeedeauField(IsTaxon = false, IsProperty = true, PropertyName = "Title", ResxKey = ResourceFieldsKeys.TITLE, ResxFile = ResourceFiles.SPCORE)]
        public string Title { get; set; }

        [SpeedeauField(ResourceFieldsKeys.OPERATION, ResourceFiles.FIELDS, IsTaxon = false)]
        public string Operation { get; set; }

        [SpeedeauField(ResourceFieldsKeys.FAMILLE_DOC, ResourceFiles.FIELDS, IsTaxon = true, IsComparable = true)]
        public TaxonomyValue FamilleDoc { get; set; }

        [SpeedeauField(ResourceFieldsKeys.NATURE_DOC, ResourceFiles.FIELDS, IsTaxon = true, IsComparable = true)]
        public TaxonomyValue NatureDoc { get; set; }

        [SpeedeauField(ResourceFieldsKeys.REQUIS, ResourceFiles.FIELDS, IsTaxon = false)]
        public bool Requis { get; set; }

        [SpeedeauField(ResourceFieldsKeys.CODIFICATION, ResourceFiles.FIELDS, IsTaxon = false)]
        public string Codification { get; set; }

        private string _codificationSystem;
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

        [SpeedeauField(ResourceFieldsKeys.INDICE, ResourceFiles.FIELDS, IsTaxon = false)]
        public string Indice { get; set; }

        [SpeedeauField(ResourceFieldsKeys.OBSERVATION_RNVO, ResourceFiles.FIELDS, IsTaxon = false)]
        public string ObservationsRNVO { get; set; }

        [SpeedeauField(ResourceFieldsKeys.FOURNITURE, ResourceFiles.FIELDS, IsTaxon = false)]
        public string Fourniture { get; set; }

        [SpeedeauField(ResourceFieldsKeys.DATECIBLE, ResourceFiles.FIELDS, IsTaxon = false)]
        public DateTime DateCible { get; set; }

        [SpeedeauField(ResourceFieldsKeys.FORMAT_DEMANDE, ResourceFiles.FIELDS, IsTaxon = false)]
        public string FormatDemande { get; set; }

        [SpeedeauField(ResourceFieldsKeys.TEMPS_ESTIME, ResourceFiles.FIELDS, IsTaxon = false)]
        public int TempsEstime { get; set; }

        [SpeedeauField(ResourceFieldsKeys.RESTE_A_FAIRE, ResourceFiles.FIELDS, IsTaxon = false)]
        public int ResteAFaire { get; set; }

        [SpeedeauField(ResourceFieldsKeys.REDACTION, ResourceFiles.FIELDS, IsTaxon = true)]
        public TaxonomyValue Redaction { get; set; }

        [SpeedeauField(ResourceFieldsKeys.VERIFICATEUR, ResourceFiles.FIELDS, IsTaxon = false, IsComplexType = true, ComplexTypeHandler = typeof(PickerEntityHandler))]
        public List<PickerEntity> Verificateur { get; set; }

        [SpeedeauField(ResourceFieldsKeys.APPROBATEUR, ResourceFiles.FIELDS, IsTaxon = false, IsComplexType = true, ComplexTypeHandler = typeof(PickerEntityHandler))]
        public List<PickerEntity> Approbateur { get; set; }
        
        [SpeedeauField(ResourceFieldsKeys.PROJECT_TAXON, ResourceFiles.FIELDS, IsTaxon= true)]
        public TaxonomyValue Projet { get; set; }

        [SpeedeauField(ResourceFieldsKeys.SITE_TAXON, ResourceFiles.FIELDS, IsTaxon = true)]
        public TaxonomyValue Site { get; set; }

        [SpeedeauField(ResourceFieldsKeys.LIVRAISON_DTG, ResourceFiles.FIELDS, IsTaxon = false)]
        public bool Livraison_dtg { get; set; }

        [SpeedeauField(ResourceFieldsKeys.LIVRAISON_EXPLOITANT, ResourceFiles.FIELDS, IsTaxon = false)]
        public bool Livraison_exploitant { get; set; }

        [SpeedeauField(ResourceFieldsKeys.LIVRAISON_INTEGRATEUR, ResourceFiles.FIELDS, IsTaxon = false)]
        public bool Livraison_integrateur { get; set; }

        [SpeedeauField(ResourceFieldsKeys.LIVRAISON_MCO, ResourceFiles.FIELDS, IsTaxon = false)]
        public bool Livraison_mco { get; set; }

        [SpeedeauField(ResourceFieldsKeys.LIVRAISON_TABLEAUTIER, ResourceFiles.FIELDS, IsTaxon = false)]
        public bool Livraison_tableautier { get; set; }



        [SpeedeauField(ResourceFieldsKeys.LIVRAISON_DATE_DTG, ResourceFiles.FIELDS, IsTaxon = false, AllowStringEmpty = true)]
        public string Livraison_date_dtg { get; set; }

        [SpeedeauField(ResourceFieldsKeys.LIVRAISON_DATE_EXPLOITANT, ResourceFiles.FIELDS, IsTaxon = false, AllowStringEmpty = true)]
        public string Livraison_date_exploitant { get; set; }

        [SpeedeauField(ResourceFieldsKeys.LIVRAISON_DATE_INTEGRATEUR, ResourceFiles.FIELDS, IsTaxon = false, AllowStringEmpty = true)]
        public string Livraison_date_integrateur { get; set; }

        [SpeedeauField(ResourceFieldsKeys.LIVRAISON_DATE_MCO, ResourceFiles.FIELDS, IsTaxon = false, AllowStringEmpty = true)]
        public string Livraison_date_mco { get; set; }

        [SpeedeauField(ResourceFieldsKeys.LIVRAISON_DATE_TABLEAUTIER, ResourceFiles.FIELDS, IsTaxon = false, AllowStringEmpty = true)]
        public string Livraison_date_tableautier { get; set; }


        [SpeedeauField(ResourceFieldsKeys.LIVRAISON_FORMAT_DTG, ResourceFiles.FIELDS, IsTaxon = false, AllowStringEmpty = true)]
        public string Livraison_format_dtg { get; set; }

        [SpeedeauField(ResourceFieldsKeys.LIVRAISON_FORMAT_EXPLOITANT, ResourceFiles.FIELDS, IsTaxon = false, AllowStringEmpty = true)]
        public string Livraison_format_exploitant { get; set; }

        [SpeedeauField(ResourceFieldsKeys.LIVRAISON_FORMAT_INTEGRATEUR, ResourceFiles.FIELDS, IsTaxon = false, AllowStringEmpty = true)]
        public string Livraison_format_integrateur { get; set; }

        [SpeedeauField(ResourceFieldsKeys.LIVRAISON_FORMAT_MCO, ResourceFiles.FIELDS, IsTaxon = false, AllowStringEmpty = true)]
        public string Livraison_format_mco { get; set; }

        [SpeedeauField(ResourceFieldsKeys.LIVRAISON_FORMAT_TABLEAUTIER, ResourceFiles.FIELDS, IsTaxon = false, AllowStringEmpty = true)]
        public string Livraison_format_tableautier { get; set; }


        [SpeedeauField(ResourceFieldsKeys.LIVRAISON_STOCKAGE_DTG, ResourceFiles.FIELDS, IsTaxon = false, AllowStringEmpty = true)]
        public string Livraison_stockage_dtg { get; set; }

        [SpeedeauField(ResourceFieldsKeys.LIVRAISON_STOCKAGE_EXPLOITANT, ResourceFiles.FIELDS, IsTaxon = false, AllowStringEmpty = true)]
        public string Livraison_stockage_exploitant { get; set; }

        [SpeedeauField(ResourceFieldsKeys.LIVRAISON_STOCKAGE_INTEGRATEUR, ResourceFiles.FIELDS, IsTaxon = false, AllowStringEmpty = true)]
        public string Livraison_stockage_integrateur { get; set; }

        [SpeedeauField(ResourceFieldsKeys.LIVRAISON_STOCKAGE_MCO, ResourceFiles.FIELDS, IsTaxon = false, AllowStringEmpty = true)]
        public string Livraison_stockage_mco { get; set; }

        [SpeedeauField(ResourceFieldsKeys.LIVRAISON_STOCKAGE_TABLEAUTIER, ResourceFiles.FIELDS, IsTaxon = false, AllowStringEmpty = true)]
        public string Livraison_stockage_tableautier { get; set; }




    }


    //public class Livraison
    //{
    //    public enum LivraisonType {
    //        DTG,
    //        Exploitant,
    //        Integrateur,
    //        MCO,
    //        Tableautier
    //    }
    // [SpeedeauField(ResourceFieldsKeys.LIVRAISON_TYPE, ResourceFiles.FIELDS, IsTaxon = false)]
    //    public LivraisonType Type { get; set; }

    //    [SpeedeauField(ResourceFieldsKeys.LIVRAISON_DATE, ResourceFiles.FIELDS, IsTaxon = false)]
    //    public DateTime Date { get; set; }

    //    [SpeedeauField(ResourceFieldsKeys.LIVRAISON_FORMAT, ResourceFiles.FIELDS, IsTaxon = false)]
    //    public string Format { get; set; }

    //    [SpeedeauField(ResourceFieldsKeys.LIVRAISON_STOCKAGE, ResourceFiles.FIELDS, IsTaxon = false)]
    //    public string Stockage { get; set; }
    //}
}
