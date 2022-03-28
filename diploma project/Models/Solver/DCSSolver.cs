using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tanks.Models;

namespace tanks.Models.Solver
{
    public static class DCSSolver
    {
        static public void SolveInstrument(FlowDiagram fd)
        {
            var dsts = new HashSet<int>[4];
            var srcs = new HashSet<int>[4];
            for (int i = 0; i < dsts.Length; i++)
            {
                dsts[i] = new HashSet<int>();
                srcs[i] = new HashSet<int>();
            }

            for (int iitem=0;iitem<fd.Items.Count;iitem++)
            {
                var item = fd.Items[iitem];
                for (int i = 0; i < item.GetPointsCount(); i++)
                {
                    if (item.GetPointDirection(i)==PointType.Destination)
                    {
                        dsts[item.GetPointLevel(i)].Add(iitem);
                    }else
                        srcs[item.GetPointLevel(i)].Add(iitem);
                }
            }

            for (int i = 0; i < dsts.Length; i++)
            {
                foreach (var item in srcs[i])
                {
                    fd.Items[item].CalcIntrument(i, PointType.Source);
                }

                foreach (var item in dsts[i])
                {
                    fd.Items[item].CalcIntrument(i,PointType.Destination);
                }
            }
        }

        public static void OneStep(FlowDiagram fd)
        {
            for (int iitem = 0; iitem < fd.Items.Count; iitem++)
                fd.Items[iitem].OneStep(fd);

        }
    }
}
