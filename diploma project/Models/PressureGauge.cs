using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tanks.Models
{
    public enum EUPressure
    {
        MPaG = 1,
        kPaG = 2,
        MPa = 3,
        kPa = 4,
        kg_cm2 = 9,
    }
    [ModelClass]
    public class PressureGauge : InstrumentUnit
    {
        [PropertyType(PropertyType.ModelTuning)]
        public Double Pl { get; set; }
        [PropertyType(PropertyType.ModelTuning)]
        public Double Ph { get; set; }
        [PropertyType(PropertyType.ModelTuning)]
        public EUPressure unit { get; set; }

        [PointType(PointType.Point0 | PointType.Source | PointType.Level1)]
        [PropertyType(PropertyType.ModelRuntime)]
        public Double P { get; set; }
        [PointType(PointType.Point1 | PointType.Destination | PointType.Level0)]
        public Double m
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
                public Double Pfail { get; set; }
                public Double Factor { get; set; }
                public Double Offset { get; set; }
        */
        public PressureGauge()
        {
            points[0] = () => P;
        }

        public override void CalcIntrument(int nLevel, PointType pointType)
        {
            if (pointType == PointType.Destination)
            {
                Debug.Assert(unit == EUPressure.kPaG);
                P = m - 101.325; // use m
            }
            else
            {
                // set P (already done)
            }
        }
    }
}
