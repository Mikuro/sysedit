using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tanks.Models;

namespace tanks.Models.Solver
{
    public class FGraph<A, B>
    {
        IReadOnlyDictionary<int, Tuple<IReadOnlyDictionary<int, B>, A, IReadOnlyDictionary<int, B>>> _rep;

        internal IReadOnlyDictionary<int, Tuple<IReadOnlyDictionary<int, B>, A, IReadOnlyDictionary<int, B>>>
            rep
        { get { return _rep; } set { _rep = value; build(); } }

        public bool isEmpty { get; private set; }
        public IReadOnlyList<int> nodes { get; private set; }
        public IReadOnlyList<Tuple<int, A>> labNodes { get; private set; }
        public IReadOnlyList<Tuple<int, int>> edges { get; private set; }
        public IReadOnlyList<Tuple<int, int, B>> labEdges { get; private set; }

        public IReadOnlyDictionary<int, A> nodeLabel { get; private set; }

        public IReadOnlyDictionary<int, IReadOnlyList<int>> neighbour { get; private set; }
        public IReadOnlyDictionary<int, IReadOnlyList<int>> pre { get; private set; }
        public IReadOnlyDictionary<int, IReadOnlyList<int>> suc { get; private set; }
        public IReadOnlyDictionary<int, IReadOnlyDictionary<int, B>> inn { get; private set; }
        public IReadOnlyDictionary<int, IReadOnlyDictionary<int, B>> outt { get; private set; }

        private void build()
        {
            isEmpty = (rep.Count == 0);

            var nodeLabel = new SortedDictionary<int, A>();
            var nodes = new int[rep.Count];
            var labNodes = new Tuple<int, A>[rep.Count];

            int nedges = 0;
            foreach (var ctx in rep)
                nedges += ctx.Value.Item3.Count;

            var edges = new Tuple<int, int>[nedges];
            var labEdges = new Tuple<int, int, B>[nedges];


            {
                int i = 0;
                foreach (var node in rep)
                {
                    nodes[i] = node.Key;
                    nodeLabel[node.Key] = node.Value.Item2;
                    labNodes[i] = new Tuple<int, A>(node.Key, node.Value.Item2);
                    i++;
                }
            }
            {
                int i = 0;
                foreach (var ctx in rep)
                    foreach (var e in ctx.Value.Item3)
                    {
                        edges[i] = new Tuple<int, int>(ctx.Key, e.Key);
                        labEdges[i] = new Tuple<int, int, B>(ctx.Key, e.Key, e.Value);
                        i++;
                    }
            }

            this.nodeLabel = nodeLabel;
            this.nodes = nodes;
            this.labNodes = labNodes;
            this.edges = edges;
            this.labEdges = labEdges;

            var pre = new SortedDictionary<int, IReadOnlyList<int>>();
            var inn = new SortedDictionary<int, IReadOnlyDictionary<int, B>>();
            foreach (var ctx in rep)
            {
                var t1 = new SortedSet<int>();
                var t2 = new SortedDictionary<int, B>();
                int i = 0;
                foreach (var e in ctx.Value.Item1)
                {
                    t1.Add(e.Key);
                    t2[e.Key] = e.Value;
                    i++;
                }
                pre.Add(ctx.Key, t1.ToList());
                inn.Add(ctx.Key, t2);
            }

            this.pre = pre;
            this.inn = inn;

            var suc = new SortedDictionary<int, IReadOnlyList<int>>();
            var outt = new SortedDictionary<int, IReadOnlyDictionary<int, B>>();
            foreach (var ctx in rep)
            {
                var t1 = new SortedSet<int>();
                var t2 = new SortedDictionary<int, B>(); ;
                int i = 0;
                foreach (var e in ctx.Value.Item3)
                {
                    t1.Add(e.Key);
                    t2[e.Key] = e.Value;
                    i++;
                }
                suc.Add(ctx.Key, t1.ToList());
                outt.Add(ctx.Key, t2);
            }

            this.suc = suc;
            this.outt = outt;

            var neighbour = new SortedDictionary<int, IReadOnlyList<int>>();

            foreach (var ctx in rep)
            {
                var t1 = new SortedSet<int>();
                foreach (var e in ctx.Value.Item1)
                    t1.Add(e.Key);
                foreach (var e in ctx.Value.Item3)
                    t1.Add(e.Key);
                neighbour.Add(ctx.Key, t1.ToList());
            }
            this.neighbour = neighbour;
        }

        public override string ToString()
        {
            string res = "";

            res += "{{";
            bool bFirst = true;
            foreach (var e in labNodes)
            {
                if (!bFirst) res += ",";
                else bFirst = false;
                res += String.Format("({0},[{1}])", e.Item1, e.Item2);
            }
            res += "},{";
            bFirst = true;
            foreach (var e in labEdges)
            {
                if (!bFirst) res += ",";
                else bFirst = false;
                res += String.Format("({0},{1},[{2}])", e.Item1, e.Item2, e.Item3);
            }
            res += "}}";

            return res;
        }
    }

    [Flags]
    public enum NodeType
    {
        Neutral = 0,
        In = 1,
        Out = 2,
        InOut = 3
    }

    public static class FGraph
    {
/*
        public static Tuple<Tuple<
                IReadOnlyList<Tuple<B, int>>,
                int, A,
                IReadOnlyList<Tuple<B, int>>>
                , FGraph<A, B>> match<A, B>(int node, FGraph<A, B> graph)
        {
            Tuple<
                IReadOnlyList<Tuple<B, int>>,
                int, A,
                IReadOnlyList<Tuple<B, int>>> context;
            {
                var ctx = graph.rep[node];

                var tpre = new Tuple<B, int>[ctx.Item1.Count];
                {
                    int i = 0;
                    foreach (var t in ctx.Item1)
                    {
                        tpre[i] = new Tuple<B, int>(t.Value, t.Key);
                        i++;
                    }
                }

                var tsuc = new Tuple<B, int>[ctx.Item3.Count];
                {
                    int i = 0;
                    foreach (var t in ctx.Item3)
                    {
                        tsuc[i] = new Tuple<B, int>(t.Value, t.Key);
                        i++;
                    }
                }
                context = new Tuple<
                    IReadOnlyList<Tuple<B, int>>,
                    int, A,
                    IReadOnlyList<Tuple<B, int>>>
                    (tpre, node, ctx.Item2, tsuc);
            }

            var r = new SortedDictionary<int,
                Tuple<IReadOnlyDictionary<int, B>, A, IReadOnlyDictionary<int, B>>>();

            foreach (var elem in graph.rep)
            {
                if (elem.Key == node) continue;
                var newvalue = elem.Value;

                if (elem.Value.Item1.ContainsKey(node))
                {
                    var tpre = new SortedDictionary<int, B>();
                    foreach (var t in newvalue.Item1)
                    {
                        if (t.Key == node) continue;
                        tpre.Add(t.Key, t.Value);
                    }
                    var lbl = newvalue.Item2;
                    var tsuc = newvalue.Item3;
                    newvalue = new Tuple<IReadOnlyDictionary<int, B>, A, IReadOnlyDictionary<int, B>>
                    (tpre, lbl, tsuc);
                }
                if (elem.Value.Item3.ContainsKey(node))
                {
                    var tsuc = new SortedDictionary<int, B>();
                    foreach (var t in newvalue.Item3)
                    {
                        if (t.Key == node) continue;
                        tsuc.Add(t.Key, t.Value);
                    }
                    var lbl = newvalue.Item2;
                    var tpre = newvalue.Item1;
                    newvalue = new Tuple<IReadOnlyDictionary<int, B>, A, IReadOnlyDictionary<int, B>>
                    (tpre, lbl, tsuc);
                }
                r.Add(elem.Key, newvalue);
            }

            return new Tuple<Tuple<IReadOnlyList<Tuple<B, int>>, int, A, IReadOnlyList<Tuple<B, int>>>, FGraph<A, B>>
                (context, new FGraph<A, B> { rep = r });
        }
*/
        public static FGraph<A, B> empty<A, B>()
        {
            return new FGraph<A, B>
            {
                rep = new SortedDictionary<int,
                Tuple<IReadOnlyDictionary<int, B>, A, IReadOnlyDictionary<int, B>>>()
            };
        }

