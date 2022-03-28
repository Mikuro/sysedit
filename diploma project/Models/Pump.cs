using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tanks.Models.Solver;

namespace tanks.Models
{
    [ModelClass]
    public class Pump : ProcessUnit
    {
        [PortType(PortType.InPort | PortType.Port0 | PortType.Part0 | PortType.PfPartKindFlow)]
        public Stream f1 { get { return ports[0]; } set { ports[0] = value; } }
        [PortType(PortType.OutPort | PortType.Port1 | PortType.Part0 | PortType.PfPartKindFlow)]
        public Stream p1 { get { return ports[1]; } set { ports[1] = value; } }

        [PropertyType(PropertyType.ModelTuning)]
        public Double Hd { get; set; }
        [PropertyType(PropertyType.ModelTuning)]
        public Double Hs { get; set; }
        [PropertyType(PropertyType.ModelTuning)]
        public Double Vd { get; set; }
        [PropertyType(PropertyType.ModelTuning)]
        public Double dd { get; set; }
        [PropertyType(PropertyType.ModelTuning)]
        public Double Wd { get; set; }
        [PropertyType(PropertyType.ModelTuning)]
        public Double Ws { get; set; }

        [PropertyType(PropertyType.ModelTuning)]
        public Double pos0 { get; set; }
        [PropertyType(PropertyType.ModelTuning)]
        public Double tlag { get; set; }

        [PointType(PointType.Point0 | PointType.Destination | PointType.Level3)]
        [PropertyType(PropertyType.ContolRuntime)]
        public Double mv
        {
            get
            {
                return points[0] == null ? points_value[0] : points[0]();
            }
            set
            {
                points_value[0] = value;
            }
        }
        [PropertyType(PropertyType.ModelRuntime)]
        public Double pos { get; set; }

        public override double[] pfGraphPFE(int j, ref double b, int n, FlowDiagram flowDiagram)
        {
            double[] a = new Double[n];

            double H0 = pos * pos * Hs;
            double alpha = (Hs - Hd) / (Vd * Vd);

            a[f1.pn] = -1;
            a[p1.pn] = 1;
            a[f1.fn] = (9.80665E-3) * alpha * f1.V * f1.Mw;
            b = (9.80665E-3) * H0 * f1.Mw / f1.V;

            return a;
        }

        public override double[] pfGraphPFE_l(int j, ref double b, int n, FlowDiagram flowDiagram)
        {
            double[] a = new Double[n];

            double H0 = pos * pos * Hs;
            double alpha = (Hs - Hd) / (Vd * Vd);

            double c1 = (9.80665E-3) * alpha * f1.V * f1.Mw;

            double c2 = (9.80665E-3) * H0 * f1.Mw / f1.V;

            a[f1.pn] = -1;
            a[p1.pn] = 1;
            a[f1.fn] = c1 * Math.Abs(f1.F);
            b = c2;

            return a;
        }

        public override void scCalcOutStreams(int part, FlowDiagram fd, bool bInitial)
        {
            // P and H known
            // so we need to calc PH flash (T & VLE eq) for p1

            Debug.Assert(f1.F >= 0);
            Debug.Assert(p1.F >= 0);

            double P; 
            double H0=pos*pos*Hs;
            double H;

            var xl = new double[f1.x.Count];
            var xv = new double[f1.x.Count];
            double psi;

            double Mw, V;
            double Mwx, Vx, Hx, Mwy, Vy, Hy;
            double T;

            if (bInitial)
            {
                if (!f1.bReverseFlow)
                {
                    P = f1.P + (9.80665E-3) * H0 * f1.Mw / f1.V;

                    T = f1.T;
                    Flash.FlashPT(fd, f1.x.ToArray(), P, T,
                            xl, xv, out psi, true, out Mwx, out Vx, out Hx, out Mwy, out Vy, out Hy,
                        p1.Type == PipeType.Liquid, p1.Type == PipeType.Vapor);

                    Mw = Mwx * (1.0 - psi) + Mwy * psi;
                    H = Hx * (1.0 - psi) + Hy * psi;
                    V = Vx * (1.0 - psi) + Vy * psi;

                    p1.P = P;
                    p1.F = 0;

                    p1.T = T;
                    p1.x = new Collection<double>(f1.x.ToArray());
                    p1.XL = new Collection<double>(xl);
                    p1.XV = new Collection<double>(xv);
                    p1.RV = psi;
                    p1.RL = 1 - psi;
                    p1.HL = Hx;
                    p1.HV = Hy;
                    p1.Mw = Mw;
                    p1.V = V;
                }else
                {
                    P = p1.P - (9.80665E-3) * H0 * p1.Mw / p1.V;

                    Debug.Assert(P >= 0);

                    T = p1.T;
                    Flash.FlashPT(fd, p1.x.ToArray(), P, T,
                            xl, xv, out psi, true, out Mwx, out Vx, out Hx, out Mwy, out Vy, out Hy,
                        p1.Type == PipeType.Liquid, p1.Type == PipeType.Vapor);

                    Mw = Mwx * (1.0 - psi) + Mwy * psi;
                    H = Hx * (1.0 - psi) + Hy * psi;
                    V = Vx * (1.0 - psi) + Vy * psi;

                    f1.P = P;
                    f1.F = 0;

                    f1.T = T;
                    f1.x = new Collection<double>(p1.x.ToArray());
                    f1.XL = new Collection<double>(xl);
                    f1.XV = new Collection<double>(xv);
                    f1.RV = psi;
                    f1.RL = 1 - psi;
                    f1.HL = Hx;
                    f1.HV = Hy;
                    f1.Mw = Mw;
                    f1.V = V;
                }
            }
            else
            {
                Debug.Assert(!f1.bReverseFlow);
                Debug.Assert(!p1.bReverseFlow);

                P = p1.P;
                H = f1.HL * (1.0 - f1.RV) + f1.HV * f1.RV;

                double Cc = 1.0;
                double Power;

                Power = (Ws * pos + (Wd - Ws) * ((f1.F * f1.V) / Vd) * ((f1.Mw / f1.V) / dd) * Cc) * pos * pos;

                //Debug.Assert(f1.F > 100.0);
                // next statement is bad
                H += 3.6 * Power/f1.F;

                T = Flash.FlashPH(fd, f1.x.ToArray(), P, H, f1.T,
                        xl, xv, out psi, true, out Mwx, out Vx, out Hx, out Mwy, out Vy, out Hy,
                    p1.Type == PipeType.Liquid, p1.Type == PipeType.Vapor);

                Mw = Mwx * (1.0 - psi) + Mwy * psi;
                H = Hx * (1.0 - psi) + Hy * psi;
                V = Vx * (1.0 - psi) + Vy * psi;

                p1.T = T;
                p1.x = new Collection<double>(f1.x.ToArray());
                p1.XL = new Collection<double>(xl);
                p1.XV = new Collection<double>(xv);
                p1.RV = psi;
                p1.RL = 1 - psi;
                p1.HL = Hx;
                p1.HV = Hy;
                p1.Mw = Mw;
                p1.V = V;
            }
        }

        public override void CalcIntrument(int nLevel, PointType pointType)
        {
            // mv
        }
    }
}
