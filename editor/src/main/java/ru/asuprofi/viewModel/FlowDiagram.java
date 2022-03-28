package ru.asuprofi.viewModel;

import javafx.beans.property.*;
import javafx.collections.FXCollections;
import javafx.collections.ListChangeListener;
import javafx.collections.ObservableList;
import javafx.geometry.Point2D;
import javafx.geometry.Rectangle2D;
import org.jetbrains.annotations.NotNull;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.NodeList;
import org.xml.sax.SAXException;
import ru.asuprofi.utils.ActionManager;
import ru.asuprofi.utils.NameManager;
import ru.asuprofi.utils.SystemsManager;
import ru.asuprofi.utils.UndoableAction;
import ru.asuprofi.viewModel.links.Link;
import ru.asuprofi.viewModel.links.LinkClass;
import ru.asuprofi.viewModel.links.LinkNode;
import ru.asuprofi.viewModel.objects.BaseObject;
import ru.asuprofi.viewModel.objects.Component;
import ru.asuprofi.viewModel.objects.LiquidTank;
import ru.asuprofi.viewModel.objects.PressureFeed;
import ru.asuprofi.viewModel.objects.factory.ObjectFactory;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;
import javax.xml.transform.OutputKeys;
import javax.xml.transform.Transformer;
import javax.xml.transform.TransformerException;
import javax.xml.transform.TransformerFactory;
import javax.xml.transform.dom.DOMSource;
import javax.xml.transform.stream.StreamResult;
import java.io.*;
import java.util.ArrayList;
import java.util.List;

public class FlowDiagram {
    private static Integer number = 0;
    private static final Logger logger = LoggerFactory.getLogger(FlowDiagram.class);
    final public DoubleProperty width = new SimpleDoubleProperty();
    final public DoubleProperty height = new SimpleDoubleProperty();
    public final SimpleStringProperty docName = new SimpleStringProperty();
    public final SimpleIntegerProperty amountSelected;
    final public ObjectFactory objectFactory = new ObjectFactory();
    final public ObservableList<BaseObject> objects = FXCollections.observableArrayList();
    public final ObservableList<Link> linkList = FXCollections.observableArrayList();
    final public NameManager nameManager = new NameManager();
    final public ObservableList<Component> components = FXCollections.observableArrayList();
    public final ObservableList<ObservableList<Double>> systems = FXCollections.observableArrayList();
    public final SystemsManager systemsManager = new SystemsManager();
    final ActionManager actionManager = new ActionManager();
    private final BooleanProperty linkPaintProperty = new SimpleBooleanProperty(false);
    private final BooleanProperty linkFromSource = new SimpleBooleanProperty(false);
    public int compToDelete = -1;
    public BooleanProperty dirtyFlag = new SimpleBooleanProperty(false);
    private File currentSaveDirectory = null;

    public FlowDiagram() {
        width.set(3000.0);
        height.set(1500.0);
        docName.set("Doc" + (number == 0 ? "" : "#" + number));
        number++;
        nameManager.setContext(this);
        amountSelected = new SimpleIntegerProperty(getSelectedObjects().size() + getSelectedLinks().size());
        linkFromSource.addListener((observableValue, aBoolean, t1) -> setSelectedObjects(new ArrayList<>()));
        logger.debug("FlowDiagram {} created", docName.get());
        systemsManager.setContext(this);
    }

    public ActionManager getActionManager() {
        return actionManager;
    }

    public void updateParent() {
        for (BaseObject obj : objects) {
            obj.setParent(this);
        }
        for (Link link : linkList) {
            link.setParent(this);
        }
    }

    public void connectSystems() {
        systemsManager.registerSystems(systems);
        components.addListener((ListChangeListener<Component>) change -> {
            while (change.next()) {
                if (change.wasAdded()) {
                    for (int i = change.getFrom(); i < change.getTo(); i++) {
                        systemsManager.addComponent();

                        for (BaseObject obj : objects) {
                            if (obj instanceof PressureFeed) {
                                ((PressureFeed) obj).getArr().add(0.0);
                            }
                            if (obj instanceof LiquidTank) {
                                ((LiquidTank) obj).getU().add(0.0);
                            }
                        }
                    }
                }
                if (change.wasRemoved()) {
                    for (Component ignored : change.getRemoved()) {
                        assert (compToDelete != -1);
                        systemsManager.removeComponent(compToDelete);

                        for (BaseObject obj : objects) {
                            if (obj instanceof PressureFeed) {
                                ((PressureFeed) obj).getArr().remove(compToDelete);
                            }
                            if (obj instanceof LiquidTank) {
                                ((LiquidTank) obj).getU().remove(compToDelete);
                            }
                        }
                    }
                }
            }
            systemsManager.updateSystems(systems);
        });
    }

