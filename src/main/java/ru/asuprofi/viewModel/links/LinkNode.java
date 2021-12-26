package ru.asuprofi.viewModel.links;

import javafx.beans.property.BooleanProperty;
import javafx.beans.property.DoubleProperty;
import javafx.beans.property.SimpleBooleanProperty;
import javafx.beans.property.SimpleDoubleProperty;
import javafx.util.Pair;
import ru.asuprofi.viewModel.FlowDiagram;
import ru.asuprofi.viewModel.objects.BaseObject;

import java.util.ArrayList;
import java.util.List;

public class LinkNode extends LinkElement {
    final List<LinkNode> neighbourOnLinkNodes = new ArrayList<>();
    private final DoubleProperty x;
    private final DoubleProperty y;
    private final BooleanProperty isOnLinkFinish = new SimpleBooleanProperty(false);
    public Link linkFinish;
    public BaseObject objectFinishOutPort;
    public FlowDiagram parent;
    public String pseudoPortName;
    private BooleanProperty isPort = new SimpleBooleanProperty(false);

    public LinkNode(FlowDiagram parent) {
        this.x = new SimpleDoubleProperty();
        this.y = new SimpleDoubleProperty();
        this.parent = parent;
    }

    public LinkNode(Double x, Double y, FlowDiagram parent) {
        this.x = new SimpleDoubleProperty(x);
        this.y = new SimpleDoubleProperty(y);
        this.parent = parent;
    }

    public LinkNode(DoubleProperty x, DoubleProperty y, FlowDiagram parent) {
        this.x = x;
        this.y = y;
        this.parent = parent;
    }

    public LinkNode(Pair<Double, Double> linkCoords) {
        this.x = new SimpleDoubleProperty(linkCoords.getKey());
        this.y = new SimpleDoubleProperty(linkCoords.getValue());
    }

    public void addNeighbourLinkNode(LinkNode linkNeighbour) {
        this.neighbourOnLinkNodes.add(linkNeighbour);
    }

    @Override
    public DoubleProperty getXProperty() {
        return x;
    }

    @Override
    public DoubleProperty getYProperty() {
        return y;
    }

    public boolean getIsPortFlag() {
        return isPort.get();
    }

    public void setIsPortFlag(boolean flag) {
        isPort = new SimpleBooleanProperty(flag);
    }

    public Boolean getIsOnLinkFinish() {
        return isOnLinkFinish.get();
    }

    public void setIsOnLinkFinish(Boolean flag) {
        isOnLinkFinish.set(flag);
        if (isOnLinkFinish.get()) {
            isSelected.bind(linkFinish.isSelected);
        } else {
            if (!getIsPortFlag()) {
                isSelected.bind(objectFinishOutPort.isSelected);
            }
        }
    }

    @Override
    public String getParentId() {
        if (isOnLinkFinish.get())
            return linkFinish.getId();
        else {
            return objectFinishOutPort.getId();
        }
    }

    @Override
    public String getName() {
        if (isOnLinkFinish.get())
            return null;
        else
            return pseudoPortName;
    }

    @Override
    public String toString() {
        return getXProperty().get() + " " + getYProperty().get() + " ";
    }
}
