using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;
using System.Reflection;

namespace ImportListeDeSuivi.model
{

    [Serializable]
    class Entry
    {
        public Entry()
        {

        }
        [EntryField(XLSFieldName = "Titre", SPFieldName = "Title")]
        public string Titre { get; set; }
        [EntryField(XLSFieldName = "Famille documentaire", IsTaxon = true, SPFieldName = "EDF_Typologie")]
        public string FamilleDoc { get; set; }
        [EntryField(XLSFieldName = "Nature documentaire", IsTaxon = true, SPFieldName = "Nature_documentaire")]
        public string NatureDoc { get; set; }
        [EntryField(XLSFieldName = "Projet", IsTaxon = true, SPFieldName = "EDF_Projet")]
        public string Projet { get; set; }
        [EntryField(XLSFieldName = "Opération", IsTaxon = false, SPFieldName = "EDF_Operation")]
        public string Operation { get; set; }
        [EntryField(XLSFieldName = "Site DPIH", IsTaxon = true, SPFieldName = "EDF_Site")]
        public string Site { get; set; }
        [EntryField(XLSFieldName = "Requis", SPFieldName = "EDF_Requis")]
        public string Requis { get; set; }
        [EntryField(XLSFieldName = "Unité", IsTaxon = true, SPFieldName = "EDF_Unite")]
        public string Unit { get; set; }
        [EntryField(XLSFieldName = "Code Centrale", SPFieldName = "EDF_Code_centrale")]
        public string CodeCentrale { get; set; }
        [EntryField(XLSFieldName = "Code Projet", IsTaxon = true, SPFieldName = "EDF_CodeProjet")]
        public string CodeProjet { get; set; }
        [EntryField(XLSFieldName = "Niveau d'études", IsTaxon = true, SPFieldName = "EDF_Niveau_Etudes")]
        public string NiveauEtude { get; set; }
        [EntryField(XLSFieldName = "Découpage - lot", SPFieldName = "EDF_Decoupage_Lot")]
        public string Decoupage { get; set; }
        [EntryField(XLSFieldName = "Système élémentaire", IsTaxon = true, SPFieldName = "EDF_Systeme_Elementaire")]
        public string SystemElet { get; set; }
        [EntryField(XLSFieldName = "Complément", SPFieldName = "EDF_Complement")]
        public string Complement { get; set; }
        [EntryField(XLSFieldName = "Numéro Chrono", SPFieldName = "EDF_Numero_Chrono")]
        public string NumChrono { get; set; }
        [EntryField(XLSFieldName = "Codification", SPFieldName = "Codification")]
        public string Codification { get; set; }
        //[EntryField(FieldName = "CodificationSystem", SPFieldName="")]
        //public string CodificationSystem { get; set; }
        [EntryField(XLSFieldName = "Indice", SPFieldName = "EDFVersion")]
        public string Indice { get; set; }
        [EntryField(XLSFieldName = "Type MSH", SPFieldName = "EDF_Type_MSH")]
        public string TypeMSH { get; set; }
        [EntryField(XLSFieldName = "RT", SPFieldName = "EDF_RT")]
        public string RT { get; set; }
        [EntryField(XLSFieldName = "Format demandé", SPFieldName = "EDF_Format_Demande")]
        public string Formatdemande { get; set; }
        [EntryField(XLSFieldName = "Rédaction", IsTaxon = true, SPFieldName = "EDF_Redaction")]
        public string Redaction { get; set; }
        [EntryField(XLSFieldName = "Temps estimé (h)", SPFieldName = "EDF_Temps_Estime")]
        public string TempsEstime { get; set; }
        [EntryField(XLSFieldName = "Reste à faire", SPFieldName = "EDF_Reste_A_Faire")]
        public string ResteAFaire { get; set; }
        [EntryField(XLSFieldName = "Fourniture", SPFieldName = "EDF_Fourniture")]
        public string Fourniture { get; set; }
        [EntryField(XLSFieldName = "Date cible", SPFieldName = "EDF_Date_cible")]
        public string DateCible { get; set; }
        [EntryField(XLSFieldName = "Observations", SPFieldName = "EDF_ListB5_Obs")]
        public string Observation { get; set; }
        [EntryField(XLSFieldName = "Thème", SPFieldName = "", IsTaxon = true)]
        public string Theme { get; set; }

        //[EntryField(XLSFieldName = "Livr. Exploitant")]
        //public string LivrExploit { get; set; }
        //[EntryField(XLSFieldName = "Livr. MCO")]
        //public string LivrMCO { get; set; }
        //[EntryField(XLSFieldName = "Livr. Intégrateur")]
        //public string LivrInteg { get; set; }
        //[EntryField(XLSFieldName = "Livr. DTG")]
        //public string LivrDTG { get; set; }
        //[EntryField(XLSFieldName = "Livr. Tableautier")]
        //public string LivrTab { get; set; }

        [EntryField(XLSFieldName = "Vérificateur", IsPeople = true, SPFieldName = "EDF_Verificateur")]
        public string Verificateur { get; set; }
        [EntryField(XLSFieldName = "Approbateur", IsPeople = true, SPFieldName = "EDF_Approbateur")]
        public string Approvateur { get; set; }
        [EntryField(XLSFieldName = "Validation MOA", SPFieldName = "")]
        public string ValidMOA { get; set; }

