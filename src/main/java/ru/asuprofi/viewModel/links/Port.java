package ru.asuprofi.viewModel.links;

import javafx.beans.property.DoubleProperty;
import javafx.beans.property.SimpleBooleanProperty;
import javafx.beans.property.SimpleDoubleProperty;
import ru.asuprofi.viewModel.objects.BaseObject;

public class Port extends LinkElement {

    public final SimpleBooleanProperty isBusy = new SimpleBooleanProperty(false);
    public final SimpleDoubleProperty x = new SimpleDoubleProperty();
    public final SimpleDoubleProperty y = new SimpleDoubleProperty();
    private final String name;
    private BaseObject parent = null;//if signal from node - this field is not null
    private LinkClass sourceOf;//what linkClass this port is source of

    private String targetName;//target name of field it get from objects

    private PortDirection portDirection;

    public Port() {
        this.name = "";
        this.sourceOf = LinkClass.Stream;
    }

    public Port(String name) {
        this.name = name;
        this.sourceOf = LinkClass.Stream;
    }

    public BaseObject getParent() {
        return parent;
    }

    public void setParent(BaseObject parent) {
        this.parent = parent;
        this.absoluteX.bind(parent.x.add(x));
        this.absoluteY.bind(parent.y.add(y));
    }

    @Override
    public DoubleProperty getXProperty() {
        return absoluteX;
    }

    @Override
    public DoubleProperty getYProperty() {
        return absoluteY;
    }

    @Override
    public String getParentId() {
        return getParent().getId();
    }

    public LinkClass getSourceOf() {
        return sourceOf;
    }

    public void setSourceOf(LinkClass linkClass) {
        this.sourceOf = linkClass;
    }

    @Override
    public String getName() {
        return this.name;
    }

    public String getTargetName() {
        return this.targetName;
    }

    public void setTargetName(String name) {
        this.targetName = name;
    }

    public PortDirection getPortDirection() {
        return this.portDirection;
    }

    public void setPortDirection(PortDirection direction) {
        this.portDirection = direction;
    }
}