    public void addObject(BaseObject object) {
        actionManager.doAction(new UndoableAction() {
            @Override
            public void Do() {
                dirtyFlag.set(true);
                objects.addAll(object);
                updateParent();
                logger.debug("method addObject called");
                logger.debug("\tDO");
                logger.debug("object added {}", object.getId().equals("") ? "unnamed object" : object.getId());
            }

            @Override
            public void Undo() {
                dirtyFlag.set(true);
                objects.removeAll(object);
                logger.debug("method addObject called");
                logger.debug("\tUNDO");
                logger.debug("object removed {}", object.getId());
            }

            @Override
            public void Redo() {
                dirtyFlag.set(true);
                objects.addAll(object);
                updateParent();
                logger.debug("method addObject called");
                logger.debug("\tREDO");
                logger.debug("object added {}", object.getId());
            }
        });
    }

    public void addLink(Link link) {
        actionManager.doAction(new UndoableAction() {
            Boolean isComplete;
            Boolean isOnStart;

            @Override
            public void Do() {
                dirtyFlag.set(true);
                linkList.add(link);
                updateParent();
                logger.debug("method addLink called");
                logger.debug("\tDO");
                logger.debug("link {} added {}", link.getId(), link.toString().equals("") ? "link is painting" : link);
            }

            @Override
            public void Undo() {
                dirtyFlag.set(true);
                linkList.remove(link);
                link.setPortsBusy(false);
                isComplete = getLinkPaintProperty();
                isOnStart = getLinkFromSourceProperty();
                if (isOnStart) {
                    setLinkFromSourceProperty(false);
                }
                if (isComplete) {
                    setLinkPaintProperty(false);
                }
                logger.debug("method addLink called");
                logger.debug("\tUNDO");
                logger.debug("link {} removed {}", link.getId(), link);
            }

            @Override
            public void Redo() {
                dirtyFlag.set(true);
                linkList.add(link);
                link.setPortsBusy(true);
                updateParent();
                if (isOnStart) {
                    setLinkFromSourceProperty(true);
                }
                if (isComplete) {
                    setLinkPaintProperty(true);
                }
                logger.debug("method addLink called");
                logger.debug("\tREDO");
                logger.debug("link {} added {}", link.getId(), link);
            }
        });
    }

    public void addObjects(List<BaseObject> object) {
        actionManager.doAction(new UndoableAction() {
            final List<BaseObject> arr = new ArrayList<>();

            @Override
            public void Do() {
                dirtyFlag.set(true);
                arr.addAll(object);
                objects.addAll(arr);
                updateParent();
                logger.debug("method addObjects called");
                logger.debug("\tDO");
                logger.debug("objects added with DO {}", object);
            }

            @Override
            public void Undo() {
                dirtyFlag.set(true);
                objects.removeAll(arr);
                logger.debug("method addObjects called");
                logger.debug("\tUNDO");
                logger.debug("objects removed{}", object);
            }

            @Override
            public void Redo() {
                dirtyFlag.set(true);
                objects.addAll(arr);
                updateParent();
                logger.debug("method addObjects called");
                logger.debug("\tREDO");
                logger.debug("objects added with {}", object);
            }
        });
    }

    public void addLinks(List<Link> links) {
        actionManager.doAction(new UndoableAction() {
            final List<Link> arr = new ArrayList<>();

            @Override
            public void Do() {
                dirtyFlag.set(true);
                arr.addAll(links);
                linkList.addAll(arr);
                updateParent();
                logger.debug("method addLinks called");
                logger.debug("\tDO");
                logger.debug("links added {}", links);
            }

            @Override
            public void Undo() {
                dirtyFlag.set(true);
                linkList.removeAll(arr);
                for (Link i : arr) {
                    i.setPortsBusy(false);
                }
                logger.debug("method addLinks called");
                logger.debug("\tUNDO");
                logger.debug("links removed {}", links);
            }

            @Override
            public void Redo() {
                dirtyFlag.set(true);
                linkList.addAll(arr);
                for (Link i : arr) {
                    i.setPortsBusy(true);
                }
                updateParent();
                logger.debug("method addLinks called");
                logger.debug("\tREDO");
                logger.debug("links added {}", links);
            }
        });
    }

