using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandBox
{
    class Voiture
    {
        public string Marque { get; set; }
        [Speed("km/h")]
        public int VMax { get; set; }
    }

    class SpeedAttribute : Attribute
    {
        public string Unit { get; set; }
        public SpeedAttribute(string unit)
        {
            Unit = unit;
        }
    }
}
