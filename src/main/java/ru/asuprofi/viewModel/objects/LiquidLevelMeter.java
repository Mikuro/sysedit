package ru.asuprofi.viewModel.objects;

import org.w3c.dom.Document;
import org.w3c.dom.Element;
import ru.asuprofi.viewModel.MeasurementUnit;
import ru.asuprofi.viewModel.ObjectClass;
import ru.asuprofi.viewModel.ObjectType;
import ru.asuprofi.viewModel.links.LinkClass;
import ru.asuprofi.viewModel.links.Port;
import ru.asuprofi.viewModel.links.PortDirection;

public class LiquidLevelMeter extends BaseObject {

    private Double Ll;
    private Double Lh;
    private Double Lbase;
    private MeasurementUnit unit;

    public LiquidLevelMeter() {
        super();
        this.type = ObjectType.LiquidLevelMeter;
        this.objectClass = ObjectClass.InstrumentUnit;

        Port L = new Port("l0");

        L.setParent(this);

        L.x.set(12);
        L.y.set(36);

        L.padding = 5.0;

        L.setSourceOf(LinkClass.Signal);
        L.setPortDirection(PortDirection.Input);
        this.setMainVariable("L");

        this.portsList.add(L);

        this.w.set(24.0);
        this.h.set(41.0);

        this.setLl(0.0);
        this.setLh(20.0);
        this.setLbase(0.0);
        this.setUnit(MeasurementUnit.m);
    }

    @Override
    public Element XMLexport(Document document) {
        Element res = document.createElement(this.type.toString());
        res.setAttribute("Id", this.getId());
        res.setAttribute("X", Double.toString(this.x.get()));
        res.setAttribute("Y", Double.toString(this.y.get()));
        res.setAttribute("Ll", Double.toString(this.getLl()));
        res.setAttribute("Lh", Double.toString(this.getLh()));
        res.setAttribute("Lbase", Double.toString(this.getLbase()));
        res.setAttribute("unit", this.getUnit().toString());
        return res;
    }

    @Override
    public void XMLimport(Document document, Element elem) {
        this.setId(elem.getAttribute("Id"));
        this.x.set(Double.parseDouble(elem.hasAttribute("X") ? elem.getAttribute("X") : String.valueOf(this.x.get())));
        this.y.set(Double.parseDouble(elem.hasAttribute("Y") ? elem.getAttribute("Y") : String.valueOf(this.y.get())));
        this.setLl(Double.parseDouble(elem.hasAttribute("Ll") ? elem.getAttribute("Ll") : "0"));
        this.setLh(Double.parseDouble(elem.hasAttribute("Lh") ? elem.getAttribute("Lh") : "0"));
        this.setLbase(Double.parseDouble(elem.hasAttribute("Lbase") ? elem.getAttribute("Lbase") : "0"));
        this.setUnit(elem.hasAttribute("unit") ? MeasurementUnit.valueOf(elem.getAttribute("unit")) : MeasurementUnit.m);
    }

    @Override
    void initialize() {

    }


    public Double getLl() {
        return Ll;
    }

    public void setLl(Double ll) {
        Ll = ll;
    }

    public Double getLh() {
        return Lh;
    }

    public void setLh(Double lh) {
        Lh = lh;
    }

    public Double getLbase() {
        return Lbase;
    }

    public void setLbase(Double lbase) {
        Lbase = lbase;
    }

    public MeasurementUnit getUnit() {
        return unit;
    }

    public void setUnit(MeasurementUnit unit) {
        this.unit = unit;
    }
}