    public void shiftObjects(List<BaseObject> shiftObjects, Point2D offset) {
        actionManager.doAction(new UndoableAction() {
            final List<BaseObject> arr = new ArrayList<>();

            @Override
            public void Do() {
                dirtyFlag.set(true);
                arr.addAll(shiftObjects);
                logger.debug("method shiftObjects called");
                logger.debug("\tDO");
                for (BaseObject obj : arr) {
                    logger.debug("Object {} shifted", obj.getId());
                    logger.debug("From ({}, {})", obj.x.get(), obj.y.get());
                    obj.x.set(obj.x.get() + offset.getX());
                    obj.y.set(obj.y.get() + offset.getY());
                    logger.debug("To ({}, {})", obj.x.get(), obj.y.get());
                }
            }

            @Override
            public void Undo() {
                dirtyFlag.set(true);
                logger.debug("method shiftObjects called");
                logger.debug("\tUNDO");
                for (BaseObject obj : arr) {
                    logger.debug("Object {} shifted", obj.getId());
                    logger.debug("From ({}, {})", obj.x.get(), obj.y.get());
                    obj.x.set(obj.x.get() - offset.getX());
                    obj.y.set(obj.y.get() - offset.getY());
                    logger.debug("To ({}, {})", obj.x.get(), obj.y.get());
                }
            }

            @Override
            public void Redo() {
                dirtyFlag.set(true);
                logger.debug("method shiftObjects called");
                logger.debug("\tREDO");
                for (BaseObject obj : arr) {
                    logger.debug("Object {} shifted", obj.getId());
                    logger.debug("From ({}, {}))", obj.x.get(), obj.y.get());
                    obj.x.set(obj.x.get() + offset.getX());
                    obj.y.set(obj.y.get() + offset.getY());
                    logger.debug("To ({}, {}))", obj.x.get(), obj.y.get());
                }
            }
        });
    }

    public void shiftObject(BaseObject shiftObject, Point2D offset) {
        actionManager.doAction(new UndoableAction() {

            @Override
            public void Do() {
                dirtyFlag.set(true);
                logger.debug("method shiftObject called");
                logger.debug("\tDO");
                logger.debug("Object {} shifted", shiftObject.getId());
                logger.debug("From ({}, {}))", shiftObject.x.get(), shiftObject.y.get());
                shiftObject.x.set(shiftObject.x.get() + offset.getX());
                shiftObject.y.set(shiftObject.y.get() + offset.getY());
                logger.debug("To ({}, {}))", shiftObject.x.get(), shiftObject.y.get());
            }

            @Override
            public void Undo() {
                dirtyFlag.set(true);
                logger.debug("method shiftObject called");
                logger.debug("\tUNDO");
                logger.debug("Object {} shifted", shiftObject.getId());
                logger.debug("From ({}, {}))", shiftObject.x.get(), shiftObject.y.get());
                shiftObject.x.set(shiftObject.x.get() - offset.getX());
                shiftObject.y.set(shiftObject.y.get() - offset.getY());
                logger.debug("To ({}, {}))", shiftObject.x.get(), shiftObject.y.get());
            }

            @Override
            public void Redo() {
                dirtyFlag.set(true);
                logger.debug("method shiftObject called");
                logger.debug("\tREDO");
                logger.debug("Object {} shifted", shiftObject.getId());
                logger.debug("From ({}, {}))", shiftObject.x.get(), shiftObject.y.get());
                shiftObject.x.set(shiftObject.x.get() + offset.getX());
                shiftObject.y.set(shiftObject.y.get() + offset.getY());
                logger.debug("To ({}, {}))", shiftObject.x.get(), shiftObject.y.get());
            }
        });
    }

    public void shiftLinks(List<Link> shiftLinks, Point2D offset) {
        actionManager.doAction(new UndoableAction() {
            final List<Link> arr = new ArrayList<>();

            @Override
            public void Do() {
                dirtyFlag.set(true);
                arr.addAll(shiftLinks);
                logger.debug("method shiftLinks called");
                logger.debug("\tDO");
                for (Link link : arr) {
                    link.shiftLink(offset);
                }
            }

            @Override
            public void Undo() {
                dirtyFlag.set(true);
                logger.debug("method shiftLinks called");
                logger.debug("\tUNDO");
                for (Link link : arr) {
                    link.shiftLink(new Point2D(-offset.getX(), -offset.getY()));
                }
            }

            @Override
            public void Redo() {
                dirtyFlag.set(true);
                logger.debug("method shiftLinks called");
                logger.debug("\tREDO");
                for (Link link : arr) {
                    link.shiftLink(offset);
                }
            }
        });
    }

