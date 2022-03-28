using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using tanks.Models;

namespace tanks.Models.Solver
{
    public static class SRKSolver
    {
        static public void ProcessLinks(FlowDiagram fd)
        {
            var Id2InstrumentUnit = new Dictionary<string, InstrumentUnit>();
            var Id2Signal = new Dictionary<string, Signal>();
            var Id2ProcessUnit = new Dictionary<string, ProcessUnit>();
            var Id2Stream = new Dictionary<string, tanks.Models.Stream>();

            foreach (var item in fd.Items)
            {
                if ((item as ProcessUnit) != null)
                {
                    if (Id2ProcessUnit.ContainsKey(item.Id))
                    {
                        throw new Exception("Duplicate item Id=\"" + item.Id + "\"");
                    }
                    Id2ProcessUnit[item.Id] = item as ProcessUnit;
                } else
                if ((item as InstrumentUnit) != null)
                {
                    if (Id2InstrumentUnit.ContainsKey(item.Id))
                    {
                        throw new Exception("Duplicate item Id=\"" + item.Id + "\"");
                    }
                    Id2InstrumentUnit[item.Id] = item as InstrumentUnit;
                } else
                    throw new Exception("Internal Error");
            }

            foreach (var link in fd.Links)
            {
                if ((link as tanks.Models.Stream) != null)
                {
                    if (Id2Stream.ContainsKey(link.Id))
                    {
                        throw new Exception("Duplicate link Id=\"" + link.Id + "\"");
                    }
                    Id2Stream[link.Id] = link as tanks.Models.Stream;
                } else
                if ((link as Signal) != null)
                {
                    if (Id2Signal.ContainsKey(link.Id))
                    {
                        throw new Exception("Duplicate link Id=\"" + link.Id + "\"");
                    }
                    Id2Signal[link.Id] = link as Signal;
                }
                else
                    throw new Exception("Internal Error");
            }

            foreach (var link in fd.Links)
            {
                string[] splitfrom;
                string[] splitto;

                if ((link as tanks.Models.Stream) != null)
                {
                    tanks.Models.ProcessUnit objfrom, objto;

                    splitfrom = link.From.Split('.');
                    Debug.Assert(splitfrom.Length == 2);
                    objfrom = Id2ProcessUnit[splitfrom[0]];
                    splitto = link.To.Split('.');
                    Debug.Assert(splitto.Length == 2);
                    objto = Id2ProcessUnit[splitto[0]];

                    objfrom.assignPort(splitfrom[1], (link as tanks.Models.Stream), 0, objto, splitto[1]);
                    objto.assignPort(splitto[1], (link as tanks.Models.Stream), 1, objfrom, splitfrom[1]);
                }
                else
                {
                    splitfrom = link.From.Split('.');
                    Debug.Assert((splitfrom.Length == 1) || (splitfrom.Length == 2));
                    splitto = link.To.Split('.');
                    Debug.Assert(splitto.Length == 2);

                    Models.ModelObject objto;
                    if (Id2InstrumentUnit.ContainsKey(splitto[0]))
                        objto = Id2InstrumentUnit[splitto[0]];
                    else
                        objto = Id2ProcessUnit[splitto[0]];

                    if (splitfrom.Length == 1)
                    {
                        Models.Stream streamfrom;

                        streamfrom = Id2Stream[splitfrom[0]];

                        objto.SetPoint(splitto[1], streamfrom.GetPoint(objto as InstrumentUnit));
                    }
                    else
                    {
                        Models.ModelObject objfrom;

                        if (Id2InstrumentUnit.ContainsKey(splitfrom[0]))
                            objfrom = Id2InstrumentUnit[splitfrom[0]];
                        else
                            objfrom = Id2ProcessUnit[splitfrom[0]];

                        objto.SetPoint(splitto[1], objfrom.GetPoint(splitfrom[1]));
                    }
                }
            }
        }

        public static bool mtcalc = true;

        public enum CalcPhase
        {
            InitialCalc,
            CalcWait,
            StepCalc,
            BuildEqSquared,
            SaveSolutionSquared,
            BuildEqLinear,
            SaveSolutionLinear,
        }

        // InitialCalc CalcWait BuildEqSquared SaveSolutionSquared
        // ( StepCalc CalcWait BuildEqSquared SaveSolutionSquared BuildEqLinear SaveSolutionLinear)*7
        // StepCalc CalcWait 