        public static FGraph<A, B> insLabNodes<A, B>(IReadOnlyList<Tuple<int, A>> labNodes, FGraph<A, B> graph)
        {
            var r = new SortedDictionary<int,
                Tuple<IReadOnlyDictionary<int, B>, A, IReadOnlyDictionary<int, B>>>();

            foreach (var node in labNodes)
            {
                var preds = new SortedDictionary<int, B>();
                var succs = new SortedDictionary<int, B>();
                var t0 = new Tuple<IReadOnlyDictionary<int, B>, A, IReadOnlyDictionary<int, B>>(
                    preds, node.Item2, succs);

                r.Add(node.Item1, t0);
            }
            foreach (var elem in graph.rep)
            {
                r.Add(elem.Key, elem.Value);
            }

            return new FGraph<A, B> { rep = r };
        }

        public static FGraph<A, B> insLabEdges<A, B>(IReadOnlyList<Tuple<int, int, B>> labEdges, FGraph<A, B> graph)
        {
            var newpre = new Dictionary<int, List<Tuple<int, B>>>();
            var newsuc = new Dictionary<int, List<Tuple<int, B>>>();

            foreach (var edge in labEdges)
            {
                if (!newpre.ContainsKey(edge.Item2))
                    newpre.Add(edge.Item2, new List<Tuple<int, B>>());

                newpre[edge.Item2].Add(new Tuple<int, B>(edge.Item1, edge.Item3));

                if (!newsuc.ContainsKey(edge.Item1))
                    newsuc.Add(edge.Item1, new List<Tuple<int, B>>());

                newsuc[edge.Item1].Add(new Tuple<int, B>(edge.Item2, edge.Item3));
            }

            var r = new SortedDictionary<int,
                Tuple<IReadOnlyDictionary<int, B>, A, IReadOnlyDictionary<int, B>>>();

            foreach (var elem in graph.rep)
            {
                var newvalue = elem.Value;

                if (newpre.ContainsKey(elem.Key))
                {
                    var tpre = new SortedDictionary<int, B>();
                    foreach (var t in newpre[elem.Key])
                        tpre.Add(t.Item1, t.Item2);
                    foreach (var t in newvalue.Item1)
                    {
                        tpre.Add(t.Key, t.Value);
                    }
                    var lbl = newvalue.Item2;
                    var tsuc = newvalue.Item3;
                    newvalue = new Tuple<IReadOnlyDictionary<int, B>, A, IReadOnlyDictionary<int, B>>
                    (tpre, lbl, tsuc);
                }
                if (newsuc.ContainsKey(elem.Key))
                {
                    var tsuc = new SortedDictionary<int, B>();
                    foreach (var t in newsuc[elem.Key])
                        tsuc.Add(t.Item1, t.Item2);
                    foreach (var t in newvalue.Item3)
                    {
                        tsuc.Add(t.Key, t.Value);
                    }
                    var lbl = newvalue.Item2;
                    var tpre = newvalue.Item1;
                    newvalue = new Tuple<IReadOnlyDictionary<int, B>, A, IReadOnlyDictionary<int, B>>
                    (tpre, lbl, tsuc);
                }
                r.Add(elem.Key, newvalue);
            }

            return new FGraph<A, B> { rep = r };
        }
        /*
        public static FGraph<A, B> embed<A, B>(
            Tuple<
                IReadOnlyList<Tuple<B, int>>,
                int, A,
                IReadOnlyList<Tuple<B, int>>> context
                , FGraph<A, B> graph)
        {
            var preds = new SortedDictionary<int, B>();
            foreach (var t in context.Item1)
                preds.Add(t.Item2, t.Item1);

            var succs = new SortedDictionary<int, B>();
            foreach (var t in context.Item4)
                succs.Add(t.Item2, t.Item1);

            var t0 = new Tuple<IReadOnlyDictionary<int, B>, A, IReadOnlyDictionary<int, B>>(
                preds, context.Item3, succs);

            var r = new SortedDictionary<int,
                Tuple<IReadOnlyDictionary<int, B>, A, IReadOnlyDictionary<int, B>>>();

            r.Add(context.Item2, t0);

            foreach (var elem in graph.rep)
            {
                var newvalue = elem.Value;

                if (succs.ContainsKey(elem.Key))
                {
                    var tpre = new SortedDictionary<int, B>();
                    tpre.Add(context.Item2, succs[elem.Key]);
                    foreach (var t in newvalue.Item1)
                    {
                        tpre.Add(t.Key, t.Value);
                    }
                    var lbl = newvalue.Item2;
                    var tsuc = newvalue.Item3;
                    newvalue = new Tuple<IReadOnlyDictionary<int, B>, A, IReadOnlyDictionary<int, B>>
                    (tpre, lbl, tsuc);
                }
                if (preds.ContainsKey(elem.Key))
                {
                    var tsuc = new SortedDictionary<int, B>();
                    tsuc.Add(context.Item2, preds[elem.Key]);
                    foreach (var t in newvalue.Item3)
                    {
                        tsuc.Add(t.Key, t.Value);
                    }
                    var lbl = newvalue.Item2;
                    var tpre = newvalue.Item1;
                    newvalue = new Tuple<IReadOnlyDictionary<int, B>, A, IReadOnlyDictionary<int, B>>
                    (tpre, lbl, tsuc);
                }
                r.Add(elem.Key, newvalue);
            }

            return new FGraph<A, B> { rep = r };
        }

        public static FGraph<FGraph<A, B>, FGraph<A, B>> prepareGlue<A, B>(FGraph<A, B> g)
        {
            var nl = new List<Tuple<int, FGraph<A, B>>>();
            var el = new List<Tuple<int, int, FGraph<A, B>>>();

            foreach (var node in g.labNodes)
            {
                var t = new List<Tuple<int, A>> { new Tuple<int, A>(node.Item1, node.Item2) };
                nl.Add(new Tuple<int, FGraph<A, B>>(node.Item1, insLabNodes(t, empty<A, B>())));
            }
            foreach (var node in g.labEdges)
            {
                var tn = new List<Tuple<int, A>> {
                    new Tuple<int, A>(node.Item1, g.rep[node.Item1].Item2),
                    new Tuple<int, A>(node.Item2, g.rep[node.Item2].Item2),
                };
                var te = new List<Tuple<int, int, B>> { new Tuple<int, int, B>(node.Item1, node.Item2, node.Item3) };
                el.Add(new Tuple<int, int, FGraph<A, B>>(node.Item1, node.Item2,
                    insLabEdges(te, insLabNodes(tn, empty<A, B>()))));
            }

            return insLabEdges(el, insLabNodes(nl, empty<FGraph<A, B>, FGraph<A, B>>()));
        }

        public static FGraph<A, B> union<A, B>(FGraph<A, B> g, FGraph<A, B> h)
        {
            var nd = new SortedDictionary<int, A>();
            var ed = new SortedDictionary<Tuple<int, int>, B>();

            foreach (var node in g.labNodes)
                nd[node.Item1] = node.Item2;

            foreach (var node in h.labNodes)
                nd[node.Item1] = node.Item2;

            foreach (var edge in g.labEdges)
                ed[new Tuple<int, int>(edge.Item1, edge.Item2)] = edge.Item3;

            foreach (var edge in h.labEdges)
                ed[new Tuple<int, int>(edge.Item1, edge.Item2)] = edge.Item3;

            var nl = new List<Tuple<int, A>>();
            var el = new List<Tuple<int, int, B>>();

            foreach (var node in nd)
                nl.Add(new Tuple<int, A>(node.Key, node.Value));

            foreach (var edge in ed)
                el.Add(new Tuple<int, int, B>(edge.Key.Item1, edge.Key.Item2, edge.Value));

            return insLabEdges(el, insLabNodes(nl, empty<A, B>()));
        }

        public static FGraph<FGraph<A, B>, FGraph<A, B>> glue<A, B>(int a, int b, int c, FGraph<FGraph<A, B>, FGraph<A, B>> g)
        {
            var nl = new List<Tuple<int, FGraph<A, B>>>();
            var el = new List<Tuple<int, int, FGraph<A, B>>>();

            FGraph<A, B> nlabel;

            if (g.rep[a].Item3.ContainsKey(b) && g.rep[b].Item3.ContainsKey(a))
                nlabel = union(g.rep[a].Item3[b], g.rep[b].Item3[a]);
            else if (g.rep[a].Item3.ContainsKey(b))
                nlabel = g.rep[a].Item3[b];
            else if (g.rep[b].Item3.ContainsKey(a))
                nlabel = g.rep[b].Item3[a];
            else
            {
                Debug.Assert(false);
                throw new Exception("fail");
            }

            nl.Add(new Tuple<int, FGraph<A, B>>(c, nlabel));

            foreach (var node in g.labNodes)
                if ((node.Item1 != a) && (node.Item1 != b))
                    nl.Add(node);

            var ed = new SortedDictionary<Tuple<int, int>, FGraph<A, B>>();

            foreach (var edge in g.labEdges)
                if ((edge.Item1 != a) && (edge.Item1 != b) && (edge.Item2 != a) && (edge.Item2 != b))
                    el.Add(edge);
                else
                {
                    var newedge = new Tuple<int, int>(edge.Item1, edge.Item2);

                    if (newedge.Item1 == a) newedge = new Tuple<int, int>(c, newedge.Item2);
                    if (newedge.Item2 == a) newedge = new Tuple<int, int>(newedge.Item1, c);

                    if (newedge.Item1 == b) newedge = new Tuple<int, int>(c, newedge.Item2);
                    if (newedge.Item2 == b) newedge = new Tuple<int, int>(newedge.Item1, c);

                    if (newedge.Item1 != newedge.Item2)
                    {
                        var backedge = new Tuple<int, int>(newedge.Item2, newedge.Item1);

                        if (ed.ContainsKey(newedge))
                        {
                            ed[newedge] = union(edge.Item3, ed[newedge]);
                        }
                        else
                        if (ed.ContainsKey(backedge))
                        {
                            ed[backedge] = union(edge.Item3, ed[backedge]);
                        }
                        else
                            ed.Add(newedge, union(edge.Item3, nlabel));
                    }
                }

            foreach (var edge in ed)
                el.Add(new Tuple<int, int, FGraph<A, B>>(edge.Key.Item1, edge.Key.Item2, edge.Value));

            return insLabEdges(el, insLabNodes(nl, empty<FGraph<A, B>, FGraph<A, B>>()));
        }
        */
        public static void labPrint<A, B>(FGraph<A, B> g)
        {
            foreach (var v in g.labNodes)
            {
                Console.WriteLine("{0} : '{1}'", v.Item1, v.Item2);
            }

            foreach (var e in g.labEdges)
            {
                Console.WriteLine("<{0},{1}> : '{2}'", e.Item1, e.Item2, e.Item3);
            }
        }

