package ru.asuprofi.viewModel.objects;

import org.w3c.dom.Document;
import org.w3c.dom.Element;
import ru.asuprofi.viewModel.ObjectClass;
import ru.asuprofi.viewModel.ObjectType;
import ru.asuprofi.viewModel.links.Port;

public class Pump extends BaseObject {

    private Double Wd;
    private Double Ws;
    private Double dd;
    private Double Hd;
    private Double Hs;
    private Double Vd;
    private Double pos;
    private Double tlag;
    private Double mv;

    public Pump() {
        super();
        this.type = ObjectType.Pump;
        this.objectClass = ObjectClass.ProcessUnit;

        Port p1 = new Port("p1");
        Port f1 = new Port("f1");

        p1.setParent(this);
        f1.setParent(this);

        p1.x.set(72);
        p1.y.set(6);

        f1.x.set(6);
        f1.y.set(18);

        p1.padding = 7.0;
        f1.padding = 7.0;

        this.portsList.add(p1);
        this.portsList.add(f1);

        this.w.set(78.0);
        this.h.set(33.0);

        this.setWd(111.0);
        this.setWs(222.0);
        this.setDd(333.0);
        this.setHd(444.0);
        this.setHs(555.0);
        this.setVd(666.0);
        this.setMv(0.0);
        this.setPos(0.0);
        this.setTlag(0.0);
    }

    @Override
    public Element XMLexport(Document document) {
        Element res = document.createElement(this.type.toString());
        res.setAttribute("Id", this.getId());
        res.setAttribute("X", Double.toString(this.x.get()));
        res.setAttribute("Y", Double.toString(this.y.get()));
        res.setAttribute("Wd", Double.toString(this.getWd()));
        res.setAttribute("Ws", Double.toString(this.getWs()));
        res.setAttribute("dd", Double.toString(this.getDd()));
        res.setAttribute("Hd", Double.toString(this.getHd()));
        res.setAttribute("Hs", Double.toString(this.getHs()));
        res.setAttribute("Vd", Double.toString(this.getVd()));
        res.setAttribute("pos", Double.toString(this.getPos()));
        res.setAttribute("tlag", Double.toString(this.getTlag()));
        res.setAttribute("mv", Double.toString(this.getMv()));
        return res;
    }

    @Override
    public void XMLimport(Document document, Element elem) {
        this.setId(elem.getAttribute("Id"));
        this.x.set(Double.parseDouble(elem.hasAttribute("X") ? elem.getAttribute("X") : String.valueOf(this.x.get())));
        this.y.set(Double.parseDouble(elem.hasAttribute("Y") ? elem.getAttribute("Y") : String.valueOf(this.y.get())));
        this.setWd(Double.parseDouble(elem.hasAttribute("Wd") ? elem.getAttribute("Wd") : "0"));
        this.setWs(Double.parseDouble(elem.hasAttribute("Ws") ? elem.getAttribute("Ws") : "0"));
        this.setDd(Double.parseDouble(elem.hasAttribute("dd") ? elem.getAttribute("dd") : "0"));
        this.setHd(Double.parseDouble(elem.hasAttribute("Hd") ? elem.getAttribute("Hd") : "0"));
        this.setHs(Double.parseDouble(elem.hasAttribute("Hs") ? elem.getAttribute("Hs") : "0"));
        this.setVd(Double.parseDouble(elem.hasAttribute("Vd") ? elem.getAttribute("Vd") : "0"));
        this.setPos(Double.parseDouble(elem.hasAttribute("pos") ? elem.getAttribute("pos") : "0"));
        this.setTlag(Double.parseDouble(elem.hasAttribute("tlag") ? elem.getAttribute("tlag") : "0"));
        this.setMv(Double.parseDouble(elem.hasAttribute("mv") ? elem.getAttribute("mv") : "0"));

    }

    @Override
    void initialize() {

    }

    public Double getWd() {
        return Wd;
    }

    public void setWd(Double wd) {
        Wd = wd;
    }

    public Double getWs() {
        return Ws;
    }

    public void setWs(Double ws) {
        Ws = ws;
    }

    public Double getDd() {
        return dd;
    }

    public void setDd(Double dd) {
        this.dd = dd;
    }

    public Double getHd() {
        return Hd;
    }

    public void setHd(Double hd) {
        Hd = hd;
    }

    public Double getHs() {
        return Hs;
    }

    public void setHs(Double hs) {
        Hs = hs;
    }

    public Double getVd() {
        return Vd;
    }

    public void setVd(Double vd) {
        Vd = vd;
    }

    public Double getPos() {
        return pos;
    }

    public void setPos(Double pos) {
        this.pos = pos;
    }

    public Double getTlag() {
        return tlag;
    }

    public void setTlag(Double tlag) {
        this.tlag = tlag;
    }

    public Double getMv() {
        return mv;
    }

    public void setMv(Double mv) {
        this.mv = mv;
    }
}
