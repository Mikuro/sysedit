package ru.asuprofi.view.links;

import javafx.beans.property.DoubleProperty;
import javafx.beans.property.IntegerProperty;
import javafx.beans.property.SimpleIntegerProperty;
import javafx.scene.input.MouseButton;
import javafx.scene.input.MouseEvent;
import javafx.scene.paint.Color;
import javafx.scene.shape.LineTo;
import javafx.scene.shape.MoveTo;
import javafx.scene.shape.Path;
import javafx.scene.shape.PathElement;
import javafx.util.Pair;
import ru.asuprofi.view.FlowDiagramView;
import ru.asuprofi.viewModel.links.Link;
import ru.asuprofi.viewModel.links.LinkNode;
import ru.asuprofi.viewModel.links.Port;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

public class LinkView extends Path {

    public final List<LinkNode> subPort = new ArrayList<>();
    public final Map<LinkNode, LinkNodeView> nodeControl;
    public Link viewModel;
    public FlowDiagramView parent;
    Line lastLine;
    private LinkNode currentSubPort = null; // current node, which is target on link or baseobject

    public LinkView() {
        nodeControl = new HashMap<>();
    }

    public void initLink(Pair<DoubleProperty, DoubleProperty> port) {
        viewModel.linkNodeList.clear();
        MoveTo startPoint = new MoveTo();
        startPoint.xProperty().bind(port.getKey().add(viewModel.getSource().getXPadding()));
        startPoint.yProperty().bind(port.getValue().add(viewModel.getSource().getYPadding()));
        lastLine = new Line(port.getKey().get(), port.getValue().get(), OrientationType.dot);
        this.getElements().add(startPoint);
        this.parent.viewModel.setLinkFromSourceProperty(false);
        lastLine.portConnect = viewModel.getSource();
        LinkNode linkNode = new LinkNode(parent.viewModel);
        linkNode.getXProperty().bind(startPoint.xProperty());
        linkNode.getYProperty().bind(startPoint.yProperty());
        linkNode.setIsPortFlag(true);
        LinkNodeView nodeView = new LinkNodeView();
        lastLine.firstLinkNode = linkNode;
        lastLine.lastLinkNode = linkNode;
        nodeView.setContext(linkNode, parent);
        nodeControl.put(linkNode, nodeView);
        viewModel.addLinkNode(linkNode);
        parent.getChildren().add(nodeView);
        viewModel.getSource().isBusy.set(true);
        if (parent.viewModel.getLinkPaintProperty())
            viewModel.linkDots.add(new Pair<>(linkNode.getXProperty(), linkNode.getYProperty()));
    }