        public static IReadOnlyList<int> shortestPathTo<A, B>(int a, Func<int, bool> to, FGraph<A, B> g)
        {
            IReadOnlyList<int> r = null;

            // bfs

            var q = new Queue<Tuple<int, IReadOnlyList<int>>>();
            var visited = new HashSet<int>();
            q.Enqueue(new Tuple<int, IReadOnlyList<int>>(a, new List<int> { a }));

            while (q.Count > 0)
            {
                var qe = q.Dequeue();

                if (visited.Contains(qe.Item1)) continue;

                if (to(qe.Item1))
                {
                    r = qe.Item2;
                    break;
                }

                visited.Add(qe.Item1);

                var list = g.neighbour[qe.Item1];

                foreach (var e in list)
                {
                    var newlist = new List<int>();
                    newlist.AddRange(qe.Item2);
                    newlist.Add(e);
                    q.Enqueue(new Tuple<int, IReadOnlyList<int>>(e, newlist));
                }
            }

            return r;
        }

        private static int doTheNet_anyOfType<A, B>(FGraph<A, B> g, Func<int, NodeType> nodeType, NodeType type)
        {
            int node = 0;
            bool bFound = false;

            foreach (var v in g.labNodes)
                if ((nodeType(v.Item1) & type) == type)
                {
                    node = v.Item1;
                    bFound = true;
                    break;
                }

            Debug.Assert(bFound);

            return node;
        }

        private static FGraph<A, B> doTheNet_removeNodesAndIsolatedNodes<A, B>(FGraph<A, B> g, IReadOnlyList<int> list)
        {
            var nr = new HashSet<int>(list);
            var nd = new HashSet<int>();
            var el = new List<Tuple<int, int, B>>();
            foreach (var l in g.labEdges)
                if ((!nr.Contains(l.Item1)) && (!nr.Contains(l.Item2)))
                {
                    el.Add(l);
                    nd.Add(l.Item1);
                    nd.Add(l.Item2);
                }

            var nl = new List<Tuple<int, A>>();

            foreach (var l in g.labNodes)
                if (nd.Contains(l.Item1))
                    nl.Add(l);

            return insLabEdges(el, insLabNodes(nl, empty<A, B>()));
        }

        private static FGraph<A, B> doTheNet_removeEdgesAndIsolatedNodesPath<A, B>(FGraph<A, B> g, IReadOnlyList<int> list)
        {
            var deg = new Dictionary<int, int>();

            var nd = new SortedDictionary<int, A>();
            var ed = new SortedDictionary<Tuple<int, int>, B>();

            foreach (var n in g.labNodes)
            {
                nd.Add(n.Item1, n.Item2);
                deg[n.Item1] = g.pre[n.Item1].Count + g.suc[n.Item1].Count;
            }
            foreach (var l in g.labEdges)
                ed.Add(new Tuple<int, int>(l.Item1, l.Item2), l.Item3);

            for (int i = 1; i < list.Count; i++)
            {
                var dir = new Tuple<int, int>(list[i - 1], list[i]);
                var rev = new Tuple<int, int>(list[i], list[i - 1]);

                if (ed.ContainsKey(dir))
                {
                    ed.Remove(dir);
                    deg[dir.Item1]--;
                    if (deg[dir.Item1] == 0)
                        nd.Remove(dir.Item1);
                    deg[dir.Item2]--;
                    if (deg[dir.Item2] == 0)
                        nd.Remove(dir.Item2);
                }

                if (ed.ContainsKey(rev))
                {
                    ed.Remove(rev);
                    deg[rev.Item1]--;
                    if (deg[rev.Item1] == 0)
                        nd.Remove(rev.Item1);
                    deg[rev.Item2]--;
                    if (deg[rev.Item2] == 0)
                        nd.Remove(rev.Item2);
                }
            }

            var nl = new List<Tuple<int, A>>();
            var el = new List<Tuple<int, int, B>>();

            foreach (var n in nd)
                nl.Add(new Tuple<int, A>(n.Key, n.Value));

            foreach (var l in ed)
                el.Add(new Tuple<int, int, B>(l.Key.Item1, l.Key.Item2, l.Value));

            return insLabEdges(el, insLabNodes(nl, empty<A, B>()));
        }

