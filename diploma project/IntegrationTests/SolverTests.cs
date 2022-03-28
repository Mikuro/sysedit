using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using tanks.Models;
using tanks.Models.Solver;

using ICSharpCode.SharpZipLib.BZip2;

namespace tanks.ModelsTests
{
    [TestClass]
    public class SolverTests
    {
        static UnmanagedMemoryStream GetResourceStream(string resName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var strResources = assembly.GetName().Name + ".g.resources";
            var rStream = assembly.GetManifestResourceStream(strResources);
            var resourceReader = new System.Resources.ResourceReader(rStream);
            var items = resourceReader.OfType<System.Collections.DictionaryEntry>();
            var stream = items.First(x => (x.Key as string) == resName.ToLower()).Value;
            return (UnmanagedMemoryStream)stream;
        }

        static public void SerializeFlowDiagram(System.IO.Stream stream, Models.FlowDiagram obj)
        {
            XNamespace defns = "clr-namespace:tanks.Models;assembly=tanks.Models";
            XNamespace xns = "http://schemas.microsoft.com/winfx/2006/xaml";
            XNamespace sysns = "clr-namespace:System;assembly=mscorlib";

            var node = XMLUtil.FlowDiagram_to_node(obj, new XNamespace[3] { defns, xns, sysns });

            var doc = new XDocument(node);

            doc.Save(stream);
        }

        static public Models.FlowDiagram DeserializeFlowDiagram(System.IO.Stream stream)//, PropertyType types)
        {
            XmlDocument file = new XmlDocument();

            file.Load(stream);

            if (file.ChildNodes[0].NodeType == XmlNodeType.XmlDeclaration)
            {
                Assert.AreEqual(file.ChildNodes.Count, 2);

                return XMLUtil.CreateFromFlowDiagram_node(file.ChildNodes[1]);
            }
            else
            {
                Assert.AreEqual(file.ChildNodes.Count, 1);

                return XMLUtil.CreateFromFlowDiagram_node(file.ChildNodes[0]);
            }
        }

        // -------------------------

        static double LogAbs(double x)
        {
            double r = Math.Log10(Math.Abs(x) + 1e-20);

            return r;
        }

        static double[] matmulsub(double[][] A, double[] x, double[] b)
        {
            double[] r = new double[A.Length];

            for (int j = 0; j < r.Length; j++)
            {
                double s = 0;

                for (int i = 0; i < x.Length; i++)
                {
                    s += A[j][i] * x[i];
                }

                r[j] = s - b[j];
            }

            return r;
        }