        private class pfInitialize_shared
        {
            public int cp;
            public int cf;
            public HashSet<Tuple<int, int>> visited;
            public Dictionary<int, Tuple<PortType, int>> unitsave;
        }

        private static void pfInitialize_doLink(FlowDiagram fd, GraphPartition gp, Tuple<int, int> link, pfInitialize_shared s)
        {
            if (s.visited.Contains(link)) return;
            s.visited.Add(link);
            
            bool bin = s.unitsave.ContainsKey(link.Item1);
            bool bout = s.unitsave.ContainsKey(link.Item2);

            if ((!bin) && (!bout))
            {
                var u = gp.graph.nodeLabel[link.Item1];
                PortType uk = (fd.Items[u.Item1] as ProcessUnit).pfGraphPartKind(u.Item2);
                var v = gp.graph.nodeLabel[link.Item2];
                PortType vk = (fd.Items[v.Item1] as ProcessUnit).pfGraphPartKind(v.Item2);

                if (uk == vk)
                {
                    if (uk == PortType.PfPartKindPressure)
                    {
                        s.unitsave[link.Item1] = new Tuple<PortType, int>(uk, s.cp);
                        s.unitsave[link.Item2] = new Tuple<PortType, int>(vk, s.cp);
                        s.cp++;
                    }
                    else
                    {
                        s.unitsave[link.Item1] = new Tuple<PortType, int>(uk, s.cf);
                        s.unitsave[link.Item2] = new Tuple<PortType, int>(vk, s.cf);
                        s.cf++;
                    }
                }
                else
                {
                    if (uk == PortType.PfPartKindPressure)
                    {
                        s.unitsave[link.Item1] = new Tuple<PortType, int>(uk, s.cp);
                        s.cp++;
                        s.unitsave[link.Item2] = new Tuple<PortType, int>(vk, s.cf);
                        s.cf++;
                    }
                    else
                    {
                        s.unitsave[link.Item1] = new Tuple<PortType, int>(uk, s.cf);
                        s.cf++;
                        s.unitsave[link.Item2] = new Tuple<PortType, int>(vk, s.cp);
                        s.cp++;
                    }
                }
            }
            else if (bin && (!bout))
            {
                PortType uk = s.unitsave[link.Item1].Item1;
                var v = gp.graph.nodeLabel[link.Item2];
                PortType vk = (fd.Items[v.Item1] as ProcessUnit).pfGraphPartKind(v.Item2);
                if (uk == vk)
                {
                    s.unitsave[link.Item2] = s.unitsave[link.Item1];
                }
                else
                {
                    if (uk == PortType.PfPartKindPressure)
                    {
                        s.unitsave[link.Item2] = new Tuple<PortType, int>(vk, s.cf);
                        s.cf++;
                    }
                    else
                    {
                        s.unitsave[link.Item2] = new Tuple<PortType, int>(vk, s.cp);
                        s.cp++;
                    }
                }
            }else if ((!bin) && bout)
            {
                var u = gp.graph.nodeLabel[link.Item1];
                PortType uk = (fd.Items[u.Item1] as ProcessUnit).pfGraphPartKind(u.Item2);
                PortType vk = s.unitsave[link.Item2].Item1;
                if (uk == vk)
                {
                    s.unitsave[link.Item1] = s.unitsave[link.Item2];
                }
                else
                {
                    if (uk == PortType.PfPartKindPressure)
                    {
                        s.unitsave[link.Item1] = new Tuple<PortType, int>(uk, s.cp);
                        s.cp++;
                    }
                    else
                    {
                        s.unitsave[link.Item1] = new Tuple<PortType, int>(uk, s.cf);
                        s.cf++;
                    }
                }
            }

            {
                int tp = -1;
                int tf = -1;
                PortType uk = s.unitsave[link.Item1].Item1;
                PortType vk = s.unitsave[link.Item2].Item1;

                if (uk == vk)
                {
                    Debug.Assert(s.unitsave[link.Item1].Item2 == s.unitsave[link.Item2].Item2);
                    if (uk == PortType.PfPartKindPressure)
                    {
                        tp = s.unitsave[link.Item1].Item2;
                        tf = s.cf; s.cf++;
                    }
                    else
                    {
                        tp = s.cp; s.cp++;
                        tf = s.unitsave[link.Item1].Item2;
                    }
                }
                else
                {
                    if (uk == PortType.PfPartKindPressure)
                    {
                        tp = s.unitsave[link.Item1].Item2;
                        tf = s.unitsave[link.Item2].Item2;
                    }
                    else
                    {
                        tf = s.unitsave[link.Item1].Item2;
                        tp = s.unitsave[link.Item2].Item2;
                    }
                }

                var stream = fd.Links[gp.graph.outt[link.Item1][link.Item2].Item2] as Models.Stream;
                stream.set_pfn(tp, tf);
            }
        }

