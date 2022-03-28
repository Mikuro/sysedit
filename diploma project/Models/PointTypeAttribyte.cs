using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tanks.Models
{
//    [Flags]
    public enum PointType
    {
        Point0 = 0,
        Point1 = 1,
        Point2 = 2,
        Point3 = 3,
        Point4 = 4,
        Point5 = 5,
        Point6 = 6,
        Point7 = 7,
        Level0 = 0,
        Level1 = 8,
        Level2 = 16,
        Level3 = 24,
        Source = 0,
        Destination = 32
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class PointTypeAttribute : System.Attribute
    {
        public PointType type { get; set; }
        public PointTypeAttribute(PointType type)
        {
            this.type = type;
        }
    }
}