    public void deleteObjects(List<BaseObject> object) {
        actionManager.doAction(new UndoableAction() {
            final List<BaseObject> arr = new ArrayList<>();

            @Override
            public void Do() {
                dirtyFlag.set(true);
                logger.debug("method deleteObjects called");
                logger.debug("\tDO");
                arr.addAll(object);
                objects.removeAll(arr);
                logger.debug("objects removed {}", object);
            }

            @Override
            public void Undo() {
                dirtyFlag.set(true);
                logger.debug("method deleteObjects called");
                logger.debug("\tUNDO");
                objects.addAll(arr);
                updateParent();
                logger.debug("objects added {}", object);
            }

            @Override
            public void Redo() {
                dirtyFlag.set(true);
                logger.debug("method deleteObjects called");
                logger.debug("\tREDO");
                objects.removeAll(arr);
                logger.debug("objects removed {}", object);
            }
        });
    }

    public void deleteLinks(List<Link> link) {
        actionManager.doAction(new UndoableAction() {
            final List<Link> arr = new ArrayList<>();

            @Override
            public void Do() {
                dirtyFlag.set(true);
                logger.debug("method deleteLinks called");
                logger.debug("\tDO");
                arr.addAll(link);
                for (Link i : arr) {
                    i.setPortsBusy(false);
                    logger.debug("link removed {}", i);
                }
                linkList.removeAll(arr);
                updateParent();
            }

            @Override
            public void Undo() {
                dirtyFlag.set(true);
                logger.debug("method deleteLinks called");
                logger.debug("\tUNDO");
                for (Link i : arr) {
                    i.setPortsBusy(true);
                    logger.debug("link added {}", i);
                }
                linkList.addAll(arr);
            }

            @Override
            public void Redo() {
                dirtyFlag.set(true);
                logger.debug("method deleteLinks called");
                logger.debug("\tREDO");
                for (Link i : arr) {
                    i.setPortsBusy(false);
                    logger.debug("link removed {}", i);
                }
                updateParent();
                linkList.removeAll(arr);
            }
        });
    }

    public void deleteLink(Link link) {
        actionManager.doAction(new UndoableAction() {

            @Override
            public void Do() {
                dirtyFlag.set(true);
                logger.debug("method deleteLink called");
                logger.debug("\tDO");
                link.setPortsBusy(false);
                linkList.remove(link);
                logger.debug("link removed {}", link);
            }

            @Override
            public void Undo() {
                dirtyFlag.set(true);
                logger.debug("method deleteLink called");
                logger.debug("\tUNDO");
                link.setPortsBusy(true);
                linkList.add(link);
                logger.debug("link added {}", link);
            }

            @Override
            public void Redo() {
                dirtyFlag.set(true);
                logger.debug("method deleteLink called");
                logger.debug("\tREDO");
                link.setPortsBusy(false);
                linkList.removeAll(link);
                logger.debug("link removed {}", link);
            }
        });
    }

    public void parseDataSource(byte[] arr, Point2D point, DragDTO dTO) {
        Document document = parsedDTO(arr, point, dTO);
        Element flowDiagram = document.getDocumentElement();

        NodeList itemElementList = flowDiagram.getElementsByTagName(flowDiagram.getTagName() + ".Items");

        for (int i = 0; i < itemElementList.getLength(); i++) {
            NodeList objectChildNodes = itemElementList.item(i).getChildNodes();
            for (int j = 0; j < objectChildNodes.getLength(); j++) {
                Element child = (Element) objectChildNodes.item(j);
                BaseObject object = objectFactory.createObject(child.getTagName());
                object.XMLimport(document, child);
                dTO.objectList.add(object);
            }
        }

        NodeList linkElementList = flowDiagram.getElementsByTagName(flowDiagram.getTagName() + ".Links");

        for (int i = 0; i < linkElementList.getLength(); i++) {
            if (linkElementList.item(i) instanceof Element) {
                NodeList streamChildNodes = ((Element) linkElementList.item(i)).getElementsByTagName("Stream");
                for (int j = 0; j < streamChildNodes.getLength(); j++) {
                    Element child = (Element) streamChildNodes.item(j);
                    Link link = new Link();
                    link.setParent(this);
                    link.XMLimport(document, child, dTO.objectList, dTO.linkList);
                    dTO.linkList.add(link);
                }

                NodeList signalChildNodes = ((Element) linkElementList.item(i)).getElementsByTagName("Signal");
                for (int j = 0; j < signalChildNodes.getLength(); j++) {
                    Element child = (Element) signalChildNodes.item(j);
                    Link link = new Link();
                    link.setParent(this);
                    link.XMLimport(document, child, dTO.objectList, dTO.linkList);
                    dTO.linkList.add(link);
                }
            }
        }

        for (BaseObject object : dTO.objectList) {
            object.x.set(object.x.get() + point.getX() - dTO.offset.getX());
            object.y.set(object.y.get() + point.getY() - dTO.offset.getY());
        }

        for (Link link : dTO.linkList) {
            link.shiftLink(new Point2D(point.getX() - dTO.offset.getX(), point.getY() - dTO.offset.getY()));
        }
    }

