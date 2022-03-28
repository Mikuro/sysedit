package ru.asuprofi.viewModel.objects;

import org.w3c.dom.Document;
import org.w3c.dom.Element;
import ru.asuprofi.viewModel.ObjectClass;
import ru.asuprofi.viewModel.ObjectType;
import ru.asuprofi.viewModel.links.LinkClass;
import ru.asuprofi.viewModel.links.Port;
import ru.asuprofi.viewModel.links.PortDirection;

public class PIDController extends BaseObject {

    private PIDDirection INCDEC;
    private Double PVSPAN;

    private Double PB;
    private Double TI;
    private Double TD;
    private PIDMode CMOD;
    private Double MVSPAN;
    private Double MVH;
    private Double MVL;
    private Double PVBASE;
    private Double MVM;
    private Double MV;
    private Double e;
    private Double PVE;

    public PIDController() {
        super();
        this.type = ObjectType.PIDController;
        this.objectClass = ObjectClass.InstrumentUnit;

        Port mve = new Port("MVE");
        Port pve = new Port("PVE");
        Port cmv = new Port("cmv");

        mve.setTargetName("mv");
        cmv.setTargetName("csv");

        mve.setParent(this);
        pve.setParent(this);
        cmv.setParent(this);

        mve.x.set(72);
        mve.y.set(6);

        pve.x.set(9);
        pve.y.set(15);

        cmv.x.set(72);
        cmv.y.set(24);

        mve.padding = 7.0;
        pve.padding = 7.0;
        cmv.padding = 7.0;

        mve.setSourceOf(LinkClass.Signal);
        pve.setSourceOf(LinkClass.Signal);
        cmv.setSourceOf(LinkClass.Signal);

        mve.setPortDirection(PortDirection.Output);
        pve.setPortDirection(PortDirection.Input);
        cmv.setPortDirection(PortDirection.Output);

        this.portsList.add(mve);
        this.portsList.add(pve);
        this.portsList.add(cmv);

        this.w.set(78.0);
        this.h.set(30.0);

        this.setPB(20.0);
        this.setTI(120.0);
        this.setTD(0.0);
        this.setINCDEC(PIDDirection.Direct);
        this.setMVSPAN(1500.0);
        this.setMVH(1500.0);
        this.setMVL(0.0);

        this.setCMOD(PIDMode.AUT);
        this.setPVSPAN(20.0);
        this.setPVBASE(0.0);
        this.setMVM(58.03);
        this.setMV(58.03);
        this.setE(0.0);
        this.setPVE(5.0);
    }

    @Override
    public Element XMLexport(Document document) {
        Element res = document.createElement(this.type.toString());
        res.setAttribute("Id", this.getId());
        res.setAttribute("X", Double.toString(this.x.get()));
        res.setAttribute("Y", Double.toString(this.y.get()));
        res.setAttribute("PB", Double.toString(this.getPB()));
        res.setAttribute("TI", Double.toString(this.getTI()));
        res.setAttribute("TD", Double.toString(this.getTD()));
        res.setAttribute("INCDEC", this.getINCDEC().toString());
        res.setAttribute("MVSPAN", Double.toString(this.getMVSPAN()));
        res.setAttribute("MVH", Double.toString(this.getMVH()));
        res.setAttribute("MVL", Double.toString(this.getMVL()));

        res.setAttribute("CMOD", this.getCMOD().toString());
        res.setAttribute("PVSPAN", Double.toString(this.getPVSPAN()));
        res.setAttribute("PVBASE", Double.toString(this.getPVBASE()));
        res.setAttribute("MVM", Double.toString(this.getMVM()));
        res.setAttribute("MV", Double.toString(this.getMV()));
        res.setAttribute("E", Double.toString(this.getE()));
        res.setAttribute("PVE", Double.toString(this.getPVE()));

        return res;
    }

