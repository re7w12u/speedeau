using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportListeDeSuivi.model
{
    [Serializable]
    class Entry
    {
        public Entry()
        {

        }
        [EntryField(FieldName = "Titre")]
        public string Titre { get; set; }
        [EntryField(FieldName = "Famille documentaire", IsTaxon = true)]
        public string FamilleDoc { get; set; }
        [EntryField(FieldName = "Nature documentaire", IsTaxon = true)]
        public string NatureDoc { get; set; }
        [EntryField(FieldName = "Projet", IsTaxon = true)]
        public string Projet { get; set; }
        [EntryField(FieldName = "Opération")]
        public string Operation { get; set; }
        [EntryField(FieldName = "Site DPIH", IsTaxon = true)]
        public string Site { get; set; }
        [EntryField(FieldName = "Requis")]
        public string Requis { get; set; }
        [EntryField(FieldName = "Unité", IsTaxon = true)]
        public string Unit { get; set; }
        [EntryField(FieldName = "Code centrale")]
        public string CodeCentrale { get; set; }
        [EntryField(FieldName = "Code Projet", IsTaxon = true)]
        public string CodeProjet { get; set; }
        [EntryField(FieldName = "Niveau d'études", IsTaxon = true)]
        public string NiveauEtude { get; set; }
        [EntryField(FieldName = "Découpage - lot")]
        public string Decoupage { get; set; }
        [EntryField(FieldName = "Système élémentaire", IsTaxon = true)]
        public string SystemElet { get; set; }
        [EntryField(FieldName = "Complément")]
        public string Complement { get; set; }
        [EntryField(FieldName = "Numéro Chrono")]
        public string NumChrono { get; set; }
        [EntryField(FieldName = "Codification")]
        public string Codification { get; set; }
        //[EntryField(FieldName = "CodificationSystem")]
        //public string CodificationSystem { get; set; }
        [EntryField(FieldName = "Indice")]
        public string Indice { get; set; }
        [EntryField(FieldName = "Type MSH")]
        public string TypeMSH { get; set; }
        [EntryField(FieldName = "R/T")]
        public string RT { get; set; }
        [EntryField(FieldName = "Format demandé")]
        public string Formatdemande { get; set; }
        [EntryField(FieldName = "Rédaction", IsTaxon = true)]
        public string Redaction { get; set; }
        [EntryField(FieldName = "Temps estimé (h)")]
        public string TempsEstime { get; set; }
        [EntryField(FieldName = "Reste … faire (h)")]
        public string ResteAFaire { get; set; }
        [EntryField(FieldName = "Fourniture")]
        public string Frouniture { get; set; }
        [EntryField(FieldName = "Date cible")]
        public string DateCible { get; set; }
        [EntryField(FieldName = "Observation_rnvo")]
        public string Observation { get; set; }
        [EntryField(FieldName = "Livr. Exploitant")]
        public string LivrExploit { get; set; }
        [EntryField(FieldName = "Livr. MCO")]
        public string LivrMCO { get; set; }
        [EntryField(FieldName = "Livr. Intégrateur")]
        public string LivrInteg { get; set; }
        [EntryField(FieldName = "Livr. DTG")]
        public string LivrDTG { get; set; }
        [EntryField(FieldName = "Livr. Tableautier")]
        public string LivrTab { get; set; }
        [EntryField(FieldName = "Vérificateur")]
        public string Verificateur { get; set; }
        [EntryField(FieldName = "Approbateur")]
        public string Approvateur { get; set; }
        [EntryField(FieldName = "Validation MOA")]
        public string ValidMOA { get; set; }
        //[EntryField(FieldName = "Type d'élément")]
        //public string TypeElet { get; set; }
        //[EntryField(FieldName = "Chemin d'accès")]
        //public string Chemin { get; set; }
    }


    class EntryFieldAttribute : Attribute
    {
        public string FieldName { get; set; }

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
                    TermSetId = Guid.Parse(ConfigurationManager.AppSettings.Get(FieldName));
                }
            }
        }
        public Guid TermSetId { get; set; }

    }

    internal class EntryBuilder
    {

        SortedList<int, string> Columns;
        public string Separator { get; set; }
        internal EntryBuilder(string cols, string sep)
        {
            Separator = @"\t";
            Columns = new SortedList<int, string>();
            string[] data = cols.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < data.Length; i++)
            {
                Columns.Add(i, data[i]);
            }
        }

        internal Entry Build(string line)
        {
            Entry e = new Entry();

            string[] data = line.Split(new char[] { '\t' }, StringSplitOptions.None);

            foreach (KeyValuePair<int, string> kvp in Columns)
            {
                var q = from p in e.GetType().GetProperties()
                        where p.GetCustomAttributes(typeof(EntryFieldAttribute), true).Any()
                        && (p.GetCustomAttributes(typeof(EntryFieldAttribute), true).FirstOrDefault() as EntryFieldAttribute).FieldName == kvp.Value
                        select p;
                if (q.Any())
                {
                    string value = data[kvp.Key];
                    q.First().SetValue(e, value);
                }
            }


            return e;
        }

    }
}