    public Document parsedDTO(byte[] arr, Point2D point, @NotNull DragDTO dTO) {
        ByteArrayInputStream data = new ByteArrayInputStream(arr);
        try {
            DocumentBuilder documentBuilder = DocumentBuilderFactory.newInstance().newDocumentBuilder();
            Document document = documentBuilder.parse(data);
            Element flowDiagram = document.getDocumentElement();

            flowDiagram.normalize();
            dTO.objectList = new ArrayList<>();
            dTO.linkList = new ArrayList<>();
            dTO.viewDragID = flowDiagram.getAttribute("viewDragID");
            dTO.offset = new Point2D(Double.parseDouble(flowDiagram.getAttribute("offsetX")),
                    Double.parseDouble(flowDiagram.getAttribute("offsetY")));
            dTO.BR = new Rectangle2D(Double.parseDouble(flowDiagram.getAttribute("Left")) + point.getX() - dTO.offset.getX(),
                    Double.parseDouble(flowDiagram.getAttribute("Right")) + point.getY() - dTO.offset.getY(),
                    Double.parseDouble(flowDiagram.getAttribute("Width")),
                    Double.parseDouble(flowDiagram.getAttribute("Height")));

            return document;
        } catch (ParserConfigurationException | IOException | SAXException e) {
            logger.error(e.getMessage());
            e.printStackTrace();
        }
        return null;
    }

    public RectCords getBoundingRect() {
        RectCords rect = new RectCords();
        double curX1 = -1;
        double curY1 = -1;
        double curX2 = -1;
        double curY2 = -1;
        for (BaseObject obj : objects) {
            if (obj.isSelected.get()) {
                if (curX1 == -1 && curY1 == -1 && curX2 == -1 && curY2 == -1) {
                    curX1 = obj.x.get();
                    curY1 = obj.y.get();
                    curX2 = curX1 + obj.w.get();
                    curY2 = curY1 + obj.h.get();
                } else {
                    if (obj.x.get() < curX1)
                        curX1 = obj.x.get();
                    if (obj.x.get() + obj.w.get() > curX2)
                        curX2 = obj.x.get() + obj.w.get();

                    if (obj.y.get() < curY1)
                        curY1 = obj.y.get();
                    if (obj.y.get() + obj.h.get() > curY2)
                        curY2 = obj.y.get() + obj.h.get();
                }
            }
        }

        rect.x = curX1;
        rect.y = curY1;
        rect.w = curX2 - curX1;
        rect.h = curY2 - curY1;

        return rect;
    }

    public void initialize() {
        updateParent();
    }

    public List<BaseObject> hitTestByRect(double x, double y, double w, double h) {
        List<BaseObject> res = new ArrayList<>();
        for (BaseObject obj : objects) {
            if (obj.x.get() >= x && obj.x.get() <= x + w &&
                    obj.y.get() >= y && obj.y.get() <= y + h) {
                res.add(obj);
            }
        }
        return res;
    }

    public List<BaseObject> getSelectedObjects() {
        List<BaseObject> res = new ArrayList<>();
        for (BaseObject obj : objects) {
            if (obj.isSelected.get())
                res.add(obj);
        }
        return res;
    }

    public void setSelectedObjects(List<BaseObject> list) {
        for (BaseObject obj : objects) {
            if (!obj.isSelected.get() && list.contains(obj))
                obj.isSelected.set(true);
            else if (obj.isSelected.get() && !list.contains(obj))
                obj.isSelected.set(false);
        }
    }

    public List<Link> getSelectedLinks() {
        List<Link> res = new ArrayList<>();
        for (Link link : linkList) {
            if (link.isSelected.get())
                res.add(link);
        }
        return res;
    }

    public void setSelectedLinks(List<Link> list) {
        for (Link link : linkList) {
            if (!link.isSelected.get() && list.contains(link))
                link.isSelected.set(true);
            else if (link.isSelected.get() && !list.contains(link))
                link.isSelected.set(false);
        }
    }

