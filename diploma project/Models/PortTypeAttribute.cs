using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tanks.Models
{
    [Flags]
    public enum PortType
    {
        InPort = 0,
        OutPort = 2048,
        IoMask = 2048,
        Port0 = 0,
        Port1 = 1,
        Port2 = 2,
        Port3 = 3,
        Port4 = 4,
        Port5 = 5,
        Port6 = 6,
        Port7 = 7,
        LastPort = 7,
        Part0 = 0,
        Part1 = 8,
        Part2 = 16,
        Part3 = 24,
        LastPart = 24,
//        ExPart0 = 0,
//        ExPart1 = 8,
//        ExPart2 = 16,
//        ExPart3 = 24,
//        LastExPart = 24,
        ExPartKindFeed = 32,
        ExPartKindProduct = 64,
        ExPartKindNoFlow = 128,
        ExPartKindMask = 224,
//        PfPart0 = 0,
//        PfPart1 = 128,
//        PfPart2 = 256,
//        PfPart3 = 384,
//        LastPfPart = 384,
        PfPartKindFlow = 0,
        PfPartKindPressure = 512,
        PfPartKindMask = 512
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class PortTypeAttribute : System.Attribute
    {
        public PortType type { get; set; }
        public PortTypeAttribute(PortType type)
        {
            this.type = type;
        }
    }
}