        private static void pfInitialize_updateSet(Tuple<int, int>link, SortedDictionary<int, SortedSet<int>> pset)
        {
            bool b1 = pset.ContainsKey(link.Item1);
            bool b2 = pset.ContainsKey(link.Item2);

            if ((!b1) && (!b2))
            {
                var ts = new SortedSet<int> { link.Item1, link.Item2 };
                pset.Add(link.Item1, ts);
                pset.Add(link.Item2, ts);
            }
            else
            if (b1 && (!b2))
            {
                pset[link.Item1].Add(link.Item2);
            }
            else
            if ((!b1) && b2)
            {
                pset[link.Item2].Add(link.Item1);
            }
            else
            {
                if (pset[link.Item1] != pset[link.Item2])
                {
                    var ts = new SortedSet<int>();
                    ts.Union(pset[link.Item1]).Union(pset[link.Item2]);

                    var psetforscan = new List<int>(pset.Keys);

                    foreach (var t in psetforscan)
                        if (ts.Contains(t))
                            pset[t] = ts;
                }
            }
        }

        private static void pfInitialize(FlowDiagram fd, GraphPartition gp)
        {
            {
                foreach (var link in gp.graph.labEdges)
                {
                    Models.Stream stream = fd.Links[link.Item3.Item2] as Models.Stream;
                    stream.pn = -1;
                    stream.fn = -1;
                }
            }

            pfInitialize_shared s = new pfInitialize_shared();
            s.cp = 0;
            s.cf = 0;
            s.visited = new HashSet<Tuple<int, int>>();
            s.unitsave = new Dictionary<int, Tuple<PortType, int>>();

            {
                var oedges = new List<Tuple<int, int>>();

                var pset = new SortedDictionary<int, SortedSet<int>>();
                var fset = new SortedDictionary<int, SortedSet<int>>();

                foreach (var nodefrom in gp.order)
                    foreach (var nodeto in gp.graph.suc[nodefrom])
                    {
                        var u = gp.graph.nodeLabel[nodefrom];
                        PortType uk = (fd.Items[u.Item1] as ProcessUnit).pfGraphPartKind(u.Item2);
                        var v = gp.graph.nodeLabel[nodeto];
                        PortType vk = (fd.Items[v.Item1] as ProcessUnit).pfGraphPartKind(v.Item2);
                        var link = new Tuple<int, int>(nodefrom, nodeto);

                        if (uk == vk)
                        {
                            if (uk == PortType.PfPartKindPressure)
                            {
                                pfInitialize_updateSet(link, pset);
                            }
                            else
                            {
                                pfInitialize_updateSet(link, fset);
                            }
                        }
                        else
                            oedges.Add(link);
                    }

                var rpset = new HashSet<SortedSet<int>>(pset.Values);
                var rfset = new HashSet<SortedSet<int>>(fset.Values);

                foreach (var ss in rpset)
                {
                    var sg = FGraph.subGraphByNodes(ss.ToList(), gp.graph);
                    var so = FGraph.topOrder(sg);
                    foreach (var nodefrom in so)
                        foreach (var nodeto in sg.suc[nodefrom])
                        {
                            var link = new Tuple<int, int>(nodefrom, nodeto);
                            pfInitialize_doLink(fd, gp, link, s);
                        }
                }

                foreach (var ss in rfset)
                {
                    var sg = FGraph.subGraphByNodes(ss.ToList(), gp.graph);
                    var so = FGraph.topOrder(sg);
                    foreach (var nodefrom in so)
                        foreach (var nodeto in sg.suc[nodefrom])
                        {
                            var link = new Tuple<int, int>(nodefrom, nodeto);
                            pfInitialize_doLink(fd, gp, link, s);
                        }
                }

                foreach (var link in oedges)
                    pfInitialize_doLink(fd, gp, link, s);
            }

            Debug.Assert((s.cp + s.cf) == gp.graph.nodes.Count);

            foreach (var link in gp.graph.labEdges)
            {
                Models.Stream stream = fd.Links[link.Item3.Item2] as Models.Stream;
                stream.fix_pfn(s.cp);
            }

            gp.bp = 0;
            gp.bf = s.cp;
            gp.np = s.cp;
            gp.nf = s.cf;
        }

