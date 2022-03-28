package ru.asuprofi.viewModel.objects;

import javafx.collections.FXCollections;
import javafx.collections.ListChangeListener;
import javafx.collections.ObservableList;
import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.NodeList;
import ru.asuprofi.viewModel.ObjectClass;
import ru.asuprofi.viewModel.ObjectType;
import ru.asuprofi.viewModel.links.Port;

import java.util.List;

public class LiquidTank extends BaseObject {

    private Integer T;
    private Integer A;

    private ObservableList<Double> u;

    public LiquidTank() {
        super();
        this.type = ObjectType.LiquidTank;
        this.objectClass = ObjectClass.ProcessUnit;

        this.setMainVariable("Level");

        Port p1 = new Port("p1");
        Port f1 = new Port("f1");
        Port fe = new Port("fe");

        p1.setParent(this);
        f1.setParent(this);
        fe.setParent(this);

        p1.x.set(40);
        p1.y.set(162);

        f1.x.set(49);
        f1.y.set(6);

        fe.x.set(31);
        fe.y.set(12);

        p1.padding = 7.0;
        f1.padding = 7.0;
        fe.padding = 7.0;

        this.portsList.add(p1);
        this.portsList.add(f1);
        this.portsList.add(fe);

        this.w.set(80.0);
        this.h.set(168.0);
        this.setT(20);
        this.setA(4);
        isErrorOccurred.set(false);
        this.u = FXCollections.observableArrayList();
        u.addListener((ListChangeListener<Double>) change -> {
            while (change.next()) {
                if (change.wasAdded() || change.wasRemoved() || change.wasUpdated()) {
                    for (Double i : u) {
                        if (i < 0) {
                            isErrorOccurred.set(true);
                            break;
                        } else {
                            isErrorOccurred.set(false);
                        }
                    }
                }
            }
        });
    }

    @Override
    public Element XMLexport(Document document) {
        Element res = document.createElement(this.type.toString());
        res.setAttribute("Id", this.getId());
        res.setAttribute("X", Double.toString(this.x.get()));
        res.setAttribute("Y", Double.toString(this.y.get()));
        res.setAttribute("T", Integer.toString(this.getT()));
        res.setAttribute("A", Integer.toString(this.getA()));

        Element u = document.createElement("LiquidTank.u");

        if (!((Element) document.getFirstChild()).getTagName().equals("BlockProperties"))
            for (Double i : this.u) {
                Element value = document.createElement("sys:Double");
                value.setTextContent(Double.toString(i));
                u.appendChild(value);
            }
        else
            for (int j = 0; j < this.u.size(); j++) {
                Element value = document.createElement(":" + j);
                value.setTextContent(Double.toString(this.u.get(j)));
                u.appendChild(value);
            }

        res.appendChild(u);

        return res;
    }

    @Override
    public void XMLimport(Document document, Element elem) {
        this.setId(elem.getAttribute("Id"));
        this.x.set(Double.parseDouble(elem.hasAttribute("X") ? elem.getAttribute("X") : String.valueOf(this.x.get())));
        this.y.set(Double.parseDouble(elem.hasAttribute("Y") ? elem.getAttribute("Y") : String.valueOf(this.y.get())));
        this.setT(Integer.parseInt(elem.hasAttribute("T") ? elem.getAttribute("T") : "0"));
        this.setA(Integer.parseInt(elem.hasAttribute("A") ? elem.getAttribute("A") : "0"));

        NodeList arr = elem.getElementsByTagName("LiquidTank.u");
        Element u = (Element) arr.item(0);
        NodeList params = u.getChildNodes();

        this.u.clear();

        for (int i = 0; i < params.getLength(); i++) {
            if (params.item(i) instanceof Element) {
                this.u.add(Double.parseDouble(params.item(i).getTextContent()));
            }
        }
    }

    @Override
    void initialize() {

    }

    public Integer getT() {
        return T;
    }

    public void setT(Integer t) {
        T = t;
    }

    public Integer getA() {
        return A;
    }

    public void setA(Integer a) {
        A = a;
    }

    public List<Double> getU() {
        return u;
    }
}
