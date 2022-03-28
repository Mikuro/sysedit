using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using tanks.ViewModels;

namespace tanks
{
	public static class XMLUtil
	{
        public static XElement double_array_to_node(Double[] arr, XNamespace[] nss)
        {
            XElement elem = new XElement(nss[1] + "Array");

            elem.Add(new XAttribute("Type", "sys:Double"));
            for(int i=0; i<arr.Length; i++)
            {
                XElement internalElem = new XElement(nss[2] + "Double");
                internalElem.Add(new XText(arr[i].ToString(CultureInfo.InvariantCulture)));
                elem.Add(internalElem);
            }

            return elem;
        }

        public static IReadOnlyList<XElement> Collection_double_array_to_node(Collection<Double[]> arr, XNamespace[] nss)
        {
            var list = new List<XElement>();

            for (int i = 0; i < arr.Count; i++)
                list.Add(double_array_to_node(arr[i],nss));

            return list;
        }

        public static IReadOnlyList<XElement> Collection_double_to_node(Collection<double> arr, XNamespace[] nss)
        {
            if (arr == null) return null;
            var list = new List<XElement>();

            for (int i = 0; i < arr.Count; i++)
            {
                XElement internalElem = new XElement(nss[2] + "Double");
                internalElem.Add(new XText(arr[i].ToString(CultureInfo.InvariantCulture)));
                list.Add(internalElem);
            }

            return list;
        }

        public static IReadOnlyList<XElement> Collection_Component_to_node(Collection<tanks.Models.Component> arr, XNamespace[] nss)
        {
            var list = new List<XElement>();

            for(int i=0; i<arr.Count; i++)
                list.Add(Component_to_node(arr[i], nss));

            return list;
        }


        public static IReadOnlyList<XElement> Collection_CalculationSystem_to_node(Collection<tanks.Models.CalculationSystem> arr, XNamespace[] nss)
        {
            var list = new List<XElement>();

            for(int i=0; i<arr.Count; i++)
                list.Add(CalculationSystem_to_node(arr[i],nss));

            return list;
        }

        public static IReadOnlyList<XElement> Collection_ModelLink_to_node(Collection<tanks.Models.ModelLink> arr, XNamespace[] nss)
        {
            var list = new List<XElement>();

            for(int i=0; i<arr.Count; i++)
            {
                XElement link;

                switch(arr[i].GetType().Name)
                {
                case "Signal":
                    link = Signal_to_node(arr[i] as ViewModels.Signal,nss);
                    break;
                case "Stream":
                    link = Stream_to_node(arr[i] as ViewModels.Stream,nss);
                    break;
                default:
                        throw new Exception("fail");
                }
                list.Add(link);
            }

            return list;
        }

        public static IReadOnlyList<XElement> Collection_ModelObject_to_node(Collection<tanks.Models.ModelObject> arr, XNamespace[] nss)
        {
            var list = new List<XElement>();

            for(int i=0; i<arr.Count; i++)
            {
                XElement obj;

                switch(arr[i].GetType().Name)
                {
                case "ControlValve":
                    obj = ControlValve_to_node(arr[i] as ControlValve,nss);
                    break;
                case "FlowMeter":
                    obj = FlowMeter_to_node(arr[i] as FlowMeter,nss);
                    break;
                case "HeatExchanger":
                    obj = HeatExchanger_to_node(arr[i] as HeatExchanger,nss);
                    break;
                case "LiquidLevelMeter":
                    obj = LiquidLevelMeter_to_node(arr[i] as LiquidLevelMeter,nss);
                    break;
                case "LiquidTank":
                    obj = LiquidTank_to_node(arr[i] as LiquidTank,nss);
                    break;
                case "Mixer":
                    obj = Mixer_to_node(arr[i] as Mixer,nss);
                    break;
                case "PIDController":
                    obj = PIDController_to_node(arr[i] as PIDController,nss);
                    break;
                case "Pipe":
                    obj = Pipe_to_node(arr[i] as Pipe,nss);
                    break;
                case "PressureFeed":
                    obj = PressureFeed_to_node(arr[i] as PressureFeed,nss);
                    break;
                case "PressureGauge":
                    obj = PressureGauge_to_node(arr[i] as PressureGauge,nss);
                    break;
                case "PressureProduct":
                    obj = PressureProduct_to_node(arr[i] as PressureProduct,nss);
                    break;
                case "Pump":
                    obj = Pump_to_node(arr[i] as Pump,nss);
                    break;
                case "Splitter":
                    obj = Splitter_to_node(arr[i] as Splitter,nss);
                    break;
                case "Strainer":
                    obj = Strainer_to_node(arr[i] as Strainer,nss);
                    break;
                case "Thermometer":
                    obj = Thermometer_to_node(arr[i] as Thermometer,nss);
                    break;
                default:
                        throw new Exception("fail");
                }
                list.Add(obj);
            }

            return list;
        }

