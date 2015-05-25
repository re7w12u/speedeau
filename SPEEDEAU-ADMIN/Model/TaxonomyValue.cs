using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPEEDEAU.ADMIN.Model
{
    [Serializable]
    public class TaxonomyTerm : IComparable
    {
        public string TermID { get; set; }
        public string Term { get; set; }
        public override string ToString()
        {
            return String.Format("{0}{2}{1}", Term, TermID, TaxonomyField.TaxonomyGuidLabelDelimiter);
        }
        
        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            TaxonomyTerm otherTax = obj as TaxonomyTerm;
            if (otherTax == null) throw new ArgumentException("obj is not a TaxonomyTerm");
            
            int result = this.TermID.CompareTo(otherTax.TermID);
            if (result != 0) return result;

            return this.Term.CompareTo(otherTax.Term);
        }
    }

    [Serializable]
    public class TaxonomyValue : IComparable
    {

        public TaxonomyValue()
        {
            Terms = new List<TaxonomyTerm>();
        }

        public Guid TermStoreID { get; set; }
        public Guid TermSetID { get; set; }
        public List<TaxonomyTerm> Terms { get; set; }
        public override string ToString()
        {
            if (Terms.Count == 0) return String.Empty;
            else return String.Join(";", Terms.Select(t => t.ToString()).ToArray());
        }

        public string ToFullString()
        {
            return String.Format("{0}#;{1}#;{2}", TermStoreID, TermSetID, String.Join(";", Terms.Select(t => t.ToString()).ToArray()));
        }   
       
        public int CompareTo(object obj)
        {
            // result < 0 means x < y
            // result == 0 means x == y
            // result < 0 means x > y

            if (obj == null) return 1;

            TaxonomyValue otherTax = obj as TaxonomyValue;
            if (otherTax == null) throw new ArgumentException("obj is not a TaxonomyValue");

            int result = this.TermStoreID.CompareTo(otherTax.TermStoreID);
            if (result != 0) return result;

            result = this.TermSetID.CompareTo(otherTax.TermSetID);
            if (result != 0) return result;

            result = this.Terms.Count - otherTax.Terms.Count;
            if (result > 0) return 1;
            else if (result < 0) return -1;

            if (this.Terms.Except(otherTax.Terms, new TaxonomyTermComparer()).Any()) return 1;
            else if (otherTax.Terms.Except(this.Terms, new TaxonomyTermComparer()).Any()) return -1;

            return result;
        }
    }

    public class TaxonomyTermComparer : IEqualityComparer<TaxonomyTerm>
    {
        public bool Equals(TaxonomyTerm x, TaxonomyTerm y)
        {
            if (Object.ReferenceEquals(x, y)) return true;
            if(x == null || y == null) return false;
            return x.Term.ToLower().Equals(y.Term.ToLower()) && x.TermID.ToLower().Equals(y.TermID.ToLower());
        }

        public int GetHashCode(TaxonomyTerm obj)
        {
            if (obj == null) return 0;
            int termHash = String.IsNullOrWhiteSpace(obj.Term) ? 0 : obj.Term.GetHashCode();
            int idHash = String.IsNullOrWhiteSpace(obj.TermID) ? 0 : obj.TermID.GetHashCode();
            return termHash ^ idHash;
        }
    }



    

   

}
