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
    public class HeatExchanger : ProcessUnit
    {
        [PortType(PortType.InPort | PortType.Port0 | PortType.Part0 | PortType.PfPartKindFlow)]
        public Stream f1 { get { return ports[0]; } set { ports[0] = value; } }
        [PortType(PortType.OutPort | PortType.Port1 | PortType.Part0 | PortType.PfPartKindFlow)]
        public Stream p1 { get { return ports[1]; } set { ports[1] = value; } }
        [PortType(PortType.InPort | PortType.Port2 | PortType.Part1 | PortType.PfPartKindFlow)]
        public Stream f2 { get { return ports[2]; } set { ports[2] = value; } }
        [PortType(PortType.OutPort | PortType.Port3 | PortType.Part1 | PortType.PfPartKindFlow)]
        public Stream p2 { get { return ports[3]; } set { ports[3] = value; } }

        [PropertyType(PropertyType.ModelTuning)]
        public Double WHdes { get; set; }
        [PropertyType(PropertyType.ModelTuning)]
        public Double DHdes { get; set; }
        [PropertyType(PropertyType.ModelTuning)]
        public Double PdHdes { get; set; }
        [PropertyType(PropertyType.ModelTuning)]
        public Double WLdes { get; set; }
        [PropertyType(PropertyType.ModelTuning)]
        public Double DLdes { get; set; }
        [PropertyType(PropertyType.ModelTuning)]
        public Double PdLdes { get; set; }
        [PropertyType(PropertyType.ModelTuning)]
        public Double A { get; set; }
        [PropertyType(PropertyType.ModelTuning)]
        public Double U { get; set; }

        public override double[] pfGraphPFE(int part, ref double b, int n, FlowDiagram flowDiagram)
        {
            double c;
            double[] a = new Double[n];
            if (part == 0)
            {
                c = ((PdHdes * f1.V * DHdes * f1.Mw) / (WHdes * WHdes));

                a[f1.pn] = -1;
                a[p1.pn] = 1;
                a[f1.fn] = c;
                b = 0;
            }
            else
            {
                c = ((PdLdes * f2.V * DLdes * f2.Mw) / (WLdes * WLdes));

                a[f2.pn] = -1;
                a[p2.pn] = 1;
                a[f2.fn] = c;
                b = 0;
            }
            return a;
        }

        public override double[] pfGraphPFE_l(int part, ref double b, int n, FlowDiagram flowDiagram)
        {
            double c;
            double[] a = new Double[n];
            if (part == 0)
            {
                c = ((PdHdes * f1.V * DHdes * f1.Mw) / (WHdes * WHdes));

                a[f1.pn] = -1;
                a[p1.pn] = 1;
                a[f1.fn] = c * Math.Abs(f1.F);
                b = 0;
            }
            else
            {
                c = ((PdLdes * f2.V * DLdes * f2.Mw) / (WLdes * WLdes));

                a[f2.pn] = -1;
                a[p2.pn] = 1;
                a[f2.fn] = c * Math.Abs(f2.F);
                b = 0;
            }
            return a;
        }

        private void LMTD(FlowDiagram fd,bool bCold)
        {
            var xlh = new double[f1.x.Count];
            var xvh = new double[f1.x.Count];
            var xlc = new double[f1.x.Count];
            var xvc = new double[f1.x.Count];
            double psih;
            double Mwxh, Vxh, Hxh, Mwyh, Vyh, Hyh;
            double psic;
            double Mwxc, Vxc, Hxc, Mwyc, Vyc, Hyc;
            double H1fin;
            double H2fin;
            double T1fin;
            double T2fin;

            bool bCoCurrent = false;

            T1fin = f2.T;
            Flash.FlashPT(fd, f1.x.ToArray(), p1.P, T1fin,
                    xlh, xvh, out psih, true, out Mwxh, out Vxh, out Hxh, out Mwyh, out Vyh, out Hyh,
                    p1.Type == PipeType.Liquid, p1.Type == PipeType.Vapor);

            H1fin = Hxh * (1.0 - psih) + Hyh * psih;

            T2fin = f1.T;
            Flash.FlashPT(fd, f2.x.ToArray(), p2.P, T2fin,
                    xlc, xvc, out psic, true, out Mwxc, out Vxc, out Hxc, out Mwyc, out Vyc, out Hyc,
                    p2.Type == PipeType.Liquid, p2.Type == PipeType.Vapor);

            H2fin = Hxc * (1.0 - psic) + Hyc * psic;

            double dQ1maxT = (f1.HL - H1fin) * f1.F;
            double dQ2maxT = -(f2.HL - H2fin) * f2.F;
            double dQ1maxF = (3.6e-3) * U * A * (f1.T - T1fin);
            double dQ2maxF = (3.6e-3) * U * A * (T2fin - f2.T);

            double Qi = Math.Min(Math.Min(dQ1maxT, dQ1maxF),
                Math.Min(dQ2maxT, dQ2maxF));

            double Qi_old = 0;
            double Hc2=0;
            double Tc2=0;
            double Hh2=0;
            double Th2=0;
            for(int i = 0;i<3;i++)
            {
                double Hc1 = f2.HL * (1.0 - f2.RV) + f2.HV * f2.RV;
                Hc2 = Hc1 + Qi / f2.F;

                double Hh1 = f1.HL * (1.0 - f1.RV) + f1.HV * f1.RV;
                Hh2 = Hh1 - Qi / f1.F;

                double Tc1 = f2.T;
                Tc2 = Flash.FlashPH(fd, f2.x.ToArray(), p2.P, Hc2, Tc1,
                    xlc, xvc, out psic, true, out Mwxc, out Vxc, out Hxc, out Mwyc, out Vyc, out Hyc,
                    p2.Type == PipeType.Liquid, p2.Type == PipeType.Vapor);

                double Th1 = f1.T;
                Th2 = Flash.FlashPH(fd, f1.x.ToArray(), p1.P, Hh2, Th1,
                    xlh, xvh, out psih, true, out Mwxh, out Vxh, out Hxh, out Mwyh, out Vyh, out Hyh,
                    p1.Type == PipeType.Liquid, p1.Type == PipeType.Vapor);

                double WWc = f2.F * (Hc2 - Hc1) / (Tc2 - Tc1) * 1000;
                double WWh = f1.F * (Hh2 - Hh1) / (Th2 - Th1) * 1000;

                double NTUc = 3.6 * U * A / WWc;
                double NTUh = 3.6 * U * A / WWh;
                double RRc = WWc / WWh;
                double RRh = WWh / WWc;
                double PPc;
                double PPh;

                if (bCoCurrent)
                {
                    PPc = (1 - Math.Exp(-NTUc * (1 + RRc))) / (1 + RRc);
                    PPh = (1 - Math.Exp(-NTUh * (1 + RRh))) / (1 + RRh);
                }
                else
                {
                    PPc = (1 - Math.Exp((RRc - 1) * NTUc)) / (1 - RRc * Math.Exp((RRc - 1) * NTUc));
                    PPh = (1 - Math.Exp((RRh - 1) * NTUh)) / (1 - RRh * Math.Exp((RRh - 1) * NTUh));
                }

                if (Double.IsNaN(PPc)) PPc = 0.0;
                if (Double.IsNaN(PPh)) PPh = 0.0;

                Tc2 = Tc1 + PPc * (Th1 - Tc1);
                Th2 = Th1 - PPh * (Th1 - Tc1);

                double LMTD;
                if (bCoCurrent)
                {
                    if ((Th1 - Tc1) / (Th2 - Tc2) == 1.0)
                        LMTD = ((Th1 - Tc1) + (Th2 - Tc2)) / 2.0;
                    else
                        LMTD = ((Th1 - Tc1) - (Th2 - Tc2)) / Math.Log((Th1 - Tc1) / (Th2 - Tc2));
                }
                else
                {
                    if (((Th1 - Tc2) / (Th2 - Tc1)) == 1.0)
                        LMTD = ((Th1 - Tc2) + (Th2 - Tc1)) / 2;
                    else
                        LMTD = ((Th1 - Tc2) - (Th2 - Tc1)) / Math.Log((Th1 - Tc2) / (Th2 - Tc1));
                }

                Qi_old = Qi;

                if (LMTD > 0)
                    Qi = 3.6 * U * A * LMTD / 1000;
                else
                {
                    Qi = f1.F * (Hh1 - Hh2);
                    LMTD = 3.6 * Qi / U / A * 1000;
                }
            } //while (Math.Abs(Qi - Qi_old) >= (Flash.Epsilon*10));
            Qi_old = Qi;

            //if (bCold)
            {
                p2.T = Tc2;
                p2.x = new Collection<double>(f2.x.ToArray());
                p2.XL = new Collection<double>(xlc);
                p2.XV = new Collection<double>(xvc);
                p2.RV = psic;
                p2.RL = 1 - psic;
                p2.HL = Hxc;
                p2.HV = Hyc;
                p2.Mw = Mwxc * (1.0 - psic) + Mwyh * psic;
                p2.V = Vxc * (1.0 - psic) + Vyh * psic;
            }
            //else
            {
                p1.T = Th2;
                p1.x = new Collection<double>(f1.x.ToArray());
                p1.XL = new Collection<double>(xlh);
                p1.XV = new Collection<double>(xvh);
                p1.RV = psih;
                p1.RL = 1 - psih;
                p1.HL = Hxh;
                p1.HV = Hyh;
                p1.Mw = Mwxh * (1.0 - psih) + Mwyh * psih;
                p1.V = Vxh * (1.0 - psih) + Vyh * psih;
            }
        }

        public override void scCalcOutStreams(int part, FlowDiagram fd, bool bInitial)
        {
            Debug.Assert(f1.F >= 0);
            Debug.Assert(p1.F >= 0);
            Debug.Assert(f2.F >= 0);
            Debug.Assert(p2.F >= 0);

            Debug.Assert(!f1.bReverseFlow);
            Debug.Assert(!p1.bReverseFlow);
            Debug.Assert(!f2.bReverseFlow);
            Debug.Assert(!p2.bReverseFlow);

            if (part == 0)
            {
                if (bInitial)
                {
                    p1.P = f1.P;
                    p1.T = f1.T;
                    p1.x = new Collection<double>(f1.x.ToArray());
                    p1.XL = new Collection<double>(f1.XL.ToArray());
                    p1.XV = new Collection<double>(f1.XV.ToArray());
                    p1.RV = f1.RV;
                    p1.RL = 1 - p1.RV;
                    p1.HL = f1.HL;
                    p1.HV = f1.HV;
                    p1.Mw = f1.Mw;
                    p1.V = f1.V;

                    p1.F = f1.F;
                }
                else
                {
                    LMTD(fd,false);
                }
            }
            else
            {
                if (bInitial)
                {
                    p2.P = f2.P;
                    p2.T = f2.T;
                    p2.x = new Collection<double>(f2.x.ToArray());
                    p2.XL = new Collection<double>(f2.XL.ToArray());
                    p2.XV = new Collection<double>(f2.XV.ToArray());
                    p2.RV = f2.RV;
                    p2.RL = 1 - p2.RV;
                    p2.HL = f2.HL;
                    p2.HV = f2.HV;
                    p2.Mw = f2.Mw;
                    p2.V = f2.V;

                    p2.F = f2.F;
                }
                else
                {
                    LMTD(fd,true);
                }
            }
        }
    }
}
