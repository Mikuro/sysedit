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
    public class ControlValve : ProcessUnit
    {
        [PortType(PortType.InPort | PortType.Port0 | PortType.Part0 | PortType.PfPartKindFlow)]
        public Stream f1 { get { return ports[0]; } set { ports[0] = value; } }
        [PortType(PortType.OutPort | PortType.Port1 | PortType.Part0 | PortType.PfPartKindFlow)]
        public Stream p1 { get { return ports[1]; } set { ports[1] = value; } }

        [PropertyType(PropertyType.ModelTuning)]
        public Double CV { get; set; }
        [PropertyType(PropertyType.ModelTuning)]
        public Double R { get; set; }
        [PropertyType(PropertyType.ModelTuning)]
        public Double pos0 { get; set; }
        [PropertyType(PropertyType.ModelTuning)]
        public Double topen { get; set; }
        [PropertyType(PropertyType.ModelTuning)]
        public Double tclose { get; set; }
        [PropertyType(PropertyType.ModelTuning)]
        public Double tlag { get; set; }
        [PropertyType(PropertyType.ModelTuning)]
        public Double Cff { get; set; }
        // -----------
        [PointType(PointType.Point0 | PointType.Destination | PointType.Level3)]
        [PropertyType(PropertyType.ContolRuntime)]
        public Double mv
        {
            get
            {
                return points[0]== null ? points_value[0] : points[0]();
            }
            set
            {
                points_value[0] = value;
            }
        }

        [PropertyType(PropertyType.ModelRuntime)]
        public Double pos { get; set; }

        [PropertyType(PropertyType.ModelRuntime)]
        public Double velosity { get; set; }

        public override double[] pfGraphPFE(int part, ref double b, int n, FlowDiagram flowDiagram)
        {
            double[] a = new Double[n];

            if (IsClosed())
            {
                if (part == 0)
                {
                    a[f1.fn] = 1.0;
                    b = 0;
                }
                else
                {
                    a[p1.fn] = 1.0;
                    b = 0;
                }
            }
            else
            {
                Debug.Assert(f1.fn == p1.fn);
                Debug.Assert(pos > pos0);
                Debug.Assert(part == 0);
                Debug.Assert(f1.V != 0);

                double d = 1.0 / f1.V; // (мольная плотность)
                double cv = CV * Math.Pow(R, (pos - 1.0));
                //double dp = (Pin - Pout);
                double c2 = 2.74 * 2.74 * cv * cv * Cff * Cff * (d / f1.Mw);
                //f = c * Math.Sqrt(dp); // kmol/h

                a[f1.pn] = -c2;
                a[p1.pn] = c2;
                a[f1.fn] = 1.0;
                b = 0;
            }

            return a;
        }

        public override double[] pfGraphPFE_l(int part, ref double b, int n, FlowDiagram flowDiagram)
        {
            double[] a = new Double[n];

            if (IsClosed())
            {
                if (part == 0)
                {
                    a[f1.fn] = 1.0;
                    b = 0;
                }
                else
                {
                    a[p1.fn] = 1.0;
                    b = 0;
                }
            }
            else
            {
                Debug.Assert(f1.fn == p1.fn);
                Debug.Assert(pos > pos0);
                Debug.Assert(part == 0);
                Debug.Assert(f1.V != 0);

                double d = 1.0 / f1.V; // (мольная плотность)
                double cv = CV * Math.Pow(R, (pos - 1.0));
                //double dp = (Pin - Pout);
                double c2 = 2.74 * 2.74 * cv * cv * Cff * Cff * (d / f1.Mw);
                //f = Math.Sqrt(c2*dp); // kmol/h

                a[f1.pn] = -c2;
                a[p1.pn] = c2;
                a[f1.fn] = Math.Abs(f1.F);
                b = 0;
            }

            return a;
        }

        public override void scCalcOutStreams(int part, FlowDiagram fd,bool bInitial)
        {
            if (IsClosed())
            {
                //Debug.Assert(false);
            }else
            {
                // P and H known
                // so we need to calc PH flash (T & VLE eq) for p1

                if (bInitial)
                {
                    Models.Stream f1d;
                    Models.Stream p1d;

                    if (!f1.bReverseFlow)
                    {
                        Debug.Assert(p1.F >= 0);
                        f1d = f1;
                        p1d = p1;
                    }
                    else
                    {
                        Debug.Assert(p1.F <= 0);
                        f1d = p1;
                        p1d = f1;
                    }
                    p1d.P = f1d.P;
                    p1d.T = f1d.T;
                    p1d.x = new Collection<double>(f1d.x.ToArray());
                    p1d.XL = new Collection<double>(f1d.XL.ToArray());
                    p1d.XV = new Collection<double>(f1d.XV.ToArray());
                    p1d.RV = f1d.RV;
                    p1d.RL = 1 - p1d.RV;
                    p1d.HL = f1d.HL;
                    p1d.HV = f1d.HV;
                    p1d.Mw = f1d.Mw;
                    p1d.V = f1d.V;

                    p1d.F = f1d.F;
                }
                else
                {
                    Models.Stream f1d;
                    Models.Stream p1d;

                    if (!f1.bReverseFlow)
                    {
                        Debug.Assert(p1.F >= 0);
                        f1d = f1;
                        p1d = p1;
                    }
                    else
                    {
                        Debug.Assert(p1.F <= 0);
                        f1d = p1;
                        p1d = f1;
                    }

                    var xl = new double[f1d.x.Count];
                    var xv = new double[f1d.x.Count];
                    double psi;

                    double Mw, V, H;
                    double Mwx, Vx, Hx, Mwy, Vy, Hy;
                    double T;

                    H = f1d.HL * (1.0 - f1d.RV) + f1d.HV * f1d.RV;

                    T = Flash.FlashPH(fd, f1d.x.ToArray(), p1d.P, H, p1d.T,
                        xl, xv, out psi, true, out Mwx, out Vx, out Hx, out Mwy, out Vy, out Hy,
                        p1d.Type == PipeType.Liquid, p1d.Type == PipeType.Vapor);

                    Mw = Mwx * (1.0 - psi) + Mwy * psi;
                    H = Hx * (1.0 - psi) + Hy * psi;
                    V = Vx * (1.0 - psi) + Vy * psi;

                    p1d.T = T;
                    p1d.x = new Collection<double>(f1d.x.ToArray());
                    p1d.XL = new Collection<double>(xl);
                    p1d.XV = new Collection<double>(xv);
                    p1d.RV = psi;
                    p1d.RL = 1 - psi;
                    p1d.HL = Hx;
                    p1d.HV = Hy;
                    p1d.Mw = Mw;
                    p1d.V = V;
                }
            }
        }

        public override void CalcIntrument(int nLevel, PointType pointType)
        {
            // mv
        }

        private UnitTopology closedValveTopology;

        private bool IsClosed()
        {
            return !(pos > pos0);
        }

        protected override UnitTopology GetTopology()
        {
            if (IsClosed())
            {
                if (closedValveTopology == null)
                {
                    closedValveTopology = new UnitTopology
                    {
                        Parts = new Collection<Collection<int>[]>
                        {
                            new Collection<int>[2] {
                                new Collection<int> { 0 },
                                new Collection<int> { }
                            },
                            new Collection<int>[2] {
                                new Collection<int> { },
                                new Collection<int> { 1 }
                            }
                        },
                        pfPartsKind = new Dictionary<int, PortType>
                        {
                            { 0, 0 },
                            { 1, 0 }
                        },
                        scPartsKind = new Dictionary<int, PortType>
                        {
                            { 0, PortType.ExPartKindNoFlow },
                            { 1, PortType.ExPartKindNoFlow }
                        },
                        portnames = new Collection<string>
                        {
                            "f1",
                            "p1"
                        }
                    };
                }

                return closedValveTopology;
            }
            else
                return base.GetTopology();
        }

        public override void OneStep(FlowDiagram fd)
        {
            if (mv > pos)
            {
                // open
                if (velosity < 0.0) velosity = 0;
                velosity += (mv - pos) * (1.0 / tlag);
                if (velosity > (1.0 / topen)) velosity = 1.0 / topen;
                pos = Math.Min(pos + velosity, mv);
            }
            else
            if (mv < pos)
            {
                // close
                if (velosity > 0.0) velosity = 0;
                velosity += (mv - pos) * (1.0 / tlag);
                if (velosity < (-1.0 / tclose)) velosity = -1.0 / tclose;
                pos = Math.Max(mv, pos + velosity);
            }

        }
    }
}
