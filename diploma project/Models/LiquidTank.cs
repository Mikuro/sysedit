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
    public class LiquidTank : ProcessUnit
    {
        [PortType(PortType.InPort | PortType.Port0 | PortType.Part0 | PortType.ExPartKindNoFlow)]
        public Stream fe { get { return ports[0]; } set { ports[0] = value; } }
        [PortType(PortType.InPort | PortType.Port1 | PortType.Part1 | PortType.ExPartKindProduct)]
        public Stream f1 { get { return ports[1]; } set { ports[1] = value; } }
        [PortType(PortType.OutPort | PortType.Port2 | PortType.Part2 | PortType.ExPartKindFeed)]
        public Stream p1 { get { return ports[2]; } set { ports[2] = value; } }

        [PropertyType(PropertyType.ModelTuning)]
        public Double T { get; set; }
        [PropertyType(PropertyType.ModelTuning)]
        public Double A { get; set; }

        [PropertyType(PropertyType.ModelRuntime)]
        public Collection<Double> u { get; set; }

        [PointType(PointType.Point0 | PointType.Source | PointType.Level0)]
        public Double Level { get; set; }

        public LiquidTank()
        {
            points[0] = () => Level;

            u = new Collection<Double>();
        }

        public override double[] pfGraphPFE(int j, ref double b, int n, FlowDiagram fd)
        {
            double[] a = new Double[n];

            switch (j)
            {
                case 0:
                    a[fe.fn] = 1;
                    b = 0;
                    break;
                case 1:
                    a[fe.pn] = -1;
                    a[f1.pn] = 1;
                    b = 0;
                    break;
                default:
                    a[fe.pn] = -1;
                    a[p1.pn] = 1;
                    {
                        double tu = 0.0;
                        for (int i = 0; i < u.Count; i++)
                        {
                            tu += u[i] * fd.Components[i].Mw;
                        }

                        b = (9.80665E-3) * tu / A;
                    }
                    break;
            }

            return a;
        }

        public override double[] pfGraphPFE_l(int j, ref double b, int n, FlowDiagram fd)
        {
            double[] a = new Double[n];

            switch (j)
            {
                case 0:
                    a[fe.fn] = 1;
                    b = 0;
                    break;
                case 1:
                    a[fe.pn] = -1;
                    a[f1.pn] = 1;
                    b = 0;
                    break;
                default:
                    a[fe.pn] = -1;
                    a[p1.pn] = 1;
                    {
                        double tu = 0.0;
                        for (int i = 0; i < u.Count; i++)
                        {
                            tu += u[i] * fd.Components[i].Mw;
                        }

                        b = (9.80665E-3) * tu / A;
                    }
                    break;
            }

            return a;
        }

        public override void scCalcOutStreams(int part,FlowDiagram fd, bool bInitial)
        {
            Debug.Assert(f1.F >= 0);
            Debug.Assert(p1.F >= 0);
            Debug.Assert(!f1.bReverseFlow);
            Debug.Assert(!p1.bReverseFlow);

            if (part == 2)
            {
                // P and T known
                // so we need to calc PT flash (VLE eq) for p1

                var x = new double[u.Count];
                var xl = new double[u.Count];
                var xv = new double[u.Count];
                double psi;
                double P;

                {
                    // u kmol
                    // Mw kg/kmol
                    // total_mass kg
                    // P kPa

                    double total_mass = 0.0;
                    for (int i = 0; i < u.Count; i++)
                    {
                        total_mass += u[i] * fd.Components[i].Mw;
                        x[i] = u[i];
                    }
                    Flash.Norm1(x);
                    P = fe.P;
                    if (bInitial) P = 101.325; // fix me later
                    P = P + (9.80665E-3) * total_mass / A;
                }

                double Mw, V, H;
                double Mwx, Vx, Hx, Mwy, Vy, Hy;
                Flash.FlashPT(fd, x, P, T,
                    xl, xv, out psi, true, out Mwx, out Vx, out Hx, out Mwy, out Vy, out Hy,
                    p1.Type == PipeType.Liquid, p1.Type == PipeType.Vapor);

                Mw = Mwx * (1.0 - psi) + Mwy * psi;
                H = Hx * (1.0 - psi) + Hy * psi;
                V = Vx * (1.0 - psi) + Vy * psi;

                {
                    // Update Level

                    double total_kmol = 0.0;
                    for (int i = 0; i < u.Count; i++)
                    {
                        total_kmol += u[i];
                    }

                    // Vx m3/kmol

                    Level = Vx * total_kmol / A;
                }

                if (bInitial)
                    p1.P = P;

                p1.T = T;
                p1.x = new Collection<double>(x.ToArray());
                p1.XL = new Collection<double>(xl);
                p1.XV = new Collection<double>(xv);
                p1.RV = psi;
                p1.RL = 1 - psi;
                p1.HL = Hx;
                p1.HV = Hy;
                p1.Mw = Mw;
                p1.V = V;

                if (bInitial)
                    p1.F = 0;
            }
            else
            {
                // nothing to do
            }
        }
        public override void CalcIntrument(int nLevel, PointType pointType)
        {
            // Level
        }

        public override void OneStep(FlowDiagram fd)
        {
            for (int i = 0; i < u.Count; i++)
            {
                double delta = (f1.x[i] * f1.F - p1.x[i] * p1.F) / 3600.0;
                u[i] = Math.Max(0.0, u[i] + delta);
            }
        }

        private UnitTopology emptyTankTopology;

        private bool IsEmpty()
        {
            bool bEmpty = true;

            if (u != null)
            {
                for (int i = 0; i < u.Count; i++)
                {
                    if (u[i] > 0)
                    {
                        bEmpty = false;
                        break;
                    }
                }
            }

            return bEmpty;
        }

        protected override UnitTopology GetTopology()
        {
            if (IsEmpty())
            {
                if (emptyTankTopology == null)
                {
                    emptyTankTopology = new UnitTopology
                    {
                        Parts = new Collection<Collection<int>[]>
                        {
                            new Collection<int>[2] {
                                new Collection<int> { 0 },
                                new Collection<int> { }
                            },
                            new Collection<int>[2] {
                                new Collection<int> { 1 },
                                new Collection<int> { }
                            },
                            new Collection<int>[2] {
                                new Collection<int> { },
                                new Collection<int> { 2 }
                            },
                        },
                        pfPartsKind = new Dictionary<int, PortType>
                        {
                            {0, PortType.PfPartKindFlow },
                            {1, PortType.PfPartKindFlow },
                            {2, PortType.PfPartKindFlow },
                        },
                        portnames = new Collection<string>
                        {
                            "fe","f1","p1"
                        },
                        scPartsKind = new Dictionary<int, PortType>
                        {
                            {0, PortType.ExPartKindNoFlow },
                            {1, PortType.ExPartKindProduct },
                            {2, PortType.ExPartKindNoFlow },
                        }
                    };
                }
                return emptyTankTopology;
            }
            else
                return base.GetTopology();
        }
    }
}
