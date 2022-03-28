using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tanks.Models
{
    [ModelClass]
    public class FlowDiagram
    {
        [PropertyType(PropertyType.ModelTuning)]
        public Collection<Models.Component> Components { get; set; }
        [PropertyType(PropertyType.ModelTuning)]
        public Collection<Models.ModelObject> Items { get; set; }
        [PropertyType(PropertyType.ModelTuning)]
        public Collection<Models.ModelLink> Links { get; set; }
        [PropertyType(PropertyType.ModelTuning)]
        public Collection<Models.CalculationSystem> Systems { get; set; }

        [PropertyType(PropertyType.Visual)]
        public double Width { get; set; }
        [PropertyType(PropertyType.Visual)]
        public double Height { get; set; }

        public FlowDiagram()
        {
            Components = new Collection<Models.Component>();
            Items = new Collection<Models.ModelObject>();
            Links = new Collection<Models.ModelLink>();
            Systems = new Collection<Models.CalculationSystem>();

        }

        public void Fix()
        {
            int n = Components.Count;

            for (int i = 0; i < Links.Count; i++)
            {
                var stream = Links[i] as Models.Stream;
                if (stream == null) continue;
                stream.x = new Collection<double>();
                for (int j = 0; j < n; j++) stream.x.Add(0);
                stream.XL = new Collection<double>();
                for (int j = 0; j < n; j++) stream.XL.Add(0);
                stream.XV = new Collection<double>();
                for (int j = 0; j < n; j++) stream.XV.Add(0);
            }
        }
        /*
//Graph<int> g;

Double[][] PrepareSRKKIJ()
{
   var s = (Systems[0] as System);
   var r = new Double[Components.Count][];
   for (int i = 0; i < r.Count(); i++)
   {
       r[i] = new Double[Components.Count];
   }

   for (int i = 0; i < (r.Count()-1); i++)
   {
       Debug.Assert(s.SRKKIJ[i].Count() == (r.Count() - 1 - i));
       for (int j = 0; j < (r.Count() - 1 - i); j++)
       {
           r[i][i + 1 + j] = s.SRKKIJ[i][j];
           r[i + 1 + j][i] = r[i][i + 1 + j];
       }
   }

   return r;
}
*/

    }
}
