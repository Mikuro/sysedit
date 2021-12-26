package ru.asuprofi.viewModel.objects;

import javafx.util.Pair;
import org.w3c.dom.Document;
import org.w3c.dom.Element;

import java.util.ArrayList;
import java.util.List;

public class Component {
    public static final Component AIR = new Component("AIR", 28.951, -140.7, 3.774, 0.313, 0.0, -7.96256817, 0.0299271972, -5.02134635E-06, 9.20414106E-09, -4.72088923E-12, 8.56099179E-16, 0.2962);
    public static final Component WATER = new Component("WATER", 18.015, 373.98, 22.055, 0.229, 0.34486, -9.08739848, 0.0334759979, -2.63024106E-06, 7.62781326E-09, -2.93966287E-12, 3.84355046E-16, 0.2338);
    public static final Component ETHANE = new Component("ETHANE", 30.07, 32.17, 4.872, 0.279, 0.09949, -8.38900273, 0.0118838227, 6.88666959E-05, 3.81411539E-09, -1.41107489E-11, 3.64938594E-15, 0.28128);
    public static final Component PROPANE = new Component("PROPANE", 44.097, 96.68, 4.248, 0.276, 0.15229, -9.66553264, -0.00104077899, 0.000145945969, -4.80681561E-08, 7.29166432E-12, -2.00533608E-16, 0.27664);
    public static final Component ISOBUTANE = new Component("ISOBUTANE", 58.123, 134.65, 3.64, 0.278, 0.18352, -11.7026535, -0.00975642624, 0.000215168731, -8.83835323E-08, 2.12953341E-11, -2.37296696E-15, 0.27569);
    public static final Component nBUTANE = new Component("nBUTANE", 58.123, 151.97, 3.796, 0.274, 0.20016, -13.4636183, 0.00241504996, 0.00018762064, -6.07309839E-08, 7.65853251E-12, 3.03431042E-16, 0.27331);
    public static final Component ISOPENTANE = new Component("ISOPENTANE", 72.15, 187.25, 3.38, 0.27, 0.22788, -14.6503964, -0.0099449694, 0.000259900395, -1.07415898E-07, 3.09186333E-11, -4.95017075E-15, 0.27167);
    public static final Component NITROGEN = new Component("NITROGEN", 28.014, -146.95, 3.4, 0.289, 0.03772, -7.99721259, 0.0299258309, -3.96218694E-06, 6.50201843E-09, -2.58173584E-12, 2.99564819E-16, 0.28971);

    public static final List<Component> consts = new ArrayList<>(List.of(AIR, WATER, ETHANE, PROPANE, ISOBUTANE, nBUTANE, ISOPENTANE, NITROGEN));

    private String Id;
    private Double Mw;
    private Double Tc;
    private Double Pc;
    private Double Zc;
    private Double Omega;
    private Double Higa;
    private Double Higb;
    private Double Higc;
    private Double Higd;
    private Double Hige;
    private Double Higf;
    private Double ZRA;

    public Component(String id, Double mw, Double tc, Double pc, Double zc, Double omega, Double higa, Double higb, Double higc, Double higd, Double hige, Double higf, Double ZRA) {
        this.Id = id;
        this.Mw = mw;
        this.Tc = tc;
        this.Pc = pc;
        this.Zc = zc;
        this.Omega = omega;
        this.Higa = higa;
        this.Higb = higb;
        this.Higc = higc;
        this.Higd = higd;
        this.Hige = hige;
        this.Higf = higf;
        this.ZRA = ZRA;
    }

    public Component() {
        this.Id = "";
        this.Mw = 0.0;
        this.Tc = 0.0;
        this.Pc = 0.0;
        this.Zc = 0.0;
        this.Omega = 0.0;
        this.Higa = 0.0;
        this.Higb = 0.0;
        this.Higc = 0.0;
        this.Higd = 0.0;
        this.Hige = 0.0;
        this.Higf = 0.0;
        this.ZRA = 0.0;
    }

    public String getId() {
        return Id;
    }

    public Double getMw() {
        return Mw;
    }

    public Double getTc() {
        return Tc;
    }

    public Double getPc() {
        return Pc;
    }