        public void SolverTest(FlowDiagram fd)
        {
            SRKSolver.ProcessLinks(fd);
            Flash.fugcnt = 0;
            Flash.funcnt = 0;
            SRKSolver.SolveStatic(fd);
            DCSSolver.SolveInstrument(fd);
            DCSSolver.OneStep(fd);

            var ld = new Dictionary<string, List<ProcessUnit>>();

            foreach (var item in fd.Items)
            {
                var unit = item as tanks.Models.ProcessUnit;
                if (unit == null) continue;

                List<ProcessUnit> list;

                if (!ld.ContainsKey(unit.GetType().Name))
                {
                    list = new List<ProcessUnit>();
                    ld[unit.GetType().Name] = list;
                }
                else
                    list = ld[unit.GetType().Name];

                list.Add(unit);
            }
            foreach (var key in ld.Keys)
            {
                Console.WriteLine("{0}", key);
                switch (key)
                {
                    case "PressureFeed":
                        break;
                    case "HeatExchanger":
                        foreach (var unit in ld[key])
                        {
                            {
                                double dP = (unit as HeatExchanger).f1.P -
                                    (unit as HeatExchanger).p1.P;

                                double c = ((unit as HeatExchanger).PdHdes *
                                        (unit as HeatExchanger).f1.V *
                                        (unit as HeatExchanger).DHdes *
                                        (unit as HeatExchanger).f1.Mw) /
                                        ((unit as HeatExchanger).WHdes * (unit as HeatExchanger).WHdes);

                                double f = Math.Sqrt(Math.Abs(dP / c)) * Math.Sign(dP / c);

                                Console.WriteLine("{0} is {1}", unit.Id,
                                    LogAbs(((unit as HeatExchanger).f1.F - f)));
                            }
                            {
                                double dP = (unit as HeatExchanger).f2.P -
                                    (unit as HeatExchanger).p2.P;

                                double c = ((unit as HeatExchanger).PdLdes *
                                        (unit as HeatExchanger).f2.V *
                                        (unit as HeatExchanger).DLdes *
                                        (unit as HeatExchanger).f2.Mw) /
                                        ((unit as HeatExchanger).WLdes * (unit as HeatExchanger).WLdes);

                                double f = Math.Sqrt(Math.Abs(dP / c)) * Math.Sign(dP / c);

                                Console.WriteLine("{0} is {1}", unit.Id,
                                    LogAbs(((unit as HeatExchanger).f2.F - f)));
                            }
                            double LMTD;
                            double Th1 = (unit as HeatExchanger).f1.T;
                            double Th2 = (unit as HeatExchanger).p1.T;
                            double Tc1 = (unit as HeatExchanger).f2.T;
                            double Tc2 = (unit as HeatExchanger).p2.T;
                            double Hh1 = (unit as HeatExchanger).f1.HL * (unit as HeatExchanger).f1.RL
                                + (unit as HeatExchanger).f1.HV * (unit as HeatExchanger).f1.RV;
                            double Hh2 = (unit as HeatExchanger).p1.HL * (unit as HeatExchanger).p1.RL
                                + (unit as HeatExchanger).p1.HV * (unit as HeatExchanger).p1.RV;
                            double Hc1 = (unit as HeatExchanger).f2.HL * (unit as HeatExchanger).f2.RL
                                + (unit as HeatExchanger).f2.HV * (unit as HeatExchanger).f2.RV;
                            double Hc2 = (unit as HeatExchanger).p2.HL * (unit as HeatExchanger).p2.RL
                                + (unit as HeatExchanger).p2.HV * (unit as HeatExchanger).p2.RV;

                            if (((Th1 - Tc2) / (Th2 - Tc1)) == 1.0)
                                LMTD = ((Th1 - Tc2) + (Th2 - Tc1)) / 2;
                            else
                                LMTD = ((Th1 - Tc2) - (Th2 - Tc1)) / Math.Log((Th1 - Tc2) / (Th2 - Tc1));

                            double Qi = 3.6 * (unit as HeatExchanger).U * (unit as HeatExchanger).A * LMTD / 1000;
                            double Qih = (unit as HeatExchanger).f1.F * (Hh1 - Hh2);
                            double Qic = (unit as HeatExchanger).f2.F * (Hc2 - Hc1);
                            Console.WriteLine("{0} | {1} {2} {3}", unit.Id, LogAbs(Qi - Qih), LogAbs(Qih - Qic), LogAbs(Qi - Qic));
                        }
                        break;
                    case "Splitter":
                        foreach (var unit in ld[key])
                        {
                            double ftot = 0;

                            if ((unit as Splitter).p1 != null)
                                ftot += (unit as Splitter).p1.F;
                            if ((unit as Splitter).p2 != null)
                                ftot += (unit as Splitter).p2.F;
                            if ((unit as Splitter).p3 != null)
                                ftot += (unit as Splitter).p3.F;

                            Console.WriteLine("{0} | {1}", unit.Id,
                                LogAbs(((unit as Splitter).f1.F - ftot) /*/ (ftot + 1.0)*/));
                        }
                        break;
                    case "ControlValve":
                        foreach (var unit in ld[key])
                        {
                            double f;
                            if ((unit as ControlValve).pos < (unit as ControlValve).pos0)
                            {
                                f = 0;
                            }
                            else
                            {

                                double dP = (unit as ControlValve).f1.P -
                                    (unit as ControlValve).p1.P;

                                double cv = (unit as ControlValve).CV * Math.Pow(
                                    (unit as ControlValve).R, ((unit as ControlValve).pos - 1.0));

                                double c = 2.74 * cv *
                                    (unit as ControlValve).Cff / Math.Sqrt(
                                        (unit as ControlValve).f1.V *
                                        (unit as ControlValve).f1.Mw);

                                f = c * Math.Sqrt(Math.Abs(dP)) * Math.Sign(dP);
                            }

                            Console.WriteLine("{0} is {1}", unit.Id,
                                LogAbs(((unit as ControlValve).f1.F - f)));
                        }
                        break;
                    case "LiquidTank":
                        foreach (var unit in ld[key])
                        {
                            double tu = 0.0;
                            for (int i = 0; i < (unit as LiquidTank).u.Count; i++)
                            {
                                tu += (unit as LiquidTank).u[i] * fd.Components[i].Mw;
                            }

                            double dP = (9.80665E-3) * tu / (unit as LiquidTank).A;

                            Console.WriteLine("{0} is {1}", unit.Id,
                                LogAbs(((unit as LiquidTank).p1.P - (unit as LiquidTank).fe.P - dP) /*/ (dP + 1.0)*/));
                        }
                        break;
                    case "Pipe":
                        foreach (var unit in ld[key])
                        {
                            double dP = (9.80665E-3) * (unit as Pipe).Hdiff *
                                (unit as Pipe).f1.Mw / (unit as Pipe).f1.V;

                            Console.WriteLine("{0} is {1}", unit.Id,
                                LogAbs(((unit as Pipe).p1.P - (unit as Pipe).f1.P + dP) /*/ (dP + 1.0)*/));
                        }
                        break;
                    case "Mixer":
                        foreach (var unit in ld[key])
                        {
                            double ftot = 0;

                            if ((unit as Mixer).f1 != null)
                                ftot += (unit as Mixer).f1.F;
                            if ((unit as Mixer).f2 != null)
                                ftot += (unit as Mixer).f2.F;
                            if ((unit as Mixer).f3 != null)
                                ftot += (unit as Mixer).f3.F;

                            Console.WriteLine("{0} | {1}", unit.Id,
                                LogAbs(((unit as Mixer).p1.F - ftot) /*/ (ftot + 1.0)*/));
                        }
                        break;
                    case "Pump":
                        foreach (var unit in ld[key])
                        {
                            double alpha = ((unit as Pump).Hs - (unit as Pump).Hd) /
                                ((unit as Pump).Vd * (unit as Pump).Vd);

                            double H0 = (unit as Pump).pos * (unit as Pump).pos * (unit as Pump).Hs;

                            double V = (unit as Pump).f1.F * (unit as Pump).f1.V;
                            double dP = (9.80665E-3) * (H0 - alpha * V * V) *
                                (unit as Pump).f1.Mw / (unit as Pump).f1.V;

                            Console.WriteLine("{0} is {1}", unit.Id,
                                LogAbs(((unit as Pump).p1.P - (unit as Pump).f1.P - dP)/*/(dP+1.0)*/));
                        }
                        break;
                    case "PressureProduct":
                        break;
                }
            }

            foreach (var lnk in fd.Links)
            {
                tanks.Models.Stream str = lnk as tanks.Models.Stream;

                if (str == null) continue;

                Console.WriteLine("link {0} {1} P={2} F={3} T={4} RV={5} Mw={6} po={7}",
                str.From,
                str.To,
                str.P.ToString("0.0##", CultureInfo.InvariantCulture),
                str.F.ToString("0.0##", CultureInfo.InvariantCulture),
                str.T.ToString("0.0##", CultureInfo.InvariantCulture),
                str.RV.ToString("0.0##", CultureInfo.InvariantCulture),
                str.Mw.ToString("0.0##", CultureInfo.InvariantCulture),
                (str.Mw / str.V).ToString("0.0##", CultureInfo.InvariantCulture)
                );
            }
            Console.WriteLine("");
        }

