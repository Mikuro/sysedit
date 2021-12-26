package ru.asuprofi.viewModel.objects;

import org.w3c.dom.Document;
import org.w3c.dom.Element;
import ru.asuprofi.viewModel.ObjectClass;
import ru.asuprofi.viewModel.ObjectType;
import ru.asuprofi.viewModel.links.Port;

public class ControlValve extends BaseObject {

    private Double CV;
    private Double R;
    private Double pos0;
    private Double topen;
    private Double tclose;
    private Double tlag;
    private Double Cff;
    private Double pos;
    private Double mv;


    public ControlValve() {
        super();
        this.type = ObjectType.ControlValve;
        this.objectClass = ObjectClass.ProcessUnit;

        Port p1 = new Port("p1");
        Port f1 = new Port("f1");

        p1.setParent(this);
        f1.setParent(this);

        p1.x.set(72);
        p1.y.set(25);

        f1.x.set(6);
        f1.y.set(25);

        p1.padding = 7.0;
        f1.padding = 7.0;

        this.portsList.add(p1);
        this.portsList.add(f1);

        this.w.set(78.0);
        this.h.set(35.0);

        this.setCV(1000.0);
        this.setR(50.0);
        this.setPos0(0.002);
        this.setTopen(5.0);
        this.setTclose(5.0);
        this.setTlag(3.0);
        this.setCff(1.0);
        this.setPos(0.0);
        this.setMv(0.0);
    }

    @Override
    public Element XMLexport(Document document) {
        Element res = document.createElement(this.type.toString());
        res.setAttribute("Id", this.getId());
        res.setAttribute("X", Double.toString(this.x.get()));
        res.setAttribute("Y", Double.toString(this.y.get()));
        res.setAttribute("CV", Double.toString(this.getCV()));
        res.setAttribute("R", Double.toString(this.getR()));
        res.setAttribute("pos0", Double.toString(this.getPos0()));
        res.setAttribute("topen", Double.toString(this.getTopen()));
        res.setAttribute("tclose", Double.toString(this.getTclose()));
        res.setAttribute("tlag", Double.toString(this.getTlag()));
        res.setAttribute("Cff", Double.toString(this.getCff()));
        res.setAttribute("pos", Double.toString(this.getPos()));
        res.setAttribute("mv", Double.toString(this.getMv()));
        return res;
    }

    @Override
    public void XMLimport(Document document, Element elem) {
        this.setId(elem.getAttribute("Id"));

        this.x.set(Double.parseDouble(elem.hasAttribute("X") ? elem.getAttribute("X") : String.valueOf(this.x.get())));
        this.y.set(Double.parseDouble(elem.hasAttribute("Y") ? elem.getAttribute("Y") : String.valueOf(this.y.get())));

        this.setCV(Double.parseDouble(elem.hasAttribute("CV") ? elem.getAttribute("CV") : "0"));
        this.setR(Double.parseDouble(elem.hasAttribute("R") ? elem.getAttribute("R") : "0"));
        this.setPos0(Double.parseDouble(elem.hasAttribute("pos0") ? elem.getAttribute("pos0") : "0"));
        this.setTopen(Double.parseDouble(elem.hasAttribute("topen") ? elem.getAttribute("topen") : "0"));
        this.setTclose(Double.parseDouble(elem.hasAttribute("tclose") ? elem.getAttribute("tclose") : "0"));
        this.setTlag(Double.parseDouble(elem.hasAttribute("tlag") ? elem.getAttribute("tlag") : "0"));
        this.setCff(Double.parseDouble(elem.hasAttribute("Cff") ? elem.getAttribute("Cff") : "0"));
        this.setPos(Double.parseDouble(elem.hasAttribute("pos") ? elem.getAttribute("pos") : "0"));
        this.setMv(Double.parseDouble(elem.hasAttribute("mv") ? elem.getAttribute("mv") : "0"));
    }

    @Override
    public void initialize() {

    }

    public Double getCV() {
        return CV;
    }

    public void setCV(Double CV) {
        this.CV = CV;
    }

    public Double getR() {
        return R;
    }

    public void setR(Double r) {
        R = r;
    }

    public Double getPos0() {
        return pos0;
    }

    public void setPos0(Double pos0) {
        this.pos0 = pos0;
    }

    public Double getTopen() {
        return topen;
    }

    public void setTopen(Double topen) {
        this.topen = topen;
    }

    public Double getTclose() {
        return tclose;
    }

    public void setTclose(Double tclose) {
        this.tclose = tclose;
    }

    public Double getTlag() {
        return tlag;
    }

    public void setTlag(Double tlag) {
        this.tlag = tlag;
    }

    public Double getCff() {
        return Cff;
    }

    public void setCff(Double cff) {
        Cff = cff;
    }

    public Double getPos() {
        return pos;
    }

    public void setPos(Double pos) {
        this.pos = pos;
    }

    public Double getMv() {
        return mv;
    }

    public void setMv(Double mv) {
        this.mv = mv;
    }
}
