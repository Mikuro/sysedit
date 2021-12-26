package ru.asuprofi.viewModel.objects;

import org.w3c.dom.Document;
import org.w3c.dom.Element;
import ru.asuprofi.viewModel.MeasurementUnit;
import ru.asuprofi.viewModel.ObjectClass;
import ru.asuprofi.viewModel.ObjectType;
import ru.asuprofi.viewModel.links.LinkClass;
import ru.asuprofi.viewModel.links.Port;
import ru.asuprofi.viewModel.links.PortDirection;

public class PressureGauge extends BaseObject {

    private Double Pl;
    private Double Ph;
    private MeasurementUnit unit;

    public PressureGauge() {
        super();
        this.type = ObjectType.PressureGauge;
        this.objectClass = ObjectClass.InstrumentUnit;

        Port m = new Port("m");

        m.setParent(this);

        m.x.set(12);
        m.y.set(36);

        m.padding = 5.0;

        m.setSourceOf(LinkClass.Signal);
        m.setPortDirection(PortDirection.Input);

        this.portsList.add(m);

        this.w.set(24.0);
        this.h.set(41.0);
        this.setPl(0.0);
        this.setPh(300.0);
        this.setUnit(MeasurementUnit.kg_cm2);
//        this.setPfail(0.0);
//        this.setFactor(1.0);
//        this.setOffset(0.0);
    }

    @Override
    public Element XMLexport(Document document) {
        Element res = document.createElement(this.type.toString());
        res.setAttribute("Id", this.getId());
        res.setAttribute("X", Double.toString(this.x.get()));
        res.setAttribute("Y", Double.toString(this.y.get()));
        res.setAttribute("Pl", Double.toString(this.getPl()));
        res.setAttribute("Ph", Double.toString(this.getPh()));
        res.setAttribute("unit", this.getUnit().toString());
//        res.setAttribute("pfail", Double.toString(this.getPfail()));
//        res.setAttribute("Factor", Double.toString(this.getFactor()));
//        res.setAttribute("Offset", Double.toString(this.getOffset()));
        return res;
    }

    @Override
    public void XMLimport(Document document, Element elem) {
        this.setId(elem.getAttribute("Id"));
        this.x.set(Double.parseDouble(elem.hasAttribute("X") ? elem.getAttribute("X") : String.valueOf(this.x.get())));
        this.y.set(Double.parseDouble(elem.hasAttribute("Y") ? elem.getAttribute("Y") : String.valueOf(this.y.get())));
        this.setPl(Double.parseDouble(elem.hasAttribute("Pl") ? elem.getAttribute("Pl") : "0"));
        this.setPh(Double.parseDouble(elem.hasAttribute("Ph") ? elem.getAttribute("Ph") : "0"));
        this.setUnit(elem.hasAttribute("unit") ? MeasurementUnit.valueOf(elem.getAttribute("unit")) : MeasurementUnit.kg_cm2);
//        this.setPfail(Double.parseDouble(elem.hasAttribute("pfail") ? elem.getAttribute("pfail") : "0"));
//        this.setFactor(Double.parseDouble(elem.hasAttribute("Factor") ? elem.getAttribute("Factor") : "0"));
//        this.setOffset(Double.parseDouble(elem.hasAttribute("offset") ? elem.getAttribute("Offset") : "0"));
    }

    @Override
    void initialize() {

    }

    public Double getPl() {
        return Pl;
    }

    public void setPl(Double pl) {
        Pl = pl;
    }

    public Double getPh() {
        return Ph;
    }

    public void setPh(Double ph) {
        Ph = ph;
    }

    public MeasurementUnit getUnit() {
        return unit;
    }

    public void setUnit(MeasurementUnit unit) {
        this.unit = unit;
    }

//    public Double getPfail() {
//        return pfail;
//    }
//
//    public void setPfail(Double pfail) {
//        this.pfail = pfail;
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
