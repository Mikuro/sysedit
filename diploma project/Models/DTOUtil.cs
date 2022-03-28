
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace tanks.Models
{
	public static class DTOUtil
	{

        public static void Update_double_array(Double[] dst, Double[] src)
        {
            for(int i=0; i<src.Length; i++)
                dst[i]=src[i];
        }

        public static void UpdateCollection_double_array(Collection<Double[]> dst, Collection<Double[]> src)
        {
            for(int i=0; i<src.Count; i++)
            Update_double_array(dst[i],src[i]);
        }

        public static void UpdateCollection_double(Collection<double> dst, Collection<double> src)
        {
            for(int i=0; i<src.Count; i++)
                dst[i]=src[i];
        }

        public static void UpdateCollection_Component(Collection<tanks.Models.Component> dst, Collection<tanks.Models.Component> src)
        {
            for(int i=0; i<src.Count; i++)
                UpdateComponent(dst[i], src[i]);
        }

        public static void UpdateCollection_CalculationSystem(Collection<tanks.Models.CalculationSystem> dst, Collection<tanks.Models.CalculationSystem> src)
        {
            for(int i=0; i<src.Count; i++)
                UpdateCalculationSystem(dst[i], src[i]);
        }

        public static void UpdateCollection_ModelLink(Collection<tanks.Models.ModelLink> dst, Collection<tanks.Models.ModelLink> src)
        {
            for(int i=0; i<src.Count; i++)
            {
                switch(src[i].GetType().Name)
                {
                case "Signal":
                    UpdateSignal(dst[i] as tanks.Models.Signal, src[i] as tanks.Models.Signal);
                    break;
                case "Stream":
                    UpdateStream(dst[i] as tanks.Models.Stream, src[i] as tanks.Models.Stream);
                    break;
                default:
                        throw new Exception("fail");
                }
            }
        }

        public static void UpdateCollection_ModelObject(Collection<tanks.Models.ModelObject> dst, Collection<tanks.Models.ModelObject> src)
        {
            for(int i=0; i<src.Count; i++)
            {

                switch(src[i].GetType().Name)
                {
                case "ControlValve":
                    UpdateControlValve(dst[i] as tanks.Models.ControlValve, src[i] as tanks.Models.ControlValve);
                    break;
                case "FlowMeter":
                    UpdateFlowMeter(dst[i] as tanks.Models.FlowMeter, src[i] as tanks.Models.FlowMeter);
                    break;
                case "HeatExchanger":
                    UpdateHeatExchanger(dst[i] as tanks.Models.HeatExchanger, src[i] as tanks.Models.HeatExchanger);
                    break;
                case "LiquidLevelMeter":
                    UpdateLiquidLevelMeter(dst[i] as tanks.Models.LiquidLevelMeter, src[i] as tanks.Models.LiquidLevelMeter);
                    break;
                case "LiquidTank":
                    UpdateLiquidTank(dst[i] as tanks.Models.LiquidTank, src[i] as tanks.Models.LiquidTank);
                    break;
                case "Mixer":
                    UpdateMixer(dst[i] as tanks.Models.Mixer, src[i] as tanks.Models.Mixer);
                    break;
                case "PIDController":
                    UpdatePIDController(dst[i] as tanks.Models.PIDController, src[i] as tanks.Models.PIDController);
                    break;
                case "Pipe":
                    UpdatePipe(dst[i] as tanks.Models.Pipe, src[i] as tanks.Models.Pipe);
                    break;
                case "PressureFeed":
                    UpdatePressureFeed(dst[i] as tanks.Models.PressureFeed, src[i] as tanks.Models.PressureFeed);
                    break;
                case "PressureGauge":
                    UpdatePressureGauge(dst[i] as tanks.Models.PressureGauge, src[i] as tanks.Models.PressureGauge);
                    break;
                case "PressureProduct":
                    UpdatePressureProduct(dst[i] as tanks.Models.PressureProduct, src[i] as tanks.Models.PressureProduct);
                    break;
                case "Pump":
                    UpdatePump(dst[i] as tanks.Models.Pump, src[i] as tanks.Models.Pump);
                    break;
                case "Splitter":
                    UpdateSplitter(dst[i] as tanks.Models.Splitter, src[i] as tanks.Models.Splitter);
                    break;
                case "Strainer":
                    UpdateStrainer(dst[i] as tanks.Models.Strainer, src[i] as tanks.Models.Strainer);
                    break;
                case "Thermometer":
                    UpdateThermometer(dst[i] as tanks.Models.Thermometer, src[i] as tanks.Models.Thermometer);
                    break;
                default:
                        throw new Exception("fail");
                }
            }
        }

		public static void UpdateCalculationSystem(tanks.Models.CalculationSystem dst, tanks.Models.CalculationSystem src)
		{
                UpdateCollection_double_array(dst.SRKKIJ, src.SRKKIJ);
		}
		public static void UpdateComponent(tanks.Models.Component dst, tanks.Models.Component src)
		{
                dst.Id = src.Id;
                dst.Mw = src.Mw;
                dst.Tc = src.Tc;
                dst.Pc = src.Pc;
                dst.Zc = src.Zc;
                dst.Omega = src.Omega;
                dst.Higa = src.Higa;
                dst.Higb = src.Higb;
                dst.Higc = src.Higc;
                dst.Higd = src.Higd;
                dst.Hige = src.Hige;
                dst.Higf = src.Higf;
                dst.ZRA = src.ZRA;
		}
		public static void UpdateControlValve(tanks.Models.ControlValve dst, tanks.Models.ControlValve src)
		{
                dst.CV = src.CV;
                dst.R = src.R;
                dst.pos0 = src.pos0;
                dst.topen = src.topen;
                dst.tclose = src.tclose;
                dst.tlag = src.tlag;
                dst.Cff = src.Cff;
                dst.mv = src.mv;
                dst.pos = src.pos;
                dst.velosity = src.velosity;
                dst.X = src.X;
                dst.Y = src.Y;
                dst.Id = src.Id;
		}
		public static void UpdateFlowDiagram(tanks.Models.FlowDiagram dst, tanks.Models.FlowDiagram src)
		{
                UpdateCollection_Component(dst.Components, src.Components);
                UpdateCollection_ModelObject(dst.Items, src.Items);
                UpdateCollection_ModelLink(dst.Links, src.Links);
                UpdateCollection_CalculationSystem(dst.Systems, src.Systems);
                dst.Width = src.Width;
                dst.Height = src.Height;
		}
		public static void UpdateFlowMeter(tanks.Models.FlowMeter dst, tanks.Models.FlowMeter src)
		{
                dst.Fmax = src.Fmax;
                dst.Fmin = src.Fmin;
                dst.unit = src.unit;
                dst.t_unit = src.t_unit;
                dst.F = src.F;
                dst.X = src.X;
                dst.Y = src.Y;
                dst.Id = src.Id;
		}
		public static void UpdateHeatExchanger(tanks.Models.HeatExchanger dst, tanks.Models.HeatExchanger src)
		{
                dst.WHdes = src.WHdes;
                dst.DHdes = src.DHdes;
                dst.PdHdes = src.PdHdes;
                dst.WLdes = src.WLdes;
                dst.DLdes = src.DLdes;
                dst.PdLdes = src.PdLdes;
                dst.A = src.A;
                dst.U = src.U;
                dst.X = src.X;
                dst.Y = src.Y;
                dst.Id = src.Id;
		}
		public static void UpdateLiquidLevelMeter(tanks.Models.LiquidLevelMeter dst, tanks.Models.LiquidLevelMeter src)
		{
                dst.Ll = src.Ll;
                dst.Lh = src.Lh;
                dst.Lbase = src.Lbase;
                dst.unit = src.unit;
                dst.L = src.L;
                dst.X = src.X;
                dst.Y = src.Y;
                dst.Id = src.Id;
		}
		public static void UpdateLiquidTank(tanks.Models.LiquidTank dst, tanks.Models.LiquidTank src)
		{
                dst.T = src.T;
                dst.A = src.A;
                UpdateCollection_double(dst.u, src.u);
                dst.X = src.X;
                dst.Y = src.Y;
                dst.Id = src.Id;
		}
		public static void UpdateMixer(tanks.Models.Mixer dst, tanks.Models.Mixer src)
		{
                dst.X = src.X;
                dst.Y = src.Y;
                dst.Id = src.Id;
		}
		public static void UpdatePIDController(tanks.Models.PIDController dst, tanks.Models.PIDController src)
		{
                dst.PB = src.PB;
                dst.TI = src.TI;
                dst.TD = src.TD;
                dst.PVSPAN = src.PVSPAN;
                dst.PVBASE = src.PVBASE;
                dst.INCDEC = src.INCDEC;
                dst.MVSPAN = src.MVSPAN;
                dst.MVH = src.MVH;
                dst.MVL = src.MVL;
                dst.SV = src.SV;
                dst.PV = src.PV;
                dst.MV = src.MV;
                dst.CMOD = src.CMOD;
                dst.SVM = src.SVM;
                dst.MVM = src.MVM;
                dst.e = src.e;
                dst.E = src.E;
                dst.X = src.X;
                dst.Y = src.Y;
                dst.Id = src.Id;
		}
		public static void UpdatePipe(tanks.Models.Pipe dst, tanks.Models.Pipe src)
		{
                dst.Hdiff = src.Hdiff;
                dst.X = src.X;
                dst.Y = src.Y;
                dst.Id = src.Id;
		}
		public static void UpdatePressureFeed(tanks.Models.PressureFeed dst, tanks.Models.PressureFeed src)
		{
                UpdateCollection_double(dst.x, src.x);
                dst.P = src.P;
                dst.T = src.T;
                dst.X = src.X;
                dst.Y = src.Y;
                dst.Id = src.Id;
		}
		public static void UpdatePressureGauge(tanks.Models.PressureGauge dst, tanks.Models.PressureGauge src)
		{
                dst.Pl = src.Pl;
                dst.Ph = src.Ph;
                dst.unit = src.unit;
                dst.P = src.P;
                dst.X = src.X;
                dst.Y = src.Y;
                dst.Id = src.Id;
		}
		public static void UpdatePressureProduct(tanks.Models.PressureProduct dst, tanks.Models.PressureProduct src)
		{
                dst.P = src.P;
                dst.X = src.X;
                dst.Y = src.Y;
                dst.Id = src.Id;
		}
		public static void UpdatePump(tanks.Models.Pump dst, tanks.Models.Pump src)
		{
                dst.Hd = src.Hd;
                dst.Hs = src.Hs;
                dst.Vd = src.Vd;
                dst.dd = src.dd;
                dst.Wd = src.Wd;
                dst.Ws = src.Ws;
                dst.pos0 = src.pos0;
                dst.tlag = src.tlag;
                dst.mv = src.mv;
                dst.pos = src.pos;
                dst.X = src.X;
                dst.Y = src.Y;
                dst.Id = src.Id;
		}
		public static void UpdateSignal(tanks.Models.Signal dst, tanks.Models.Signal src)
		{
                dst.Id = src.Id;
                dst.From = src.From;
                dst.To = src.To;
                dst.Figures = src.Figures;
		}
		public static void UpdateSplitter(tanks.Models.Splitter dst, tanks.Models.Splitter src)
		{
                dst.X = src.X;
                dst.Y = src.Y;
                dst.Id = src.Id;
		}
		public static void UpdateStrainer(tanks.Models.Strainer dst, tanks.Models.Strainer src)
		{
                dst.Wd = src.Wd;
                dst.DP = src.DP;
                dst.dd = src.dd;
                dst.X = src.X;
                dst.Y = src.Y;
                dst.Id = src.Id;
		}
		public static void UpdateStream(tanks.Models.Stream dst, tanks.Models.Stream src)
		{
                dst.Type = src.Type;
                UpdateCollection_double(dst.x, src.x);
                dst.F = src.F;
                dst.P = src.P;
                dst.T = src.T;
                dst.RL = src.RL;
                dst.HL = src.HL;
                UpdateCollection_double(dst.XL, src.XL);
                dst.RV = src.RV;
                dst.HV = src.HV;
                UpdateCollection_double(dst.XV, src.XV);
                dst.Mw = src.Mw;
                dst.V = src.V;
                dst.Id = src.Id;
                dst.From = src.From;
                dst.To = src.To;
                dst.Figures = src.Figures;
		}
		public static void UpdateThermometer(tanks.Models.Thermometer dst, tanks.Models.Thermometer src)
		{
                dst.Tl = src.Tl;
                dst.Th = src.Th;
                dst.unit = src.unit;
                dst.T = src.T;
                dst.X = src.X;
                dst.Y = src.Y;
                dst.Id = src.Id;
		}

// ---------------------------------
    
        public static Double[] CreateFrom_double_array(Double[] src)
        {
            var dst = new Double[src.Length];

            for(int i=0; i<src.Length; i++)
                dst[i]=src[i];

            return dst;
        }

        public static Collection<Double[]> CreateFromCollection_double_array(Collection<Double[]> src)
        {
            var dst = new Collection<Double[]>();

            for(int i=0; i<src.Count; i++)
                dst.Add(CreateFrom_double_array(src[i]));

            return dst;
        }

        public static Collection<double> CreateFromCollection_double(Collection<double> src)
        {
            var dst = new Collection<double>();

            for(int i=0; i<src.Count; i++)
                dst.Add(src[i]);

            return dst;
        }

        public static Collection<tanks.Models.Component> CreateFromCollection_Component(Collection<tanks.Models.Component> src)
        {
            var dst = new Collection<tanks.Models.Component>();

            for(int i=0; i<src.Count; i++)
                dst.Add(CreateFromComponent(src[i]));

            return dst;
        }

        public static Collection<tanks.Models.CalculationSystem> CreateFromCollection_CalculationSystem(Collection<tanks.Models.CalculationSystem> src)
        {
            var dst = new Collection<tanks.Models.CalculationSystem>();

            for(int i=0; i<src.Count; i++)
                dst.Add(CreateFromCalculationSystem(src[i]));

            return dst;
        }

        public static Collection<tanks.Models.ModelLink> CreateFromCollection_ModelLink(Collection<tanks.Models.ModelLink> src)
        {
            var dst = new Collection<tanks.Models.ModelLink>();

            for(int i=0; i<src.Count; i++)
            {
                tanks.Models.ModelLink link;

                switch(src[i].GetType().Name)
                {
                case "Signal":
                    link = CreateFromSignal(src[i] as tanks.Models.Signal);
                    break;
                case "Stream":
                    link = CreateFromStream(src[i] as tanks.Models.Stream);
                    break;
                default:
                        throw new Exception("fail");
                }
                dst.Add(link);
            }

            return dst;
        }

        public static Collection<tanks.Models.ModelObject> CreateFromCollection_ModelObject(Collection<tanks.Models.ModelObject> src)
        {
            var dst = new Collection<tanks.Models.ModelObject>();

            for(int i=0; i<src.Count; i++)
            {
                tanks.Models.ModelObject obj;

                switch(src[i].GetType().Name)
                {
                case "ControlValve":
                    obj = CreateFromControlValve(src[i] as tanks.Models.ControlValve);
                    break;
                case "FlowMeter":
                    obj = CreateFromFlowMeter(src[i] as tanks.Models.FlowMeter);
                    break;
                case "HeatExchanger":
                    obj = CreateFromHeatExchanger(src[i] as tanks.Models.HeatExchanger);
                    break;
                case "LiquidLevelMeter":
                    obj = CreateFromLiquidLevelMeter(src[i] as tanks.Models.LiquidLevelMeter);
                    break;
                case "LiquidTank":
                    obj = CreateFromLiquidTank(src[i] as tanks.Models.LiquidTank);
                    break;
                case "Mixer":
                    obj = CreateFromMixer(src[i] as tanks.Models.Mixer);
                    break;
                case "PIDController":
                    obj = CreateFromPIDController(src[i] as tanks.Models.PIDController);
                    break;
                case "Pipe":
                    obj = CreateFromPipe(src[i] as tanks.Models.Pipe);
                    break;
                case "PressureFeed":
                    obj = CreateFromPressureFeed(src[i] as tanks.Models.PressureFeed);
                    break;
                case "PressureGauge":
                    obj = CreateFromPressureGauge(src[i] as tanks.Models.PressureGauge);
                    break;
                case "PressureProduct":
                    obj = CreateFromPressureProduct(src[i] as tanks.Models.PressureProduct);
                    break;
                case "Pump":
                    obj = CreateFromPump(src[i] as tanks.Models.Pump);
                    break;
                case "Splitter":
                    obj = CreateFromSplitter(src[i] as tanks.Models.Splitter);
                    break;
                case "Strainer":
                    obj = CreateFromStrainer(src[i] as tanks.Models.Strainer);
                    break;
                case "Thermometer":
                    obj = CreateFromThermometer(src[i] as tanks.Models.Thermometer);
                    break;
                default:
                        throw new Exception("fail");
                }
                dst.Add(obj);
            }

            return dst;
        }

		public static tanks.Models.CalculationSystem CreateFromCalculationSystem(tanks.Models.CalculationSystem src)
		{
			return new tanks.Models.CalculationSystem{
                SRKKIJ = CreateFromCollection_double_array(src.SRKKIJ),
			};
		}
		public static tanks.Models.Component CreateFromComponent(tanks.Models.Component src)
		{
			return new tanks.Models.Component{
                Id = src.Id,
                Mw = src.Mw,
                Tc = src.Tc,
                Pc = src.Pc,
                Zc = src.Zc,
                Omega = src.Omega,
                Higa = src.Higa,
                Higb = src.Higb,
                Higc = src.Higc,
                Higd = src.Higd,
                Hige = src.Hige,
                Higf = src.Higf,
                ZRA = src.ZRA,
			};
		}
		public static tanks.Models.ControlValve CreateFromControlValve(tanks.Models.ControlValve src)
		{
			return new tanks.Models.ControlValve{
                CV = src.CV,
                R = src.R,
                pos0 = src.pos0,
                topen = src.topen,
                tclose = src.tclose,
                tlag = src.tlag,
                Cff = src.Cff,
                mv = src.mv,
                pos = src.pos,
                velosity = src.velosity,
                X = src.X,
                Y = src.Y,
                Id = src.Id,
			};
		}
		public static tanks.Models.FlowDiagram CreateFromFlowDiagram(tanks.Models.FlowDiagram src)
		{
			return new tanks.Models.FlowDiagram{
                Components = CreateFromCollection_Component(src.Components),
                Items = CreateFromCollection_ModelObject(src.Items),
                Links = CreateFromCollection_ModelLink(src.Links),
                Systems = CreateFromCollection_CalculationSystem(src.Systems),
                Width = src.Width,
                Height = src.Height,
			};
		}
		public static tanks.Models.FlowMeter CreateFromFlowMeter(tanks.Models.FlowMeter src)
		{
			return new tanks.Models.FlowMeter{
                Fmax = src.Fmax,
                Fmin = src.Fmin,
                unit = src.unit,
                t_unit = src.t_unit,
                F = src.F,
                X = src.X,
                Y = src.Y,
                Id = src.Id,
			};
		}
		public static tanks.Models.HeatExchanger CreateFromHeatExchanger(tanks.Models.HeatExchanger src)
		{
			return new tanks.Models.HeatExchanger{
                WHdes = src.WHdes,
                DHdes = src.DHdes,
                PdHdes = src.PdHdes,
                WLdes = src.WLdes,
                DLdes = src.DLdes,
                PdLdes = src.PdLdes,
                A = src.A,
                U = src.U,
                X = src.X,
                Y = src.Y,
                Id = src.Id,
			};
		}
		public static tanks.Models.LiquidLevelMeter CreateFromLiquidLevelMeter(tanks.Models.LiquidLevelMeter src)
		{
			return new tanks.Models.LiquidLevelMeter{
                Ll = src.Ll,
                Lh = src.Lh,
                Lbase = src.Lbase,
                unit = src.unit,
                L = src.L,
                X = src.X,
                Y = src.Y,
                Id = src.Id,
			};
		}
		public static tanks.Models.LiquidTank CreateFromLiquidTank(tanks.Models.LiquidTank src)
		{
			return new tanks.Models.LiquidTank{
                T = src.T,
                A = src.A,
                u = CreateFromCollection_double(src.u),
                X = src.X,
                Y = src.Y,
                Id = src.Id,
			};
		}
		public static tanks.Models.Mixer CreateFromMixer(tanks.Models.Mixer src)
		{
			return new tanks.Models.Mixer{
                X = src.X,
                Y = src.Y,
                Id = src.Id,
			};
		}
		public static tanks.Models.PIDController CreateFromPIDController(tanks.Models.PIDController src)
		{
			return new tanks.Models.PIDController{
                PB = src.PB,
                TI = src.TI,
                TD = src.TD,
                PVSPAN = src.PVSPAN,
                PVBASE = src.PVBASE,
                INCDEC = src.INCDEC,
                MVSPAN = src.MVSPAN,
                MVH = src.MVH,
                MVL = src.MVL,
                SV = src.SV,
                PV = src.PV,
                MV = src.MV,
                CMOD = src.CMOD,
                SVM = src.SVM,
                MVM = src.MVM,
                e = src.e,
                E = src.E,
                X = src.X,
                Y = src.Y,
                Id = src.Id,
			};
		}
		public static tanks.Models.Pipe CreateFromPipe(tanks.Models.Pipe src)
		{
			return new tanks.Models.Pipe{
                Hdiff = src.Hdiff,
                X = src.X,
                Y = src.Y,
                Id = src.Id,
			};
		}
		public static tanks.Models.PressureFeed CreateFromPressureFeed(tanks.Models.PressureFeed src)
		{
			return new tanks.Models.PressureFeed{
                x = CreateFromCollection_double(src.x),
                P = src.P,
                T = src.T,
                X = src.X,
                Y = src.Y,
                Id = src.Id,
			};
		}
		public static tanks.Models.PressureGauge CreateFromPressureGauge(tanks.Models.PressureGauge src)
		{
			return new tanks.Models.PressureGauge{
                Pl = src.Pl,
                Ph = src.Ph,
                unit = src.unit,
                P = src.P,
                X = src.X,
                Y = src.Y,
                Id = src.Id,
			};
		}
		public static tanks.Models.PressureProduct CreateFromPressureProduct(tanks.Models.PressureProduct src)
		{
			return new tanks.Models.PressureProduct{
                P = src.P,
                X = src.X,
                Y = src.Y,
                Id = src.Id,
			};
		}
		public static tanks.Models.Pump CreateFromPump(tanks.Models.Pump src)
		{
			return new tanks.Models.Pump{
                Hd = src.Hd,
                Hs = src.Hs,
                Vd = src.Vd,
                dd = src.dd,
                Wd = src.Wd,
                Ws = src.Ws,
                pos0 = src.pos0,
                tlag = src.tlag,
                mv = src.mv,
                pos = src.pos,
                X = src.X,
                Y = src.Y,
                Id = src.Id,
			};
		}
		public static tanks.Models.Signal CreateFromSignal(tanks.Models.Signal src)
		{
			return new tanks.Models.Signal{
                Id = src.Id,
                From = src.From,
                To = src.To,
                Figures = src.Figures,
			};
		}
		public static tanks.Models.Splitter CreateFromSplitter(tanks.Models.Splitter src)
		{
			return new tanks.Models.Splitter{
                X = src.X,
                Y = src.Y,
                Id = src.Id,
			};
		}
		public static tanks.Models.Strainer CreateFromStrainer(tanks.Models.Strainer src)
		{
			return new tanks.Models.Strainer{
                Wd = src.Wd,
                DP = src.DP,
                dd = src.dd,
                X = src.X,
                Y = src.Y,
                Id = src.Id,
			};
		}
		public static tanks.Models.Stream CreateFromStream(tanks.Models.Stream src)
		{
			return new tanks.Models.Stream{
                Type = src.Type,
                x = CreateFromCollection_double(src.x),
                F = src.F,
                P = src.P,
                T = src.T,
                RL = src.RL,
                HL = src.HL,
                XL = CreateFromCollection_double(src.XL),
                RV = src.RV,
                HV = src.HV,
                XV = CreateFromCollection_double(src.XV),
                Mw = src.Mw,
                V = src.V,
                Id = src.Id,
                From = src.From,
                To = src.To,
                Figures = src.Figures,
			};
		}
		public static tanks.Models.Thermometer CreateFromThermometer(tanks.Models.Thermometer src)
		{
			return new tanks.Models.Thermometer{
                Tl = src.Tl,
                Th = src.Th,
                unit = src.unit,
                T = src.T,
                X = src.X,
                Y = src.Y,
                Id = src.Id,
			};
		}
	}
}