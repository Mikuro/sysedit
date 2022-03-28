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
    public class Splitter : ProcessUnit
    {
        [PortType(PortType.InPort | PortType.Port0 | PortType.Part0 | PortType.PfPartKindPressure)]
        public Stream f1 { get { return ports[0]; } set { ports[0] = value; } }
        [PortType(PortType.OutPort | PortType.Port1 | PortType.Part0 | PortType.PfPartKindPressure)]
        public Stream p1 { get { return ports[1]; } set { ports[1] = value; } }
        [PortType(PortType.OutPort | PortType.Port2 | PortType.Part0 | PortType.PfPartKindPressure)]
        public Stream p2 { get { return ports[2]; } set { ports[2] = value; } }
        [PortType(PortType.OutPort | PortType.Port3 | PortType.Part0 | PortType.PfPartKindPressure)]
        public Stream p3 { get { return ports[3]; } set { ports[3] = value; } }

//        double[] cf = new double[3];

        double cf_in;
        double cf_out;

        double[] cf = new double[4];

        public override double[] pfGraphPFE(int j, ref double b, int n, FlowDiagram flowDiagram)
        {
            double[] a = new Double[n];

            var ipd = GraphInPortsByPartDynamic(0);
            var opd = GraphOutPortsByPartDynamic(0);

            foreach (var idx in ipd)
            {
                if (idx < 1)
                    a[ports[idx].fn] = cf_out;
                else
                    a[ports[idx].fn] = -cf_out;
            }

            foreach (var idx in opd)
            {
                if (idx < 1)
                    a[ports[idx].fn] = cf_in;
                else
                    a[ports[idx].fn] = -cf_in;
            }
            b = 0;

            return a;
        }

        public override double[] pfGraphPFE_l(int j, ref double b, int n, FlowDiagram flowDiagram)
        {
            double[] a = new Double[n];

            var ipd = GraphInPortsByPartDynamic(0);
            var opd = GraphOutPortsByPartDynamic(0);

            foreach (var idx in ipd)
            {
                if (idx < 1)
                    a[ports[idx].fn] = 1;
                else
                    a[ports[idx].fn] = -1;
            }

            foreach (var idx in opd)
            {
                if (idx < 1)
                    a[ports[idx].fn] = 1;
                else
                    a[ports[idx].fn] = -1;
            }
            b = 0;

            return a;
        }

        public override void scCalcOutStreams(int part, FlowDiagram fd, bool bInitial)
        {
            var ipd = GraphInPortsByPartDynamic(part);
            var opd = GraphOutPortsByPartDynamic(part);

            Debug.Assert(ipd.Length > 0);
            Debug.Assert(opd.Length > 0);

            double ftot_in = 0.0;
            double ftot_out = 0.0;
            
            foreach (var idx in ipd)
                ftot_in += Math.Abs(ports[idx].F);

            foreach (var idx in opd)
                ftot_out += Math.Abs(ports[idx].F);

            double P = ports[ipd[0]].P;

            double Tk = 0;
            double H = 0.0;
            double[] x = new double[fd.Components.Count];

            if (bInitial)
            {
                foreach (var idx in ipd)
                {
                    cf[idx] = 1.0 / ipd.Length;
                    P = Math.Max(P, ports[ipd[0]].P);
                }

                foreach (var idx in opd)
                    cf[idx] = 1.0 / opd.Length;
            }
            else
            {
                // next if looks bad
                if ((ftot_in == 0) || (ftot_out == 0))
                {
                    foreach (var idx in ipd)
                        cf[idx] = 1.0 / ipd.Length;
                    foreach (var idx in opd)
                        cf[idx] = 1.0 / opd.Length;
                }
                else
                {

                    foreach (var i in ipd)
                        cf[i] = Math.Abs(ports[i].F) / ftot_in;

                    foreach (var i in opd)
                        cf[i] = Math.Abs(ports[i].F) / ftot_out;
                }
            }

            cf_in = 0;
            foreach (var i in ipd)
                cf_in += cf[i] * cf[i];

            cf_out = 0;
            foreach (var i in opd)
                cf_out += cf[i] * cf[i];

            foreach (var i in ipd)
            {
                H += (ports[i].HL * (1.0 - ports[i].RV) + ports[i].HV * ports[i].RV) * cf[i];
                Tk += (ports[i].T + 273.15) * cf[i];

                for (int j = 0; j < x.Length; j++)
                    x[j] += ports[i].x[j] * cf[i];
            }

            double[] xl = new double[x.Length];
            double[] xv = new double[x.Length];
            double psi;
            double Mwx;
            double Mwy;
            double Vx;
            double Vy;
            double Hx;
            double Hy;
            double T;

            bool bForceLiquid = true;
            bool bForceVapor = true;
            foreach (var i in opd)
            {
                if (ports[i].Type != PipeType.Liquid)
                    bForceLiquid = false;

                if (ports[i].Type != PipeType.Vapor)
                    bForceVapor = false;
            }

            T = Flash.FlashPH(fd, x, P, H, Tk - 273.15,
                xl, xv, out psi, true, out Mwx, out Vx, out Hx, out Mwy, out Vy, out Hy,
                bForceLiquid, bForceVapor);

            double Mw = Mwx * (1.0 - psi) + Mwy * psi;
            H = Hx * (1.0 - psi) + Hy * psi;
            double V = Vx * (1.0 - psi) + Vy * psi;

            foreach (var idx in opd)
            {
                if (bInitial)
                    ports[idx].P = P;
                ports[idx].T = T;
                ports[idx].x = new Collection<double>(x);
                ports[idx].XL = new Collection<double>(xl);
                ports[idx].XV = new Collection<double>(xv);
                ports[idx].RV = psi;
                ports[idx].RL = 1 - psi;
                ports[idx].HL = Hx;
                ports[idx].HV = Hy;
                ports[idx].Mw = Mw;
                ports[idx].V = V;
            }
        }
    }
}
