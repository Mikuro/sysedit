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
    public class Pipe : ProcessUnit
    {
        [PortType(PortType.InPort | PortType.Port0 | PortType.Part0 | PortType.PfPartKindFlow)]
        public Stream f1 { get { return ports[0]; } set { ports[0] = value; } }
        [PortType(PortType.OutPort | PortType.Port1 | PortType.Part0 | PortType.PfPartKindFlow)]
        public Stream p1 { get { return ports[1]; } set { ports[1] = value; } }

        [PropertyType(PropertyType.ModelTuning)]
        public Double Hdiff { get; set; }
        /*
        public Double CV { get; set; }
        public Double Asur { get; set; }
        public Double Uql { get; set; }
        */

        public override double[] pfGraphPFE(int j, ref double b, int n, FlowDiagram flowDiagram)
        {
            double[] a = new Double[n];

            a[f1.pn] = -1;
            a[p1.pn] = 1;
            b = -(9.80665E-3) * Hdiff * f1.Mw / f1.V;

            return a;
        }

        public override double[] pfGraphPFE_l(int j, ref double b, int n, FlowDiagram flowDiagram)
        {
            double[] a = new Double[n];

            a[f1.pn] = -1;
            a[p1.pn] = 1;
            b = -(9.80665E-3) * Hdiff * f1.Mw / f1.V;

            return a;
        }

        public override void scCalcOutStreams(int part, FlowDiagram fd, bool bInitial)
        {
            Debug.Assert(f1.F >= 0);
            Debug.Assert(p1.F >= 0);
            Debug.Assert(!f1.bReverseFlow);
            Debug.Assert(!p1.bReverseFlow);

            if (bInitial)
                p1.P = Math.Max(0.0,f1.P - (9.80665E-3) * Hdiff * f1.Mw / f1.V);

            var xl = new double[f1.x.Count];
            var xv = new double[f1.x.Count];
            double psi;

            double Mw, V, H;
            double Mwx, Vx, Hx, Mwy, Vy, Hy;
            double T;

            H = f1.HL * (1.0 - f1.RV) + f1.HV * f1.RV;

            if (bInitial)
                T = f1.T;
            else
                T = p1.T;

            T = Flash.FlashPH(fd, f1.x.ToArray(), p1.P, H, T,
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

            if (bInitial)
            {
                p1.F = 0;
            }
        }
    }
}
