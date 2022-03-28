using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tanks.Models
{
    public enum EUMassOrVolume
    {
        kg = 1,
        t = 2,
        Nm3 = 3,
        kNm3 = 4,
        m3 = 5,
        g = 8,
        l = 9,
    }
    public enum EUTime
    {
        h = 1,
        min = 2,
        sec = 4,
    }

    [ModelClass]
    public class FlowMeter : InstrumentUnit
    {
        [PropertyType(PropertyType.ModelTuning)]
        public Double Fmax { get; set; }
        [PropertyType(PropertyType.ModelTuning)]
        public Double Fmin { get; set; }
        [PropertyType(PropertyType.ModelTuning)]
        public EUMassOrVolume unit { get; set; }
        [PropertyType(PropertyType.ModelTuning)]
        public EUTime t_unit { get; set; }

        [PointType(PointType.Point0 | PointType.Source | PointType.Level1)]
        [PropertyType(PropertyType.ModelRuntime)]
        public Double F { get; set; }

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

        public FlowMeter()
        {
            points[0] = () => F;
        }

        public double Flow(Stream stream)
        {
            // stream.F  // кмоль/ч
            // stream.Mw // кг/кмоль
            // stream.V  // м3/кмоль

            // unit
            //  1 = kg
            //  2 = t
            //  3 = Nm3  // 1 Nm3 = 44.6428 моль (vapor only)
            //  4 = kNm3
            //  5 - m3
            //  8 - g
            //  9 - l

            // t_unit
            // 1 - h
            // 2 - min
            // 4 - sec

            Debug.Assert(unit == EUMassOrVolume.m3);
            Debug.Assert(t_unit == EUTime.h);

            return stream.V*stream.F;
        }

        public override void CalcIntrument(int nLevel, PointType pointType)
        {
            if (pointType == PointType.Destination)
            {
                F = m1; // use m1
            }
            else
            {
                // set F (already done)
            }
        }
    }
}