    public List<Link> getDeletingLinks() {
        List<Link> res = new ArrayList<>();
        for (Link link : linkList) {
            if (link.checkDeletion() || link.isSelected.get()) {
                res.add(link);
            }
            if (link.getTarget() instanceof LinkNode && ((LinkNode) link.getTarget()).getIsOnLinkFinish()) {
                if (res.contains(((LinkNode) link.getTarget()).linkFinish) && !res.contains(link)) {
                    res.add(link);
                }
            }
        }
        return res;
    }

    public byte[] prepareDataDrag(Point2D offset, String viewDragID) {
        return prepareDataDragHelper(offset, viewDragID, getSelectedObjects(), getSelectedLinks(), getBoundingRect());
    }

    public byte[] prepareDataDragHelper(Point2D offset, String viewDragID, List<BaseObject> arrayObjects, List<Link> arrayLinks, RectCords boundingRect) {
        ByteArrayOutputStream data = new ByteArrayOutputStream();
        try {
            DocumentBuilder documentBuilder = DocumentBuilderFactory.newInstance().newDocumentBuilder();
            Document document = documentBuilder.newDocument();
            document.setTextContent("");

            Element element = document.createElement("FlowDiagram");
            element.setAttribute("viewDragID", viewDragID);
            element.setAttribute("offsetX", Double.toString(offset.getX()));
            element.setAttribute("offsetY", Double.toString(offset.getY()));

            element.setAttribute("Left", Double.toString(boundingRect.x));
            element.setAttribute("Right", Double.toString(boundingRect.y));
            element.setAttribute("Width", Double.toString(boundingRect.w));
            element.setAttribute("Height", Double.toString(boundingRect.h));

            document.appendChild(element);

            prepareDataItems(document, arrayObjects, element);
            prepareDragLinks(document, arrayLinks, element);

            Transformer tr = TransformerFactory.newInstance().newTransformer();
            DOMSource source = new DOMSource(document);
            StreamResult result = new StreamResult(data);
            tr.transform(source, result);
        } catch (TransformerException | ParserConfigurationException e) {
            logger.error(e.getMessage());
            e.printStackTrace();
        }
        return data.toByteArray();
    }

    public byte[] prepareDataDragHelper(Point2D offset, String viewDragID, List<BaseObject> arrayObjects, RectCords boundingRect) {
        ByteArrayOutputStream data = new ByteArrayOutputStream();
        try {
            DocumentBuilder documentBuilder = DocumentBuilderFactory.newInstance().newDocumentBuilder();
            Document document = documentBuilder.newDocument();
            document.setTextContent("");

            Element element = document.createElement("FlowDiagram");
            element.setAttribute("viewDragID", viewDragID);
            element.setAttribute("offsetX", Double.toString(offset.getX()));
            element.setAttribute("offsetY", Double.toString(offset.getY()));

            element.setAttribute("Left", Double.toString(boundingRect.x));
            element.setAttribute("Right", Double.toString(boundingRect.y));
            element.setAttribute("Width", Double.toString(boundingRect.w));
            element.setAttribute("Height", Double.toString(boundingRect.h));

            document.appendChild(element);

            prepareDataItems(document, arrayObjects, element);

            Transformer tr = TransformerFactory.newInstance().newTransformer();
            DOMSource source = new DOMSource(document);
            StreamResult result = new StreamResult(data);
            tr.transform(source, result);

        } catch (TransformerException | ParserConfigurationException e) {
            logger.error(e.getMessage());
            e.printStackTrace();
        }
        return data.toByteArray();
    }

    public byte[] prepareForBlockProperties() {
        ByteArrayOutputStream data = new ByteArrayOutputStream();
        Element element;
        Document document;
        try {
            DocumentBuilder documentBuilder = DocumentBuilderFactory.newInstance().newDocumentBuilder();
            document = documentBuilder.newDocument();

            element = document.createElement("BlockProperties");

            document.appendChild(element);

            List<BaseObject> arrayObjects = new ArrayList<>(objects);
            prepareDataItems(document, arrayObjects, element);

            Transformer tr = TransformerFactory.newInstance().newTransformer();
            DOMSource source = new DOMSource(document);
            StreamResult result = new StreamResult(data);
            tr.transform(source, result);
        } catch (TransformerException | ParserConfigurationException e) {
            logger.error(e.getMessage());
            e.printStackTrace();
        }
        return data.toByteArray();
    }

    public boolean prepareDataSave(Document document, Element flowDiagram) {
        boolean hasErrors;
        List<BaseObject> arrayObjects = new ArrayList<>(objects);
        hasErrors = prepareDataItems(document, arrayObjects, flowDiagram);

        return hasErrors;
    }

