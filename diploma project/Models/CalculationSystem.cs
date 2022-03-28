using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tanks.Models
{
    [ModelClass]
    public class CalculationSystem
    {
        [PropertyType(PropertyType.ModelTuning)]
        public Collection<Double[]> SRKKIJ { get; set; }

        //public string ContentType { get { return ""; } set { } }

        public CalculationSystem()
        {
            SRKKIJ = new Collection<Double[]>();
        }
    }
}