        private static void CalcGraphNormal(CalcPhase step, FlowDiagram fd, GraphPartition gp)
        {
            switch (step)
            {
                case CalcPhase.InitialCalc:
                    // Step0
                    {
                        gp.order = new List<int>(FGraph.FindNetOrderInComponent(gp.base_graph, (a) =>
                        {
                            var pk = (fd.Items[a.Item1] as ProcessUnit).scGraphPartKind(a.Item2);

                            return (pk == PortType.ExPartKindFeed) ? NodeType.In :
                            ((pk == PortType.ExPartKindProduct) ? NodeType.Out : NodeType.Neutral);
                        }));
                        gp.graph = FGraph.rebuildByOrder(gp.order,
                            (efrom, eto, b) =>
                            {
                                (fd.Links[b.Item2] as Models.Stream).bReverseFlow = true;
                                return new Tuple<int, int, int>(b.Item3, b.Item2, b.Item1);
                            }, gp.base_graph);

                        pfInitialize(fd, gp);

                        foreach (var link in gp.graph.labEdges)
                        {
                            Models.Stream stream = fd.Links[link.Item3.Item2] as Models.Stream;
                            stream.x = new Collection<double>(new double[fd.Components.Count]);
                            stream.F = 0;
                        }

                        if (SRKSolver.mtcalc)
                        {
                            try
                            {
                                var upmap = new Dictionary<int, Task>();
                                var upmapWait = new Dictionary<int, Task[]>();

                                foreach (var u in gp.order)
                                {
                                    var lu = gp.graph.nodeLabel[u];
                                    var arr = new List<Task>();

                                    foreach (var p in gp.graph.pre[u])
                                        arr.Add(upmap[p]);

                                    if (arr.Count == 0)
                                    {
                                        upmap[u] =
                                            Task.Factory.StartNew(() => (fd.Items[lu.Item1] as ProcessUnit).scCalcOutStreams(lu.Item2, fd, true)
                                            //                            ,new CancellationToken()
                                            //                            ,TaskCreationOptions.LongRunning
                                            //                          , lcts
                                            );
                                    }
                                    else
                                    {
                                        upmap[u] =
                                            Task.Factory.ContinueWhenAll(arr.ToArray(),
                                            (aa) => (fd.Items[lu.Item1] as ProcessUnit).scCalcOutStreams(lu.Item2, fd, true)
                                            //                            ,new CancellationToken()
                                            //                              ,TaskContinuationOptions.LongRunning
                                            //                            , lcts
                                            );
                                    }
                                }

                                gp.WaitTask = Task.Factory.ContinueWhenAll(upmap.Values.ToArray(), (aa) =>
                                {
                                    foreach (var a in aa)
                                    {
                                        Debug.Assert(a.Status == TaskStatus.RanToCompletion);
                                    }
                                });
                            }
                            catch (Exception e)
                            {
                                Debug.Assert(false);
                            }
                        }
                        else
                        {
                            foreach (var u in gp.order)
                            {
                                var lu = gp.graph.nodeLabel[u];
                                (fd.Items[lu.Item1] as ProcessUnit).scCalcOutStreams(lu.Item2, fd, true);
                            }
                            gp.WaitTask = Task.Factory.StartNew(() => { });
                        }
                    }
                    break;
                case CalcPhase.CalcWait:
                    gp.WaitTask.Wait();
                    break;
                case CalcPhase.BuildEqSquared:
                    // Step1 
                    {
                        int i = gp.bp;
                        foreach (var v in gp.graph.labNodes)
                        {
                            gp.A[i] = (fd.Items[v.Item2.Item1] as ProcessUnit)
                                .pfGraphPFE(v.Item2.Item2, ref gp.B[i], gp.dim, fd);
                            i++;
                        }
                    }
                    break;
                case CalcPhase.SaveSolutionSquared:
                    // Step2
                    {
                        foreach (var me in gp.graph.labEdges)
                        {
                            (fd.Links[me.Item3.Item2] as Models.Stream).save_solution(gp.x);
                        }
                    }
                    break;
                case CalcPhase.StepCalc:
                    {
                        {
                            // rebuild graph
                            var el = new List<Tuple<int, int, Tuple<int, int, int>>>();

                            foreach (var link in gp.base_graph.labEdges)
                            {
                                Models.Stream stream = fd.Links[link.Item3.Item2] as Models.Stream;
                                if (stream.bReverseFlow)
                                {
                                    el.Add(new Tuple<int, int, Tuple<int, int, int>>(link.Item2, link.Item1,
                                        new Tuple<int, int, int>(link.Item3.Item3, link.Item3.Item2, link.Item3.Item1)));
                                } else
                                    el.Add(link);
                            }
                            gp.graph = FGraph.insLabEdges(el, FGraph.insLabNodes(gp.base_graph.labNodes, FGraph.empty<Tuple<int, int>, Tuple<int, int, int>>()));
                        }

                        gp.order = new List<int>(FGraph.topOrder(gp.graph));

                        if (SRKSolver.mtcalc)
                        {
                            try
                            {
                                var upmap = new Dictionary<int, Task>();
                                var upmapWait = new Dictionary<int, Task[]>();

                                foreach (var u in gp.order)
                                {
                                    var lu = gp.graph.nodeLabel[u];
                                    var arr = new List<Task>();

                                    foreach (var p in gp.graph.pre[u])
                                        arr.Add(upmap[p]);

                                    if (arr.Count == 0)
                                    {
                                        upmap[u] =
                                            Task.Factory.StartNew(() => (fd.Items[lu.Item1] as ProcessUnit).scCalcOutStreams(lu.Item2, fd, false)
                                            //                            ,new CancellationToken()
                                            //                            ,TaskCreationOptions.LongRunning
                                            //                          , lcts
                                            );
                                    }
                                    else
                                    {
                                        upmap[u] =
                                            Task.Factory.ContinueWhenAll(arr.ToArray(),
                                            (aa) => (fd.Items[lu.Item1] as ProcessUnit).scCalcOutStreams(lu.Item2, fd, false)
                                            //                            ,new CancellationToken()
                                            //                              ,TaskContinuationOptions.LongRunning
                                            //                            , lcts
                                            );
                                    }
                                }

                                gp.WaitTask = Task.Factory.ContinueWhenAll(upmap.Values.ToArray(), (aa) =>
                                {
                                    foreach (var a in aa)
                                    {
                                        Debug.Assert(a.Status == TaskStatus.RanToCompletion);
                                    }
                                });
                            }
                            catch (Exception e)
                            {
                                Debug.Assert(false);
                            }
                        }
                        else
                        {
                            foreach (var u in gp.order)
                            {
                                var lu = gp.graph.nodeLabel[u];
                                (fd.Items[lu.Item1] as ProcessUnit).scCalcOutStreams(lu.Item2, fd, false);
                            }
                            gp.WaitTask = Task.Factory.StartNew(() => { });
                        }
                    }
                    break;
                case CalcPhase.BuildEqLinear:
                    {
                        int i = gp.bp;
                        foreach (var v in gp.graph.labNodes)
                        {
                            gp.A[i] = (fd.Items[v.Item2.Item1] as ProcessUnit)
                                .pfGraphPFE_l(v.Item2.Item2, ref gp.B[i], gp.dim, fd);
                            i++;
                        }
                    }
                    break;
                case CalcPhase.SaveSolutionLinear:
                    {
                        foreach (var me in gp.graph.labEdges)
                        {
                            (fd.Links[me.Item3.Item2] as Models.Stream).save_solution_l(gp.x);
                        }
                    }
                    break;
            }
        }

