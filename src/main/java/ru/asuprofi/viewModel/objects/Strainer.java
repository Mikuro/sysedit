package ru.asuprofi.viewModel.objects;

import org.w3c.dom.Document;
import org.w3c.dom.Element;
import ru.asuprofi.viewModel.ObjectClass;
import ru.asuprofi.viewModel.ObjectType;
import ru.asuprofi.viewModel.links.Port;

public class Strainer extends BaseObject {

    private Double Wd;
    private Double DP;
    private Double dd;

    public Strainer() {
        super();

        this.type = ObjectType.Strainer;
        this.objectClass = ObjectClass.ProcessUnit;

        Port p1 = new Port("p1");
        Port f1 = new Port("f1");

        p1.setParent(this);
        f1.setParent(this);

        p1.x.set(72);
        p1.y.set(15);

        f1.x.set(6);
        f1.y.set(15);

        p1.padding = 7.0;
        f1.padding = 7.0;

        this.portsList.add(p1);
        this.portsList.add(f1);

        this.w.set(78.0);
        this.h.set(30.0);

        this.setWd(111.0);
        this.setDP(222.0);
        this.setDd(333.0);

    }

    @Override
    public Element XMLexport(Document document) {
        Element res = document.createElement(this.type.toString());
        res.setAttribute("Id", this.getId());
        res.setAttribute("X", Double.toString(this.x.get()));
        res.setAttribute("Y", Double.toString(this.y.get()));
        res.setAttribute("Wd", Double.toString(this.getWd()));
        res.setAttribute("DP", Double.toString(this.getDP()));
        res.setAttribute("dd", Double.toString(this.getDd()));
        return res;
    }

    @Override
    public void XMLimport(Document document, Element elem) {
        this.setId(elem.getAttribute("Id"));
        this.x.set(Double.parseDouble(elem.hasAttribute("X") ? elem.getAttribute("X") : String.valueOf(this.x.get())));
        this.y.set(Double.parseDouble(elem.hasAttribute("Y") ? elem.getAttribute("Y") : String.valueOf(this.y.get())));
        this.setWd(Double.parseDouble(elem.hasAttribute("Wd") ? elem.getAttribute("Wd") : "0"));
        this.setDP(Double.parseDouble(elem.hasAttribute("DP") ? elem.getAttribute("DP") : "0"));
        this.setDd(Double.parseDouble(elem.hasAttribute("dd") ? elem.getAttribute("dd") : "0"));
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

    public Double getDP() {
        return DP;
    }

    public void setDP(Double DP) {
        this.DP = DP;
    }

    public Double getDd() {
        return dd;
    }

    public void setDd(Double dd) {
        this.dd = dd;
    }
}