        public static FGraph<A, B> subGraphByNodes<A, B>(IReadOnlyList<int> nodes, FGraph<A, B> g)
        {
            var nd = new HashSet<int>(nodes);

            var el = new List<Tuple<int, int, B>>();
            var nl = new List<Tuple<int, A>>();

            foreach (var n in nd)
                nl.Add(new Tuple<int, A>(n, g.rep[n].Item2));

            foreach (var e in g.labEdges)
            {
                if (nd.Contains(e.Item1) && nd.Contains(e.Item2))
                    el.Add(new Tuple<int, int, B>(e.Item1, e.Item2, e.Item3));
            }

            return insLabEdges(el, insLabNodes(nl, empty<A, B>()));
        }


        public static FGraph<A, B> rebuildByOrder<A, B>(IReadOnlyList<int> order, Func<int, int, B, B> flipEdge, FGraph<A, B> g)
        {
            var el = new List<Tuple<int, int, B>>();

            var tmp = new Dictionary<int, Dictionary<int, B>>();

            var order1 = new List<int>(order);

            foreach (var e in g.labEdges)
            {
                int u = e.Item1;
                int v = e.Item2;
                B l = e.Item3;

                if (order1.IndexOf(u) > order1.IndexOf(v))
                {
                    l = flipEdge(u, v, e.Item3);
                    u = e.Item2;
                    v = e.Item1;
                }

                if (!tmp.ContainsKey(u))
                    tmp.Add(u, new Dictionary<int, B>());

                tmp[u][v] = l;
            }

            foreach (var u in order)
                if (tmp.ContainsKey(u))
                {
                    foreach (var v in order)
                        if (tmp[u].ContainsKey(v))
                        {
                            el.Add(new Tuple<int, int, B>(u, v, tmp[u][v]));
                        }
                }

            return insLabEdges(el, insLabNodes(g.labNodes, empty<A, B>()));
        }

        public static IReadOnlyList<IReadOnlyList<int>> CC<A, B>(FGraph<A, B> g)
        {
            var cc = new List<List<int>>();

            int num = 0;
            var WGN = new Dictionary<int, int>();
            var S = new Stack<int>();
            foreach (var vv in g.nodes)
                if (!WGN.ContainsKey(vv))
                {
                    var open = new Stack<Tuple<int, int, int, List<int>>>();

                    int v = vv;
                    int p = -1;
                    num++; WGN[v] = num;
                    List<int> l = new List<int>(g.neighbour[v]);

                    while (true)
                    {
                        while (l.Count > 0)
                        {
                            int u = l[0]; l.RemoveAt(0);

                            if (WGN.ContainsKey(u))
                            {
                            }
                            else
                            {
                                S.Push(u);

                                open.Push(new Tuple<int, int, int, List<int>>(u, v, p, l));
                                v = u;
                                p = v;
                                num++; WGN[v] = num;
                                l = new List<int>(g.neighbour[v]);
                            }
                        }

                        if (open.Count > 0)
                        {
                            int u;
                            int oldv;

                            {
                                var tmpt = open.Pop();
                                oldv = v;

                                u = tmpt.Item1;
                                v = tmpt.Item2;
                                p = tmpt.Item3;
                                l = tmpt.Item4;
                            }

                            Debug.Assert(u == oldv);

                        }
                        else
                        {
                            {
                                var tmp = new List<int>();
                                tmp.Add(vv);
                                while (S.Count > 0)
                                {
                                    var e = S.Pop();
                                    tmp.Add(e);
                                }
                                cc.Add(tmp);
                            }
                            break;
                        }
                    }
                }

            return cc;
        }

        public static Tuple<IReadOnlyList<IReadOnlyList<int>>, IReadOnlyList<int>> BCC<A, B>(FGraph<A, B> g)
        {
            var bcc = new List<List<int>>();
            var ap = new List<int>();

            int num = 0;
            var WGN = new Dictionary<int, int>();
            var L = new Dictionary<int, int>();
            var S = new Stack<int>();

            foreach (var vv in g.nodes)
                if (!WGN.ContainsKey(vv))
                {
                    var open = new Stack<Tuple<int, int, int, List<int>>>();

                    int v = vv;
                    int p = -1;
                    num++; WGN[v] = num; L[v] = WGN[v];
                    List<int> l = new List<int>(g.neighbour[v]);

                    while (true)
                    {
                        while (l.Count > 0)
                        {
                            int u = l[0]; l.RemoveAt(0);

                            if (WGN.ContainsKey(u))
                            {
                                if ((u != p) && (WGN[u] < WGN[v]))
                                {
                                    if (L[v] > WGN[u]) L[v] = WGN[u];
                                }
                            }
                            else
                            {
                                S.Push(u);

                                open.Push(new Tuple<int, int, int, List<int>>(u, v, p, l));
                                v = u;
                                p = v;
                                num++; WGN[v] = num; L[v] = WGN[v];
                                l = new List<int>(g.neighbour[v]);
                            }
                        }

                        if (open.Count > 0)
                        {
                            int u;
                            int oldv;

                            {
                                var tmpt = open.Pop();
                                oldv = v;

                                u = tmpt.Item1;
                                v = tmpt.Item2;
                                p = tmpt.Item3;
                                l = tmpt.Item4;
                            }

                            Debug.Assert(u == oldv);

                            int ppp = v;
                            int vvv = u;

                            {
                                if (L[ppp] > L[vvv]) L[ppp] = L[vvv];
                                if (L[vvv] >= WGN[ppp])
                                {
                                    var tmp = new List<int>();
                                    tmp.Add(ppp);
                                    if (ppp != vv)
                                        ap.Add(ppp);

                                    while (true)
                                    {
                                        var e = S.Pop();
                                        tmp.Add(e);
                                        if (e == vvv) break;
                                    }
                                    bcc.Add(tmp);
                                }
                            }
                        }
                        else
                            break;
                    }

                    if ((ap.Count + 1) < bcc.Count)
                        ap.Add(vv);
                }

            return new Tuple<IReadOnlyList<IReadOnlyList<int>>, IReadOnlyList<int>>(bcc, new List<int>(new HashSet<int>(ap)));
        }

        public static IReadOnlyList<int> FindNetOrderInBlock<A, B>(FGraph<A, B> g, Func<int, NodeType> nodeType)
        {
            int theIn = doTheNet_anyOfType(g, nodeType, NodeType.In);
            int theOut = doTheNet_anyOfType(g, nodeType, NodeType.Out);

            var dst = new HashSet<int>();

            FGraph<A, B> tg = g;

            var path = new List<int>();

            path.AddRange(shortestPathTo(theIn, (a) => (a == theOut), tg));

            IReadOnlyList<int> lastpath = path;

            while (true)
            {
                tg = doTheNet_removeEdgesAndIsolatedNodesPath(tg, lastpath);

                foreach (var n in lastpath)
                    dst.Add(n);

                var toRemove = new HashSet<int>();

                foreach (var n in dst)
                    if (!tg.rep.ContainsKey(n))
                        toRemove.Add(n);

                foreach (var n in toRemove)
                    dst.Remove(n);

                int from = 0;
                bool bFound = false;

                foreach (var n in path)
                    if (dst.Contains(n))
                    {
                        from = n;
                        bFound = true;
                        break;
                    }

                if (!bFound) break;

                lastpath = shortestPathTo(from, (a) => (from != a) && dst.Contains(a), tg);

                if (lastpath.Count < 2) break;

                if (lastpath.Count > 2)
                {
                    var to = lastpath[lastpath.Count - 1];

                    int brk = path.IndexOf(to);
                    Debug.Assert(brk > 0);

                    var tmppath = new List<int>();

                    tmppath.AddRange(lastpath);

                    tmppath.RemoveAt(lastpath.Count - 1);
                    tmppath.RemoveAt(0);

                    path.InsertRange(brk, tmppath);
                }
            }

            return path;
        }

