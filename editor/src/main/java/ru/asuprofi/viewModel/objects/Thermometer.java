package ru.asuprofi.viewModel.objects;

import org.w3c.dom.Document;
import org.w3c.dom.Element;
import ru.asuprofi.viewModel.MeasurementUnit;
import ru.asuprofi.viewModel.ObjectClass;
import ru.asuprofi.viewModel.ObjectType;
import ru.asuprofi.viewModel.links.LinkClass;
import ru.asuprofi.viewModel.links.Port;
import ru.asuprofi.viewModel.links.PortDirection;

public class Thermometer extends BaseObject {

    private Double Tl;
    private Double Th;
    private MeasurementUnit unit;

    public Thermometer() {
        super();
        this.type = ObjectType.Thermometer;
        this.objectClass = ObjectClass.InstrumentUnit;

        Port m1 = new Port("m1");

        m1.setParent(this);

        m1.x.set(12);
        m1.y.set(36);

        m1.padding = 5.0;

        m1.setSourceOf(LinkClass.Signal);
        m1.setPortDirection(PortDirection.Input);
        this.setMainVariable("T");

        this.portsList.add(m1);

        this.w.set(24.0);
        this.h.set(41.0);
        this.setTl(0.0);
        this.setTh(0.0);
        this.setUnit(MeasurementUnit.K);
    }

    @Override
    public Element XMLexport(Document document) {
        Element res = document.createElement(this.type.toString());
        res.setAttribute("Id", this.getId());
        res.setAttribute("X", Double.toString(this.x.get()));
        res.setAttribute("Y", Double.toString(this.y.get()));
        res.setAttribute("Tl", Double.toString(this.getTl()));
        res.setAttribute("Th", Double.toString(this.getTh()));
        res.setAttribute("unit", this.getUnit().toString());
        return res;
    }

    @Override
    public void XMLimport(Document document, Element elem) {
        this.setId(elem.getAttribute("Id"));
        this.x.set(Double.parseDouble(elem.hasAttribute("X") ? elem.getAttribute("X") : String.valueOf(this.x.get())));
        this.y.set(Double.parseDouble(elem.hasAttribute("Y") ? elem.getAttribute("Y") : String.valueOf(this.y.get())));
        this.setTl(Double.parseDouble(elem.hasAttribute("Tl") ? elem.getAttribute("Tl") : "0"));
        this.setTh(Double.parseDouble(elem.hasAttribute("Th") ? elem.getAttribute("Th") : "0"));
        this.setUnit(elem.hasAttribute("unit") ? MeasurementUnit.valueOf(elem.getAttribute("unit")) : MeasurementUnit.K);
    }

    @Override
    void initialize() {

    }

    public Double getTl() {
        return Tl;
    }

    public void setTl(Double tl) {
        Tl = tl;
    }

    public Double getTh() {
        return Th;
    }

    public void setTh(Double th) {
        Th = th;
    }

//    public Double getTs() {
//        return Ts;
//    }
//
//    public void setTs(Double ts) {
//        Ts = ts;
//    }

    public MeasurementUnit getUnit() {
        return unit;
    }

    public void setUnit(MeasurementUnit unit) {
        this.unit = unit;
    }

}
