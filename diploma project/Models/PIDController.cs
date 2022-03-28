using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tanks.Models
{
    public enum PIDDirection
    {
        Direct=0,
        Reverse=1,
    }
    public enum PIDMode
    {
        MAN = 1,
        AUT = 2,
        CAS = 3,
    }

    [ModelClass]
    public class PIDController : InstrumentUnit
    {
        [PropertyType(PropertyType.ContolTuning)]
        public Double PB { get; set; }
        [PropertyType(PropertyType.ContolTuning)]
        public Double TI { get; set; }
        [PropertyType(PropertyType.ContolTuning)]
        public Double TD { get; set; }
        [PropertyType(PropertyType.ContolTuning)]
        public Double PVSPAN { get; set; }
        [PropertyType(PropertyType.ContolTuning)]
        public Double PVBASE { get; set; }
        [PropertyType(PropertyType.ContolTuning)]
        public PIDDirection INCDEC { get; set; }
        [PropertyType(PropertyType.ContolTuning)]
        public Double MVSPAN { get; set; }
        [PropertyType(PropertyType.ContolTuning)]
        public Double MVH { get; set; }
        [PropertyType(PropertyType.ContolTuning)]
        public Double MVL { get; set; }

        [PropertyType(PropertyType.Calculated)]
        public Double SV { get; set; }
        [PropertyType(PropertyType.Calculated)]
        public Double PV { get; set; }
        [PropertyType(PropertyType.Calculated)]
        public Double MV { get; set; }

        [PropertyType(PropertyType.ContolRuntime)]
        public PIDMode CMOD { get; set; }
        [PropertyType(PropertyType.ContolRuntime)]
        public Double SVM { get; set; }
        [PropertyType(PropertyType.ContolRuntime)]
        public Double MVM { get; set; }
        [PropertyType(PropertyType.ModelRuntime)]
        public Double e { get; set; }
        [PropertyType(PropertyType.ModelRuntime)]
        public Double E { get; set; }

        double old_e;

        [PointType(PointType.Point0 | PointType.Source | PointType.Level2)]
        public Double cmv { get; set; }
        [PointType(PointType.Point1 | PointType.Source | PointType.Level3)]
        public Double MVE { get; set; }

        [PointType(PointType.Point2 | PointType.Destination | PointType.Level1)]
        public Double PVE
        {
            get
            {
                return points[2] == null ? points_value[2] : points[2]();
            }
            set
            {
                points_value[2] = value;
            }
        }
        [PointType(PointType.Point3 | PointType.Destination | PointType.Level2)]
        public Double csv
        {
            get
            {
                return points[3] == null ? points_value[3] : points[3]();
            }
            set
            {
                points_value[3] = value;
            }
        }

        public PIDController()
        {
            points[0] = () => cmv;
            points[1] = () => MVE;
        }

        public override void CalcIntrument(int nLevel, PointType pointType )
        {
            if (pointType == PointType.Destination)
            {
                switch (nLevel)
                {
                    case 1:
                        // PVE
                        break;
                    case 2:
                        // csv
                        break;
                }
            }
            else
            {
                switch (nLevel)
                {
                    case 2:
                        if (CMOD == PIDMode.MAN)
                        {
                            cmv = MVM;
                            SVM = PVE;
                            this.PV = PVE;
                            this.SV = PVE;
                        }
                        else
                        {
                            double dt = 1.0;
                            double sv = (((CMOD == PIDMode.AUT) ? SVM : csv) - PVBASE) / PVSPAN;
                            if (sv < 0) sv = 0;
                            if (sv > 1) sv = 1;
                            double pv = (PVE - PVBASE) / PVSPAN;
                            if (pv < 0) pv = 0;
                            if (pv > 1) pv = 1;
                            double e = (INCDEC == PIDDirection.Direct) ? pv - sv : sv - pv;
                            double old_mv = MV / MVSPAN;
                            this.E = this.e + TI * (old_mv * PB / 100 - this.e - (TD * ((this.e - this.old_e) / dt)));
                            double E = this.E + e;
                            double mv = ((TD * ((e - this.e) / dt)) + (this.E / TI) + e) * 100.0 / PB;
                            if (mv < 0) mv = 0;
                            if (mv > 1) mv = 1;

                            cmv = mv * MVSPAN;
                            MVE = mv;

                            if (CMOD == PIDMode.CAS)
                                SVM = sv * PVSPAN + PVBASE;
                            MVM = mv * MVSPAN;

                            this.PV = pv * PVSPAN + PVBASE;
                            this.SV = sv * PVSPAN + PVBASE;
                        }
                        break;
                    case 3:
                        if (CMOD == PIDMode.MAN)
                            MVE = MVM / MVSPAN;
                        break;
                }
            }
        }

        public override void OneStep(FlowDiagram fd)
        {
            double sv = 0.0;
            double pv = 0.0;
            double e = 0.0;
            double mv = 0.0;

            if (CMOD != PIDMode.MAN)
            {
                double dt = 1.0;
                sv = (((CMOD == PIDMode.AUT) ? SVM : csv) - PVBASE) / PVSPAN;
                if (sv < 0) sv = 0;
                if (sv > 1) sv = 1;
                pv = (PVE - PVBASE) / PVSPAN;
                if (pv < 0) pv = 0;
                if (pv > 1) pv = 1;
                e = (INCDEC == PIDDirection.Direct) ? pv - sv : sv - pv;
                double old_mv = MV / MVSPAN;
                this.E = this.e + TI * (old_mv * PB / 100 - this.e - (TD * ((this.e - this.old_e) / dt)));
                double E = this.E + e;
                mv = ((TD * ((e - this.e) / dt)) + (this.E / TI) + e) * 100.0 / PB;
                if (mv < 0) mv = 0;
                if (mv > 1) mv = 1;
            }else
            {
                e = 0;
                this.e = 0;
                E = 0;
                mv = MVM / MVSPAN;
            }

            this.old_e = this.e;
            this.e = e;
            this.MV = mv * MVSPAN;
        }
    }
}