    public boolean prepareDataItems(Document document, List<BaseObject> arrayObjects, Element flowDiagram) {
        boolean hasErrors = false;
        Element rootObject = document.createElement(flowDiagram.getTagName() + ".Items");

        flowDiagram.appendChild(rootObject);

        for (BaseObject obj : arrayObjects) {
            hasErrors = obj.isErrorOccurred.get();
            Element child = obj.XMLexport(document);
            rootObject.appendChild(child);
        }

        return hasErrors;
    }

    public void parseFlowDiagram(Document document, Element flowDiagram) {
        docName.set(flowDiagram.getAttribute("DocName"));
        width.set(Double.parseDouble(flowDiagram.getAttribute("Width")));
        height.set(Double.parseDouble(flowDiagram.getAttribute("Height")));

        parseFlowDiagramComponents(document, flowDiagram);
        parseFlowDiagramSystems(document, flowDiagram);

        NodeList itemList = flowDiagram.getElementsByTagName("FlowDiagram.Items");
        for (int i = 0; i < itemList.getLength(); i++) {
            Element items = (Element) itemList.item(i);
            NodeList objectsList = items.getChildNodes();
            for (int j = 0; j < objectsList.getLength(); j++) {
                if (objectsList.item(j) instanceof Element) {
                    Element objectElem = (Element) objectsList.item(j);
                    BaseObject object = objectFactory.createObject(objectElem.getTagName());
                    object.XMLimport(document, objectElem);
                    object.x.set(object.x.get());
                    object.y.set(object.y.get());
                    objects.add(object);
                }
            }
        }

        NodeList linkList = flowDiagram.getElementsByTagName("FlowDiagram.Links");
        for (int i = 0; i < linkList.getLength(); i++) {
            Element links = (Element) linkList.item(i);
            NodeList streamNodeList = links.getElementsByTagName("Stream");
            parseLinkList(document, streamNodeList);

            NodeList signalNodeList = links.getElementsByTagName("Signal");
            parseLinkList(document, signalNodeList);
        }
    }

    private void parseLinkList(Document document, NodeList signalNodeList) {
        logger.debug("method parseLinkList called");
        for (int j = 0; j < signalNodeList.getLength(); j++) {
            if (signalNodeList.item(j) instanceof Element) {
                Element linkElem = (Element) signalNodeList.item(j);
                Link link = new Link();
                link.setParent(this);
                link.XMLimport(document, linkElem);

                logger.debug("link {} parsed {}", link.getId(), link);

                this.linkList.add(link);
            }
        }
    }

    public void prepareDragLinks(Document document, List<Link> arrayLinks, Element flowDiagram) {
        Element rootObject = document.createElement(flowDiagram.getTagName() + ".Links");

        flowDiagram.appendChild(rootObject);

        for (Link link : arrayLinks) {
            Element child = link.XMLexport(document);
            rootObject.appendChild(child);
        }
    }

    public void prepareLinksOnFlowDiagram(Document document, Element flowDiagram) {
        Element rootObject = document.createElement(flowDiagram.getTagName() + ".Links");

        flowDiagram.appendChild(rootObject);

        prepareLinksSource(document, rootObject, linkList);
    }

    public void prepareLinksSource(Document document, Element flowDiagram, List<Link> links) {
        for (Link link : links) {
            if (link.getLinkClass() == LinkClass.Stream) {
                Element child = link.XMLexport(document);
                flowDiagram.appendChild(child);
            }
        }

        for (Link link : links) {
            if (link.getLinkClass() == LinkClass.Signal) {
                Element child = link.XMLexport(document);
                flowDiagram.appendChild(child);
            }
        }
    }

    public Boolean getLinkPaintProperty() {
        return linkPaintProperty.get();
    }

    public void setLinkPaintProperty(Boolean value) {
        linkPaintProperty.set(value);
    }

    public Boolean getLinkFromSourceProperty() {
        return linkFromSource.get();
    }

    public void setLinkFromSourceProperty(Boolean value) {
        linkFromSource.set(value);
    }

    public void prepareFlowDiagramComponents(Document document, Element flowDiagram) {
        Element rootObject = document.createElement(flowDiagram.getTagName() + ".Components");

        flowDiagram.appendChild(rootObject);

        for (Component i : components) {
            Element child = i.XMLexport(document);
            rootObject.appendChild(child);
        }
    }

    public void parseFlowDiagramComponents(Document document, Element flowDiagram) {
        NodeList componentsList = flowDiagram.getElementsByTagName("FlowDiagram.Components");
        for (int i = 0; i < componentsList.getLength(); i++) {
            Element components = (Element) componentsList.item(i);
            NodeList componentsChildNodes = components.getChildNodes();
            for (int j = 0; j < componentsChildNodes.getLength(); j++) {
                if (componentsChildNodes.item(j) instanceof Element) {
                    Element componentElem = (Element) componentsChildNodes.item(j);
                    Component object = new Component();
                    object.XMLimport(document, componentElem);
                    this.components.add(object);
                }
            }
        }
    }

