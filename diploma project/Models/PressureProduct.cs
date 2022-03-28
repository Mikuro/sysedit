using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tanks.Models
{
    [ModelClass]
    public class PressureProduct : ProcessUnit
    {
        [PortType(PortType.InPort | PortType.Port0 | PortType.Part0 | PortType.ExPartKindProduct)]
        public Stream f1 { get { return ports[0]; } set { ports[0] = value; } }

        [PropertyType(PropertyType.ModelTuning)]
        public Double P { get; set; }

        public override double[] pfGraphPFE(int j, ref double b, int n, FlowDiagram flowDiagram)
        {
            Debug.Assert(j == 0);
            double[] a = new Double[n];

            a[f1.pn] = 1;
            b = P;

            return a;
        }

        public override double[] pfGraphPFE_l(int j, ref double b, int n, FlowDiagram flowDiagram)
        {
            Debug.Assert(j == 0);
            double[] a = new Double[n];

            a[f1.pn] = 1;
            b = P;

            return a;
        }

        public override void scCalcOutStreams(int part,FlowDiagram fd, bool bInitial)
        {
            Debug.Assert(f1.F >= 0);
            Debug.Assert(!f1.bReverseFlow);
            // nothing to do
        }

    }
}
