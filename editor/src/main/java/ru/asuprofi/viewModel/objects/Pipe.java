package ru.asuprofi.viewModel.objects;

import org.w3c.dom.Document;
import org.w3c.dom.Element;
import ru.asuprofi.viewModel.ObjectClass;
import ru.asuprofi.viewModel.ObjectType;
import ru.asuprofi.viewModel.links.Port;

public class Pipe extends BaseObject {

    private Double Hdiff;

    public Pipe() {
        super();
        this.type = ObjectType.Pipe;
        this.objectClass = ObjectClass.ProcessUnit;

        Port p1 = new Port("p1");
        Port f1 = new Port("f1");

        p1.setParent(this);
        f1.setParent(this);

        p1.x.set(6);
        p1.y.set(6);

        f1.x.set(6);
        f1.y.set(130);

        p1.padding = 7.0;
        f1.padding = 7.0;

        this.portsList.add(p1);
        this.portsList.add(f1);

        this.w.set(12.0);
        this.h.set(136.0);
//        this.setCV(111.0);
        this.setHdiff(222.0);
//        this.setUql(333.0);
//        this.setAsur(444.0);
    }

    @Override
    public Element XMLexport(Document document) {
        Element res = document.createElement(this.type.toString());
        res.setAttribute("Id", this.getId());
        res.setAttribute("X", Double.toString(this.x.get()));
        res.setAttribute("Y", Double.toString(this.y.get()));
//        res.setAttribute("CV", Double.toString(this.getCV()));
        res.setAttribute("Hdiff", Double.toString(this.getHdiff()));
//        res.setAttribute("Uql", Double.toString(this.getUql()));
//        res.setAttribute("Asur", Double.toString(this.getAsur()));
        return res;
    }

    @Override
    public void XMLimport(Document document, Element elem) {
        this.setId(elem.getAttribute("Id"));
        this.x.set(Double.parseDouble(elem.hasAttribute("X") ? elem.getAttribute("X") : String.valueOf(this.x.get())));
        this.y.set(Double.parseDouble(elem.hasAttribute("Y") ? elem.getAttribute("Y") : String.valueOf(this.y.get())));
//        this.setCV(Double.parseDouble(elem.hasAttribute("CV") ? elem.getAttribute("CV") : "0"));
        this.setHdiff(Double.parseDouble(elem.hasAttribute("Hdiff") ? elem.getAttribute("Hdiff") : "0"));
//        this.setUql(Double.parseDouble(elem.hasAttribute("Uql") ? elem.getAttribute("Uql") : "0"));
//        this.setAsur(Double.parseDouble(elem.hasAttribute("Asur") ? elem.getAttribute("Asur") : "0"));
    }

    @Override
    void initialize() {

    }

//    public Double getCV() {
//        return CV;
//    }
//
//    public void setCV(Double CV) {
//        this.CV = CV;
//    }

    public Double getHdiff() {
        return Hdiff;
    }

    public void setHdiff(Double hdiff) {
        Hdiff = hdiff;
    }

//    public Double getUql() {
//        return Uql;
//    }
//
//    public void setUql(Double uql) {
//        Uql = uql;
//    }
//
//    public Double getAsur() {
//        return Asur;
//    }

//    public void setAsur(Double asur) {
//        Asur = asur;
//    }
}