    public void prepareFlowDiagramSystems(Document document, Element flowDiagram) {
        Element rootObject = document.createElement(flowDiagram.getTagName() + ".Systems");
        flowDiagram.appendChild(rootObject);
        Element calculationSystem = document.createElement("CalculationSystem");
        rootObject.appendChild(calculationSystem);
        Element SRKKIJ = document.createElement("CalculationSystem.SRKKIJ");
        for (List<Double> i : systems) {
            Element array = document.createElement("x:Array");
            array.setAttribute("Type", "sys:Double");
            for (Double j : i) {
                Element value = document.createElement("sys:Double");
                value.setTextContent(j.toString());
                array.appendChild(value);
            }
            SRKKIJ.appendChild(array);
        }
        calculationSystem.appendChild(SRKKIJ);
    }

    public void parseFlowDiagramSystems(Document document, Element flowDiagram) {
        NodeList flowDiagramSystems = flowDiagram.getElementsByTagName("FlowDiagram.Systems");
        for (int i = 0; i < flowDiagramSystems.getLength(); i++) {
            if (flowDiagramSystems.item(i) instanceof Element) {//get <CalculationSystem>
                NodeList SRKKIJ = flowDiagram.getElementsByTagName("CalculationSystem.SRKKIJ");
                for (int j = 0; j < SRKKIJ.getLength(); j++) {//get <CalculationSystem.SRKKIJ>
                    if (SRKKIJ.item(i) instanceof Element) {
                        NodeList valueArrays = SRKKIJ.item(i).getChildNodes();
                        for (int k = 0; k < valueArrays.getLength(); k++) {//get  <x:Array Type="sys:Double">
                            ObservableList<Double> values = FXCollections.observableArrayList();
                            for (int e = 0; e < valueArrays.item(k).getChildNodes().getLength(); e++) {
                                if (valueArrays.item(k).getChildNodes().item(e) instanceof Element) {
                                    values.add(Double.parseDouble(valueArrays.item(k).getChildNodes().item(e).getTextContent()));
                                }
                            }
                            if (values.size() > 0) {
                                systems.add(values);
                            }
                        }
                    }
                }
            }
        }
    }

    public void saveFile(File saveFile) {
        OutputStream outputStream;
        ByteArrayOutputStream byteOutputStream = new ByteArrayOutputStream();
        try {
            DocumentBuilder documentBuilder = DocumentBuilderFactory.newInstance().newDocumentBuilder();
            Document document = documentBuilder.newDocument();
            outputStream = new FileOutputStream(saveFile);
            Element rootObject = document.createElement("FlowDiagram");
            document.appendChild(rootObject);
            rootObject.setAttribute("Width", String.valueOf(this.width.get()));
            rootObject.setAttribute("Height", String.valueOf(this.height.get()));
            rootObject.setAttribute("xmlns:x", "http://schemas.microsoft.com/winfx/2006/xaml");
            rootObject.setAttribute("xmlns", "clr-namespace:tanks.Models;assembly=tanks.Models");
            rootObject.setAttribute("xmlns:sys", "clr-namespace:System;assembly=mscorlib");

            this.prepareFlowDiagramComponents(document, rootObject);
            this.prepareFlowDiagramSystems(document, rootObject);
            this.prepareDataSave(document, rootObject);
            this.prepareLinksOnFlowDiagram(document, rootObject);

            Transformer tr = TransformerFactory.newInstance().newTransformer();
            tr.setOutputProperty(OutputKeys.OMIT_XML_DECLARATION, "yes");
            tr.setOutputProperty(OutputKeys.ENCODING, "UTF-8");
            tr.setOutputProperty(OutputKeys.INDENT, "yes");
            DOMSource source = new DOMSource(document);
            StreamResult result = new StreamResult(byteOutputStream);
            tr.transform(source, result);
            byteOutputStream.writeTo(outputStream);
            dirtyFlag.set(false);
        } catch (TransformerException | IOException | ParserConfigurationException e) {
            logger.error(e.getMessage());
            e.printStackTrace();
        }
    }

    public File getCurrentSaveDirectory() {
        return currentSaveDirectory;
    }

    public void setCurrentSaveDirectory(File currentSaveDirectory) {
        this.currentSaveDirectory = currentSaveDirectory;
    }
}