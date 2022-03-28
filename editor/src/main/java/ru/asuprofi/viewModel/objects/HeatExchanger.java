package ru.asuprofi.viewModel.objects;

import org.w3c.dom.Document;
import org.w3c.dom.Element;
import ru.asuprofi.viewModel.ObjectClass;
import ru.asuprofi.viewModel.ObjectType;
import ru.asuprofi.viewModel.links.Port;

public class HeatExchanger extends BaseObject {

    private Double WHdes;
    private Double DHdes;
    private Double PdHdes;
    private Double WLdes;
    private Double DLdes;
    private Double PdLdes;
    private Double A;
    private Double U;

    public HeatExchanger() {
        super();
        this.type = ObjectType.HeatExchanger;
        this.objectClass = ObjectClass.ProcessUnit;

        Port p1 = new Port("p1");
        Port p2 = new Port("p2");
        Port f1 = new Port("f1");
        Port f2 = new Port("f2");

        p1.setParent(this);
        p2.setParent(this);
        f1.setParent(this);
        f2.setParent(this);

        f1.x.set(49);
        f1.y.set(6);

        f2.x.set(6);
        f2.y.set(49);

        p1.x.set(49);
        p1.y.set(92);

        p2.x.set(92);
        p2.y.set(49);

        p1.padding = 7.0;
        f1.padding = 7.0;
        p2.padding = 7.0;
        f2.padding = 7.0;

        this.portsList.add(p1);
        this.portsList.add(f1);
        this.portsList.add(p2);
        this.portsList.add(f2);

        this.w.set(98.0);
        this.h.set(98.0);
        this.setWHdes(111.0);
        this.setDHdes(222.0);
        this.setPdHdes(333.0);
        this.setWLdes(444.0);
        this.setDLdes(555.0);
        this.setPdLdes(666.0);
        this.setA(777.0);
        this.setU(888.0);
    }

    @Override
    public Element XMLexport(Document document) {
        Element res = document.createElement(this.type.toString());
        res.setAttribute("Id", this.getId());
        res.setAttribute("X", Double.toString(this.x.get()));
        res.setAttribute("Y", Double.toString(this.y.get()));
        res.setAttribute("WHdes", Double.toString(this.getWHdes()));
        res.setAttribute("DHdes", Double.toString(this.getDHdes()));
        res.setAttribute("PdHdes", Double.toString(this.getPdHdes()));
        res.setAttribute("WLdes", Double.toString(this.getWLdes()));
        res.setAttribute("DLdes", Double.toString(this.getDLdes()));
        res.setAttribute("PdLdes", Double.toString(this.getPdLdes()));
        res.setAttribute("A", Double.toString(this.getA()));
        res.setAttribute("U", Double.toString(this.getU()));
        return res;
    }

    @Override
    public void XMLimport(Document document, Element elem) {
        this.setId(elem.getAttribute("Id"));
        this.x.set(Double.parseDouble(elem.hasAttribute("X") ? elem.getAttribute("X") : String.valueOf(this.x.get())));
        this.y.set(Double.parseDouble(elem.hasAttribute("Y") ? elem.getAttribute("Y") : String.valueOf(this.y.get())));
        this.setWHdes(Double.parseDouble(elem.hasAttribute("WHdes") ? elem.getAttribute("WHdes") : "0"));
        this.setDHdes(Double.parseDouble(elem.hasAttribute("DHdes") ? elem.getAttribute("DHdes") : "0"));
        this.setPdHdes(Double.parseDouble(elem.hasAttribute("PdHdes") ? elem.getAttribute("PdHdes") : "0"));
        this.setWLdes(Double.parseDouble(elem.hasAttribute("WLdes") ? elem.getAttribute("WLdes") : "0"));
        this.setDLdes(Double.parseDouble(elem.hasAttribute("DLdes") ? elem.getAttribute("DLdes") : "0"));
        this.setPdLdes(Double.parseDouble(elem.hasAttribute("PdLdes") ? elem.getAttribute("PdLdes") : "0"));
        this.setA(Double.parseDouble(elem.hasAttribute("A") ? elem.getAttribute("A") : "0"));
        this.setU(Double.parseDouble(elem.hasAttribute("U") ? elem.getAttribute("U") : "0"));
    }

    @Override
    void initialize() {

    }

    public Double getWHdes() {
        return WHdes;
    }

    public void setWHdes(Double WHdes) {
        this.WHdes = WHdes;
    }

    public Double getDHdes() {
        return DHdes;
    }

    public void setDHdes(Double DHdes) {
        this.DHdes = DHdes;
    }

    public Double getPdHdes() {
        return PdHdes;
    }

    public void setPdHdes(Double pdHdes) {
        PdHdes = pdHdes;
    }

    public Double getWLdes() {
        return WLdes;
    }

    public void setWLdes(Double WLdes) {
        this.WLdes = WLdes;
    }

    public Double getDLdes() {
        return DLdes;
    }

    public void setDLdes(Double DLdes) {
        this.DLdes = DLdes;
    }

    public Double getPdLdes() {
        return PdLdes;
    }

    public void setPdLdes(Double pdLdes) {
        PdLdes = pdLdes;
    }

    public Double getA() {
        return A;
    }

    public void setA(Double a) {
        A = a;
    }

    public Double getU() {
        return U;
    }

    public void setU(Double u) {
        U = u;
    }
}
