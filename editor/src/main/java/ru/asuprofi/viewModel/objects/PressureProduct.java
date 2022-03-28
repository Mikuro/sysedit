package ru.asuprofi.viewModel.objects;

import org.w3c.dom.Document;
import org.w3c.dom.Element;
import ru.asuprofi.viewModel.ObjectClass;
import ru.asuprofi.viewModel.ObjectType;
import ru.asuprofi.viewModel.links.Port;

public class PressureProduct extends BaseObject {

    private Double P;

    public PressureProduct() {
        super();
        this.type = ObjectType.PressureProduct;
        this.objectClass = ObjectClass.ProcessUnit;

        Port f1 = new Port("f1");

        f1.setParent(this);

        f1.x.set(6);
        f1.y.set(10);

        f1.padding = 7.0;

        this.portsList.add(f1);

        this.w.set(56.0);
        this.h.set(20.0);
        this.setP(1808.21);
    }

    @Override
    public Element XMLexport(Document document) {
        Element res = document.createElement(this.type.toString());
        res.setAttribute("Id", this.getId());
        res.setAttribute("X", Double.toString(this.x.get()));
        res.setAttribute("Y", Double.toString(this.y.get()));
        res.setAttribute("P", Double.toString(this.getP()));
        return res;
    }

    @Override
    public void XMLimport(Document document, Element elem) {
        this.setId(elem.getAttribute("Id"));
        this.x.set(Double.parseDouble(elem.hasAttribute("X") ? elem.getAttribute("X") : String.valueOf(this.x.get())));
        this.y.set(Double.parseDouble(elem.hasAttribute("Y") ? elem.getAttribute("Y") : String.valueOf(this.y.get())));
        this.setP(Double.parseDouble(elem.hasAttribute("P") ? elem.getAttribute("P") : "0"));
    }

    @Override
    void initialize() {

    }

    public Double getP() {
        return P;
    }

    public void setP(Double p) {
        P = p;
    }
}