    @Override
    public void XMLimport(Document document, Element elem) {
        this.setId(elem.getAttribute("Id"));
        this.x.set(Double.parseDouble(elem.hasAttribute("X") ? elem.getAttribute("X") : String.valueOf(this.x.get())));
        this.y.set(Double.parseDouble(elem.hasAttribute("Y") ? elem.getAttribute("Y") : String.valueOf(this.y.get())));
        this.setPB(Double.parseDouble(elem.hasAttribute("PB") ? elem.getAttribute("PB") : "0"));
        this.setTI(Double.parseDouble(elem.hasAttribute("TI") ? elem.getAttribute("TI") : "0"));
        this.setTD(Double.parseDouble(elem.hasAttribute("TD") ? elem.getAttribute("TD") : "0"));
        this.setINCDEC(elem.hasAttribute("INCDEC") ? PIDDirection.valueOf(elem.getAttribute("INCDEC")) : PIDDirection.Direct);
        this.setMVSPAN(Double.parseDouble(elem.hasAttribute("MVSPAN") ? elem.getAttribute("MVSPAN") : "0"));
        this.setMVH(Double.parseDouble(elem.hasAttribute("MVH") ? elem.getAttribute("MVH") : "0"));
        this.setMVL(Double.parseDouble(elem.hasAttribute("MVL") ? elem.getAttribute("MVL") : "0"));

        this.setCMOD(elem.hasAttribute("CMOD") ? PIDMode.valueOf(elem.getAttribute("CMOD")) : PIDMode.AUT);
        this.setPVSPAN(Double.parseDouble(elem.hasAttribute("PVSPAN") ? elem.getAttribute("PVSPAN") : "0"));
        this.setPVBASE(Double.parseDouble(elem.hasAttribute("PVBASE") ? elem.getAttribute("PVBASE") : "0"));
        this.setMVM(Double.parseDouble(elem.hasAttribute("MVM") ? elem.getAttribute("MVM") : "0"));
        this.setMV(Double.parseDouble(elem.hasAttribute("MV") ? elem.getAttribute("MV") : "0"));
        this.setE(Double.parseDouble(elem.hasAttribute("E") ? elem.getAttribute("PVSPAN") : "0"));
        this.setPVE(Double.parseDouble(elem.hasAttribute("PVE") ? elem.getAttribute("PVE") : "0"));
    }

    public PIDDirection getINCDEC() {
        return INCDEC;
    }

    public void setINCDEC(PIDDirection INCDEC) {
        this.INCDEC = INCDEC;
    }

    @Override
    void initialize() {

    }

    public Double getPB() {
        return PB;
    }

    public void setPB(Double PB) {
        this.PB = PB;
    }

    public Double getTI() {
        return TI;
    }

    public void setTI(Double TI) {
        this.TI = TI;
    }

    public Double getTD() {
        return TD;
    }

    public void setTD(Double TD) {
        this.TD = TD;
    }

    public Double getPVSPAN() {
        return PVSPAN;
    }

    public void setPVSPAN(Double PVSPAN) {
        this.PVSPAN = PVSPAN;
    }

    public Double getMVSPAN() {
        return MVSPAN;
    }

    public void setMVSPAN(Double MVSPAN) {
        this.MVSPAN = MVSPAN;
    }

    public Double getMVH() {
        return MVH;
    }

    public void setMVH(Double MVH) {
        this.MVH = MVH;
    }

    public Double getMVL() {
        return MVL;
    }

    public void setMVL(Double MVL) {
        this.MVL = MVL;
    }

    public PIDMode getCMOD() {
        return CMOD;
    }

    public void setCMOD(PIDMode CMOD) {
        this.CMOD = CMOD;
    }

    public Double getPVBASE() {
        return PVBASE;
    }

    public void setPVBASE(Double PVBASE) {
        this.PVBASE = PVBASE;
    }

    public Double getMVM() {
        return MVM;
    }

    public void setMVM(Double MVM) {
        this.MVM = MVM;
    }

    public Double getMV() {
        return MV;
    }

    public void setMV(Double MV) {
        this.MV = MV;
    }

    public Double getE() {
        return e;
    }

    public void setE(Double e) {
        this.e = e;
    }

    public Double getPVE() {
        return PVE;
    }

    public void setPVE(Double PVE) {
        this.PVE = PVE;
    }

    public enum PIDDirection {
        Direct,
        Reverse
    }

    public enum PIDMode {
        MAN,
        AUT,
        CAS
    }
}