    public Double getZc() {
        return Zc;
    }

    public Double getOmega() {
        return Omega;
    }

    public Double getHiga() {
        return Higa;
    }

    public Double getHigb() {
        return Higb;
    }

    public Double getHigc() {
        return Higc;
    }

    public Double getHigd() {
        return Higd;
    }

    public Double getHige() {
        return Hige;
    }

    public Double getHigf() {
        return Higf;
    }

    public Double getZRA() {
        return ZRA;
    }

    public Element XMLexport(Document document) {
        Element res = document.createElement("Component");
        res.setAttribute("Id", this.Id);
        res.setAttribute("Mw", Double.toString(this.Mw));
        res.setAttribute("Tc", Double.toString(this.Tc));
        res.setAttribute("Pc", Double.toString(this.Pc));
        res.setAttribute("Zc", Double.toString(this.Zc));
        res.setAttribute("Omega", Double.toString(this.Omega));
        res.setAttribute("Higa", Double.toString(this.Higa));
        res.setAttribute("Higb", Double.toString(this.Higb));
        res.setAttribute("Higc", Double.toString(this.Higc));
        res.setAttribute("Higd", Double.toString(this.Higd));
        res.setAttribute("Hige", Double.toString(this.Hige));
        res.setAttribute("Higf", Double.toString(this.Higf));
        res.setAttribute("ZRA", Double.toString(this.ZRA));
        return res;
    }

    public void XMLimport(Document document, Element element) {
        this.Id = element.getAttribute("Id");
        this.Mw = Double.parseDouble(element.hasAttribute("Mw") ? element.getAttribute("Mw") : "0.0");
        this.Tc = Double.parseDouble(element.hasAttribute("Tc") ? element.getAttribute("Tc") : "0.0");
        this.Pc = Double.parseDouble(element.hasAttribute("Pc") ? element.getAttribute("Pc") : "0.0");
        this.Zc = Double.parseDouble(element.hasAttribute("Zc") ? element.getAttribute("Zc") : "0.0");
        this.Omega = Double.parseDouble(element.hasAttribute("Omega") ? element.getAttribute("Omega") : "0.0");
        this.Higa = Double.parseDouble(element.hasAttribute("Higa") ? element.getAttribute("Higa") : "0.0");
        this.Higb = Double.parseDouble(element.hasAttribute("Higb") ? element.getAttribute("Higb") : "0.0");
        this.Higc = Double.parseDouble(element.hasAttribute("Higc") ? element.getAttribute("Higc") : "0.0");
        this.Higd = Double.parseDouble(element.hasAttribute("Higd") ? element.getAttribute("Higd") : "0.0");
        this.Hige = Double.parseDouble(element.hasAttribute("Hige") ? element.getAttribute("Hige") : "0.0");
        this.Higf = Double.parseDouble(element.hasAttribute("Higf") ? element.getAttribute("Higf") : "0.0");
        this.ZRA = Double.parseDouble(element.hasAttribute("ZRA") ? element.getAttribute("ZRA") : "0.0");
    }

    public ArrayList<Pair<String, String>> getAllFields() {
        ArrayList<Pair<String, String>> allFieldsList = new ArrayList<>();
        allFieldsList.add(new Pair<>("Mw", Double.toString(Mw)));
        allFieldsList.add(new Pair<>("Tc", Double.toString(Tc)));
        allFieldsList.add(new Pair<>("Pc", Double.toString(Pc)));
        allFieldsList.add(new Pair<>("Zc", Double.toString(Zc)));
        allFieldsList.add(new Pair<>("Omega", Double.toString(Omega)));
        allFieldsList.add(new Pair<>("Higa", Double.toString(Higa)));
        allFieldsList.add(new Pair<>("Higb", Double.toString(Higb)));
        allFieldsList.add(new Pair<>("Higc", Double.toString(Higc)));
        allFieldsList.add(new Pair<>("Higd", Double.toString(Higd)));
        allFieldsList.add(new Pair<>("Hige", Double.toString(Hige)));
        allFieldsList.add(new Pair<>("Higf", Double.toString(Higf)));
        allFieldsList.add(new Pair<>("ZRA", Double.toString(ZRA)));
        return allFieldsList;
    }
}