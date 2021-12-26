package ru.asuprofi.viewModel.links;

import javafx.beans.property.*;
import javafx.collections.FXCollections;
import javafx.collections.ObservableList;
import javafx.geometry.Point2D;
import javafx.util.Pair;
import org.jetbrains.annotations.NotNull;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.w3c.dom.Document;
import org.w3c.dom.Element;
import ru.asuprofi.viewModel.FlowDiagram;
import ru.asuprofi.viewModel.objects.BaseObject;

import java.util.Arrays;
import java.util.Collections;
import java.util.List;
import java.util.Scanner;

public class Link {

    private static final Logger logger = LoggerFactory.getLogger(Link.class);
    public final BooleanProperty isSelected = new SimpleBooleanProperty(false);
    public final ObservableList<LinkNode> linkNodeList = FXCollections.observableArrayList();//current nodes of link - added from init, add and finish
    public final ObservableList<Pair<DoubleProperty, DoubleProperty>> linkDots = FXCollections.observableArrayList();//dots from figures
    LinkType type;
    LinkClass linkClass;
    FlowDiagram parent;
    private Port source = null;
    private LinkElement target = null;
    private StringProperty Id = new SimpleStringProperty();
    private String figures;

    public Link() {
        this.type = LinkType.Liquid;
    }

    private void createFigures() {
        StringBuilder str = new StringBuilder();
        str.append("M ").append(linkDots.get(0).getKey().get()).append(",").append(linkDots.get(0).getValue().get());
        for (Pair<DoubleProperty, DoubleProperty> node : linkDots) {
            if (!node.equals(linkDots.get(0))) {
                str.append(" L ").append(node.getKey().get()).append(",").append(node.getValue().get());
            }
        }
        figures = str.toString();
        logger.debug("Link figures created");
        logger.debug("Figures: {}", this.figures);
    }

    private void createDots(String figures) {
        Scanner sc = new Scanner(figures);
        this.figures = figures;
        linkDots.clear();
        linkNodeList.clear();
        sc.useDelimiter(" ");
        char action;
        while (sc.hasNext()) {
            action = sc.next().charAt(0);
            List<String> args;
            switch (action) {
                case 'M', 'L' -> {
                    args = Arrays.asList(sc.next().split(","));
                    linkDots.add(new Pair<>(
                                    new SimpleDoubleProperty(Double.parseDouble(args.get(0))),
                                    new SimpleDoubleProperty(Double.parseDouble(args.get(1)))
                            )
                    );
                }
                default -> {
                }
            }
        }
        logger.debug("Link dots created");
        logger.debug("{}", linkDots);
    }

    public Element XMLexport(Document document) {
        Element res = document.createElement(this.linkClass.toString());
        res.setAttribute("Id", this.Id.get());
        if (linkClass == LinkClass.Stream)
            res.setAttribute("Type", this.type.toString());

        //if linkClass is signal
        //and if the way was constructing from port with direction Input or to Stream
        //this.source is target
        //this.target is source
        if (this.linkClass == LinkClass.Signal) {
            if (source.getPortDirection() == PortDirection.Output) {
                logger.debug("Export of signal link from object to valuable");
                res.setAttribute("From", source.getParent().getId() + "." + source.getName());
                logger.debug("From is {}", res.getAttribute("From"));
                res.setAttribute("To", target.getParentId() + (target.getName() == null ? "" : "." + target.getName()));
                logger.debug("To is {}", res.getAttribute("To"));
                createFigures();
            } else {
                logger.debug("Export of signal link from valuable or link to object");
                res.setAttribute("From", target.getParentId() + (target.getName() == null ? "" : "." + target.getName()));
                logger.debug("From is {}", res.getAttribute("From"));
                res.setAttribute("To", source.getParent().getId() + "." + source.getName());
                logger.debug("To is {}", res.getAttribute("To"));
                Collections.reverse(linkDots);
                createFigures();
                Collections.reverse(linkDots);
            }
        } else {
            logger.debug("Export of stream link from object to object");
            res.setAttribute("From", source.getParent().getId() + "." + source.getName());
            logger.debug("From is {}", res.getAttribute("From"));
            res.setAttribute("To", target.getParentId() + (target.getName() == null ? "" : "." + target.getName()));
            logger.debug("To is {}", res.getAttribute("To"));
            createFigures();
        }
        res.setAttribute("Figures", this.figures);
        logger.debug("Link exported data to document");
        logger.debug("{}", this);
        return res;
    }

    public void XMLimport(Document document, @NotNull Element elem) {
        XMLimport(document, elem, parent.objects, parent.linkList);
    }

