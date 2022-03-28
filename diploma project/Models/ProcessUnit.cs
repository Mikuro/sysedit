using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace tanks.Models
{
    public class ProcessUnit : ModelObject
    {
        public Stream[] ports;
        public Tuple<ProcessUnit, int>[] units;

        public void assignPort(string name, Stream link, int idx, ProcessUnit other, string othername)
        {
            int iport;
            iport = GetPortByName(name);

            if (ports[iport] != null)
            {
                throw new Exception("port name \"" + name + "\" already assigned");
            }

            ports[iport] = link;
            units[iport] = new Tuple<ProcessUnit, int>(other, other.GetPortByName(othername));

            link.sports[idx] = new Tuple<ProcessUnit, int > ( this, iport);
        }

        public Tuple<ProcessUnit, int> GetOtherUnit(int iii)
        {
            return units[iii];
        }

        public class UnitTopology
        {
            public Collection<string> portnames; // GetPortsCount, GetPortByName

            public Collection<Collection<int>[]> Parts;
            public Dictionary<int, PortType> scPartsKind;
            public Dictionary<int, PortType> pfPartsKind;
        }

        static Dictionary<string, UnitTopology> topology = new Dictionary<string, UnitTopology>();

        static public int type2port(PortType type)
        {
            return ((int)type) & ((int)PortType.LastPort);
        }

        static public int type2Part(PortType type)
        {
            return ((((int)type) & ((int)PortType.LastPart)) / ((int)PortType.Part1));
        }

        static public PortType type2exPartKind(PortType type)
        {
            return type & PortType.ExPartKindMask;
        }

        static public PortType type2pfPartKind(PortType type)
        {
            return type & PortType.PfPartKindMask;
        }

        static public PortType type2io(PortType type)
        {
            return type & PortType.IoMask;
        }

        private void InitTopology()
        {
            if (!ProcessUnit.topology.ContainsKey(GetType().Name))
            {
                var topology = new UnitTopology();
                topology.portnames = new Collection<string>();
                topology.Parts = new Collection<Collection<int>[]>();
                topology.scPartsKind = new Dictionary<int, PortType>();
                topology.pfPartsKind = new Dictionary<int, PortType>();
                int current_port = 0;

                foreach (var propertyInfo in GetType().GetProperties())
                {
                    var portAttribyte = propertyInfo.GetCustomAttribute<PortTypeAttribute>();
                    if (portAttribyte == null) continue;

                    if (current_port != type2port(portAttribyte.type))
                        throw new Exception("Internal Error"); // ports numbering problem

                    int current_part = type2Part(portAttribyte.type);

                    while (current_part>= topology.Parts.Count)
                        topology.Parts.Add(new Collection<int>[2]
                            { new Collection<int>(),new Collection<int>()});
                    
                    topology.Parts[current_part][type2io(portAttribyte.type) == PortType.InPort ? 0 : 1].Add(current_port);
                    if (topology.scPartsKind.ContainsKey(current_part))
                    {
                        if (topology.scPartsKind[current_part] != type2exPartKind(portAttribyte.type))
                            throw new Exception("Internal Error"); // sc ports kind problem
                    }
                    else
                    {
                        topology.scPartsKind[current_part] = type2exPartKind(portAttribyte.type);
                    }

                    if (topology.pfPartsKind.ContainsKey(current_part))
                    {
                        if (topology.pfPartsKind[current_part] != type2pfPartKind(portAttribyte.type))
                            throw new Exception("Internal Error"); // pf ports kind problem
                    }
                    else
                    {
                        topology.pfPartsKind[current_part] = type2pfPartKind(portAttribyte.type);
                    }

                    topology.portnames.Add(propertyInfo.Name);
                    current_port++;
                }
                ProcessUnit.topology[GetType().Name] = topology;
            }
        }


        protected virtual UnitTopology GetTopology()
        {
            return ProcessUnit.topology[GetType().Name];
        }

        public ProcessUnit()
        {
            InitTopology();

            ports = new Stream[GetPortsCount()];
            units = new Tuple<ProcessUnit, int>[GetPortsCount()];
        }

        // ==

        public int GetPortsCount()
        {
            var topology = GetTopology();
            return topology.portnames.Count;
        }

        public string GetPortName(int idx)
        {
            var topology = GetTopology();

            return topology.portnames[idx];
        }
        public int GetPortByName(string s)
        {
            var topology = GetTopology();
            int r = topology.portnames.IndexOf(s);

            if (r < 0)
            {
                throw new Exception("Invalid port name \"" + s + "\"");
            }

            return r;
        }

        // ====

        public int GraphPartsCount()
        {
            var topology = GetTopology();
            return topology.Parts.Count;
        }

        public int[] GraphInPortsByPart(int part)
        {
            var topology = GetTopology();
            return topology.Parts[part][0].ToArray();
        }

        public int[] GraphOutPortsByPart(int part)
        {
            var topology = GetTopology();
            return topology.Parts[part][1].ToArray();
        }

        public int[] GraphPortsByPart(int part)
        {
            var topology = GetTopology();

            var r = new int[topology.Parts[part][0].Count+ topology.Parts[part][1].Count];

            topology.Parts[part][0].CopyTo(r, 0);
            topology.Parts[part][1].CopyTo(r, topology.Parts[part][0].Count);

            return r;
        }

        public int[] GraphInPortsByPartDynamic(int part)
        {
            var inPortsStatic = GraphInPortsByPart(part);
            var allPorts = GraphPortsByPart(part);
            var r = new List<int>();

            foreach (var idx in allPorts)
            {
                if (ports[idx] != null)
                {
                    if (inPortsStatic.Contains(idx))
                    {
                        if (!ports[idx].bReverseFlow)
                            r.Add(idx);
                    }
                    else
                    {
                        if (ports[idx].bReverseFlow)
                            r.Add(idx);
                    }
                }
            }

            return r.ToArray();
        }

        public int[] GraphOutPortsByPartDynamic(int part)
        {
            var outPortsStatic = GraphOutPortsByPart(part);
            var allPorts = GraphPortsByPart(part);
            var r = new List<int>();

            foreach (var idx in allPorts)
            {
                if (ports[idx] != null)
                {
                    if (outPortsStatic.Contains(idx))
                    {
                        if (!ports[idx].bReverseFlow)
                            r.Add(idx);
                    }
                    else
                    {
                        if (ports[idx].bReverseFlow)
                            r.Add(idx);
                    }
                }
            }

            return r.ToArray();
        }

        public int[] GraphPortsByPartDynamic(int part)
        {
            var ar1 = GraphInPortsByPartDynamic(part);
            var ar2 = GraphOutPortsByPartDynamic(part);

            var r = new int[ar1.Length + ar2.Length];

            ar1.CopyTo(r, 0);
            ar2.CopyTo(r, ar1.Length);

            return r;
        }

        public int GraphPartByPort(int iport)
        {
            var topology = GetTopology();

            int part;

            for (part = 0; part < topology.Parts.Count; part++)
            {
                if (topology.Parts[part][0].Contains(iport))
                    break;
                if (topology.Parts[part][1].Contains(iport))
                    break;
            }

            if(part == topology.Parts.Count)
                throw new Exception("exPartByPort failed");

            return part;
        }

        public PortType scGraphPartKind(int part)
        {
            var topology = GetTopology();
            return topology.scPartsKind[part];
        }

        public PortType pfGraphPartKind(int part)
        {
            var topology = GetTopology();
            return topology.pfPartsKind[part];
        }

        public virtual double[] pfGraphPFE(int part, ref double b, int n, FlowDiagram flowDiagram)
        {
            throw new NotImplementedException();
        }
        public virtual void scCalcOutStreams(int part, FlowDiagram fd, bool bInitial)
        {
            throw new NotImplementedException();
        }

        public virtual double[] pfGraphPFE_l(int part, ref double b, int n, FlowDiagram fd)
        {
            throw new NotImplementedException();
        }
    }
}