		public static XElement CalculationSystem_to_node(tanks.Models.CalculationSystem obj, XNamespace[] nss)
		{
			return new XElement(nss[0] + "CalculationSystem"
                ,new XElement(nss[0]+"CalculationSystem.SRKKIJ", Collection_double_array_to_node(obj.SRKKIJ,nss))
			);
		}
		public static XElement Component_to_node(tanks.Models.Component obj, XNamespace[] nss)
		{
			return new XElement(nss[0] + "Component"
                ,new XAttribute("Id", obj.Id)
                ,new XAttribute("Mw", obj.Mw.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("Tc", obj.Tc.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("Pc", obj.Pc.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("Zc", obj.Zc.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("Omega", obj.Omega.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("Higa", obj.Higa.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("Higb", obj.Higb.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("Higc", obj.Higc.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("Higd", obj.Higd.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("Hige", obj.Hige.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("Higf", obj.Higf.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("ZRA", obj.ZRA.ToString(CultureInfo.InvariantCulture))
			);
		}
		public static XElement ControlValve_to_node(tanks.ViewModels.ControlValve obj, XNamespace[] nss)
		{
			return new XElement(nss[0] + "ControlValve"
                ,new XAttribute("CV", obj.CV.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("R", obj.R.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("pos0", obj.pos0.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("topen", obj.topen.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("tclose", obj.tclose.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("tlag", obj.tlag.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("Cff", obj.Cff.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("mv", obj.mv.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("pos", obj.pos.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("velosity", obj.velosity.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("X", obj.X.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("Y", obj.Y.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("Id", obj.Id)
			);
		}
		public static XElement FlowDiagram_to_node(tanks.ViewModels.FlowDiagram obj, XNamespace[] nss)
		{
			return new XElement(nss[0] + "FlowDiagram"
                ,new XAttribute("xmlns", nss[0])
                ,new XAttribute(XNamespace.Xmlns + "x", nss[1])
                ,new XAttribute(XNamespace.Xmlns + "sys", nss[2])
                ,new XElement(nss[0]+"FlowDiagram.Components", Collection_Component_to_node(obj.Components,nss))
                ,new XElement(nss[0]+"FlowDiagram.Items", Collection_ModelObject_to_node(obj.Items,nss))
                ,new XElement(nss[0]+"FlowDiagram.Links", Collection_ModelLink_to_node(obj.Links,nss))
                ,new XElement(nss[0]+"FlowDiagram.Systems", Collection_CalculationSystem_to_node(obj.Systems,nss))
                ,new XAttribute("Width", obj.Width.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("Height", obj.Height.ToString(CultureInfo.InvariantCulture))
			);
		}
		public static XElement FlowMeter_to_node(tanks.ViewModels.FlowMeter obj, XNamespace[] nss)
		{
			return new XElement(nss[0] + "FlowMeter"
                ,new XAttribute("Fmax", obj.Fmax.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("Fmin", obj.Fmin.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("unit", obj.unit.ToString())
                ,new XAttribute("t_unit", obj.t_unit.ToString())
                ,new XAttribute("F", obj.F.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("X", obj.X.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("Y", obj.Y.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("Id", obj.Id)
			);
		}
		public static XElement HeatExchanger_to_node(tanks.ViewModels.HeatExchanger obj, XNamespace[] nss)
		{
			return new XElement(nss[0] + "HeatExchanger"
                ,new XAttribute("WHdes", obj.WHdes.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("DHdes", obj.DHdes.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("PdHdes", obj.PdHdes.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("WLdes", obj.WLdes.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("DLdes", obj.DLdes.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("PdLdes", obj.PdLdes.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("A", obj.A.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("U", obj.U.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("X", obj.X.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("Y", obj.Y.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("Id", obj.Id)
			);
		}
		public static XElement LiquidLevelMeter_to_node(tanks.ViewModels.LiquidLevelMeter obj, XNamespace[] nss)
		{
			return new XElement(nss[0] + "LiquidLevelMeter"
                ,new XAttribute("Ll", obj.Ll.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("Lh", obj.Lh.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("Lbase", obj.Lbase.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("unit", obj.unit.ToString())
                ,new XAttribute("L", obj.L.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("X", obj.X.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("Y", obj.Y.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("Id", obj.Id)
			);
		}
		public static XElement LiquidTank_to_node(tanks.ViewModels.LiquidTank obj, XNamespace[] nss)
		{
			return new XElement(nss[0] + "LiquidTank"
                ,new XAttribute("T", obj.T.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("A", obj.A.ToString(CultureInfo.InvariantCulture))
                ,new XElement(nss[0]+"LiquidTank.u", Collection_double_to_node(obj.u,nss))
                ,new XAttribute("X", obj.X.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("Y", obj.Y.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("Id", obj.Id)
			);
		}
		public static XElement Mixer_to_node(tanks.ViewModels.Mixer obj, XNamespace[] nss)
		{
			return new XElement(nss[0] + "Mixer"
                ,new XAttribute("X", obj.X.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("Y", obj.Y.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("Id", obj.Id)
			);
		}
		public static XElement PIDController_to_node(tanks.ViewModels.PIDController obj, XNamespace[] nss)
		{
			return new XElement(nss[0] + "PIDController"
                ,new XAttribute("PB", obj.PB.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("TI", obj.TI.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("TD", obj.TD.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("PVSPAN", obj.PVSPAN.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("PVBASE", obj.PVBASE.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("INCDEC", obj.INCDEC.ToString())
                ,new XAttribute("MVSPAN", obj.MVSPAN.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("MVH", obj.MVH.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("MVL", obj.MVL.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("SV", obj.SV.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("PV", obj.PV.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("MV", obj.MV.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("CMOD", obj.CMOD.ToString())
                ,new XAttribute("SVM", obj.SVM.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("MVM", obj.MVM.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("e", obj.e.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("E", obj.E.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("X", obj.X.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("Y", obj.Y.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("Id", obj.Id)
			);
		}
		public static XElement Pipe_to_node(tanks.ViewModels.Pipe obj, XNamespace[] nss)
		{
			return new XElement(nss[0] + "Pipe"
                ,new XAttribute("Hdiff", obj.Hdiff.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("X", obj.X.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("Y", obj.Y.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("Id", obj.Id)
			);
		}
		public static XElement PressureFeed_to_node(tanks.ViewModels.PressureFeed obj, XNamespace[] nss)
		{
			return new XElement(nss[0] + "PressureFeed"
                ,new XElement(nss[0]+"PressureFeed.x", Collection_double_to_node(obj.x,nss))
                ,new XAttribute("P", obj.P.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("T", obj.T.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("X", obj.X.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("Y", obj.Y.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("Id", obj.Id)
			);
		}
		public static XElement PressureGauge_to_node(tanks.ViewModels.PressureGauge obj, XNamespace[] nss)
		{
			return new XElement(nss[0] + "PressureGauge"
                ,new XAttribute("Pl", obj.Pl.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("Ph", obj.Ph.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("unit", obj.unit.ToString())
                ,new XAttribute("P", obj.P.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("X", obj.X.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("Y", obj.Y.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("Id", obj.Id)
			);
		}
		public static XElement PressureProduct_to_node(tanks.ViewModels.PressureProduct obj, XNamespace[] nss)
		{
			return new XElement(nss[0] + "PressureProduct"
                ,new XAttribute("P", obj.P.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("X", obj.X.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("Y", obj.Y.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("Id", obj.Id)
			);
		}
		public static XElement Pump_to_node(tanks.ViewModels.Pump obj, XNamespace[] nss)
		{
			return new XElement(nss[0] + "Pump"
                ,new XAttribute("Hd", obj.Hd.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("Hs", obj.Hs.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("Vd", obj.Vd.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("dd", obj.dd.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("Wd", obj.Wd.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("Ws", obj.Ws.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("pos0", obj.pos0.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("tlag", obj.tlag.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("mv", obj.mv.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("pos", obj.pos.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("X", obj.X.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("Y", obj.Y.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("Id", obj.Id)
			);
		}
		public static XElement Signal_to_node(tanks.ViewModels.Signal obj, XNamespace[] nss)
		{
			return new XElement(nss[0] + "Signal"
                ,new XAttribute("Id", obj.Id)
                ,new XAttribute("From", obj.From)
                ,new XAttribute("To", obj.To)
                ,new XAttribute("Figures", obj.Figures)
			);
		}
		public static XElement Splitter_to_node(tanks.ViewModels.Splitter obj, XNamespace[] nss)
		{
			return new XElement(nss[0] + "Splitter"
                ,new XAttribute("X", obj.X.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("Y", obj.Y.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("Id", obj.Id)
			);
		}
		public static XElement Strainer_to_node(tanks.ViewModels.Strainer obj, XNamespace[] nss)
		{
			return new XElement(nss[0] + "Strainer"
                ,new XAttribute("Wd", obj.Wd.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("DP", obj.DP.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("dd", obj.dd.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("X", obj.X.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("Y", obj.Y.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("Id", obj.Id)
			);
		}
		public static XElement Stream_to_node(tanks.ViewModels.Stream obj, XNamespace[] nss)
		{
			return new XElement(nss[0] + "Stream"
                ,new XAttribute("Type", obj.Type.ToString())
                ,new XElement(nss[0]+"Stream.x", Collection_double_to_node(obj.x,nss))
                ,new XAttribute("F", obj.F.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("P", obj.P.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("T", obj.T.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("RL", obj.RL.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("HL", obj.HL.ToString(CultureInfo.InvariantCulture))
                ,new XElement(nss[0]+"Stream.XL", Collection_double_to_node(obj.XL,nss))
                ,new XAttribute("RV", obj.RV.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("HV", obj.HV.ToString(CultureInfo.InvariantCulture))
                ,new XElement(nss[0]+"Stream.XV", Collection_double_to_node(obj.XV,nss))
                ,new XAttribute("Mw", obj.Mw.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("V", obj.V.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("Id", obj.Id)
                ,new XAttribute("From", obj.From)
                ,new XAttribute("To", obj.To)
                ,new XAttribute("Figures", obj.Figures)
			);
		}
		public static XElement Thermometer_to_node(tanks.ViewModels.Thermometer obj, XNamespace[] nss)
		{
			return new XElement(nss[0] + "Thermometer"
                ,new XAttribute("Tl", obj.Tl.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("Th", obj.Th.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("unit", obj.unit.ToString())
                ,new XAttribute("T", obj.T.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("X", obj.X.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("Y", obj.Y.ToString(CultureInfo.InvariantCulture))
                ,new XAttribute("Id", obj.Id)
			);
		}
// ---------------------------------
        public static Double[] CreateFrom_double_array_node(XmlNode node)
        {
            var dst = new Double[node.ChildNodes.Count];

            for (int i = 0; i < dst.Length; i++)
                dst[i] = Double.Parse(XValue(node.ChildNodes[i]), CultureInfo.InvariantCulture);

            return dst;
        }

        public static Collection<Double[]> CreateFromCollection_double_array_node(XmlNode node)
        {
            var dst = new Collection<Double[]>();

            for(int i=0; i<node.ChildNodes.Count; i++)
                dst.Add(CreateFrom_double_array_node(node.ChildNodes[i]));

            return dst;
        }

        public static string XValue(XmlNode node)
        {
            string retstring;

            if(node.Value==null)
            {
                if(node.ChildNodes.Count!=1) throw new Exception("fail");
                retstring=node.ChildNodes[0].Value;
            }else
                retstring=node.Value;

            return retstring;
        }

        public static Collection<double> CreateFromCollection_double_node(XmlNode node)
        {
            if (node == null) return null;
            var dst = new Collection<double>();

            for(int i=0; i<node.ChildNodes.Count; i++)
                dst.Add(Double.Parse(XValue(node.ChildNodes[i]), CultureInfo.InvariantCulture));

            return dst;
        }

        public static Collection<tanks.Models.Component> CreateFromCollection_Component_node(XmlNode node)
        {
            var dst = new Collection<tanks.Models.Component>();

            for(int i=0; i<node.ChildNodes.Count; i++)
                dst.Add(CreateFromComponent_node(node.ChildNodes[i]));

            return dst;
        }

        public static Collection<tanks.Models.CalculationSystem> CreateFromCollection_CalculationSystem_node(XmlNode node)
        {
            var dst = new Collection<tanks.Models.CalculationSystem>();

            for(int i=0; i<node.ChildNodes.Count; i++)
                dst.Add(CreateFromCalculationSystem_node(node.ChildNodes[i]));

            return dst;
        }

        public static Collection<tanks.Models.ModelLink> CreateFromCollection_ModelLink_node(XmlNode node)
        {
            var dst = new Collection<tanks.Models.ModelLink>();

            for(int i=0; i<node.ChildNodes.Count; i++)
            {
                tanks.Models.ModelLink link;

                switch(node.ChildNodes[i].Name)
                {
                case "Signal":
                    link = CreateFromSignal_node(node.ChildNodes[i]);
                    break;
                case "Stream":
                    link = CreateFromStream_node(node.ChildNodes[i]);
                    break;
                default:
                        throw new Exception("fail");
                }
                dst.Add(link);
            }

            return dst;
        }

        public static Collection<tanks.Models.ModelObject> CreateFromCollection_ModelObject_node(XmlNode node)
        {
            var dst = new Collection<tanks.Models.ModelObject>();

            for(int i=0; i<node.ChildNodes.Count; i++)
            {
                tanks.Models.ModelObject obj;

                switch(node.ChildNodes[i].Name)
                {
                case "ControlValve":
                    obj = CreateFromControlValve_node(node.ChildNodes[i]);
                    break;
                case "FlowMeter":
                    obj = CreateFromFlowMeter_node(node.ChildNodes[i]);
                    break;
                case "HeatExchanger":
                    obj = CreateFromHeatExchanger_node(node.ChildNodes[i]);
                    break;
                case "LiquidLevelMeter":
                    obj = CreateFromLiquidLevelMeter_node(node.ChildNodes[i]);
                    break;
                case "LiquidTank":
                    obj = CreateFromLiquidTank_node(node.ChildNodes[i]);
                    break;
                case "Mixer":
                    obj = CreateFromMixer_node(node.ChildNodes[i]);
                    break;
                case "PIDController":
                    obj = CreateFromPIDController_node(node.ChildNodes[i]);
                    break;
                case "Pipe":
                    obj = CreateFromPipe_node(node.ChildNodes[i]);
                    break;
                case "PressureFeed":
                    obj = CreateFromPressureFeed_node(node.ChildNodes[i]);
                    break;
                case "PressureGauge":
                    obj = CreateFromPressureGauge_node(node.ChildNodes[i]);
                    break;
                case "PressureProduct":
                    obj = CreateFromPressureProduct_node(node.ChildNodes[i]);
                    break;
                case "Pump":
                    obj = CreateFromPump_node(node.ChildNodes[i]);
                    break;
                case "Splitter":
                    obj = CreateFromSplitter_node(node.ChildNodes[i]);
                    break;
                case "Strainer":
                    obj = CreateFromStrainer_node(node.ChildNodes[i]);
                    break;
                case "Thermometer":
                    obj = CreateFromThermometer_node(node.ChildNodes[i]);
                    break;
                default:
                        throw new Exception("fail");
                }
                dst.Add(obj);
            }

            return dst;
        }

        public static XmlNode XAttr(XmlNode node,string name)
        {
            XmlNode retnode = null;
            for(int i=0; i<node.ChildNodes.Count; i++)
            {
                var split = node.ChildNodes[i].Name.Split('.');
                if(split.Length!=2)continue;
                if(split[1] == name)
                {
                    retnode=node.ChildNodes[i];
                    break;
                }
            }
            return retnode;
        }

        public static string NAttr(XmlNode node,string name,Type type)
        {
            string retstring = "";
            for(int i=0; i<node.Attributes.Count; i++)
            {
                if(node.Attributes[i].Name == name)
                {
                    retstring = node.Attributes[i].Value;
                    break;
                }
            }
            if (retstring == "")
                retstring = Activator.CreateInstance(type).ToString();
            return retstring;
        }
        
		public static tanks.ViewModels.CalculationSystem CreateFromCalculationSystem_node(XmlNode node)
		{
			return new tanks.ViewModels.CalculationSystem{
                SRKKIJ = CreateFromCollection_double_array_node(XAttr(node,"SRKKIJ")),
			};
		}
		public static tanks.ViewModels.Component CreateFromComponent_node(XmlNode node)
		{
			return new tanks.ViewModels.Component{
                Id = NAttr(node,"Id",typeof(System.String)),
                Mw = Double.Parse(NAttr(node,"Mw",typeof(System.Double)), CultureInfo.InvariantCulture),
                Tc = Double.Parse(NAttr(node,"Tc",typeof(System.Double)), CultureInfo.InvariantCulture),
                Pc = Double.Parse(NAttr(node,"Pc",typeof(System.Double)), CultureInfo.InvariantCulture),
                Zc = Double.Parse(NAttr(node,"Zc",typeof(System.Double)), CultureInfo.InvariantCulture),
                Omega = Double.Parse(NAttr(node,"Omega",typeof(System.Double)), CultureInfo.InvariantCulture),
                Higa = Double.Parse(NAttr(node,"Higa",typeof(System.Double)), CultureInfo.InvariantCulture),
                Higb = Double.Parse(NAttr(node,"Higb",typeof(System.Double)), CultureInfo.InvariantCulture),
                Higc = Double.Parse(NAttr(node,"Higc",typeof(System.Double)), CultureInfo.InvariantCulture),
                Higd = Double.Parse(NAttr(node,"Higd",typeof(System.Double)), CultureInfo.InvariantCulture),
                Hige = Double.Parse(NAttr(node,"Hige",typeof(System.Double)), CultureInfo.InvariantCulture),
                Higf = Double.Parse(NAttr(node,"Higf",typeof(System.Double)), CultureInfo.InvariantCulture),
                ZRA = Double.Parse(NAttr(node,"ZRA",typeof(System.Double)), CultureInfo.InvariantCulture),
			};
		}
		public static tanks.ViewModels.ControlValve CreateFromControlValve_node(XmlNode node)
		{
			return new tanks.ViewModels.ControlValve{
                CV = Double.Parse(NAttr(node,"CV",typeof(System.Double)), CultureInfo.InvariantCulture),
                R = Double.Parse(NAttr(node,"R",typeof(System.Double)), CultureInfo.InvariantCulture),
                pos0 = Double.Parse(NAttr(node,"pos0",typeof(System.Double)), CultureInfo.InvariantCulture),
                topen = Double.Parse(NAttr(node,"topen",typeof(System.Double)), CultureInfo.InvariantCulture),
                tclose = Double.Parse(NAttr(node,"tclose",typeof(System.Double)), CultureInfo.InvariantCulture),
                tlag = Double.Parse(NAttr(node,"tlag",typeof(System.Double)), CultureInfo.InvariantCulture),
                Cff = Double.Parse(NAttr(node,"Cff",typeof(System.Double)), CultureInfo.InvariantCulture),
                mv = Double.Parse(NAttr(node,"mv",typeof(System.Double)), CultureInfo.InvariantCulture),
                pos = Double.Parse(NAttr(node,"pos",typeof(System.Double)), CultureInfo.InvariantCulture),
                velosity = Double.Parse(NAttr(node,"velosity",typeof(System.Double)), CultureInfo.InvariantCulture),
                X = Double.Parse(NAttr(node,"X",typeof(System.Double)), CultureInfo.InvariantCulture),
                Y = Double.Parse(NAttr(node,"Y",typeof(System.Double)), CultureInfo.InvariantCulture),
                Id = NAttr(node,"Id",typeof(System.String)),
			};
		}
		public static tanks.ViewModels.FlowDiagram CreateFromFlowDiagram_node(XmlNode node)
		{
			return new tanks.ViewModels.FlowDiagram{
                Components = CreateFromCollection_Component_node(XAttr(node,"Components")),
                Items = CreateFromCollection_ModelObject_node(XAttr(node,"Items")),
                Links = CreateFromCollection_ModelLink_node(XAttr(node,"Links")),
                Systems = CreateFromCollection_CalculationSystem_node(XAttr(node,"Systems")),
                Width = Double.Parse(NAttr(node,"Width",typeof(System.Double)), CultureInfo.InvariantCulture),
                Height = Double.Parse(NAttr(node,"Height",typeof(System.Double)), CultureInfo.InvariantCulture),
			};
		}
		public static tanks.ViewModels.FlowMeter CreateFromFlowMeter_node(XmlNode node)
		{
			return new tanks.ViewModels.FlowMeter{
                Fmax = Double.Parse(NAttr(node,"Fmax",typeof(System.Double)), CultureInfo.InvariantCulture),
                Fmin = Double.Parse(NAttr(node,"Fmin",typeof(System.Double)), CultureInfo.InvariantCulture),
                unit = (tanks.Models.EUMassOrVolume)Enum.Parse(typeof(tanks.Models.EUMassOrVolume),
                NAttr(node,"unit",typeof(tanks.Models.EUMassOrVolume))),
                t_unit = (tanks.Models.EUTime)Enum.Parse(typeof(tanks.Models.EUTime),
                NAttr(node,"t_unit",typeof(tanks.Models.EUTime))),
                F = Double.Parse(NAttr(node,"F",typeof(System.Double)), CultureInfo.InvariantCulture),
                X = Double.Parse(NAttr(node,"X",typeof(System.Double)), CultureInfo.InvariantCulture),
                Y = Double.Parse(NAttr(node,"Y",typeof(System.Double)), CultureInfo.InvariantCulture),
                Id = NAttr(node,"Id",typeof(System.String)),
			};
		}
		public static tanks.ViewModels.HeatExchanger CreateFromHeatExchanger_node(XmlNode node)
		{
			return new tanks.ViewModels.HeatExchanger{
                WHdes = Double.Parse(NAttr(node,"WHdes",typeof(System.Double)), CultureInfo.InvariantCulture),
                DHdes = Double.Parse(NAttr(node,"DHdes",typeof(System.Double)), CultureInfo.InvariantCulture),
                PdHdes = Double.Parse(NAttr(node,"PdHdes",typeof(System.Double)), CultureInfo.InvariantCulture),
                WLdes = Double.Parse(NAttr(node,"WLdes",typeof(System.Double)), CultureInfo.InvariantCulture),
                DLdes = Double.Parse(NAttr(node,"DLdes",typeof(System.Double)), CultureInfo.InvariantCulture),
                PdLdes = Double.Parse(NAttr(node,"PdLdes",typeof(System.Double)), CultureInfo.InvariantCulture),
                A = Double.Parse(NAttr(node,"A",typeof(System.Double)), CultureInfo.InvariantCulture),
                U = Double.Parse(NAttr(node,"U",typeof(System.Double)), CultureInfo.InvariantCulture),
                X = Double.Parse(NAttr(node,"X",typeof(System.Double)), CultureInfo.InvariantCulture),
                Y = Double.Parse(NAttr(node,"Y",typeof(System.Double)), CultureInfo.InvariantCulture),
                Id = NAttr(node,"Id",typeof(System.String)),
			};
		}
		public static tanks.ViewModels.LiquidLevelMeter CreateFromLiquidLevelMeter_node(XmlNode node)
		{
			return new tanks.ViewModels.LiquidLevelMeter{
                Ll = Double.Parse(NAttr(node,"Ll",typeof(System.Double)), CultureInfo.InvariantCulture),
                Lh = Double.Parse(NAttr(node,"Lh",typeof(System.Double)), CultureInfo.InvariantCulture),
                Lbase = Double.Parse(NAttr(node,"Lbase",typeof(System.Double)), CultureInfo.InvariantCulture),
                unit = (tanks.Models.EULevel)Enum.Parse(typeof(tanks.Models.EULevel),
                NAttr(node,"unit",typeof(tanks.Models.EULevel))),
                L = Double.Parse(NAttr(node,"L",typeof(System.Double)), CultureInfo.InvariantCulture),
                X = Double.Parse(NAttr(node,"X",typeof(System.Double)), CultureInfo.InvariantCulture),
                Y = Double.Parse(NAttr(node,"Y",typeof(System.Double)), CultureInfo.InvariantCulture),
                Id = NAttr(node,"Id",typeof(System.String)),
			};
		}
		public static tanks.ViewModels.LiquidTank CreateFromLiquidTank_node(XmlNode node)
		{
			return new tanks.ViewModels.LiquidTank{
                T = Double.Parse(NAttr(node,"T",typeof(System.Double)), CultureInfo.InvariantCulture),
                A = Double.Parse(NAttr(node,"A",typeof(System.Double)), CultureInfo.InvariantCulture),
                u = CreateFromCollection_double_node(XAttr(node,"u")),
                X = Double.Parse(NAttr(node,"X",typeof(System.Double)), CultureInfo.InvariantCulture),
                Y = Double.Parse(NAttr(node,"Y",typeof(System.Double)), CultureInfo.InvariantCulture),
                Id = NAttr(node,"Id",typeof(System.String)),
			};
		}
		public static tanks.ViewModels.Mixer CreateFromMixer_node(XmlNode node)
		{
			return new tanks.ViewModels.Mixer{
                X = Double.Parse(NAttr(node,"X",typeof(System.Double)), CultureInfo.InvariantCulture),
                Y = Double.Parse(NAttr(node,"Y",typeof(System.Double)), CultureInfo.InvariantCulture),
                Id = NAttr(node,"Id",typeof(System.String)),
			};
		}
		public static tanks.ViewModels.PIDController CreateFromPIDController_node(XmlNode node)
		{
			return new tanks.ViewModels.PIDController{
                PB = Double.Parse(NAttr(node,"PB",typeof(System.Double)), CultureInfo.InvariantCulture),
                TI = Double.Parse(NAttr(node,"TI",typeof(System.Double)), CultureInfo.InvariantCulture),
                TD = Double.Parse(NAttr(node,"TD",typeof(System.Double)), CultureInfo.InvariantCulture),
                PVSPAN = Double.Parse(NAttr(node,"PVSPAN",typeof(System.Double)), CultureInfo.InvariantCulture),
                PVBASE = Double.Parse(NAttr(node,"PVBASE",typeof(System.Double)), CultureInfo.InvariantCulture),
                INCDEC = (tanks.Models.PIDDirection)Enum.Parse(typeof(tanks.Models.PIDDirection),
                NAttr(node,"INCDEC",typeof(tanks.Models.PIDDirection))),
                MVSPAN = Double.Parse(NAttr(node,"MVSPAN",typeof(System.Double)), CultureInfo.InvariantCulture),
                MVH = Double.Parse(NAttr(node,"MVH",typeof(System.Double)), CultureInfo.InvariantCulture),
                MVL = Double.Parse(NAttr(node,"MVL",typeof(System.Double)), CultureInfo.InvariantCulture),
                SV = Double.Parse(NAttr(node,"SV",typeof(System.Double)), CultureInfo.InvariantCulture),
                PV = Double.Parse(NAttr(node,"PV",typeof(System.Double)), CultureInfo.InvariantCulture),
                MV = Double.Parse(NAttr(node,"MV",typeof(System.Double)), CultureInfo.InvariantCulture),
                CMOD = (tanks.Models.PIDMode)Enum.Parse(typeof(tanks.Models.PIDMode),
                NAttr(node,"CMOD",typeof(tanks.Models.PIDMode))),
                SVM = Double.Parse(NAttr(node,"SVM",typeof(System.Double)), CultureInfo.InvariantCulture),
                MVM = Double.Parse(NAttr(node,"MVM",typeof(System.Double)), CultureInfo.InvariantCulture),
                e = Double.Parse(NAttr(node,"e",typeof(System.Double)), CultureInfo.InvariantCulture),
                E = Double.Parse(NAttr(node,"E",typeof(System.Double)), CultureInfo.InvariantCulture),
                X = Double.Parse(NAttr(node,"X",typeof(System.Double)), CultureInfo.InvariantCulture),
                Y = Double.Parse(NAttr(node,"Y",typeof(System.Double)), CultureInfo.InvariantCulture),
                Id = NAttr(node,"Id",typeof(System.String)),
			};
		}
		public static tanks.ViewModels.Pipe CreateFromPipe_node(XmlNode node)
		{
			return new tanks.ViewModels.Pipe{
                Hdiff = Double.Parse(NAttr(node,"Hdiff",typeof(System.Double)), CultureInfo.InvariantCulture),
                X = Double.Parse(NAttr(node,"X",typeof(System.Double)), CultureInfo.InvariantCulture),
                Y = Double.Parse(NAttr(node,"Y",typeof(System.Double)), CultureInfo.InvariantCulture),
                Id = NAttr(node,"Id",typeof(System.String)),
			};
		}
		public static tanks.ViewModels.PressureFeed CreateFromPressureFeed_node(XmlNode node)
		{
			return new tanks.ViewModels.PressureFeed{
                x = CreateFromCollection_double_node(XAttr(node,"x")),
                P = Double.Parse(NAttr(node,"P",typeof(System.Double)), CultureInfo.InvariantCulture),
                T = Double.Parse(NAttr(node,"T",typeof(System.Double)), CultureInfo.InvariantCulture),
                X = Double.Parse(NAttr(node,"X",typeof(System.Double)), CultureInfo.InvariantCulture),
                Y = Double.Parse(NAttr(node,"Y",typeof(System.Double)), CultureInfo.InvariantCulture),
                Id = NAttr(node,"Id",typeof(System.String)),
			};
		}
		public static tanks.ViewModels.PressureGauge CreateFromPressureGauge_node(XmlNode node)
		{
			return new tanks.ViewModels.PressureGauge{
                Pl = Double.Parse(NAttr(node,"Pl",typeof(System.Double)), CultureInfo.InvariantCulture),
                Ph = Double.Parse(NAttr(node,"Ph",typeof(System.Double)), CultureInfo.InvariantCulture),
                unit = (tanks.Models.EUPressure)Enum.Parse(typeof(tanks.Models.EUPressure),
                NAttr(node,"unit",typeof(tanks.Models.EUPressure))),
                P = Double.Parse(NAttr(node,"P",typeof(System.Double)), CultureInfo.InvariantCulture),
                X = Double.Parse(NAttr(node,"X",typeof(System.Double)), CultureInfo.InvariantCulture),
                Y = Double.Parse(NAttr(node,"Y",typeof(System.Double)), CultureInfo.InvariantCulture),
                Id = NAttr(node,"Id",typeof(System.String)),
			};
		}
		public static tanks.ViewModels.PressureProduct CreateFromPressureProduct_node(XmlNode node)
		{
			return new tanks.ViewModels.PressureProduct{
                P = Double.Parse(NAttr(node,"P",typeof(System.Double)), CultureInfo.InvariantCulture),
                X = Double.Parse(NAttr(node,"X",typeof(System.Double)), CultureInfo.InvariantCulture),
                Y = Double.Parse(NAttr(node,"Y",typeof(System.Double)), CultureInfo.InvariantCulture),
                Id = NAttr(node,"Id",typeof(System.String)),
			};
		}
		public static tanks.ViewModels.Pump CreateFromPump_node(XmlNode node)
		{
			return new tanks.ViewModels.Pump{
                Hd = Double.Parse(NAttr(node,"Hd",typeof(System.Double)), CultureInfo.InvariantCulture),
                Hs = Double.Parse(NAttr(node,"Hs",typeof(System.Double)), CultureInfo.InvariantCulture),
                Vd = Double.Parse(NAttr(node,"Vd",typeof(System.Double)), CultureInfo.InvariantCulture),
                dd = Double.Parse(NAttr(node,"dd",typeof(System.Double)), CultureInfo.InvariantCulture),
                Wd = Double.Parse(NAttr(node,"Wd",typeof(System.Double)), CultureInfo.InvariantCulture),
                Ws = Double.Parse(NAttr(node,"Ws",typeof(System.Double)), CultureInfo.InvariantCulture),
                pos0 = Double.Parse(NAttr(node,"pos0",typeof(System.Double)), CultureInfo.InvariantCulture),
                tlag = Double.Parse(NAttr(node,"tlag",typeof(System.Double)), CultureInfo.InvariantCulture),
                mv = Double.Parse(NAttr(node,"mv",typeof(System.Double)), CultureInfo.InvariantCulture),
                pos = Double.Parse(NAttr(node,"pos",typeof(System.Double)), CultureInfo.InvariantCulture),
                X = Double.Parse(NAttr(node,"X",typeof(System.Double)), CultureInfo.InvariantCulture),
                Y = Double.Parse(NAttr(node,"Y",typeof(System.Double)), CultureInfo.InvariantCulture),
                Id = NAttr(node,"Id",typeof(System.String)),
			};
		}
		public static tanks.ViewModels.Signal CreateFromSignal_node(XmlNode node)
		{
			return new tanks.ViewModels.Signal{
                Id = NAttr(node,"Id",typeof(System.String)),
                From = NAttr(node,"From",typeof(System.String)),
                To = NAttr(node,"To",typeof(System.String)),
                Figures = NAttr(node,"Figures",typeof(System.String)),
			};
		}
		public static tanks.ViewModels.Splitter CreateFromSplitter_node(XmlNode node)
		{
			return new tanks.ViewModels.Splitter{
                X = Double.Parse(NAttr(node,"X",typeof(System.Double)), CultureInfo.InvariantCulture),
                Y = Double.Parse(NAttr(node,"Y",typeof(System.Double)), CultureInfo.InvariantCulture),
                Id = NAttr(node,"Id",typeof(System.String)),
			};
		}
		public static tanks.ViewModels.Strainer CreateFromStrainer_node(XmlNode node)
		{
			return new tanks.ViewModels.Strainer{
                Wd = Double.Parse(NAttr(node,"Wd",typeof(System.Double)), CultureInfo.InvariantCulture),
                DP = Double.Parse(NAttr(node,"DP",typeof(System.Double)), CultureInfo.InvariantCulture),
                dd = Double.Parse(NAttr(node,"dd",typeof(System.Double)), CultureInfo.InvariantCulture),
                X = Double.Parse(NAttr(node,"X",typeof(System.Double)), CultureInfo.InvariantCulture),
                Y = Double.Parse(NAttr(node,"Y",typeof(System.Double)), CultureInfo.InvariantCulture),
                Id = NAttr(node,"Id",typeof(System.String)),
			};
		}
		public static tanks.ViewModels.Stream CreateFromStream_node(XmlNode node)
		{
			return new tanks.ViewModels.Stream{
                Type = (tanks.Models.PipeType)Enum.Parse(typeof(tanks.Models.PipeType),
                NAttr(node,"Type",typeof(tanks.Models.PipeType))),
                x = CreateFromCollection_double_node(XAttr(node,"x")),
                F = Double.Parse(NAttr(node,"F",typeof(System.Double)), CultureInfo.InvariantCulture),
                P = Double.Parse(NAttr(node,"P",typeof(System.Double)), CultureInfo.InvariantCulture),
                T = Double.Parse(NAttr(node,"T",typeof(System.Double)), CultureInfo.InvariantCulture),
                RL = Double.Parse(NAttr(node,"RL",typeof(System.Double)), CultureInfo.InvariantCulture),
                HL = Double.Parse(NAttr(node,"HL",typeof(System.Double)), CultureInfo.InvariantCulture),
                XL = CreateFromCollection_double_node(XAttr(node,"XL")),
                RV = Double.Parse(NAttr(node,"RV",typeof(System.Double)), CultureInfo.InvariantCulture),
                HV = Double.Parse(NAttr(node,"HV",typeof(System.Double)), CultureInfo.InvariantCulture),
                XV = CreateFromCollection_double_node(XAttr(node,"XV")),
                Mw = Double.Parse(NAttr(node,"Mw",typeof(System.Double)), CultureInfo.InvariantCulture),
                V = Double.Parse(NAttr(node,"V",typeof(System.Double)), CultureInfo.InvariantCulture),
                Id = NAttr(node,"Id",typeof(System.String)),
                From = NAttr(node,"From",typeof(System.String)),
                To = NAttr(node,"To",typeof(System.String)),
                Figures = NAttr(node,"Figures",typeof(System.String)),
			};
		}
		public static tanks.ViewModels.Thermometer CreateFromThermometer_node(XmlNode node)
		{
			return new tanks.ViewModels.Thermometer{
                Tl = Double.Parse(NAttr(node,"Tl",typeof(System.Double)), CultureInfo.InvariantCulture),
                Th = Double.Parse(NAttr(node,"Th",typeof(System.Double)), CultureInfo.InvariantCulture),
                unit = (tanks.Models.EUTemperature)Enum.Parse(typeof(tanks.Models.EUTemperature),
                NAttr(node,"unit",typeof(tanks.Models.EUTemperature))),
                T = Double.Parse(NAttr(node,"T",typeof(System.Double)), CultureInfo.InvariantCulture),
                X = Double.Parse(NAttr(node,"X",typeof(System.Double)), CultureInfo.InvariantCulture),
                Y = Double.Parse(NAttr(node,"Y",typeof(System.Double)), CultureInfo.InvariantCulture),
                Id = NAttr(node,"Id",typeof(System.String)),
			};
		}
	}
}
