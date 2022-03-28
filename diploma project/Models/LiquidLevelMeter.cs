using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tanks.Models
{
    public enum EULevel
    {
        m = 1,
        mm = 3,
    }
    [ModelClass]
    public class LiquidLevelMeter : InstrumentUnit
    {
        [PropertyType(PropertyType.ModelTuning)]
        public Double Ll { get; set; }
        [PropertyType(PropertyType.ModelTuning)]
        public Double Lh { get; set; }
        [PropertyType(PropertyType.ModelTuning)]
        public Double Lbase { get; set; }
        [PropertyType(PropertyType.ModelTuning)]
        public EULevel unit { get; set; }

        [PointType(PointType.Point0 | PointType.Source | PointType.Level1)]
        [PropertyType(PropertyType.ModelRuntime)]
        public Double L { get; set; }

        [PointType(PointType.Point1 | PointType.Destination | PointType.Level0)]
        public Double l0
        {
            get
            {
                return points[1] == null ? points_value[1] : points[1]();
            }
            set
            {
                points_value[1] = value;
            }
        }

        /*
                public Double Lfail { get; set; }
                public Double Factor { get; set; }
                public Double Offset { get; set; }
        */

        public LiquidLevelMeter()
        {
            points[0] = () => L;
        }

        public override void CalcIntrument(int nLevel, PointType pointType)
        {
            if (pointType == PointType.Destination)
            {
                Debug.Assert(unit == EULevel.m);
                L = l0; // use l0
            }
            else
            {
                // set L (already done)
            }
        }
    }
}
