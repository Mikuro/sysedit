package ru.asuprofi.viewModel.objects;

import org.w3c.dom.Document;
import org.w3c.dom.Element;
import ru.asuprofi.viewModel.ObjectClass;
import ru.asuprofi.viewModel.ObjectType;
import ru.asuprofi.viewModel.links.Port;

public class Splitter extends BaseObject {

    public Splitter() {
        super();
        this.type = ObjectType.Splitter;
        this.objectClass = ObjectClass.ProcessUnit;

        Port p1 = new Port("p1");
        Port p2 = new Port("p2");
        Port p3 = new Port("p3");
        Port f1 = new Port("f1");

        p1.setParent(this);
        f1.setParent(this);
        p2.setParent(this);
        p3.setParent(this);

        f1.x.set(6);
        f1.y.set(29);

        p1.x.set(29);
        p1.y.set(6);

        p2.x.set(52);
        p2.y.set(29);

        p3.x.set(29);
        p3.y.set(52);

        p1.padding = 7.0;
        f1.padding = 7.0;
        p2.padding = 7.0;
        p3.padding = 7.0;

        this.portsList.add(p1);
        this.portsList.add(f1);
        this.portsList.add(p2);
        this.portsList.add(p3);

        this.w.set(58.0);
        this.h.set(58.0);
    }

    @Override
    public Element XMLexport(Document document) {
        Element res = document.createElement(this.type.toString());
        res.setAttribute("Id", this.getId());
        res.setAttribute("X", Double.toString(this.x.get()));
        res.setAttribute("Y", Double.toString(this.y.get()));
        return res;
    }

    @Override
    public void XMLimport(Document document, Element elem) {
        this.setId(elem.getAttribute("Id"));
        this.x.set(Double.parseDouble(elem.hasAttribute("X") ? elem.getAttribute("X") : String.valueOf(this.x.get())));
        this.y.set(Double.parseDouble(elem.hasAttribute("Y") ? elem.getAttribute("Y") : String.valueOf(this.y.get())));
    }

    @Override
    void initialize() {

    }

}
