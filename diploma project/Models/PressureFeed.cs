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
    public class PressureFeed : ProcessUnit
    {
        [PortType(PortType.OutPort | PortType.Port0 | PortType.Part0 | PortType.ExPartKindFeed)]
        public Stream p1 { get { return ports[0]; } set { ports[0] = value; } }

        [PropertyType(PropertyType.ModelTuning)]
        public Collection<Double> x { get; set; }
        [PropertyType(PropertyType.ModelTuning)]
        public Double P { get; set; }
        [PropertyType(PropertyType.ModelTuning)]
        public Double T { get; set; }

        public PressureFeed()
        {
            x = new Collection<Double>();
        }

        public override double[] pfGraphPFE(int j, ref double b, int n, FlowDiagram flowDiagram)
        {
            Debug.Assert(j == 0);
            double[] a = new Double[n];

            a[p1.pn] = 1;
            b = P;

            return a;
        }

        public override double[] pfGraphPFE_l(int j, ref double b, int n, FlowDiagram flowDiagram)
        {
            Debug.Assert(j == 0);
            double[] a = new Double[n];

            a[p1.pn] = 1;
            b = P;

            return a;
        }
/*
        public override double[] scGetxByPart(int item2)
        {
            double[] x = new double[this.x.Count];

            for (int i = 0; i < x.Length; i++)
            {
                x[i] = this.x[i];
            }

            return x;
        }

        public override double scGettByPart(int item2)
        {
            return T;
        }

        public override double scGetFakepByPart(int item2)
        {
            return P;
        }
        */
        public override void scCalcOutStreams(int part, FlowDiagram fd, bool bInitial)
        {
            if (!p1.bReverseFlow)
            {
                // P and T known
                // so we need to calc PT flash (VLE eq) for p1

                Debug.Assert(p1.F >= 0);

                var xl = new double[x.Count];
                var xv = new double[x.Count];
                double psi;

                double Mw, V, H;
                double Mwx, Vx, Hx, Mwy, Vy, Hy;
                Flash.FlashPT(fd, x.ToArray(), P, T,
                    xl, xv, out psi, true, out Mwx, out Vx, out Hx, out Mwy, out Vy, out Hy,
                        p1.Type == PipeType.Liquid, p1.Type == PipeType.Vapor);

                Mw = Mwx * (1.0 - psi) + Mwy * psi;
                H = Hx * (1.0 - psi) + Hy * psi;
                V = Vx * (1.0 - psi) + Vy * psi;

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
        }
    }
}
