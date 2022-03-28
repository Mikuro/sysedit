using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tanks.Models
{
    [ModelClass]
    public class Component
    {
        [PropertyType(PropertyType.Id)]
        public string Id { get; set; }
        [PropertyType(PropertyType.ModelTuning)]
        public double Mw { get; set; }
        [PropertyType(PropertyType.ModelTuning)]
        public double Tc { get; set; }
        [PropertyType(PropertyType.ModelTuning)]
        public double Pc { get; set; }
        [PropertyType(PropertyType.ModelTuning)]
        public double Zc { get; set; }
        [PropertyType(PropertyType.ModelTuning)]
        public double Omega { get; set; }
        [PropertyType(PropertyType.ModelTuning)]
        public double Higa { get; set; }
        [PropertyType(PropertyType.ModelTuning)]
        public double Higb { get; set; }
        [PropertyType(PropertyType.ModelTuning)]
        public double Higc { get; set; }
        [PropertyType(PropertyType.ModelTuning)]
        public double Higd { get; set; }
        [PropertyType(PropertyType.ModelTuning)]
        public double Hige { get; set; }
        [PropertyType(PropertyType.ModelTuning)]
        public double Higf { get; set; }
        [PropertyType(PropertyType.ModelTuning)]
        public double ZRA { get; set; }
/*
        public double V { get; set; }
        public int LIB { get; set; }
        public double Nbp { get; set; }
        public double Hg { get; set; }
        public double Lh { get; set; }
        public double Delta { get; set; }
        public double UniqR { get; set; }
        public double UniqQ { get; set; }
        public double PvA { get; set; }
        public double PvB { get; set; }
        public double PvC { get; set; }
        public double Hla { get; set; }
        public double Hlb { get; set; }
        public double Hlc { get; set; }
        public double Hva { get; set; }
        public double Hvb { get; set; }
        public double Hvc { get; set; }
        public double Dla { get; set; }
        public double Dlb { get; set; }
        public double Dlc { get; set; }
        public double Dl2a { get; set; }
        public double Dl2b { get; set; }
        public double Dl2c { get; set; }
        public double Mla { get; set; }
        public double Mlb { get; set; }
        public double Mlc { get; set; }
        public double Ml2a { get; set; }
        public double Ml2b { get; set; }
        public double Ml2c { get; set; }
        public double Mva { get; set; }
        public double Mvb { get; set; }
        public double Mvc { get; set; }
        public double Vstar { get; set; }
        public double OmegaSRK { get; set; }

        public int DPRVP_eqn { get; set; }
        public double DPRVP_LT { get; set; }
        public double DPRVP_HT { get; set; }
        public double DPRVP_A { get; set; }
        public double DPRVP_B { get; set; }
        public double DPRVP_C { get; set; }
        public double DPRVP_D { get; set; }
        public double DPRVP_E { get; set; }
        public double DPRVP_F { get; set; }
        public double DPRVP_G { get; set; }

        public int DPRLCP_eqn { get; set; }
        public double DPRLCP_LT { get; set; }
        public double DPRLCP_HT { get; set; }
        public double DPRLCP_A { get; set; }
        public double DPRLCP_B { get; set; }
        public double DPRLCP_C { get; set; }
        public double DPRLCP_D { get; set; }
        public double DPRLCP_E { get; set; }
        public double DPRLCP_F { get; set; }
        public double DPRLCP_G { get; set; }

        public int DPRHVP_eqn { get; set; }
        public double DPRHVP_LT { get; set; }
        public double DPRHVP_HT { get; set; }
        public double DPRHVP_A { get; set; }
        public double DPRHVP_B { get; set; }
        public double DPRHVP_C { get; set; }
        public double DPRHVP_D { get; set; }
        public double DPRHVP_E { get; set; }
        public double DPRHVP_F { get; set; }
        public double DPRHVP_G { get; set; }

        public int DPRLDN_eqn { get; set; }
        public double DPRLDN_LT { get; set; }
        public double DPRLDN_HT { get; set; }
        public double DPRLDN_A { get; set; }
        public double DPRLDN_B { get; set; }
        public double DPRLDN_C { get; set; }
        public double DPRLDN_D { get; set; }
        public double DPRLDN_E { get; set; }
        public double DPRLDN_F { get; set; }
        public double DPRLDN_G { get; set; }

        public int DPRLVS_eqn { get; set; }
        public double DPRLVS_LT { get; set; }
        public double DPRLVS_HT { get; set; }
        public double DPRLVS_A { get; set; }
        public double DPRLVS_B { get; set; }
        public double DPRLVS_C { get; set; }
        public double DPRLVS_D { get; set; }
        public double DPRLVS_E { get; set; }
        public double DPRLVS_F { get; set; }
        public double DPRLVS_G { get; set; }

        public int DPRVVS_eqn { get; set; }
        public double DPRVVS_LT { get; set; }
        public double DPRVVS_HT { get; set; }
        public double DPRVVS_A { get; set; }
        public double DPRVVS_B { get; set; }
        public double DPRVVS_C { get; set; }
        public double DPRVVS_D { get; set; }
        public double DPRVVS_E { get; set; }
        public double DPRVVS_F { get; set; }
        public double DPRVVS_G { get; set; }
*/

    }
}