    public void finishLink(Pair<DoubleProperty, DoubleProperty> port) {
        Line newLine;

        if (
                Math.abs(port.getKey().get() - lastLine.lastLinkNode.getXProperty().get())
                        >= Math.abs(port.getValue().get() - lastLine.lastLinkNode.getYProperty().get())
        ) {
            newLine = new Line(port.getKey().get(), lastLine.getY(), OrientationType.horizontal);
        } else {
            newLine = new Line(lastLine.getX(), port.getValue().get(), OrientationType.vertical);
        }

        if (viewModel.getTarget() instanceof Port) {
            switch (lastLine.type) {
                case horizontal -> lastLine.lastLinkNode.getXProperty().bind(port.getKey().add(viewModel.getTarget().getXPadding()));
                case vertical -> lastLine.lastLinkNode.getYProperty().bind(port.getValue().add(viewModel.getTarget().getYPadding()));
            }
        } else {
            switch (lastLine.type) {
                case horizontal -> lastLine.lastLinkNode.getXProperty().bind(port.getKey());
                case vertical -> lastLine.lastLinkNode.getYProperty().bind(port.getValue());
            }
        }

        LinkNode linkNode;
        LinkNodeView nodeView;

        if (viewModel.getTarget() instanceof Port) {
            linkNode = new LinkNode(port.getKey().get(), port.getValue().get(), parent.viewModel);
            nodeView = new LinkNodeView();
            linkNode.setIsPortFlag(true);
        } else {
            linkNode = (LinkNode) viewModel.getTarget();
            nodeView = nodeControl.get(linkNode);
            linkNode.setIsPortFlag(false);
        }

        newLine.firstLinkNode = lastLine.lastLinkNode;

        if (lastLine.type.equals(OrientationType.dot)) {
            switch (newLine.type) {
                case horizontal -> viewModel.getSource().getXPadding().bind(newLine.getDirectionNumber().multiply(viewModel.getSource().padding));
                case vertical -> viewModel.getSource().getYPadding().bind(newLine.getDirectionNumber().multiply(viewModel.getSource().padding));
            }
        }

        newLine.xProperty().unbind();
        newLine.xProperty().bind(linkNode.getXProperty());
        newLine.yProperty().unbind();
        newLine.yProperty().bind(linkNode.getYProperty());

        if (lastLine.type.equals(newLine.type)) {
            switch (lastLine.type) {
                case horizontal -> {
                    lastLine.lastLinkNode.getXProperty().bind(viewModel.getTarget().getXProperty().add(viewModel.getTarget().getXPadding()));
                    lastLine.lastLinkNode.getYProperty().unbind();
                    lastLine.lastLinkNode.getYProperty().bind(viewModel.getTarget().getYProperty().add(viewModel.getTarget().getYPadding()));
                }
                case vertical -> {
                    lastLine.lastLinkNode.getYProperty().bind(viewModel.getTarget().getYProperty().add(viewModel.getTarget().getYPadding()));
                    lastLine.lastLinkNode.getXProperty().unbind();
                    lastLine.lastLinkNode.getXProperty().bind(viewModel.getTarget().getXProperty().add(viewModel.getTarget().getXPadding()));
                }
            }

            lastLine.controlOrientation();
        } else {

            if (parent.viewModel.getLinkPaintProperty())
                viewModel.linkDots.add(new Pair<>(linkNode.getXProperty(), linkNode.getYProperty()));

            this.getElements().add(newLine);
            viewModel.addLinkNode(linkNode);
            if (viewModel.getTarget() instanceof Port) {
                nodeView.setContext(linkNode, parent);
                newLine.lastLinkNode = linkNode;
                parent.getChildren().add(nodeView);
                nodeControl.put(linkNode, nodeView);
                newLine.controlOrientation();
            }
            lastLine = newLine;
        }

        if (viewModel.getTarget() instanceof Port) {
            switch (lastLine.type) {
                case horizontal -> {
                    viewModel.getTarget().getXPadding().bind(lastLine.getDirectionNumber().multiply(viewModel.getTarget().padding * (-1)));
                    linkNode.getXProperty().bind(viewModel.getTarget().getXPadding().add(viewModel.getTarget().getXProperty()));
                    linkNode.getYProperty().bind(viewModel.getTarget().getYProperty());
                }
                case vertical -> {
                    viewModel.getTarget().getYPadding().bind(lastLine.getDirectionNumber().multiply(viewModel.getTarget().padding * (-1)));
                    linkNode.getYProperty().bind(viewModel.getTarget().getYPadding().add(viewModel.getTarget().getYProperty()));
                    linkNode.getXProperty().bind(viewModel.getTarget().getXProperty());
                }
            }
        }

        lastLine = null;
        if (viewModel.getTarget() instanceof Port)
            ((Port) viewModel.getTarget()).isBusy.set(true);
        parent.currentLinkView = null;
    }