        class Unit
        {
            public override string ToString() => "";
        }

        public static IReadOnlyList<int> FindNetOrderInComponent<A, B>(FGraph<A, B> g, Func<A, NodeType> nodeType)
        {
            var hap = new HashSet<int>();

            var r = FGraph.BCC(g);

            for (int i = 0; i < r.Item2.Count; i++)
            {
                Debug.Assert(nodeType(g.rep[r.Item2[i]].Item2) == NodeType.Neutral);

                hap.Add(r.Item2[i]);
            }

            // assume no Ins or Outs in real bcc
            // assume all Ins and Outs must be leafs
            // so ap not Ins or Outs 
            for (int i = 0; i < r.Item1.Count; i++)
            {
                if (r.Item1[i].Count > 2)
                {
                    for (int j = 0; j < r.Item1[i].Count; j++)
                    {
                        Debug.Assert(nodeType(g.rep[r.Item1[i][j]].Item2) == NodeType.Neutral);
                    }
                }
            }

            int realBccCount = 0;

            for (int i = 0; i < r.Item1.Count; i++)
                if (r.Item1[i].Count > 2)
                    realBccCount++;

            var nl = new List<Tuple<int, Unit>>();
            var el = new List<Tuple<int, int, Unit>>();

            var nd = new Dictionary<int, NodeType>();
            var unit = new Unit();
            var blocks = new Dictionary<int, Tuple<HashSet<int>, FGraph<A, B>>>();
            var blocksDone = new HashSet<int>();

            int nc = g.nodes.Max() + 1;
            int pmi = 0;

            for (int i = 0; i < r.Item1.Count; i++)
                if (r.Item1[i].Count > 2)
                {
                    var hs = new HashSet<int>();
                    nd[nc + pmi] = NodeType.Neutral;

                    for (int j = 0; j < r.Item1[i].Count; j++)
                        if (hap.Contains(r.Item1[i][j]))
                        {
                            hs.Add(r.Item1[i][j]);

                            nd[r.Item1[i][j]] = nodeType(g.rep[r.Item1[i][j]].Item2);
                            el.Add(new Tuple<int, int, Unit>(r.Item1[i][j], pmi + nc, unit));
                            // add edge r[i][j], pmi + nc
                        }
                    var sub = FGraph.subGraphByNodes(r.Item1[i], g);
                    blocks[nc + pmi] = new Tuple<HashSet<int>, FGraph<A, B>>(hs, sub);
                    pmi++;
                }
                else
                {
                    nd[r.Item1[i][0]] = nodeType(g.rep[r.Item1[i][0]].Item2);
                    nd[r.Item1[i][1]] = nodeType(g.rep[r.Item1[i][1]].Item2);
                    el.Add(new Tuple<int, int, Unit>(r.Item1[i][0], r.Item1[i][1], unit));
                    // add edge r[i][0], r[i][1]
                }


            foreach (var n in nd)
                nl.Add(new Tuple<int, Unit>(n.Key, unit));

            var lt = new List<FGraph<Unit, Unit>>{
                insLabEdges(el,insLabNodes(nl,FGraph.empty<Unit,Unit>()))
                };

            var pathset = new HashSet<List<int>>();

            while (lt.Count > 0)
            {
                var t = lt[0]; lt.RemoveAt(0);

                // we need additional test here if tree has blocks whitch isdDone;

                {
                    var splitPoints = new List<int>();
                    foreach (var n in t.nodes)
                        if (blocksDone.Contains(n))
                            splitPoints.Add(n);

                    if (splitPoints.Count > 0)
                    {

                        t = doTheNet_removeNodesAndIsolatedNodes(t, splitPoints);

                        foreach (var n in splitPoints)
                            nd[n] = NodeType.InOut;

                        var cc = CC(t);

                        foreach (var c in cc)
                            lt.Add(subGraphByNodes(c, t));
                        continue;
                    }

                }

                List<int> lastpath;

                {
                    int theIn = 0;
                    bool bFound = false;

                    foreach (var v in t.labNodes)
                        if (nd[v.Item1] == NodeType.In)
                        {
                            theIn = v.Item1;
                            bFound = true;
                            break;
                        }

                    if (!bFound)
                    {
                        foreach (var v in t.labNodes)
                            if (nd[v.Item1] == NodeType.InOut)
                            {
                                theIn = v.Item1;
                                bFound = true;
                                break;
                            }
                    }

                    if (!bFound)
                    {
                        foreach (var v in t.labNodes)
                            if (t.neighbour[v.Item1].Count == 1)
                                if (nd[v.Item1] == NodeType.Neutral)
                                {
                                    theIn = v.Item1;
                                    bFound = true;
                                    break;
                                }
                    }

                    Debug.Assert(bFound);

                    int theOut = 0;
                    bFound = false;

                    foreach (var v in t.labNodes)
                        if (v.Item1 != theIn)
                            if (nd[v.Item1] == NodeType.Out)
                            {
                                theOut = v.Item1;
                                bFound = true;
                                break;
                            }

                    if (!bFound)
                    {
                        foreach (var v in t.labNodes)
                            if (v.Item1 != theIn)
                                if (nd[v.Item1] == NodeType.InOut)
                                {
                                    theOut = v.Item1;
                                    bFound = true;
                                    break;
                                }
                    }

                    if (!bFound)
                    {
                        foreach (var v in t.labNodes)
                            if (v.Item1 != theIn)
                                if (t.neighbour[v.Item1].Count == 1)
                                    if (nd[v.Item1] == NodeType.Neutral)
                                    {
                                        theOut = v.Item1;
                                        bFound = true;
                                        break;
                                    }
                    }

                    Debug.Assert(bFound);

                    lastpath = new List<int>(shortestPathTo(theIn, (a) => a == theOut, t));
                }

                {
                    t = doTheNet_removeEdgesAndIsolatedNodesPath(t, lastpath);

                    foreach (var n in lastpath)
                        nd[n] = NodeType.InOut;

                    var cc = CC(t);

                    foreach (var c in cc)
                        lt.Add(subGraphByNodes(c, t));
                }

                var pathtoedit = new List<int>(lastpath);

                foreach (var bk in lastpath)
                    if (blocksDone.Contains(bk))
                        pathtoedit.Remove(bk);

                foreach (var bk in lastpath)
                {
                    if (!blocks.ContainsKey(bk)) continue;
                    if (blocksDone.Contains(bk)) continue;

                    var bv = blocks[bk];
                    blocksDone.Add(bk);

                    int idx = pathtoedit.IndexOf(bk);

                    Debug.Assert(idx >= 0);

                    int theIn = pathtoedit[idx - 1];
                    int theOut = pathtoedit[idx + 1];

                    Debug.Assert(bv.Item1.Contains(theIn));
                    Debug.Assert(bv.Item1.Contains(theOut));

                    var lpath = new List<int>(FindNetOrderInBlock(bv.Item2,
                        (v) => (v == theIn) ? NodeType.In : ((v == theOut) ? NodeType.Out : NodeType.Neutral)));


                    var savepath = new List<int>(pathtoedit);
                    var savelastpath = new List<int>(lpath);


                    lpath.RemoveAt(lpath.Count - 1);
                    lpath.RemoveAt(0);
                    pathtoedit.RemoveAt(idx);
                    pathtoedit.InsertRange(idx, lpath);

                    if (pathtoedit.Count != (new HashSet<int>(pathtoedit)).Count)
                    {
                        Debug.Assert(false);
                    }

                }

                if (pathset.Count == 0) pathset.Add(pathtoedit);
                else
                {
                    var pathtoscan = new List<int>(pathtoedit);

                    foreach (var n in pathtoscan)
                    {
                        if (!hap.Contains(n)) continue;

                        var pathsettoscan = new HashSet<List<int>>(pathset);
                        foreach (var p in pathsettoscan)
                        {
                            if (!p.Contains(n)) continue;

                            int idx = pathtoedit.IndexOf(n);

                            pathtoedit.RemoveAt(idx);
                            pathtoedit.InsertRange(idx, p);
                            pathset.Remove(p);
                        }
                    }
                    if (pathtoedit.Count != (new HashSet<int>(pathtoedit)).Count)
                    {
                        Debug.Assert(false);
                    }
                    pathset.Add(pathtoedit);
                }
            }

            Debug.Assert(pathset.Count == 1);

            return pathset.First();
        }