        private static void CalcGraphZeroA(CalcPhase step, FlowDiagram fd, GraphPartition gp)
        {
            switch (step)
            {
                case CalcPhase.InitialCalc:
                    CalcGraphNormal(step, fd, gp);
                    break;
                case CalcPhase.CalcWait:
                    CalcGraphNormal(step, fd, gp);
                    break;
                case CalcPhase.BuildEqSquared:
                    CalcGraphNormal(step, fd, gp);
                    break;
                case CalcPhase.SaveSolutionSquared:
                    CalcGraphNormal(step, fd, gp);
                    break;
                case CalcPhase.StepCalc:
                    CalcGraphNormal(step, fd, gp);
                    break;
                case CalcPhase.BuildEqLinear:
                    CalcGraphNormal(step, fd, gp);
                    break;
                case CalcPhase.SaveSolutionLinear:
                    CalcGraphNormal(step, fd, gp);
                    break;
            }
        }

        private static void CalcGraphZeroB(CalcPhase step, FlowDiagram fd, GraphPartition gp)
        {
            switch (step)
            {
                case CalcPhase.InitialCalc:
                    {
                        gp.order = new List<int>(FGraph.FindNetOrderInComponent(gp.base_graph, (a) =>
                        {
                            var pk = (fd.Items[a.Item1] as ProcessUnit).scGraphPartKind(a.Item2);

                            return (pk == PortType.ExPartKindFeed) ? NodeType.In :
                            ((pk == PortType.ExPartKindProduct) ? NodeType.Out : NodeType.Neutral);
                        }));
                        gp.graph = FGraph.rebuildByOrder(gp.order,
                            (efrom, eto, b) =>
                            {
                                (fd.Links[b.Item2] as Models.Stream).bReverseFlow = true;
                                return new Tuple<int, int, int>(b.Item3, b.Item2, b.Item1);
                            }, gp.base_graph);

                        foreach (var link in gp.graph.labEdges)
                        {
                            Models.Stream stream = fd.Links[link.Item3.Item2] as Models.Stream;
                            stream.x = new Collection<double>(new double[fd.Components.Count]);
                            stream.F = 0;
                            stream.set_pfn(0, 1);
                        }

                        gp.bp = 0;
                        gp.bf = 1;
                        gp.np = 1;
                        gp.nf = 1;
                    }
                    break;
                case CalcPhase.CalcWait:
                    break;
                case CalcPhase.BuildEqSquared:
                    {
                        foreach (var v in gp.graph.labNodes)
                        {
                            var pu = (fd.Items[v.Item2.Item1] as ProcessUnit);
                            if (pu.scGraphPartKind(v.Item2.Item2) == PortType.ExPartKindProduct)
                            {
                                // need better
                                gp.A[gp.bp] = pu.pfGraphPFE(v.Item2.Item2, ref gp.B[gp.bp], gp.dim, fd);
                                gp.A[gp.bf] = new double[gp.dim];
                                gp.A[gp.bf][gp.bf] = 1.0;
                                gp.B[gp.bf] = 0;
                                break;
                            }
                        }
                    }
                    break;
                case CalcPhase.SaveSolutionSquared:
                    {
                        foreach (var me in gp.graph.labEdges)
                        {
                            (fd.Links[me.Item3.Item2] as Models.Stream).save_solution(gp.x);
                        }
                    }
                    break;
                case CalcPhase.StepCalc:
                    break;
                case CalcPhase.BuildEqLinear:
                    {
                        foreach (var v in gp.graph.labNodes)
                        {
                            var pu = (fd.Items[v.Item2.Item1] as ProcessUnit);
                            if (pu.scGraphPartKind(v.Item2.Item2) == PortType.ExPartKindProduct)
                            {
                                // need better
                                gp.A[gp.bp] = pu.pfGraphPFE_l(v.Item2.Item2, ref gp.B[gp.bp], gp.dim, fd);
                                gp.A[gp.bf] = new double[gp.dim];
                                gp.A[gp.bf][gp.bf] = 1.0;
                                gp.B[gp.bf] = 0;
                                break;
                            }
                        }
                    }
                    break;
                case CalcPhase.SaveSolutionLinear:
                    {
                        foreach (var me in gp.graph.labEdges)
                        {
                            (fd.Links[me.Item3.Item2] as Models.Stream).save_solution_l(gp.x);
                        }
                    }
                    break;
            }
        }

