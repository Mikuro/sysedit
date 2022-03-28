using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tanks.Models
{
    [ModelClass]
    public class Strainer : ProcessUnit
    {
        [PortType(PortType.InPort | PortType.Port0 | PortType.Part0 | PortType.PfPartKindFlow)]
        public Stream f1 { get { return ports[0]; } set { ports[0] = value; } }
        [PortType(PortType.OutPort | PortType.Port1 | PortType.Part0 | PortType.PfPartKindFlow)]
        public Stream p1 { get { return ports[1]; } set { ports[1] = value; } }

        [PropertyType(PropertyType.ModelTuning)]
        public Double Wd { get; set; }
        [PropertyType(PropertyType.ModelTuning)]
        public Double DP { get; set; }
        [PropertyType(PropertyType.ModelTuning)]
        public Double dd { get; set; }
    }
}