        [EntryField(XLSFieldName = "Livr. Exploitant", SPFieldName = "Exploitant_Livr")]
        public bool Exploitant_Livr { get; set; }
        [EntryField(XLSFieldName = "Date Exploitant", SPFieldName = "Exploitant_Date")]
        public string Exploitant_Date { get; set; }
        [EntryField(XLSFieldName = "Format Exploitant", SPFieldName = "Exploitant_Format")]
        public string Exploitant_Format { get; set; }
        [EntryField(XLSFieldName = "Stockage Exploitant", SPFieldName = "Exploitant_Stockage")]
        public string Exploitant_Stockage { get; set; }

        [EntryField(XLSFieldName = "Livr. MCO", SPFieldName = "MCO_Livr")]
        public bool MCO_Livr { get; set; }
        [EntryField(XLSFieldName = "Date MCO", SPFieldName = "MCO_Date")]
        public String MCO_Date { get; set; }
        [EntryField(XLSFieldName = "Format MCO", SPFieldName = "MCO_Format")]
        public string MCO_Format { get; set; }
        [EntryField(XLSFieldName = "Stockage MCO", SPFieldName = "MCO_Stockage")]
        public string MCO_Stockage { get; set; }

        [EntryField(XLSFieldName = "Livr. Intégrateur", SPFieldName = "Integrateur_Livr")]
        public bool Integrateur_Livr { get; set; }
        [EntryField(XLSFieldName = "Date Intégrateur", SPFieldName = "Integrateur_Date")]
        public string Integrateur_Date { get; set; }
        [EntryField(XLSFieldName = "Format Intégrateur", SPFieldName = "Integrateur_Format")]
        public string Integrateur_Format { get; set; }
        [EntryField(XLSFieldName = "Stockage Intégrateur", SPFieldName = "Integrateur_Stockage")]
        public string Integrateur_Stockage { get; set; }

        [EntryField(XLSFieldName = "Livr. DTG", SPFieldName = "DTG_Livr")]
        public bool DTG_Livr { get; set; }
        [EntryField(XLSFieldName = "Date DTG", SPFieldName = "DTG_Date")]
        public string DTG_Date { get; set; }
        [EntryField(XLSFieldName = "Format DTG", SPFieldName = "DTG_Format")]
        public string DTG_Format { get; set; }
        [EntryField(XLSFieldName = "Stockage DTG", SPFieldName = "DTG_Stockage")]
        public string DTG_Stockage { get; set; }

        [EntryField(XLSFieldName = "Livr. Tableautier", SPFieldName = "Tableautier_Livr")]
        public bool Tableautier_Livr { get; set; }
        [EntryField(XLSFieldName = "Date Tableautier", SPFieldName = "Tableautier_Date")]
        public string Tableautier_Date { get; set; }
        [EntryField(XLSFieldName = "Format Tableautier", SPFieldName = "Tableautier_Format")]
        public string Tableautier_Format { get; set; }
        [EntryField(XLSFieldName = "Stockage Tableautier", SPFieldName = "Tableautier_Stockage")]
        public string Tableautier_Stockage { get; set; }


    }


    class EntryFieldAttribute : Attribute
    {
        public string XLSFieldName { get; set; }
        public string SPFieldName { get; set; }

        public bool IsPeople { get; set; }

        private bool _istaxon;
        public bool IsTaxon
        {
            get
            {
                return _istaxon;
            }
            set
            {
                _istaxon = value;
                if (value)
                {
                    TermSetId = Guid.Parse(ConfigurationManager.AppSettings.Get(XLSFieldName));
                }
            }
        }
        public Guid TermSetId { get; private set; }

    }

    internal class EntryBuilder
    {

        SortedList<int, string> ColumnsHeader;
        public string Separator { get; set; }
        internal EntryBuilder(string cols, string sep)
        {
            Separator = @"\t";
            ColumnsHeader = new SortedList<int, string>();
            string[] data = cols.Split(new char[] { '\t' }, StringSplitOptions.None);
            for (int i = 0; i < data.Length; i++)
            {
                ColumnsHeader.Add(i, data[i].Trim());
            }
        }

        internal Entry Build(string line)
        {
            Entry e = new Entry();

            string[] data = line.Split(new char[] { '\t' }, StringSplitOptions.None);

            foreach (KeyValuePair<int, string> kvp in ColumnsHeader)
            {

                // skip out of range value
                if (kvp.Key == data.Length) break;

                // look for property with EntryFieldAttribute FieldName value match current column header
                var q = from p in e.GetType().GetProperties()
                        where p.GetCustomAttributes(typeof(EntryFieldAttribute), true).Any()
                        && (p.GetCustomAttributes(typeof(EntryFieldAttribute), true).FirstOrDefault() as EntryFieldAttribute).XLSFieldName == kvp.Value
                        select p;
                if (q.Any())
                {
                    PropertyInfo p = q.First();                    
                    if (p.PropertyType == typeof(string))
                    {
                        string value = data[kvp.Key].Trim('"');
                        if (!String.IsNullOrWhiteSpace(value)) p.SetValue(e, value);
                    }
                    else
                    {
                        string stringValue = data[kvp.Key].Trim('"');
                        object value = Convert.ChangeType(stringValue, p.PropertyType);
                        
                        if (value != null && value.GetType().FullName == p.PropertyType.FullName)
                        {
                            p.SetValue(e, value);
                        }
                    }
                }
            }


            return e;
        }

    }
}
