using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tanks.Models
{
    public enum PipeType
    {
        Liquid = 1,
        Vapor,
        VaporAndLiquid
    }

    [ModelClass]
    public class Stream : ModelLink
    {
        [PropertyType(PropertyType.ModelTuning)]
        public PipeType Type { get; set; }

        public Tuple<ProcessUnit, int>[] sports = new Tuple<ProcessUnit, int>[2];
        public Stream()
        { 
        }

        [PropertyType(PropertyType.Calculated)]
        public Collection<Double> x { get; set; }

        double _F;
        [PropertyType(PropertyType.Calculated)]
        public double F // кмоль/ч
        {
            get { return _F; }
            set
            {
                if (value > 0) bReverseFlow = false;
                if (value < 0) bReverseFlow = true;

                _F = value;
            }
        }

        [PropertyType(PropertyType.Calculated)]
        public double P { get; set; } // кПа
        [PropertyType(PropertyType.Calculated)]
        public double T { get; set; } // С
        [PropertyType(PropertyType.Calculated)]
        public double RL { get; set; }
        [PropertyType(PropertyType.Calculated)]
        public double HL { get; set; } // Дж/моль == кДж/кмоль
        [PropertyType(PropertyType.Calculated)]
        public Collection<Double> XL { get; set; }
        [PropertyType(PropertyType.Calculated)]
        public double RV { get; set; }
        [PropertyType(PropertyType.Calculated)]
        public double HV { get; set; }
        [PropertyType(PropertyType.Calculated)]
        public Collection<Double> XV { get; set; }
        [PropertyType(PropertyType.Calculated)]
        public double Mw { get; set; } // г/моль == кг/кмоль
        [PropertyType(PropertyType.Calculated)]
        public double V { get; set; }  // л/моль == м^3/кмоль

        public bool bReverseFlow;

        public int pn;
        public int fn;
        public void set_pfn(int pn, int fn)
        {
            this.pn = pn;
            this.fn = fn;
        }
        public void fix_pfn(int cp)
        {
            this.fn += cp;
        }

        public void save_solution(double[] x)
        {
            Debug.Assert(x[pn] >= 0);

            P = x[pn];
            F = Math.Sqrt(Math.Abs(x[fn]))*Math.Sign(x[fn]);
        }

        public void save_solution_l(double[] x)
        {
            Debug.Assert(x[pn] >= 0);
            
            P = x[pn];
            F = x[fn];

            //F = 0.5 * (F + x[fn]);

        }

        public Func<double> GetPoint(InstrumentUnit obj)
        {
            Func<double> ret;

            string name = obj.GetType().Name;

            switch (name)
            {
                case "FlowMeter":
                    ret = () => (obj as FlowMeter).Flow(this);
                    break;
                case "PressureGauge":
                    ret = () => P;
                    break;
                case "Thermometer":
                    ret = () => T;
                    break;
                default:
                    throw new Exception("Stream.GetPoint failed!");
            }
            return ret;
        }
    }
}
