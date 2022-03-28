using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tanks.Models
{
//    [Flags]
    public enum PropertyType
    {
        Id = 1,
        Topology = 2,
        Visual = 3,
        ModelTuning = 4,
        ModelRuntime = 5,
        ContolTuning = 6,
        ContolRuntime = 7,
        Calculated = 8,
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyTypeAttribute : System.Attribute
    {
        public PropertyType type { get; set; }
        public PropertyTypeAttribute(PropertyType type)
        {
            this.type = type;
        }
    }
}