    public void XMLimport(Document document, @NotNull Element elem, List<BaseObject> objects, List<Link> links) {
        switch (elem.getNodeName()) {
            case "Stream" -> this.setLinkClass(LinkClass.Stream);
            case "Signal" -> this.setLinkClass(LinkClass.Signal);
        }

        createDots(elem.getAttribute("Figures"));

        this.setId(elem.getAttribute("Id"));

        if (linkClass == LinkClass.Stream)
            this.setType(elem.getAttribute("Type"));

        List<String> namesFrom = Arrays.asList(elem.getAttribute("From").split("\\."));
        List<String> namesTo = Arrays.asList(elem.getAttribute("To").split("\\."));

        switch (namesFrom.size()) {
            case 2 -> {
                //if true
                //usual stream path
                if (linkClass == LinkClass.Stream) {
                    for (BaseObject i : objects) {
                        if (i.getId().equals(namesFrom.get(0))) {
                            for (Port j : i.portsList) {
                                if (j.getName().equals(namesFrom.get(1))) {
                                    this.source = j;
                                    this.source.isBusy.set(true);
                                }
                            }
                        }
                    }
                    for (BaseObject i : objects) {
                        if (i.getId().equals(namesTo.get(0))) {
                            for (Port j : i.portsList) {
                                if (j.getName().equals(namesTo.get(1))) {
                                    this.target = j;
                                    ((Port) target).isBusy.set(true);
                                }
                            }
                        }
                    }
                    // if false
                    // starting Signal path
                } else {
                    //signal from output to variable
                    for (BaseObject i : objects) {
                        if (i.getId().equals(namesFrom.get(0))) {
                            for (Port j : i.portsList) {
                                if (j.getName().equals(namesFrom.get(1))) {
                                    setSource(j);//source is port of object, which takes variable from another object

                                    LinkNode targetNode = new LinkNode(linkDots.get(linkDots.size() - 1).getKey(), linkDots.get(linkDots.size() - 1).getValue(), parent);

                                    setTarget(targetNode);//create link node of variable on Object

                                    ((LinkNode) target).pseudoPortName = source.getTargetName();

                                    for (BaseObject obj : objects) {
                                        if (obj.getId().equals(namesTo.get(0))) {
                                            ((LinkNode) target).objectFinishOutPort = obj;
                                            double offsetX = linkDots.get(linkDots.size() - 1).getKey().get() - obj.x.get();
                                            double offsetY = linkDots.get(linkDots.size() - 1).getValue().get() - obj.y.get();
                                            (target).getXProperty().bind(obj.x.add(offsetX));
                                            (target).getYProperty().bind(obj.y.add(offsetY));
                                            target.isSelected.bind(obj.isSelected);
                                            break;
                                        }
                                    }
                                    source.isBusy.set(true);
                                }
                            }
                            //from input to variable
                            //from target to source
                            if (this.source == null && i.getId().equals(namesFrom.get(0))) {
                                Collections.reverse(linkDots);

                                LinkNode targetNode = new LinkNode(linkDots.get(linkDots.size() - 1).getKey(), linkDots.get(linkDots.size() - 1).getValue(), parent);

                                setTarget(targetNode);

                                ((LinkNode) target).pseudoPortName = namesFrom.get(1);
                                ((LinkNode) target).objectFinishOutPort = i;

                                double offsetX = linkDots.get(linkDots.size() - 1).getKey().get() - i.x.get();
                                double offsetY = linkDots.get(linkDots.size() - 1).getValue().get() - i.y.get();

                                target.getXProperty().bind(i.x.add(offsetX));
                                target.getYProperty().bind(i.y.add(offsetY));

                                target.isSelected.bind(i.isSelected);

                                for (BaseObject obj : objects) {
                                    if (obj.getId().equals(namesTo.get(0))) {
                                        for (Port tmp : obj.portsList) {
                                            if (tmp.getName().equals(namesTo.get(1))) {
                                                source = tmp;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //if path from link to sensor
            case 1 -> {
                Collections.reverse(linkDots);
                for (Link i : links) {
                    if (i.getId().equals(namesFrom.get(0))) {

                        LinkNode targetNode = new LinkNode(linkDots.get(linkDots.size() - 1).getKey(), linkDots.get(linkDots.size() - 1).getValue(), parent);

                        setTarget(targetNode);

                        ((LinkNode) target).linkFinish = i;
                        ((LinkNode) target).setIsOnLinkFinish(true);

                        for (int j = 1; j < i.linkDots.size(); j++) {
                            if (i.linkDots.get(j).getKey().get() == i.linkDots.get(j - 1).getKey().get()) {//is vertical

                                if (Math.abs(linkDots.get(linkDots.size() - 1).getKey().get() - i.linkDots.get(j).getKey().get()) <= 2 &&
                                        linkDots.get(linkDots.size() - 1).getValue().get() > Math.min(i.linkDots.get(j - 1).getValue().get(), i.linkDots.get(j).getValue().get()) &&
                                        linkDots.get(linkDots.size() - 1).getValue().get() < Math.max(i.linkDots.get(j - 1).getValue().get(), i.linkDots.get(j).getValue().get())
                                ) {
                                    (target).getXProperty().bind(i.linkDots.get(j - 1).getKey());
                                    (target).getYProperty().set(linkDots.get(linkDots.size() - 1).getValue().get());
                                }
                            } else {//is horizontal
                                if (Math.abs(linkDots.get(linkDots.size() - 1).getValue().get() - i.linkDots.get(j).getValue().get()) <= 2 &&
                                        linkDots.get(linkDots.size() - 1).getKey().get() > Math.min(i.linkDots.get(j - 1).getKey().get(), i.linkDots.get(j).getKey().get()) &&
                                        linkDots.get(linkDots.size() - 1).getKey().get() < Math.max(i.linkDots.get(j - 1).getKey().get(), i.linkDots.get(j).getKey().get())
                                ) {
                                    (target).getYProperty().bind(i.linkDots.get(j - 1).getValue());
                                    (target).getXProperty().set(linkDots.get(linkDots.size() - 1).getKey().get());
                                }
                            }
                        }

                        for (BaseObject obj : objects) {
                            if (obj.getId().equals(namesTo.get(0))) {
                                for (Port tmp : obj.portsList) {
                                    if (tmp.getName().equals(namesTo.get(1)))
                                        source = tmp;
                                }
                                if (source != null)
                                    break;
                            }
                        }
                    }
                }
            }
        }
        logger.debug("Link imported data from document");
        logger.debug("{}", this);
    }

    public String getId() {
        return Id.get();
    }

    public void setId(String id) {
        Id = new SimpleStringProperty(id);
    }

    public LinkClass getLinkClass() {
        return linkClass;
    }

    public void setLinkClass(LinkClass linkClass) {
        this.linkClass = linkClass;
    }

    public Port getSource() {
        return source;
    }

    public void setSource(Port source) {
        this.linkClass = source.getSourceOf();
        this.source = source;
    }

    public LinkElement getTarget() {
        return target;
    }

    public void setTarget(LinkElement target) {
        this.target = target;
    }

    public void setParent(FlowDiagram parent) {
        this.parent = parent;
    }

    public LinkType getType() {
        return type;
    }

    public void setType(LinkType type) {
        this.type = type;
    }

    public void setType(String type) {
        switch (type) {
            case "Liquid" -> this.setType(LinkType.Liquid);
            case "Vapor" -> this.setType(LinkType.Vapor);
            case "VaporAndLiquid" -> this.setType(LinkType.VaporAndLiquid);
        }
    }

    public void checkSelection() {
        this.isSelected.set(source.isSelected.get() && target.isSelected.get());
    }

    public Boolean checkDeletion() {
        return (source.isSelected.get() || target.isSelected.get());
    }

    public void shiftLink(Point2D offset) {
        logger.debug("link {} shifted", getId());
        logger.debug("From {}", this);
        for (Pair<DoubleProperty, DoubleProperty> node : this.linkDots) {
            if (!node.getKey().isBound()) {
                node.getKey().set(node.getKey().get() + offset.getX());
            }
            if (!node.getValue().isBound()) {
                node.getValue().set(node.getValue().get() + offset.getY());
            }
        }
        logger.debug("To {}", this);
    }

    public void addLinkNode(LinkNode linkNode) {
        this.linkNodeList.add(linkNode);
    }

    public void setPortsBusy(Boolean flag) {
        this.source.isBusy.set(flag);
        if (this.target != null && this.target instanceof Port) {
            ((Port) this.target).isBusy.set(flag);
        }
    }

    @Override
    public String toString() {
        StringBuilder str = new StringBuilder();
        for (Pair<DoubleProperty, DoubleProperty> node : linkDots) {
            str.append("(");
            str.append(node.getKey().get());
            str.append(", ");
            str.append(node.getValue().get());
            str.append(") ");
        }
        return "Link{" +
                "isSelected=" + isSelected +
                ", linkNodeList=" + linkNodeList +
                ", linkDots=" + str +
                ", type=" + type +
                ", linkClass=" + linkClass +
                ", parent=" + parent +
                ", source=" + source +
                ", target=" + target +
                ", Id=" + Id +
                ", figures='" + figures + '\'' +
                '}';
    }
}