        private static void CalcGraphNoFlow(CalcPhase step, FlowDiagram fd, GraphPartition gp)
        {
            switch (step)
            {
                case CalcPhase.InitialCalc:
                    CalcGraphZeroB(step, fd, gp);
                    break;
                case CalcPhase.CalcWait:
                    CalcGraphZeroB(step, fd, gp);
                    break;
                case CalcPhase.BuildEqSquared:
                    {
                        // need better
                        gp.A[gp.bp] = new double[gp.dim];
                        gp.A[gp.bp][gp.bp] = 1.0;
                        gp.B[gp.bp] = 101.325;
                        gp.A[gp.bf] = new double[gp.dim];
                        gp.A[gp.bf][gp.bf] = 1.0;
                        gp.B[gp.bf] = 0;
                    }
                    break;
                case CalcPhase.SaveSolutionSquared:
                    CalcGraphZeroB(step, fd, gp);
                    break;
                case CalcPhase.StepCalc:
                    CalcGraphZeroB(step, fd, gp);
                    break;
                case CalcPhase.BuildEqLinear:
                    {
                        // need better
                        gp.A[gp.bp] = new double[gp.dim];
                        gp.A[gp.bp][gp.bp] = 1.0;
                        gp.B[gp.bp] = 101.325;
                        gp.A[gp.bf] = new double[gp.dim];
                        gp.A[gp.bf][gp.bf] = 1.0;
                        gp.B[gp.bf] = 0;
                    }
                    break;
                case CalcPhase.SaveSolutionLinear:
                    CalcGraphZeroB(step, fd, gp);
                    break;
            }
        }