        public static IReadOnlyList<int> topOrder<A, B>(FGraph<A, B> g)
        {
            var order = new List<int>();
            var visited = new Dictionary<int, int>();
            int count = 0;

            foreach (int uu in g.nodes)
            {
                if (!visited.ContainsKey(uu))
                {
                    int u = uu;
                    int v;

                    var open = new Stack<Tuple<int, IEnumerator<int>>>();
                    count++;
                    visited[u] = count;

                    open.Push(new Tuple<int, IEnumerator<int>>(u, g.suc[u].GetEnumerator()));

                    while (open.Count > 0)
                    {
                        var v2i = open.Pop();

                        u = v2i.Item1;

                        while (v2i.Item2.MoveNext())
                        {
                            v = v2i.Item2.Current;

                            if (!visited.ContainsKey(v))
                            {
                                open.Push(v2i);
                                u = v;
                                count++;
                                visited[u] = count;
                                v2i = new Tuple<int, IEnumerator<int>>(u, g.suc[u].GetEnumerator());
                            }
                        }

                        //here u
                        order.Add(u);
                    }
                }
            }
            order.Reverse();
            return order;
        }
    }

    public static class GaussianElimination
    {
        public static double[] Solve(double[][] sA, double[] sB)
        {
            int n = sB.Length;
            double[] x = new Double[n];
            int[] p = new int[n];
            for (int i = 0; i < n; i++) p[i] = i;

            double[][] A = new Double[n][];
            double[] B = new Double[n];

            {
                for (int j = 0; j < n; j++)
                {
                    B[j] = sB[j];
                    A[j] = new Double[n];
                    for (int i = 0; i < n; i++)
                    {
                        A[j][i] = sA[j][i];
                    }
                }
            }

            int k;

            for (k = 0; k < n; k++)
            {
                double m;
                int im, jm;
                im = k; jm = k; m = A[im][p[jm]];


                for (int i = k; i < n; i++)
                    for (int j = k; j < k + 1/*n*/; j++) // fixed ?
                    {
                        if (Math.Abs(m) < Math.Abs(A[i][p[j]]))
                        {
                            im = i; jm = j; m = A[im][p[jm]];
                        }
                    }

                if (m == 0)
                {
                    throw new Exception("Gauss Ellimination Failed");
                }

                if (im != k)
                {
                    {
                        double[] tmp;
                        tmp = A[im]; A[im] = A[k]; A[k] = tmp;
                    }
                    {
                        double tmp;
                        tmp = B[im]; B[im] = B[k]; B[k] = tmp;
                    }
                }

                if (jm != k)
                {
                    int tmp;
                    tmp = p[jm]; p[jm] = p[k]; p[k] = tmp;
                }

                for (int j = k; j < n; j++)
                {
                    A[k][p[j]] /= m;
                }

                B[k] /= m;

                for (int i = 0; i < n; i++)
                    if (i != k)
                    {
                        double Aik = A[i][p[k]];

                        for (int j = k; j < n; j++)
                        {
                            A[i][p[j]] -= Aik * A[k][p[j]];
                        }

                        B[i] -= Aik * B[k];
                    }
            }

            for (int i = 0; i < n; i++)
                x[p[i]] = B[i];

            return x;
        }
    }

    public static class Flash
    {
        public const double Epsilon = 1e-3;

        static public int fugcnt;
        static public int funcnt;
        public static void Norm1(Double[] v)
        {
            double d = 0.0;

            for (int i = 0; i < v.Length; i++)
            {
                d += v[i];
            }
            for (int i = 0; i < v.Length; i++)
            {
                v[i] /= d;
            }
        }

        public static Double[] CubicSolver(Double a, Double b, Double c, Double d)
        {
            var roots = CubicSolver1(a, b, c, d);

            foreach (var r in roots)
            {
                double z = a * r * r * r + b * r * r + c * r + d;

                if (Math.Abs(z) > 1e-3)
                {
                    Debug.Assert(false);
                }
            }

            return roots;
        }

        public static Double[] CubicSolver1(Double a, Double b, Double c, Double d)
        {
            Double[] r;

            Debug.Assert(a != 0);

            Double p;
            Double q;

            Double D;

            p = (3.0 * a * c - b * b) / (3.0 * a * a);
            q = (2.0 * b * b * b - 9.0 * a * b * c + 27.0 * a * a * d) / (27.0 * a * a * a);

            D = q * q + 4.0 * p * p * p / 27.0;

            if (Math.Abs(D) < 1e-14)
            {
                Double a0 = b / a;
                Double tmp;
                tmp = q / 2;
                if (tmp >= 0)
                    tmp = Math.Exp(Math.Log(tmp) / 3.0);
                else if (tmp < 0)
                    tmp = -Math.Exp(Math.Log(-tmp) / 3.0);

                r = new double[3];

                r[0] = -2.0 * tmp - a0 / 3.0;
                r[1] = tmp - a0 / 3.0;
                r[2] = tmp - a0 / 3.0;
                Array.Sort(r);
            }
            else if (D > 0)
            {
                double tmp1;
                tmp1 = (Math.Sqrt(D) - q) / 2;
                if (tmp1 > 0)
                    tmp1 = Math.Exp(Math.Log(tmp1) / 3.0);
                else if (tmp1 < 0)
                    tmp1 = -Math.Exp(Math.Log(-tmp1) / 3.0);

                double tmp2;
                tmp2 = (-Math.Sqrt(D) - q) / 2;
                if (tmp2 > 0)
                    tmp2 = Math.Exp(Math.Log(tmp2) / 3.0);
                else if (tmp2 < 0)
                    tmp2 = -Math.Exp(Math.Log(-tmp2) / 3.0);

                double tmp3 = tmp1 + tmp2;
                r = new double[1];

                r[0] = tmp3 - b / (3.0 * a);
            }
            else
            {
                Double Q;
                Double R;
                Double S;

                Double a0 = b / a;
                Double b0 = c / a;
                Double c0 = d / a;

                Q = -p / 3;
                //Q = (a0 * a0 - 3.0 * b0) / 9.0;
                R = q / 2;
                //R = (2.0 * a0 * a0 * a0 - 9.0 * a0 * b0 + 27.0 * c0) / 54.0;
                S = -D / 4;
                //S = Q * Q * Q - R * R;
                Debug.Assert(S > 0);

                double phi, sr;

                sr = Math.Sqrt(Q);

                phi = Math.Acos(R / (sr * sr * sr)) / 3.0;

                r = new double[3];

                r[0] = -2.0 * sr * Math.Cos(phi) - a0 / 3.0;
                r[1] = -2.0 * sr * Math.Cos(phi + 2.0 * Math.PI / 3.0) - a0 / 3.0;
                r[2] = -2.0 * sr * Math.Cos(phi - 2.0 * Math.PI / 3.0) - a0 / 3.0;
                Array.Sort(r);
            }
            return r;
        }

