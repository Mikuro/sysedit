using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tanks.Models
{
    public enum EUTemperature
    {
        C = 1,
        K = 2,
    }

    [ModelClass]
    public class Thermometer : InstrumentUnit
    {
        [PropertyType(PropertyType.ModelTuning)]
        public Double Tl { get; set; }
        [PropertyType(PropertyType.ModelTuning)]
        public Double Th { get; set; }
        [PropertyType(PropertyType.ModelTuning)]
        public EUTemperature unit { get; set; }

        [PointType(PointType.Point0 | PointType.Source | PointType.Level1)]
        [PropertyType(PropertyType.ModelRuntime)]
        public Double T { get; set; }

        [PointType(PointType.Point1 | PointType.Destination | PointType.Level0)]
        public Double m1
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

        public Thermometer()
        {
            points[0] = () => T;
        }

        public override void CalcIntrument(int nLevel, PointType pointType)
        {
            if (pointType == PointType.Destination)
            {
                Debug.Assert(unit == EUTemperature.C);
                T = m1; //use m1
            }
            else
            {
                // set T (already done)
            }
        }
    }
}