        private class GraphPartition
        {
            public FGraph<Tuple<int, int>, Tuple<int, int, int>> base_graph;
            public FGraph<Tuple<int, int>, Tuple<int, int, int>> graph;
            public List<int> order;
            public bool haveFeed;
            public bool haveProduct;
            public int bp;
            public int bf;
            public int np;
            public int nf;
            public int dim;
            public double[][] A; // global
            public double[] B; // global
            public double[] x; // global
            public Task WaitTask;
        }

        private static FGraph<Tuple<int, int>, Tuple<int, int, int>> GlobalGraphBuild(FlowDiagram fd)
        {
            // A - (unit,part)
            // B - (port,link,port)

            var nl = new List<Tuple<int, Tuple<int, int>>>();
            var el = new List<Tuple<int, int,Tuple<int, int, int>>>();

            var nid = new Dictionary<Tuple<int, int>, int>();

            int ncnt = 0;
            foreach (var item in fd.Items)
            {
                if ((item as ProcessUnit) == null) continue;
                int iitem = fd.Items.IndexOf(item);
                for (int i = 0; i < (item as ProcessUnit).GraphPartsCount(); i++)
                {
                    var tmp = new Tuple<int, int>(iitem, i);
                    nl.Add(new Tuple<int, Tuple<int, int>>(ncnt,tmp));
                    nid.Add(tmp, ncnt);
                    ncnt++;
                }
            }
            foreach (var link in fd.Links)
            {
                if ((link as tanks.Models.Stream) == null) continue;

                Tuple<int, int> tmpfrom, tmpto;
                int ifrom, ito;
                ProcessUnit objfrom, objto;

                objfrom = (link as tanks.Models.Stream).sports[0].Item1;
                ifrom = fd.Items.IndexOf(objfrom);
                tmpfrom = new Tuple<int, int>(ifrom, objfrom.GraphPartByPort((link as tanks.Models.Stream).sports[0].Item2));

                objto = (link as tanks.Models.Stream).sports[1].Item1;
                ito = fd.Items.IndexOf(objto);
                tmpto = new Tuple<int, int>(ito, objto.GraphPartByPort((link as tanks.Models.Stream).sports[1].Item2));

                el.Add(new Tuple<int, int, Tuple<int, int, int>>(nid[tmpfrom], nid[tmpto],
                    new Tuple<int, int, int>(
                        (link as tanks.Models.Stream).sports[0].Item2,
                        fd.Links.IndexOf(link),
                        (link as tanks.Models.Stream).sports[1].Item2)));
            }

            return FGraph.insLabEdges(el,FGraph.insLabNodes(nl,FGraph.empty<Tuple<int, int>, Tuple<int, int, int>>()));
        }