    public void addLine(Pair<DoubleProperty, DoubleProperty> point) {
        LinkNode linkNode = new LinkNode(point.getKey(), point.getValue(), parent.viewModel);
        linkNode.setIsPortFlag(false);

        OrientationType type;

        if (Math.abs(point.getKey().get() - lastLine.lastLinkNode.getXProperty().get()) >= Math.abs(point.getValue().get() - lastLine.lastLinkNode.getYProperty().get())) {
            linkNode.getYProperty().bind(lastLine.lastLinkNode.getYProperty());
            type = OrientationType.horizontal;
        } else {
            linkNode.getXProperty().bind(lastLine.lastLinkNode.getXProperty());
            type = OrientationType.vertical;
        }

        Line newLine = new Line(linkNode.getXProperty().get(), linkNode.getYProperty().get(), type);

        newLine.xProperty().bind(linkNode.getXProperty());
        newLine.yProperty().bind(linkNode.getYProperty());

        if (lastLine.type.equals(OrientationType.dot)) {
            switch (newLine.type) {
                case horizontal -> viewModel.getSource().getXPadding().bind(newLine.getDirectionNumber().multiply(viewModel.getSource().padding));
                case vertical -> viewModel.getSource().getYPadding().bind(newLine.getDirectionNumber().multiply(viewModel.getSource().padding));
            }
        }

        if (lastLine.type.equals(newLine.type)) {
            switch (lastLine.type) {
                case horizontal -> lastLine.lastLinkNode.getXProperty().set(newLine.xProperty().get());
                case vertical -> lastLine.lastLinkNode.getYProperty().set(newLine.yProperty().get());
            }
        } else {
            this.getElements().add(newLine);

            if (parent.viewModel.getLinkPaintProperty())
                viewModel.linkDots.add(new Pair<>(linkNode.getXProperty(), linkNode.getYProperty()));

            newLine.firstLinkNode = lastLine.lastLinkNode;
            lastLine = newLine;
            LinkNodeView nodeView = new LinkNodeView();
            nodeView.setContext(linkNode, parent);
            nodeControl.put(linkNode, nodeView);
            viewModel.addLinkNode(linkNode);
            lastLine.lastLinkNode = linkNode;
            newLine.controlOrientation();
            parent.getChildren().add(nodeView);
        }
    }

