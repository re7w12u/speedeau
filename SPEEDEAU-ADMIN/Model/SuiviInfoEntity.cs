using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPEEDEAU.ADMIN.Model
{
    public class SuiviInfoEntity
    {
        public int DocID { get; set; }
        public string DocUrl { get; set; }
        public string DocName { get; set; }
        public string Indice { get; set; }
        public string Revision { get; set; }
        public string StatutDoc { get; set; }
        public string Verification { get; set; }
        public string Auteur { get; set; }
        public string DateModif { get; set; }
    }
}
