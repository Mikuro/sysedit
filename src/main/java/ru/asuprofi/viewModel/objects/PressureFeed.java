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

public class PressureFeed extends BaseObject {

    private final ObservableList<Double> arr;
    private Double P;
    private Double T;

    public PressureFeed() {
        super();
        this.type = ObjectType.PressureFeed;
        this.objectClass = ObjectClass.ProcessUnit;

        Port p1 = new Port("p1");

        p1.setParent(this);

        p1.x.set(48);
        p1.y.set(10);

        p1.padding = 7.0;

        this.portsList.add(p1);

        this.w.set(55.0);
        this.h.set(20.0);

        this.setP(101.135);
        this.setT(20.0);

        this.arr = FXCollections.observableArrayList();
        isErrorOccurred.set(true);
        arr.addListener((ListChangeListener<Double>) change -> {
            while (change.next()) {
                if (change.wasAdded() || change.wasRemoved() || change.wasUpdated()) {
                    Double sum = 0.0;
                    for (Double i : arr) {
                        sum += i;
                    }
                    isErrorOccurred.set(Math.abs(sum - 1) > 0.00001);
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
        res.setAttribute("P", Double.toString(getP()));
        res.setAttribute("T", Double.toString(getT()));

        Element arr = document.createElement("PressureFeed.x");

        if (!((Element) document.getFirstChild()).getTagName().equals("BlockProperties"))
            for (Double i : this.arr) {
                Element value = document.createElement("sys:Double");
                value.setTextContent(Double.toString(i));
                arr.appendChild(value);
            }
        else
            for (int j = 0; j < this.arr.size(); j++) {
                Element value = document.createElement(":" + j);
                value.setTextContent(Double.toString(this.arr.get(j)));
                arr.appendChild(value);
            }

        res.appendChild(arr);
        return res;
    }

    @Override
    public void XMLimport(Document document, Element elem) {
        this.setId(elem.getAttribute("Id"));
        this.x.set(Double.parseDouble(elem.hasAttribute("X") ? elem.getAttribute("X") : String.valueOf(this.x.get())));
        this.y.set(Double.parseDouble(elem.hasAttribute("Y") ? elem.getAttribute("Y") : String.valueOf(this.y.get())));
        this.setP(Double.parseDouble(elem.hasAttribute("P") ? elem.getAttribute("P") : "0"));
        this.setT(Double.parseDouble(elem.hasAttribute("T") ? elem.getAttribute("T") : "0"));

        NodeList arrElems = elem.getElementsByTagName("PressureFeed.x");
        Element arr = (Element) arrElems.item(0);
        NodeList params = arr.getChildNodes();

        this.arr.clear();

        for (int i = 0; i < params.getLength(); i++) {
            if (params.item(i) instanceof Element) {
                this.arr.add(Double.parseDouble(params.item(i).getTextContent()));
            }
        }
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

    public Double getT() {
        return T;
    }

    public void setT(Double t) {
        T = t;
    }

    public List<Double> getArr() {
        return arr;
    }
}