        public static double[] CalcFug(FlowDiagram fd, double[] x, Double P, Double T, bool bVapor, bool bPlusCalc,
            out double Mw, out double V, out double H)
        {
            Double R = 8.31446261815324;

            fugcnt++;

            var k = new Double[fd.Components.Count][];
            var SRK_a = new Double[fd.Components.Count];
            var SRK_b = new Double[fd.Components.Count];

            {
                var s = (fd.Systems[0] as CalculationSystem);
                for (int i = 0; i < k.Count(); i++)
                {
                    k[i] = new Double[fd.Components.Count];
                }

                for (int i = 0; i < (k.Count() - 1); i++)
                {
                    Debug.Assert(s.SRKKIJ[i].Count() == (k.Count() - 1 - i));
                    for (int j = 0; j < (k.Count() - 1 - i); j++)
                    {
                        k[i][i + 1 + j] = s.SRKKIJ[i][j];
                        k[i + 1 + j][i] = k[i][i + 1 + j];
                    }
                }

                for (int i = 0; i < fd.Components.Count; i++)
                {
                    SRK_a[i] = (fd.Components[i].Tc + 273.15) * (fd.Components[i].Tc + 273.15)
                        * 0.42748 * R * R / (fd.Components[i].Pc * 1000.0);

                    SRK_b[i] = (fd.Components[i].Tc + 273.15)
                        * 0.08664 * R / (fd.Components[i].Pc * 1000.0);
                }

            }
            Double[] alpha = new Double[x.Length];
            Double[] c = new Double[x.Length];
            Double tmp;

            for (int i = 0; i < fd.Components.Count(); i++)
            {
                c[i] = (0.48 + 1.574 * fd.Components[i].Omega - 0.176 * fd.Components[i].Omega * fd.Components[i].Omega);
                tmp = 1 + c[i]
                    * (1 - Math.Sqrt((T + 273.15) / (fd.Components[i].Tc + 273.15)));
                alpha[i] = tmp * tmp;
            }

            int n = fd.Components.Count();

            tmp = 0; 
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                {
                    tmp += Math.Sqrt(SRK_a[i] * alpha[i]) * x[i] *
                        Math.Sqrt(SRK_a[j] * alpha[j]) * x[j] * (1.0 - k[i][j]);
                }
            Double aaml;
            Double Al;
            aaml = tmp;
            Al = tmp * P / (R * R * (T + 273.15) * (T + 273.15));

            tmp = 0;
            for (int i = 0; i < fd.Components.Count(); i++)
                tmp += SRK_b[i] * x[i];

            Double bml;
            Double Bl;
            bml = tmp;
            Bl = tmp * P / (R * (T + 273.15));

            Double Zl;
            var roots = CubicSolver(1.0, -1.0, Al - Bl - Bl * Bl, -Al * Bl);
            if(bVapor)
                Zl = roots.Last();
            else
                Zl = roots.First();

            Double tmp2;

            Double[] fi_l = new Double[fd.Components.Count()];

            for (int i = 0; i < fd.Components.Count(); i++)
            {
                tmp = 0;
                for (int j = 0; j < fd.Components.Count(); j++)
                    tmp += Math.Sqrt(SRK_a[i] * alpha[i]) *
                        Math.Sqrt(SRK_a[j] * alpha[j]) * x[j] * (1.0 - k[i][j]);

                tmp2 = (Zl - 1.0) * SRK_b[i] / bml - Math.Log(Zl - Bl) +
                    -(Al / Bl) * (2.0 * tmp / aaml - SRK_b[i] / bml) * Math.Log(1 + Bl / Zl);

                fi_l[i] = Math.Exp(tmp2);
            }

            if (!bPlusCalc)
            {
                Mw = 0.0; V = 0.0; H = 0.0;

                return fi_l;
            }

            // Mw and Vcor starts here

            double Vl;
            Vl = Zl * R * (T + 273.15) / P;

            Double MMml = 0.0;
            double Vcorl;

            Vcorl = Vl;

            for (int i = 0; i < fd.Components.Count(); i++)
            {
                MMml += x[i] * fd.Components[i].Mw;
                Vcorl -= x[i] * 0.40768 * R * (0.29441 - fd.Components[i].ZRA) * (273.15 + fd.Components[i].Tc) / (fd.Components[i].Pc * 1000);
            }

            Mw = MMml; V = Vcorl; 
            // H starts here

            Double aux1 = -R / 2 * Math.Sqrt(0.42748 / (T + 273.15));

            Double aux2 = 0.0;

            for (int i = 0; i < fd.Components.Count(); i++)
                for (int j = 0; j < fd.Components.Count(); j++)
                {
                    aux2 += x[i] * x[j] * (1.0 - k[i][j]) *
                        (c[j] * Math.Sqrt((SRK_a[i] * alpha[i] / 1000.0) * (fd.Components[j].Tc + 273.15) / fd.Components[j].Pc)
                        + c[i] * Math.Sqrt((SRK_a[j] * alpha[j] / 1000.0) * (fd.Components[i].Tc + 273.15) / fd.Components[i].Pc)) / 1000.0;
                }

            Double dadT = aux1 * aux2;

            Double uu, ww;
            uu = 1;
            ww = 0;

            Double DAres = aaml / (bml * Math.Sqrt(uu * uu - 4 * ww)) * Math.Log(
                    (2 * Zl + Bl * (uu - Math.Sqrt(uu * uu - 4 * ww))) /
                    (2 * Zl + Bl * (uu + Math.Sqrt(uu * uu - 4 * ww))))
                    - R * (T + 273.15) * Math.Log((Zl - Bl) / Zl) - R * (T + 273.15) * Math.Log(Zl);

            Double DSres = R * Math.Log((Zl - Bl) / Zl) + R * Math.Log(Zl)
                - 1 / (Math.Sqrt(uu * uu - 4 * ww) * bml / 1000.0) * dadT * Math.Log((2 * Zl + Bl * (uu - Math.Sqrt(uu * uu - 4 * ww))) / (2 * Zl + Bl * (uu + Math.Sqrt(uu * uu - 4 * ww))));

            Double DHresl = DAres + (T + 273.15) * (DSres) + R * (T + 273.15) * (Zl - 1);

            double Tk = (T + 273.15);
            double hpgl;
            hpgl = 0;
            for (int i = 0; i < fd.Components.Count(); i++)
            {
                tmp = fd.Components[i].Higa + fd.Components[i].Higb * Tk + fd.Components[i].Higc * Tk * Tk
                + fd.Components[i].Higd * Tk * Tk * Tk + fd.Components[i].Hige * Tk * Tk * Tk * Tk + fd.Components[i].Higf * Tk * Tk * Tk * Tk * Tk;

                hpgl += tmp * x[i];
            }

            H = hpgl + DHresl / 1000.0;

            Debug.Assert(!Double.IsNaN(H));

            return fi_l;
        }

        public static double CalcFun(Double[] z, double[] K, double psi, out double df)
        {
            double f = 0.0;
            df = 0.0;
            double tmp;

            funcnt++;

            for (int i = 0; i < z.Length; i++)
            {
                tmp = (K[i] - 1.0) / (1.0 + psi * (K[i] - 1.0));
                f += z[i]*tmp;
                df += -z[i] * tmp * tmp;
            }

            return f;
        }