    public void setContext(Link viewModel, FlowDiagramView parent) {
        this.viewModel = viewModel;

        switch (viewModel.getLinkClass()) {
            case Stream -> {
                switch (viewModel.getType()) {
                    case Liquid -> setStroke(new Color(0.0, 212.0 / 255.0, 245.0 / 255.0, 1));
                    case Vapor -> setStroke(Color.ORANGE);
                    case VaporAndLiquid -> setStroke(Color.MAGENTA);
                }
                setStrokeWidth(5);
            }
            case Signal -> {
                this.getStrokeDashArray().addAll(1.0, 2.0);
                this.setStroke(Color.BLACK);
                setStrokeWidth(1);
            }
        }

        viewModel.isSelected.set(false);

        this.setOnMouseClicked(mouseEvent -> {
            if (parent.viewModel.getLinkPaintProperty()) {
                if (mouseEvent.getTarget() instanceof LinkView) {
                    for (PathElement i : getElements()) {
                        if (i instanceof Line) {
                            LinkNode linkNode = new LinkNode(parent.viewModel);
                            LinkNodeView nodeView = new LinkNodeView();
                            nodeView.setContext(linkNode, parent);
                            Line tmp = (Line) i;
                            switch (tmp.type) {
                                case vertical -> {
                                    if (Math.abs(mouseEvent.getX() - tmp.xProperty().get()) <= 2 &&
                                            mouseEvent.getY() > Math.min(tmp.firstLinkNode.getYProperty().get(), tmp.lastLinkNode.getYProperty().get()) &&
                                            mouseEvent.getY() < Math.max(tmp.firstLinkNode.getYProperty().get(), tmp.lastLinkNode.getYProperty().get())
                                    ) {
                                        linkNode.getXProperty().bind(tmp.firstLinkNode.getXProperty());
                                        linkNode.getYProperty().set(parent.currentLinkView.lastLine.lastLinkNode.getYProperty().get());
                                        parent.currentLinkView.nodeControl.put(linkNode, nodeView);
                                        parent.getChildren().add(nodeView);
                                        currentSubPort = linkNode;
                                        nodeView.removeEventHandler(MouseEvent.MOUSE_CLICKED, nodeView.getOnMouseMove());
                                        nodeView.setOnMouseMoved(mouseEvent12 -> {
                                            if (mouseEvent12.getY() > Math.min(tmp.firstLinkNode.getYProperty().get(), tmp.lastLinkNode.getYProperty().get()) &&
                                                    mouseEvent12.getY() < Math.max(tmp.firstLinkNode.getYProperty().get(), tmp.lastLinkNode.getYProperty().get())) {
                                                if (nodeView.toDrag) {
                                                    if (!linkNode.getXProperty().isBound())
                                                        linkNode.getXProperty().set(mouseEvent12.getX());
                                                    if (!linkNode.getYProperty().isBound())
                                                        linkNode.getYProperty().set(mouseEvent12.getY());
                                                }
                                            }
                                        });
                                    }
                                }

                                case horizontal -> {
                                    if (Math.abs(mouseEvent.getY() - tmp.yProperty().get()) <= 2 &&
                                            mouseEvent.getX() > Math.min(tmp.firstLinkNode.getXProperty().get(), tmp.lastLinkNode.getXProperty().get()) &&
                                            mouseEvent.getX() < Math.max(tmp.firstLinkNode.getXProperty().get(), tmp.lastLinkNode.getXProperty().get())
                                    ) {
                                        linkNode.getYProperty().bind(tmp.firstLinkNode.getYProperty());
                                        linkNode.getXProperty().set(parent.currentLinkView.lastLine.lastLinkNode.getXProperty().get());
                                        parent.currentLinkView.nodeControl.put(linkNode, nodeView);
                                        parent.getChildren().add(nodeView);
                                        currentSubPort = linkNode;
                                        nodeView.removeEventHandler(MouseEvent.MOUSE_CLICKED, nodeView.getOnMouseMove());
                                        nodeView.setOnMouseMoved(mouseEvent1 -> {
                                            if (mouseEvent1.getX() > Math.min(tmp.firstLinkNode.getXProperty().get(), tmp.lastLinkNode.getXProperty().get()) &&
                                                    mouseEvent1.getX() < Math.max(tmp.firstLinkNode.getXProperty().get(), tmp.lastLinkNode.getXProperty().get())) {
                                                if (nodeView.toDrag) {
                                                    if (!linkNode.getXProperty().isBound())
                                                        linkNode.getXProperty().set(mouseEvent1.getX());
                                                    if (!linkNode.getYProperty().isBound())
                                                        linkNode.getYProperty().set(mouseEvent1.getY());
                                                }
                                            }
                                        });
                                    }
                                }
                            }
                            if (currentSubPort != null) {
                                currentSubPort.linkFinish = viewModel;
                                currentSubPort.setIsOnLinkFinish(true);
                                break;
                            }
                        }
                    }
                }
                if (currentSubPort != null) {
                    try {
                        parent.currentLinkView.viewModel.setTarget(currentSubPort);
                        Pair<DoubleProperty, DoubleProperty> currentSubPortDot = new Pair<>(currentSubPort.getXProperty(), currentSubPort.getYProperty());
                        currentSubPort.padding = 0.0;
                        parent.currentLinkView.finishLink(currentSubPortDot);
                        parent.viewModel.setLinkPaintProperty(false);
                        subPort.add(currentSubPort);
                        currentSubPort = null;
                    } catch (StackOverflowError e) {
                        System.out.println(currentSubPort.getXProperty().get() + " " + currentSubPort.getYProperty().get());
                    }
                }
            } else if (mouseEvent.getButton() == MouseButton.PRIMARY) {
                if (!mouseEvent.isControlDown() && !mouseEvent.isShiftDown()) {
                    if (viewModel.isSelected.get()) {
                        parent.viewModel.amountSelected.set(0);//bad code
                        parent.viewModel.setSelectedLinks(new ArrayList<>());//clear selection
                        parent.viewModel.setSelectedObjects(new ArrayList<>());
                    } else {
                        parent.viewModel.amountSelected.set(0);//bad code
                        parent.viewModel.setSelectedLinks(new ArrayList<>());//clear selection
                        parent.viewModel.setSelectedObjects(new ArrayList<>());
                        viewModel.isSelected.set(!viewModel.isSelected.get());
                    }
                    parent.viewModel.amountSelected.set(parent.viewModel.getSelectedLinks().size() + parent.viewModel.getSelectedObjects().size());
                }
                if (mouseEvent.isControlDown() && !mouseEvent.isShiftDown()) {
                    parent.viewModel.amountSelected.set(0);//bad code
                    viewModel.isSelected.set(!viewModel.isSelected.get());
                    parent.viewModel.setSelectedObjects(new ArrayList<>());
                    parent.viewModel.amountSelected.set(parent.viewModel.getSelectedLinks().size() + parent.viewModel.getSelectedObjects().size());
                }
                if (!mouseEvent.isControlDown() && mouseEvent.isShiftDown()) {
                    parent.viewModel.amountSelected.set(0);//bad code
                    viewModel.isSelected.set(true);
                    parent.viewModel.setSelectedObjects(new ArrayList<>());
                    parent.viewModel.amountSelected.set(parent.viewModel.getSelectedLinks().size() + parent.viewModel.getSelectedObjects().size());
                }
            }
        });

        viewModel.isSelected.addListener((observableValue, aBoolean, t1) -> {
            if (!parent.viewModel.getLinkPaintProperty()) {
                if (t1) {
                    setStroke(Color.RED);
                } else {
                    switch (viewModel.getLinkClass()) {
                        case Stream -> {
                            switch (viewModel.getType()) {
                                case Liquid -> setStroke(new Color(0.0, 212.0 / 255.0, 245.0 / 255.0, 1));
                                case Vapor -> setStroke(Color.ORANGE);
                                case VaporAndLiquid -> setStroke(Color.MAGENTA);
                            }
                        }
                        case Signal -> setStroke(Color.BLACK);
                    }
                }
            }
        });

        this.parent = parent;
    }