        public static void SolveStatic(FlowDiagram fd)
        {
            foreach (var link in fd.Links)
            {
                Models.Stream stream = link as Models.Stream;
                if (stream == null) continue;
                stream.x = new Collection<double>(new double[fd.Components.Count]);
                stream.F = 0;
                stream.bReverseFlow = false;
            }

            var globalGraph= GlobalGraphBuild(fd);

            var cc = FGraph.CC(globalGraph);

            var gpa = new GraphPartition[cc.Count];

            double[][] A = null;
            double[] B = null;
            double[] x = null;

            for (int i = 0; i < cc.Count; i++)
            {
                bool haveFeed = false;
                bool haveProduct = false;

                foreach (var vk in cc[i])
                {
                    int unit = globalGraph.labNodes[vk].Item2.Item1;
                    int part = globalGraph.labNodes[vk].Item2.Item2;
                    var pk = (fd.Items[unit] as ProcessUnit).scGraphPartKind(part);
                    if (pk == PortType.ExPartKindFeed) haveFeed = true;
                    if (pk == PortType.ExPartKindProduct) haveProduct = true;
                }

                gpa[i] = new GraphPartition();

                gpa[i].base_graph = FGraph.subGraphByNodes(cc[i], globalGraph);
                gpa[i].graph = null;
                
                gpa[i].haveFeed = haveFeed;
                gpa[i].haveProduct = haveProduct;
                gpa[i].bp = 0;
                gpa[i].bf = 0;
                gpa[i].np = 0;
                gpa[i].nf = 0;
                gpa[i].A = null;
                gpa[i].B = null;
                gpa[i].x = null;
                gpa[i].WaitTask = null;
            }

            int ni = 8;

            CalcPhase[] calcPhase = new CalcPhase[(ni+1)*6];

            calcPhase[0] = CalcPhase.InitialCalc;
            calcPhase[1] = CalcPhase.CalcWait;
            calcPhase[2] = CalcPhase.BuildEqSquared;
            calcPhase[3] = CalcPhase.SaveSolutionSquared; 

            for (int i = 0; i < ni;i++)
            {
                calcPhase[i * 6 + 4] = CalcPhase.StepCalc;
                calcPhase[i * 6 + 5] = CalcPhase.CalcWait;
                calcPhase[i * 6 + 6] = CalcPhase.BuildEqSquared;
                calcPhase[i * 6 + 7] = CalcPhase.SaveSolutionSquared;
                calcPhase[i * 6 + 8] = CalcPhase.BuildEqLinear;
                calcPhase[i * 6 + 9] = CalcPhase.SaveSolutionLinear;
            }

            calcPhase[ni * 6 + 4] = CalcPhase.StepCalc;
            calcPhase[ni * 6 + 5] = CalcPhase.CalcWait;

            foreach (var phase in calcPhase)
            {
                for (int i = 0; i < gpa.Length; i++)
                {
                    if (gpa[i].haveFeed && gpa[i].haveProduct)
                    {
                        CalcGraphNormal(phase, fd, gpa[i]);
                    }
                    else
                    if (gpa[i].haveFeed && (!gpa[i].haveProduct))
                    {
                        CalcGraphZeroA(phase, fd, gpa[i]);
                    }
                    else
                    if ((!gpa[i].haveFeed) && gpa[i].haveProduct)
                    {
                        CalcGraphZeroB(phase, fd, gpa[i]);
                    }
                    else
                    if ((!gpa[i].haveFeed) && (!gpa[i].haveProduct))
                    {
                        CalcGraphNoFlow(phase, fd, gpa[i]);
                    }

                    if (phase == CalcPhase.InitialCalc)
                    {
                        FixFPNumbering(i, fd, gpa);
                    }
                }
                if (phase == CalcPhase.InitialCalc)
                {
                    int dim = gpa[gpa.Length - 1].bf + gpa[gpa.Length - 1].nf;
                    A = new double[dim][];
                    B = new double[dim];
                    for (int i = 0; i < gpa.Length; i++)
                    {
                        gpa[i].dim = dim;

                        gpa[i].B = B;
                        gpa[i].A = A;
                    }
                }
                else
                if ((phase == CalcPhase.BuildEqSquared) || (phase == CalcPhase.BuildEqLinear))
                {
                    x = GaussianElimination.Solve(A, B);
                    for (int i = 0; i < gpa.Length; i++)
                    {
                        gpa[i].x = x;
                    }
                }
            }
        }

        private static void FixFPNumbering(int i, FlowDiagram fd, GraphPartition[] gpa)
        {
            if (i == 0) return;

            int offset = gpa[i - 1].bf + gpa[i - 1].nf;

            gpa[i].bp += offset;
            gpa[i].bf += offset;

            foreach (var link in gpa[i].graph.labEdges)
            {
                Models.Stream stream = fd.Links[link.Item3.Item2] as Models.Stream;
                stream.pn += offset;
                stream.fn += offset;
            }
        }
    }
}
