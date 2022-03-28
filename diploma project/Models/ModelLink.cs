using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tanks.Models
{
    public abstract class ModelLink
    {
        [PropertyType(PropertyType.Id)]
        public string Id { get; set; }
        [PropertyType(PropertyType.Topology)]
        public string From { get; set; }
        [PropertyType(PropertyType.Topology)]
        public string To { get; set; }
        [PropertyType(PropertyType.Visual)]
        public string Figures { get; set; }
    }
}
