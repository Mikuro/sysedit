package ru.asuprofi.viewModel.objects;

import org.w3c.dom.Document;
import org.w3c.dom.Element;
import ru.asuprofi.viewModel.MeasurementUnit;
import ru.asuprofi.viewModel.ObjectClass;
import ru.asuprofi.viewModel.ObjectType;
import ru.asuprofi.viewModel.links.LinkClass;
import ru.asuprofi.viewModel.links.Port;
import ru.asuprofi.viewModel.links.PortDirection;

public class FlowMeter extends BaseObject {

    private Double Fmax;
    private Double Fmin;
    private MeasurementUnit unit;
    private MeasurementUnit t_unit;
//    private Integer s1;
//    private Integer FD;
//    private Double Ffail;
//    private Double Factor;
//    private Double Offset;

    public FlowMeter() {
        super();
        this.type = ObjectType.FlowMeter;
        this.objectClass = ObjectClass.InstrumentUnit;

        Port f = new Port("m1");

        f.setParent(this);

        f.x.set(12);
        f.y.set(36);

        f.padding = 5.0;

        f.setSourceOf(LinkClass.Signal);
        f.setPortDirection(PortDirection.Input);
        this.setMainVariable("F");

        this.portsList.add(f);

        this.w.set(24.0);
        this.h.set(41.0);

        this.setFmax(2000.0);
        this.setFmin(0.0);
        this.setUnit(MeasurementUnit.m3);
        this.setT_unit(MeasurementUnit.h);
//        this.setS1(1);
//        this.setFD(0);
//        this.setFfail(0.0);
//        this.setFactor(1.0);
//        this.setOffset(0.0);
    }

    @Override
    public Element XMLexport(Document document) {
        Element res = document.createElement(this.type.toString());
        res.setAttribute("Id", this.getId());
        res.setAttribute("X", Double.toString(this.x.get()));
        res.setAttribute("Y", Double.toString(this.y.get()));
        res.setAttribute("Fmax", Double.toString(this.getFmax()));
        res.setAttribute("Fmin", Double.toString(this.getFmin()));
        res.setAttribute("unit", this.getUnit().toString());
        res.setAttribute("t_unit", this.getT_unit().toString());
//        res.setAttribute("s1", Integer.toString(this.getS1()));
//        res.setAttribute("FD", Integer.toString(this.getFD()));
//        res.setAttribute("Ffail", Double.toString(this.getFfail()));
//        res.setAttribute("Factor", Double.toString(this.getFactor()));
//        res.setAttribute("Offset", Double.toString(this.getOffset()));
        return res;
    }

    @Override
    public void XMLimport(Document document, Element elem) {
        this.setId(elem.getAttribute("Id"));
        this.x.set(Double.parseDouble(elem.hasAttribute("X") ? elem.getAttribute("X") : String.valueOf(this.x.get())));
        this.y.set(Double.parseDouble(elem.hasAttribute("Y") ? elem.getAttribute("Y") : String.valueOf(this.y.get())));
        this.setFmax(Double.parseDouble(elem.hasAttribute("Fmax") ? elem.getAttribute("Fmax") : "0"));
        this.setFmin(Double.parseDouble(elem.hasAttribute("Fmin") ? elem.getAttribute("Fmin") : "0"));
        this.setUnit(elem.hasAttribute("unit") ? MeasurementUnit.valueOf(elem.getAttribute("unit")) : MeasurementUnit.m3);
        this.setT_unit(elem.hasAttribute("t_unit") ? MeasurementUnit.valueOf(elem.getAttribute("t_unit")) : MeasurementUnit.h);
//        this.setS1(Integer.parseInt(elem.hasAttribute("s1") ? elem.getAttribute("s1") : "0"));
//        this.setFD(Integer.parseInt(elem.hasAttribute("FD") ? elem.getAttribute("FD") : "0"));
//        this.setFfail(Double.parseDouble(elem.hasAttribute("Ffail") ? elem.getAttribute("Ffail") : "0"));
//        this.setOffset(Double.parseDouble(elem.hasAttribute("Offset") ? elem.getAttribute("Offset") : "0"));
    }

    @Override
    void initialize() {

    }

    public Double getFmax() {
        return Fmax;
    }

    public void setFmax(Double fmax) {
        Fmax = fmax;
    }

    public Double getFmin() {
        return Fmin;
    }

    public void setFmin(Double fmin) {
        Fmin = fmin;
    }

    public MeasurementUnit getUnit() {
        return unit;
    }

    public void setUnit(MeasurementUnit unit) {
        this.unit = unit;
    }

    public MeasurementUnit getT_unit() {
        return t_unit;
    }

    public void setT_unit(MeasurementUnit t_unit) {
        this.t_unit = t_unit;
    }

//    public Integer getS1() {
//        return s1;
//    }
//
//    public void setS1(Integer s1) {
//        this.s1 = s1;
//    }
//
//    public Integer getFD() {
//        return FD;
//    }
//
//    public void setFD(Integer FD) {
//        this.FD = FD;
//    }
//
//    public Double getFfail() {
//        return Ffail;
//    }
//
//    public void setFfail(Double ffail) {
//        Ffail = ffail;
//    }
//
//    public Double getFactor() {
//        return Factor;
//    }
//
//    public void setFactor(Double factor) {
//        Factor = factor;
//    }
//
//    public Double getOffset() {
//        return Offset;
//    }
//
//    public void setOffset(Double offset) {
//        Offset = offset;
//    }
}