        public static string Base64Encode(byte[] plainTextBytes)
        {
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static byte[] Base64Decode(string base64EncodedData)
        {
            return System.Convert.FromBase64String(base64EncodedData);
        }

        string docz1 = "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";
        string docz2 = "QlpoOTFBWSZTWW/HvjUAC3rfiVUSUAf//7//3/A/7//gAAiAAOAGPzPakwAC3ZsilAqEkoamTEj1NpoIZABoDR6amQaABoGg1MaBCUymo0aaMIYjEAZMRkAGmmQOMmTRiGJpgIGBNMEYJiaaaADCDT1E0SkzRtU9qmamJkwhhBkGjQYABAOMmTRiGJpgIGBNMEYJiaaaADCCRIKYmp6p5op+mip+jUmh6myg0aDaTTaTQbTUekKpAlVCSXBCSUIEhGhISViDzeZuYHqmJC7hNNmQxBH6QAvceAoSS/dkuGfOc/Rxn2BoLhuxgJ2aWWS+SWGhMORmC1s4OIhBUBAiRaIB9n1eDAsZvWjQN4KojDERsRCJsQPOi0uKgACHLsRNSoqPf3exJmZq0WimtmSmZqKFhqCZrEQNNg00mI9rD4F1UjmaLhhiyTWqXWSXvLKycE40ZHZMFJksXSpqUgsqXVtNKRY4pY4lVzruFiMFvnSdZZ1Gz6fDig60B1MSAoxYbAx5oSWsl1OQ5OA6s1EOCCGNQSq7QzJAxMLAwuSvYa5x3QJ3R5oRDiUJAgDzw6hDKBVFUoIYpGFCFnTROh7HkrM4WVzCPiCOMnDYhkXeMYGOqb6whtY9I45umg/IieRcBQQoHApuVl1ZPbckhmdVxJnqRfgAubsCeXFvOhtOl3rh24SrUNWyQmnARt3Z6CAzjqfqOhFQD3Dw9s09D8I/AXckMOQPNYfm6pznSFBiys4dSgIQHcberNvEBgJTA5mCcb1UZTMcYpjJtNVKHauci7e5zZaRwpCyc0+s2cNS1pczqn4ZEyMy63Nwq7FNoVhEuTF0NZvYJpDKOKV3FNOcCSEEo+YaAIhUszcwQgZBMvQOaSdB6qqbWkW8o7tkFCA29KlPkbqM72m7gvFzTooDsrxtrdy1OjU4eA9GyxQyyxrCbjOak6mjt0y7SqsHXbKVWFXFLDZbbJMstPiqmKqdnOxkhtolLGmzV5SWRtxtCrh4MbUaqydaV2tMvlNVZiIeIXGjGw0MHoMmaY2rLlxLumdvAibp7zlUZaImiHpCdrIlun3QMdIwBgDsohlN3dwS3IgTCxpLDE734vLJLNImZr5dMGMVI6xf7p0oL+YIk0xfWFRptNAxoQNAxdK2p/RX9gfeZoH2gGY4zwFUd4+Zj9MWeWULmkEon+oEACAf+a9+Zj67hGLr0Iz7nie2kyzmDzbilLho1Ys5Fe7RrVq6CdOL1ni7vRbkNKbPyrpnIrLgTXO5dotSjz2HEgmlJoESLSRdoRWrGJsTbdpEP1wzOEgIIMdCXndnXzlygvfBhmkGeeSqGviIRAgzI6ceSmqlw9A+8/VWgqyQwM6KH1t271k1gYslpmZtkGVYSSPeaDZIFke8ocSYzidYVLOlkFUHb96Jx3WjokYFIS1am3rsn3pWEGOyDEwLS4YxjGM8hyZMqu8yxwuogO42EGR8Nwf1kUDmAZAc4l5DvoREGLVMKBhJrRiqHcucjCEFdel8RAgqakuiYvIlC5IYyUes9GGg1eOd9+AYyGaUuAKLPzVH69AuwsVzDMCYJAbUIuDtChLXj4SdHZKPBJf1EEMpGMitaNNdrBgnr3WoSnSYIJCGW+OzP/BG7xbzszEIv3nZw/U4eIIIxDSbKXhsDtECDOq24YCIIK1GggYlbDIhttedKDf88NUQUDv54lmw7iX1SQcS0ZwVEps+Y/s1bNzhnJBajyoIzijNJXkIEECNpW09oGG9Bq2mBaU+UReC+RGkAZ0oo5cTXkCyu8DC3eaxAg6Cs9BOeN7tiIfoejaa4LS8aLkqIlFEJAUiQFNhKbgw7Lr0YuIa7vzA4oxgFhScdogQWfE1BejnNufDA8qC4Ruy3CgWXIUe6xZCtyDwt0CZIEFxLRo/v8XckU4UJBvx741A";
        string docz3 = "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";
        string docz4 = "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";
        string docz5 = "QlpoOTFBWSZTWeE9rg4AEydfiVUSUAf//69+3fA/7//gAAiAAOAGfvnRLQYBM+ZlIihXhlU9RoAAbKaAAAADRkAYQaCUJkCaajSjQBoAAAAAAADjJkyYjEwAmTBMgBowjAEMBGnqpNJiZMCBpgJgE0A0NGAEwBFJNTQ01PRlPRTxJpphPUDTaTINGg0yNGgIkpkCFMU9PVHqZhRoenqT1DynqMgaNDaRoKRQWkAV1gCshBRMIqLiV8xsIECEIMZIwsB6VsIp7SIvIIQAKfUUiB+ovvpA9pAkUQ9kLc2c6erp6eq/Hx9JqsyJuER0RlEus5RWtaRS17QIiSpSIcYYuZmRCUVFXuNMcBc9KQHEykDMhlEsL+UQuEZBNStmF1izBJBcliUcXh1ggBhirvm6wKlAB2DywieubywcfJuMCAoaKg0lZ1Dg6wu42GR3bWEgGZGCU0R6sQ7E0GZESKRLaxKIlxCbCgiJy9K2vXF8TxgV9umNr6W1y0y11vPhNwLDr9/kV9CgHCIKYzZwEK7IzguaMF1lNEqJiJNYBWRDaUXTlIiUVNORaoW8AC3wSSXKokkkorIRz2ZyWi62klFlc1gJrEcLyFNQAtSkxlBIl86MSNocDO0kwcyGEW+VGJYKMfMqJvXPmRvWNZ6+Cm5dsJeuMpM5IGtl/YqMjIyBIEjIyBIUyoyMhI6lh8RLnEMb14ZwhInSsmO6FNZ3ORwORbequpsSRN3C+lGZt+S1TcZJHK2rWk02hhdjEonkKBvoGg8VnOtBRokWtZZonJaRtfExXgsOZAXS8sBSlIlqqE9Ssy0100e9lluaSius0SxhQZ3iRAEb7uLDjO5h4GVVrMqqUNsoWERUM0UUVcWSESQTyr2xachQy0GimBmh2GZkLoVS1y+F10B4pW6zUnRnmT1cyBFrFZY1zEZJ01pey1jS5TFqJWpPMGLsQUZdFKKSyyKqmNJF4LYLNnBC643wwzvs28XxG9QU6ZJzThw3h58DAgiQXjNyono/29xMcgpwIoQRLiJ2RVOg2k9h6xesWUAvSL1izF+YvgLtW1eY8M9fvI3JpNNiyl35Vrew/FmYyUEyJyO5Z5KDBRJlhIWXPISoJYkoWhWz3kO0VkpbJDVnd9wAAwDAAAwAAADu7wNYgZKGX3NohagyXQ3sozpnBE1Ry0BSu0zF5xaFsolC6C2AMDW+hkVoHrxQUtb7+Urw9R1rriSlo7SLziXUFOnmg5ueXt4qhcwEC1FcRERBJUe02SVbDtJlxMQgXtF4QWkvBapFKoSKiCCOx/TZdfce4oFzMgDMWCwWCwWCwXYaIJmAAzQq5RYFyi9y5gXsW3AAYgFhQ5A6jAveJiDjXOL2IQIDGU5OU83gahia7T/FBTE41E6LHkO0w7zhuXI4t9tu18QchgLA3HSq0fW2j3rj3mRovMrYQU8aiugHpA3nPqvccqd+woNi9HjDA8ue4XYRSecXoW08i31aljQXkKUCQLj0YtXeJpy6xduoGvWLt3ug3soSFyFC/rdDcL1wrYF4fBQU1b5hdC2raJIkYVwkIkiBRKpQLJGpJH+yjy9FRuFw/rqXDBYHgVQdBkQ3tiywcYucsudELKBeAMaXhJdC4KBcgtO+oKSEuFpxnUrYu00hui2C2tRwibhrUrAFgXYpkhoXeFu5Sqr+GnGvILcoKcBTNcIs53VxjlKUbYwrnLZAYyshSpcCleJBTuQU7txxvD6flv8a6j2E5Tm8/IGItucAXch4yuFvUFMT4C5grU3xb9K61sA4gCoIUlSE2gAAie1IKUkywi+SaABBIUTBB+/i7kinChIcJ7XBwA==";
        string docz6 = "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";
        string docz6a = "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";
        string docz7 = "QlpoOTFBWSZTWUVBjQIADrDfiVXSUAf//69+3fA/7//gAAiAAOAGXvq4JUAGz6AA0C8c0yMhkwQ0YTBGmjRiBpkyMAAQSmkaZTTRJqan6poGgANNAAAAAAc0yMhkwQ0YTBGmjRiBpkyMAAQao/1VAAAAAAAAAAAAAIpJNQemoAeUGm1AAAAADQ0A0CpJT1MjRGIyU8TKG1NNNNB5T1DEGg0wRoYgiJXkUAilwEUtCqrbEFKVNs3FKVVSqUpS488XIPYqI6tUqKqrAZjsJKD5ifkkb8MQqN6C1cuwal2awxppJqRhmhwtfDw58fDp26Ui2Ge+m6yyozJnI72V7DF5dmAo2SKUpi4x9iZoMY3xN4rAty01TCYVmuhjDDvZHT9/bZgBrTxlCZI7WeAYCNbpccPp3iKq13cS94bvQmPvXYzQ0y5XLjlk2dJxok1TzeiOrtAK7F3TwmhVi6wNOE5FPVRNPtLUja97q5Vy9+w3UQMx4ZqHVGWbHXmz6sc2F6czIink/zpkfVITy1AYVlrbyOWpJLRSI1VVPLsi62+8jbU20OTCGNIApDUwmAdNYgccQNd2aEzynBIdYvZVXp5VTBTNntdqy7JJJvjRhzwrL2jHjjWd+TzJDkjNhdNUx+LOmSBJkkmSEzjg2xhrHZ+perNdUjbNWKp663PIFW76qdzoM3TvKjXeoseCTuhh2O7AUIJ7rBh7OmRwT5amJxiFZOjjr3YuaCcGALS0xgw+iK2WGE/HlHWCeJlK20Zy6CMYa1tW2p7zUhKYbWZnJe6NrSdPBN1uaM2tfT4coV9EF4qi5hyka42bAwmxf3UeRcdIycPsNzusbkYe+khjN4OUZJIhVDbZLIkMlq6LoWkmtKFtN7oTaxrHG056gZnbmzG9+bDVW9YpITcbPZBhQrINItJXHBze/HDPmIvvq3EwPviGLUh2MIOI1oMiDG8ZW4cqcPzvi1njWM3tx1808LMAHppLzl4/H2D3LFhDMyGOpbVXf/p5wMLDCTMQjArOK8MKBYUkXj7EelFqI/FHpRdH6o+CPujdHc+HDX51cV5eVX0x79ka3qf1x7Wq94q/si+m62K2EppjljzxZGqbLvldspwmFATCFRZ9ACABkZAAEAFszszIz7Od6s7ejF9+iktS1soY6QW7mhHUiyLySWRpRcmZlnyYraT9sIC9/NvW9voeOMsFWiz7lR1IzyA/Dtp9vfbu7MkbRSMmuqqqpaRHreO0kcb1ruRclI9aOwjBvIyswSSzJSlNT06s8e57liNDGRoRSKRSKRSKR9DmpdnJtqSNiKRsR7I2keqN2cmgRUk8R6GePejQfNHBHqhSQVseL63Z7WRgy1vhIDBtkk67ul2s3e8nFGLZwvr1uM3MyKOJ8ckWfZL074w72LTHPIugN6JGkfiODqyjucsd+pY1R17zN4NHEjUpF+CO+Nz4435dEVzN6pGZFI082jo96OfZ0o7egmvpR2+DveDYWRqzR7uU4m+MY4zeeaQHR14zmjdG5FlmMYqgsQKtEoEkhKIiH7JGlpyhnCcPqxJpkgL61jrYqcJdeKfMjg4+RzVFqR8hpjeWjmjPmRqRh1yAsjkRhpeiRxx285yo40bmTwo4nSjWEUjxo1UzR4Ecn1ssv8c+mPEjkkB8jC8Yovfk11pta1dtYxwbrJpa1Iykzo16IDugO7ibXk8vhz9EZJ9SuXn+jcYIv1BHFJ1a6jfIDQ+CNprR1o388dMY46wNAldVcqrAGWT24rVUlJLYUygQCtcGC7+LuSKcKEgioMaBAA==";

        [TestMethod]
        public void SolverDoc1Test()
        {
            FlowDiagram fd = null;
            var stream = new MemoryStream(Base64Decode(docz1));
            using (BZip2InputStream inStream = new BZip2InputStream(stream))
                 fd = DeserializeFlowDiagram(inStream);

            {
                var ms = new MemoryStream();
                var outStream = new BZip2OutputStream(ms);

                SerializeFlowDiagram(outStream, fd);

                outStream.Close();

                ms.Flush();

                string result = Base64Encode(ms.ToArray());

                Assert.AreEqual(docz1, result);
            }

            var fd1= DTOUtil.CreateFromFlowDiagram(fd);

            SolverTest(fd1);

            DTOUtil.UpdateFlowDiagram(fd, fd1);
        }

        [TestMethod]
        public void SolverDoc2Test()
        {
            FlowDiagram fd = null;
            var stream = new MemoryStream(Base64Decode(docz2));
            using (BZip2InputStream inStream = new BZip2InputStream(stream))
                fd = DeserializeFlowDiagram(inStream);

            SolverTest(fd);
        }

        [TestMethod]
        public void SolverDoc3Test()
        {
            FlowDiagram fd = null;
            var stream = new MemoryStream(Base64Decode(docz3));
            using (BZip2InputStream inStream = new BZip2InputStream(stream))
                fd = DeserializeFlowDiagram(inStream);

            {
                var ms = new MemoryStream();
                var outStream = new BZip2OutputStream(ms);

                SerializeFlowDiagram(outStream, fd);

                outStream.Close();

                ms.Flush();

                string result = Base64Encode(ms.ToArray());

                Assert.AreEqual(docz3, result);
            }

            var fd1 = DTOUtil.CreateFromFlowDiagram(fd);

            SolverTest(fd1);

            DTOUtil.UpdateFlowDiagram(fd, fd1);
        }

        [TestMethod]
        public void SolverDoc4Test()
        {
            FlowDiagram fd = null;
            var stream = new MemoryStream(Base64Decode(docz4));
            using (BZip2InputStream inStream = new BZip2InputStream(stream))
                fd = DeserializeFlowDiagram(inStream);

            SolverTest(fd);
        }

        [TestMethod]
        public void SolverDoc5Test()
        {
            FlowDiagram fd = null;
            var stream = new MemoryStream(Base64Decode(docz5));
            using (BZip2InputStream inStream = new BZip2InputStream(stream))
                fd = DeserializeFlowDiagram(inStream);

            SolverTest(fd);
        }

        [TestMethod]
        public void SolverDoc6Test()
        {
            FlowDiagram fd = null;
            var stream = new MemoryStream(Base64Decode(docz6));
            using (BZip2InputStream inStream = new BZip2InputStream(stream))
                fd = DeserializeFlowDiagram(inStream);

            SolverTest(fd);
        }

        [TestMethod]
        public void SolverDoc6aTest()
        {
            FlowDiagram fd = null;
            var stream = new MemoryStream(Base64Decode(docz6a));
            using (BZip2InputStream inStream = new BZip2InputStream(stream))
                fd = DeserializeFlowDiagram(inStream);

            SolverTest(fd);
        }

        [TestMethod]
        public void SolverDoc7Test()
        {
            FlowDiagram fd = null;
            var stream = new MemoryStream(Base64Decode(docz7));
            using (BZip2InputStream inStream = new BZip2InputStream(stream))
                fd = DeserializeFlowDiagram(inStream);

            SolverTest(fd);
        }
    }
}