        public static double CalcPsi(Double[] z, double[] K)
        {
            double a, b;
            double fa, fb;
            double f, psi, psi_old;
            double dfa, dfb, df;

            a = 0.0;
            b = 1.0;
            fa = CalcFun(z, K, a, out dfa);
            fb = CalcFun(z, K, b, out dfb);

            if (fa <= 0) return 0.0;
            if (fb >= 0) return 1.0;

            Debug.Assert((fa > 0.0) && (fb < 0.0));

            psi_old = a;
            psi = (a + b) / 2;

            while (Math.Abs(psi - psi_old) > Flash.Epsilon)
            {
                f = CalcFun(z, K, psi, out df);

                psi_old = psi;

                psi += -f / df;
            }

            return psi;
        }
        public static void FlashPT(FlowDiagram fd, Double[] z, Double P, Double T, 
            /*out*/ Double[] x, /*out*/ Double[] y, out Double psi, 
            bool bPlusCalc,
            out double Mwx, out double Vx, out double Hx,
            out double Mwy, out double Vy, out double Hy,
            bool bForceLiquid, bool bForceVapor)
        {
            double oldpsi;

            // need to reimplement 
            // better liquid only and vapor only determination

            var K = new Double[z.Length];
            for (int i = 0; i < z.Length; i++)
            {
                K[i] = (fd.Components[i].Pc * 1000.0 / P) * 
                    Math.Exp(5.373 * (1.0 + fd.Components[i].Omega) * (1.0 - fd.Components[i].Tc / T)) ;

                // dirty crash fix
                if (K[i] > 1e15) K[i] = 1e15;
                if (K[i] < 1e-15) K[i] = 1e-15;
            }

            oldpsi = 0;
            psi = 0.5;

            bool bForceVapor1 = true;

            for (int i = 0; i < z.Length; i++)
            {
                if ((z[i] > 0) && (T < fd.Components[i].Tc))
                {
                    bForceVapor1 = false;
                }
            }

            Mwx = 0.0;
            Vx = 0.0;
            Hx = 0.0;
            Mwy = 0.0;
            Vy = 0.0;
            Hy = 0.0;

            if (bForceVapor1|| bForceVapor)
            {
                psi = 1.0;
                for (int i = 0; i < z.Length; i++)
                {
                    x[i] = z[i];
                    y[i] = z[i];
                }

                if (bPlusCalc)
                {
                    CalcFug(fd, x, P, T, false, true, out Mwx, out Vx, out Hx);
                    CalcFug(fd, y, P, T, true, true, out Mwy, out Vy, out Hy);
                }
                return;
            }

            if (bForceLiquid)
            {
                psi = 0;
                for (int i = 0; i < z.Length; i++)
                {
                    x[i] = z[i];
                    y[i] = z[i];
                }

                if (bPlusCalc)
                {
                    CalcFug(fd, x, P, T, false, true, out Mwx, out Vx, out Hx);
                    CalcFug(fd, y, P, T, true, true, out Mwy, out Vy, out Hy);
                }
                return;
            }


            while (Math.Abs(oldpsi - psi) > Flash.Epsilon)
            {
                for (int i = 0; i < z.Length; i++)
                {
                    y[i] = z[i] * K[i] / ((K[i] - 1.0) * psi + 1.0);
                    x[i] = y[i] / K[i];
                }

                Norm1(x);
                Norm1(y);

                var fi_l = CalcFug(fd, x, P, T, false, false, out Mwx, out Vx, out Hx);
                var fi_v = CalcFug(fd, y, P, T, true, false, out Mwy, out Vy, out Hy);

                for (int i = 0; i < z.Length; i++)
                    K[i] = fi_l[i] / fi_v[i];

                bool bTrivial = true;

                for (int i = 0; i < z.Length; i++)
                {
                    if (Math.Abs(K[i]- 1.0)>= Flash.Epsilon*10)
                    {
                        bTrivial = false;
                        break;
                    }    
                }

                if (bTrivial)
                {
                    psi = 0.0;
                    for (int i = 0; i < z.Length; i++)
                    {
                        x[i] = z[i];
                        y[i] = z[i];
                    }
                    break;
                }

                oldpsi = psi;
                psi = CalcPsi(z, K);

                if ((oldpsi == 1.0) && (psi == 0.0))
                {
                    //Debug.Assert(false);
                    // dirty hack
                    psi = 0.0;
                    for (int i = 0; i < z.Length; i++)
                    {
                        x[i] = z[i];
                        y[i] = z[i];
                    }
                    break;
                }
            }

            if (bPlusCalc)
            {
                CalcFug(fd, x, P, T, false, true, out Mwx, out Vx, out Hx);
                CalcFug(fd, y, P, T, true, true, out Mwy, out Vy, out Hy);
            }
        }
        public static double FlashPH(FlowDiagram fd, Double[] z, Double P, Double H, Double T0,
            /*out*/ Double[] x, /*out*/ Double[] y, out Double psi,
            bool bPlusCalc,
            out double Mwx, out double Vx, out double Hx,
            out double Mwy, out double Vy, out double Hy,
            bool bForceLiquid, bool bForceVapor)
        {
            double T, dH;
            double Ta, Tb, dHa, dHb;

            // need reimplement
            // split in 3 cases:
            // liquid only - like now adjust temperature
            // vapor only - like now adjust temperature
            // liquid and vapor - adjust vapor fraction
            // mostly impotant for single component (temperature does not change while boiling)
            // in liquid only when pressure lowers temperature raises 
            // in vapor only when pressure lowers temperature lowers
            // so we need to know if liquid only state

            T = T0;

            //FlashPT(fd, z, P, T, x, y, out psi, true, out Mwx, out Vx, out Hx, out Mwy, out Vy, out Hy);
            //dH = Hx * (1.0 - psi) + Hy * psi - H;

            Ta = (T + 273.15) * 1.02 - 273.15;
            Tb = (T + 273.15) / 1.02 - 273.15;

            FlashPT(fd, z, P, Ta, x, y, out psi, true, out Mwx, out Vx, out Hx, out Mwy, out Vy, out Hy,
                bForceLiquid, bForceVapor);
            dHa = Hx * (1.0 - psi) + Hy * psi - H;

            FlashPT(fd, z, P, Tb, x, y, out psi, true, out Mwx, out Vx, out Hx, out Mwy, out Vy, out Hy,
                bForceLiquid, bForceVapor);
            dHb = Hx * (1.0 - psi) + Hy * psi - H;

            while (dHa < 0)
            {
                dHb = dHa;Tb = Ta;
                Ta = (Ta + 273.15) * 1.02 - 273.15;
                FlashPT(fd, z, P, Ta, x, y, out psi, true, out Mwx, out Vx, out Hx, out Mwy, out Vy, out Hy,
                bForceLiquid, bForceVapor);
                dHa = Hx * (1.0 - psi) + Hy * psi - H;
            }

            while (dHb > 0)
            {
                dHa = dHb; Ta = Tb;
                Tb = (Tb + 273.15) / 1.02 - 273.15;
                FlashPT(fd, z, P, Tb, x, y, out psi, true, out Mwx, out Vx, out Hx, out Mwy, out Vy, out Hy,
                bForceLiquid, bForceVapor);
                dHb = Hx * (1.0 - psi) + Hy * psi - H;
            }

            Debug.Assert((dHa > 0.0) && (dHb < 0.0));

            T = (Ta + Tb) / 2;

            while (Math.Abs(Ta - Tb) > Flash.Epsilon)
            {
                T = (Ta + Tb) / 2;

                FlashPT(fd, z, P, T, x, y, out psi, true, out Mwx, out Vx, out Hx, out Mwy, out Vy, out Hy,
                bForceLiquid, bForceVapor);
                dH = Hx * (1.0 - psi) + Hy * psi - H;

                if (dH < 0)
                {
                    Tb = T;
                    dHb = dH;
                }
                else
                {
                    Ta = T;
                    dHa = dH;
                }
            }

            return T;
        }
    }
}