    public enum OrientationType {
        horizontal,
        vertical,
        dot
    }

    public enum Direction {
        up,
        down,
        left,
        right,
        dot
    }

    public class Line extends LineTo {
        private final IntegerProperty directionNumber = new SimpleIntegerProperty(0);
        public Direction direction = Direction.dot;
        public LinkNode firstLinkNode;
        public LinkNode lastLinkNode;
        public Port portConnect = null;
        private OrientationType type;

        public Line() {
            super();
        }

        public Line(Double x, Double y, OrientationType type) {
            super(x, y);
            this.type = type;
            switch (type) {
                case dot -> direction = Direction.dot;
                case vertical -> {
                    if (y - lastLine.getY() > 0)
                        direction = Direction.down;
                    else
                        direction = Direction.up;
                }
                case horizontal -> {
                    if (x - lastLine.getX() > 0)
                        direction = Direction.right;
                    else
                        direction = Direction.left;
                }
            }
        }

        public void controlOrientation() {
            switch (this.type) {
                case horizontal -> {
                    this.lastLinkNode.getXProperty().addListener((observableValue, number, t1) -> {
                        if (lastLinkNode.getXProperty().get() - firstLinkNode.getXProperty().get() > 0)
                            direction = Direction.right;
                        else
                            direction = Direction.left;
                        if (Math.abs(lastLinkNode.getXProperty().get() - firstLinkNode.getXProperty().get()) >= 14)
                            getDirectionNumber();
                    });

                    this.firstLinkNode.getXProperty().addListener((observableValue, number, t1) -> {
                        if (lastLinkNode.getXProperty().get() - firstLinkNode.getXProperty().get() > 0)
                            direction = Direction.right;
                        else {
                            direction = Direction.left;
                        }
                        if (Math.abs(lastLinkNode.getXProperty().get() - firstLinkNode.getXProperty().get()) >= 14)
                            getDirectionNumber();
                    });
                }

                case vertical -> {
                    this.lastLinkNode.getYProperty().addListener((observableValue, number, t1) -> {
                        if (lastLinkNode.getYProperty().get() - firstLinkNode.getYProperty().get() > 0)
                            direction = Direction.down;
                        else
                            direction = Direction.up;
                        if (Math.abs(lastLinkNode.getYProperty().get() - firstLinkNode.getYProperty().get()) >= 14)
                            getDirectionNumber();
                    });

                    this.firstLinkNode.getYProperty().addListener((observableValue, number, t1) -> {
                        if (lastLinkNode.getYProperty().get() - firstLinkNode.getYProperty().get() > 0)
                            direction = Direction.down;
                        else
                            direction = Direction.up;
                        if (Math.abs(lastLinkNode.getYProperty().get() - firstLinkNode.getYProperty().get()) >= 14)
                            getDirectionNumber();
                    });
                }
            }
        }

        public IntegerProperty getDirectionNumber() {
            switch (this.direction) {
                case up, left -> this.directionNumber.set(-1);
                case down, right -> this.directionNumber.set(1);
                case dot -> this.directionNumber.set(0);
            }

            return this.directionNumber;
        }

        public OrientationType getType() {
            return type;
        }
    }
}