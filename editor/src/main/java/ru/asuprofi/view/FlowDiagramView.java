package ru.asuprofi.view;

import javafx.beans.property.DoubleProperty;
import javafx.beans.property.SimpleDoubleProperty;
import javafx.collections.ListChangeListener;
import javafx.event.ActionEvent;
import javafx.geometry.Point2D;
import javafx.scene.control.Alert;
import javafx.scene.control.ContextMenu;
import javafx.scene.control.MenuItem;
import javafx.scene.input.*;
import javafx.scene.layout.Pane;
import javafx.scene.paint.Color;
import javafx.scene.shape.Rectangle;
import javafx.stage.FileChooser;
import javafx.stage.Stage;
import javafx.util.Pair;
import ru.asuprofi.command.Command;
import ru.asuprofi.command.CommandManager;
import ru.asuprofi.command.CommandSource;
import ru.asuprofi.view.links.LinkView;
import ru.asuprofi.view.links.PortView;
import ru.asuprofi.view.objects.BaseObjectView;
import ru.asuprofi.view.objects.factory.ObjectViewFactory;
import ru.asuprofi.viewModel.DragDTO;
import ru.asuprofi.viewModel.FlowDiagram;
import ru.asuprofi.viewModel.FormatObject;
import ru.asuprofi.viewModel.links.Link;
import ru.asuprofi.viewModel.links.LinkNode;
import ru.asuprofi.viewModel.objects.BaseObject;

import java.io.File;
import java.util.*;

public class FlowDiagramView extends Pane {

    private static final KeyCombination cutShortcut = new KeyCodeCombination(KeyCode.X, KeyCombination.SHORTCUT_DOWN);
    private static final KeyCombination copyShortcut = new KeyCodeCombination(KeyCode.C, KeyCombination.SHORTCUT_DOWN);
    private static final KeyCombination pasteShortcut = new KeyCodeCombination(KeyCode.V, KeyCombination.SHORTCUT_DOWN);
    private static final KeyCombination deleteShortcut = new KeyCodeCombination(KeyCode.DELETE);
    private static final KeyCombination cancelShortcut = new KeyCodeCombination(KeyCode.ESCAPE);
    private static final KeyCombination upShortcut = new KeyCodeCombination(KeyCode.W);
    private static final KeyCombination downShortcut = new KeyCodeCombination(KeyCode.S);
    private static final KeyCombination leftShortcut = new KeyCodeCombination(KeyCode.A);
    private static final KeyCombination rightShortcut = new KeyCodeCombination(KeyCode.D);
    private static final KeyCombination equalAlignmentShortcut = new KeyCodeCombination(KeyCode.L, KeyCombination.ALT_DOWN, KeyCombination.SHORTCUT_DOWN);

    final public ObjectViewFactory objectViewFactory = new ObjectViewFactory();

    public final String viewDragID = UUID.randomUUID().toString();
    final Pane objectPane = new Pane();
    final Rectangle rubberBand = new Rectangle();
    final Rectangle dragRect = new Rectangle();
    final Map<BaseObject, BaseObjectView> objectControl;
    final Map<Link, LinkView> linkControl;
    private final ContextMenu contextMenu = new ContextMenu();
    public FlowDiagram viewModel;
    public LinkView currentLinkView;
    double eventX;
    double eventY;
    List<BaseObject> inRubberBand;
    private Stage stage;
    private CommandSource commandSource;

    public FlowDiagramView() {

        getChildren().add(this.objectPane);

        Color color = new Color(173.0 / 255, 216.0 / 255, 230.0 / 255, 0.5);
        rubberBand.fillProperty().set(color);
        rubberBand.visibleProperty().set(false);
        getChildren().add(rubberBand);

        Color colorDrag = new Color(173.0 / 255, 216.0 / 255, 230.0 / 255, 0.5);
        dragRect.fillProperty().set(colorDrag);
        dragRect.visibleProperty().set(false);
        getChildren().add(dragRect);

        objectControl = new HashMap<>();
        linkControl = new HashMap<>();

        FlowDiagramView self = this;

        this.objectPane.setOnMouseClicked(mouseEvent -> {
            if (mouseEvent.getTarget().equals(objectPane)) {
                viewModel.setSelectedObjects(new ArrayList<>());
                viewModel.amountSelected.set(0);
            }

            for (Link i : viewModel.linkList) {
                i.checkSelection();
            }

            if (viewModel.getLinkPaintProperty()) {
                if (viewModel.getLinkFromSourceProperty()) {
                    currentLinkView = new LinkView();
                    Link newLink = new Link();
                    linkControl.put(newLink, currentLinkView);
                    newLink.setParent(viewModel);

                    if (mouseEvent.getTarget() instanceof PortView) {
                        newLink.setSource(((PortView) mouseEvent.getTarget()).viewModel);
                    } else {
                        System.err.println("PortView hasn't been found");
                    }

                    Pair<DoubleProperty, DoubleProperty> port = new Pair<>(newLink.getSource().getXProperty(), newLink.getSource().getYProperty());

                    viewModel.addLink(newLink);
                    currentLinkView.setContext(newLink, self);

                    currentLinkView.initLink(port);

                    viewModel.nameManager.setLinkName(newLink);
                    getChildren().add(currentLinkView);
                } else {
                    if (mouseEvent.getTarget().equals(objectPane)) {
                        currentLinkView.addLine(new Pair<>(new SimpleDoubleProperty(mouseEvent.getX()), new SimpleDoubleProperty(mouseEvent.getY())));
                    } else if (!viewModel.getLinkFromSourceProperty()) {
                        if (mouseEvent.getTarget() instanceof PortView) {
                            PortView target = (PortView) mouseEvent.getTarget();
                            currentLinkView.viewModel.setTarget(target.viewModel);
                            if (!target.viewModel.isBusy.get()) {
                                Pair<DoubleProperty, DoubleProperty> port = new Pair<>(target.viewModel.getXProperty(), target.viewModel.getYProperty());
                                currentLinkView.finishLink(port);

                                viewModel.setLinkPaintProperty(false);
                            } else {
                                Alert alert = new Alert(Alert.AlertType.WARNING);
                                alert.setTitle("Warning");
                                alert.setHeaderText(null);
                                alert.setContentText("This port is busy");
                                alert.showAndWait();
                            }
                        } else if (mouseEvent.getTarget() instanceof BaseObjectView) {
                            BaseObjectView clickedBaseObjectView = (BaseObjectView) mouseEvent.getTarget();
                            LinkNode target = new LinkNode(viewModel);
                            target.getXPadding().set(mouseEvent.getX() - clickedBaseObjectView.viewModel.x.get());
                            target.getYPadding().set(mouseEvent.getY() - clickedBaseObjectView.viewModel.y.get());
                            target.getXProperty().bind(clickedBaseObjectView.viewModel.x.add(target.getXPadding()));
                            target.getYProperty().bind(clickedBaseObjectView.viewModel.y.add(target.getYPadding()));

                            target.objectFinishOutPort = clickedBaseObjectView.viewModel;
                            target.setIsOnLinkFinish(false);
                            currentLinkView.viewModel.setTarget(target);
                            currentLinkView.finishLink(new Pair<>(target.getXProperty(), target.getYProperty()));
                            viewModel.setLinkPaintProperty(false);
                        } else {
                            Alert alert = new Alert(Alert.AlertType.WARNING);
                            alert.setTitle("Warning");
                            alert.setHeaderText(null);
                            alert.setContentText("You must click on port only!");
                            alert.showAndWait();
                        }
                    }
                }
            }
        });

        this.setOnContextMenuRequested(event -> contextMenu.show(objectPane, event.getScreenX(), event.getScreenY()));

        objectPane.addEventHandler(MouseEvent.MOUSE_CLICKED, e -> contextMenu.hide());

        setOnDragDetected(mouseEvent -> {
            eventX = mouseEvent.getX();
            eventY = mouseEvent.getY();

            Dragboard dragBoard = startDragAndDrop(TransferMode.MOVE);
            inRubberBand = new ArrayList<>();

            ClipboardContent content = new ClipboardContent();

            content.put(FormatObject.fmt, viewDragID);

            dragBoard.setContent(content);
            mouseEvent.consume();
        });

        setOnDragOver(dragEvent -> {
            Object obj = dragEvent.getDragboard().getContent(FormatObject.fmt);
            Object objDrag = dragEvent.getDragboard().getContent(FormatObject.fmtDrag);
            if ((obj != null) && (obj.toString().equals(viewDragID))) {
                double x;//left up
                double w;//delta x
                double y;//left up
                double h;//delta y
                x = Math.min(eventX, dragEvent.getX());
                y = Math.min(eventY, dragEvent.getY());
                w = Math.abs(eventX - dragEvent.getX());
                h = Math.abs(eventY - dragEvent.getY());

                rubberBand.xProperty().set(x);
                rubberBand.yProperty().set(y);
                rubberBand.widthProperty().set(w);
                rubberBand.heightProperty().set(h);
                rubberBand.visibleProperty().set(true);
                List<BaseObject> visible = viewModel.hitTestByRect(x, y, w, h);
                visible.addAll(inRubberBand);
                viewModel.setSelectedObjects(visible);

                for (Link i : viewModel.linkList) {
                    i.checkSelection();
                }

                viewModel.amountSelected.set(viewModel.getSelectedObjects().size());
                dragEvent.acceptTransferModes(TransferMode.MOVE);
            } else if (objDrag != null) {
                DragDTO dTO = new DragDTO();

                viewModel.parsedDTO((byte[]) objDrag, new Point2D(dragEvent.getX(), dragEvent.getY()), dTO);

                dragRect.xProperty().set(dTO.BR.getMinX());
                dragRect.yProperty().set(dTO.BR.getMinY());
                dragRect.widthProperty().set(dTO.BR.getWidth());
                dragRect.heightProperty().set(dTO.BR.getHeight());
                dragRect.visibleProperty().set(true);
                dragEvent.acceptTransferModes(TransferMode.COPY_OR_MOVE);
            }
            dragEvent.consume();
        });

        setOnDragDropped(dragEvent -> {
            Object objDrag = dragEvent.getDragboard().getContent(FormatObject.fmtDrag);
            if (objDrag != null) {
                DragDTO dTO = new DragDTO();
                viewModel.parseDataSource((byte[]) objDrag, new Point2D(dragEvent.getX(), dragEvent.getY()), dTO);
                if (viewDragID.equals(dTO.viewDragID) && dragEvent.getTransferMode() == TransferMode.MOVE) {
                    dragEvent.acceptTransferModes(TransferMode.MOVE);
                    Point2D newOffset = new Point2D(dragEvent.getX() - dTO.offset.getX(),
                            dragEvent.getY() - dTO.offset.getY());
                    viewModel.shiftObjects(viewModel.getSelectedObjects(), newOffset);
                    if (viewModel.getSelectedLinks().size() > 0)
                        viewModel.shiftLinks(viewModel.getSelectedLinks(), newOffset);
                    viewModel.setSelectedObjects(dTO.objectList);
                    viewModel.setSelectedLinks(dTO.linkList);
                    viewModel.amountSelected.set(viewModel.getSelectedObjects().size());
                } else {
                    dragEvent.acceptTransferModes(TransferMode.COPY_OR_MOVE);
                    viewModel.addObjects(dTO.objectList);
                    viewModel.setSelectedObjects(dTO.objectList);
                    if (dTO.linkList.size() > 0)
                        viewModel.addLinks(dTO.linkList);
                    viewModel.setSelectedLinks(dTO.linkList);
                    dragEvent.setDropCompleted(true);
                    viewModel.amountSelected.set(0);
                    viewModel.amountSelected.set(viewModel.getSelectedObjects().size() + viewModel.getSelectedLinks().size());
                }
            }
            dragEvent.consume();
        });

        setOnDragExited(dragEvent -> {
            rubberBand.visibleProperty().set(false);
            dragRect.visibleProperty().set(false);
            dragEvent.consume();
            viewModel.amountSelected.set(viewModel.getSelectedObjects().size() + viewModel.getSelectedLinks().size());
        });
    }

    public void setStage(Stage stage) {
        this.stage = stage;
    }

    public void registerCommands(CommandManager commandManager) {
        Command cutCommand = new Command("Edit/Cut", cutShortcut) {
            @Override
            public void handle(ActionEvent event) {
                ClipboardContent content = new ClipboardContent();

                Clipboard.getSystemClipboard().clear();

                Point2D startPoint = new Point2D(0.0, 0.0);

                byte[] dataArray = viewModel.prepareDataDrag(startPoint, viewDragID);

                content.put(FormatObject.fmtSave, dataArray);
                Clipboard.getSystemClipboard().setContent(content);

                if (viewModel.getDeletingLinks().size() > 0) {
                    viewModel.deleteLinks(viewModel.getDeletingLinks());
                }
                if (viewModel.getSelectedObjects().size() > 0) {
                    viewModel.deleteObjects(viewModel.getSelectedObjects());
                }
                event.consume();
            }
        };

        Command copyCommand = new Command("Edit/Copy", copyShortcut) {
            @Override
            public void handle(ActionEvent event) {
                ClipboardContent content = new ClipboardContent();

                Clipboard.getSystemClipboard().clear();

                Point2D startPoint = new Point2D(0.0, 0.0);

                byte[] dataArray = viewModel.prepareDataDrag(startPoint, viewDragID);

                content.put(FormatObject.fmtSave, dataArray);
                Clipboard.getSystemClipboard().setContent(content);
            }
        };

        Command pasteCommand = new Command("Edit/Paste", pasteShortcut) {
            @Override
            public void handle(ActionEvent event) {
                Object objPaste = Clipboard.getSystemClipboard().getContent(FormatObject.fmtSave);
                if (objPaste != null) {
                    DragDTO dTO = new DragDTO();

                    viewModel.parseDataSource((byte[]) objPaste, new Point2D(0, 0), dTO);

                    if (dTO.objectList.size() > 0)
                        viewModel.addObjects(dTO.objectList);
                    if (dTO.linkList.size() > 0)
                        viewModel.addLinks(dTO.linkList);
                }
                event.consume();
            }
        };

        Command deleteCommand = new Command("Edit/Delete", deleteShortcut) {
            @Override
            public void handle(ActionEvent event) {
                if (viewModel.getDeletingLinks().size() > 0) {
                    viewModel.deleteLinks(viewModel.getDeletingLinks());
                }
                if (viewModel.getSelectedObjects().size() > 0) {
                    viewModel.deleteObjects(viewModel.getSelectedObjects());
                }
                event.consume();
            }
        };

        Command cancelCommand = new Command("Edit/Cancel", cancelShortcut) {
            @Override
            public void handle(ActionEvent actionEvent) {
                if (currentLinkView != null) {
                    viewModel.setLinkPaintProperty(false);
                    viewModel.setLinkFromSourceProperty(false);
                    viewModel.deleteLink(currentLinkView.viewModel);
                }
                viewModel.setSelectedObjects(new ArrayList<>());
                viewModel.setSelectedLinks(new ArrayList<>());
            }
        };

        Command equalAlignmentCommand = new Command("Edit/Equal Alignment", equalAlignmentShortcut) {
            @Override
            public void handle(ActionEvent actionEvent) {
                if (!viewModel.getLinkPaintProperty()) {
                    for (Link link : viewModel.linkList) {
                        if (link.linkNodeList.size() == 2) {
                            switch (((LinkView.Line) linkControl.get(link).getElements().get(1)).getType()) {
                                case horizontal -> {
                                    if (link.getTarget().getYProperty().get() - link.getSource().getYProperty().get() != 0) {
                                        viewModel.shiftObject(link.getSource().getParent(), new Point2D(0.0, link.getTarget().getYProperty().get() - link.getSource().getYProperty().get()));
                                    }
                                }
                                case vertical -> {
                                    if (link.getTarget().getXProperty().get() - link.getSource().getXProperty().get() != 0) {
                                        viewModel.shiftObject(link.getSource().getParent(), new Point2D(link.getTarget().getXProperty().get() - link.getSource().getXProperty().get(), 0.0));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };

        Command saveCommand = new Command("File/Save", new KeyCodeCombination(KeyCode.S, KeyCombination.SHORTCUT_DOWN)) {
            @Override
            public void handle(ActionEvent actionEvent) {
                File file = viewModel.getCurrentSaveDirectory();
                if (file == null) {
                    FileChooser fc = new FileChooser();
                    fc.getExtensionFilters().addAll(new FileChooser.ExtensionFilter("All Files", "*.*"));
                    fc.setInitialFileName(viewModel.docName.get() + ".xml");
                    file = fc.showSaveDialog(stage);
                    viewModel.setCurrentSaveDirectory(file);
                }
                try {
                    viewModel.saveFile(file);
                } catch (NullPointerException e) {
                    System.out.println("File wasn't found");
                }
            }
        };

        Command newDirectorySaveCommand = new Command("File/Save as ...", new KeyCodeCombination(KeyCode.S, KeyCombination.SHIFT_DOWN, KeyCombination.SHORTCUT_DOWN)) {
            @Override
            public void handle(ActionEvent actionEvent) {
                FileChooser fc = new FileChooser();
                fc.getExtensionFilters().addAll(new FileChooser.ExtensionFilter("All Files", "*.*"));
                File file = fc.showSaveDialog(stage);
                viewModel.setCurrentSaveDirectory(file);
                try {
                    viewModel.saveFile(file);
                    viewModel.docName.set(file.getName().replaceFirst("[.][^.]+$", ""));

                } catch (NullPointerException e) {
                    Alert alert = new Alert(Alert.AlertType.ERROR);
                    alert.setTitle("ERROR");
                    alert.setHeaderText(null);
                    alert.setContentText("Document has been crashed");
                    alert.showAndWait();
                }
            }
        };

        Command upCommand = new Command("Edit/Shift/Shift up", upShortcut) {
            @Override
            public void handle(ActionEvent actionEvent) {
                viewModel.shiftObjects(viewModel.getSelectedObjects(), new Point2D(0.0, -1.0));
                viewModel.shiftLinks(viewModel.getSelectedLinks(), new Point2D(0.0, -1.0));
                actionEvent.consume();
            }
        };

        Command downCommand = new Command("Edit/Shift/Shift down", downShortcut) {
            @Override
            public void handle(ActionEvent actionEvent) {
                viewModel.shiftObjects(viewModel.getSelectedObjects(), new Point2D(0.0, 1.0));
                viewModel.shiftLinks(viewModel.getSelectedLinks(), new Point2D(0.0, 1.0));
                actionEvent.consume();
            }
        };

        Command leftCommand = new Command("Edit/Shift/Shift left", leftShortcut) {
            @Override
            public void handle(ActionEvent actionEvent) {
                viewModel.shiftObjects(viewModel.getSelectedObjects(), new Point2D(-1.0, 0));
                viewModel.shiftLinks(viewModel.getSelectedLinks(), new Point2D(-1.0, 0.0));
                actionEvent.consume();
            }
        };

        Command rightCommand = new Command("Edit/Shift/Shift right", rightShortcut) {
            @Override
            public void handle(ActionEvent actionEvent) {
                viewModel.shiftObjects(viewModel.getSelectedObjects(), new Point2D(1.0, 0));
                viewModel.shiftLinks(viewModel.getSelectedLinks(), new Point2D(1.0, 0.0));
                actionEvent.consume();
            }
        };

        Command selectAllCommand = new Command("Edit/Select All", new KeyCodeCombination(KeyCode.A, KeyCombination.SHORTCUT_DOWN)) {
            @Override
            public void handle(ActionEvent actionEvent) {
                viewModel.setSelectedObjects(viewModel.objects);
                for (Link i : viewModel.linkList) {
                    i.checkSelection();
                }
                actionEvent.consume();
            }
        };

        MenuItem cut = new MenuItem("Cut");
        cut.setOnAction(cutCommand);
        cut.setAccelerator(cutShortcut);

        MenuItem copy = new MenuItem("Copy");
        copy.setOnAction(copyCommand);
        copy.setAccelerator(copyShortcut);

        MenuItem paste = new MenuItem("Paste");
        paste.setOnAction(pasteCommand);
        paste.setAccelerator(pasteShortcut);

        MenuItem delete = new MenuItem("Delete");
        delete.setOnAction(deleteCommand);
        delete.setAccelerator(deleteShortcut);

        commandSource = new CommandSource(
                new Command("Edit/Undo", new KeyCodeCombination(KeyCode.Z, KeyCombination.SHORTCUT_DOWN)) {
                    @Override
                    public void handle(ActionEvent actionEvent) {
                        viewModel.getActionManager().undo();
                    }
                },
                new Command("Edit/Redo", new KeyCodeCombination(KeyCode.Y, KeyCombination.SHORTCUT_DOWN)) {
                    @Override
                    public void handle(ActionEvent actionEvent) {
                        viewModel.getActionManager().redo();
                    }
                },
                newDirectorySaveCommand, saveCommand, cutCommand, copyCommand, pasteCommand, deleteCommand, cancelCommand, upCommand, downCommand, leftCommand, rightCommand, selectAllCommand, equalAlignmentCommand
        );

        commandManager.register(commandSource);

        contextMenu.getItems().addAll(cut, copy, paste, delete);
    }

    void enableCommands() {
        commandSource.enable();
    }

    void disableCommands() {
        commandSource.disable();
    }

    void setContext(FlowDiagram flowDiagram) {
        viewModel = flowDiagram;
        FlowDiagramView self = this;
        viewModel.objects.addListener((ListChangeListener<BaseObject>) change -> {
            while (change.next()) {
                if (change.wasAdded()) {
                    for (int i = change.getFrom(); i < change.getTo(); i++) {
                        BaseObject obj = viewModel.objects.get(i);
                        viewModel.nameManager.setObjectName(obj);
                        BaseObjectView objView = objectViewFactory.createObjectView(obj.type.toString());

                        objView.parent = self;

                        objView.setLayoutX(obj.x.get());
                        objView.setLayoutY(obj.y.get());

                        objectPane.getChildren().add(objView);
                        objView.setContext(obj);
                        objView.setPorts();
                        objectControl.put(obj, objView);
                    }
                }
                if (change.wasRemoved()) {
                    for (BaseObject obj : change.getRemoved()) {
                        viewModel.nameManager.freeObjectName(obj);
                        BaseObjectView objView = objectControl.get(obj);
                        getChildren().remove(objView.textID);
                        objectPane.getChildren().remove(objView);
                    }
                }
            }
        });

        viewModel.linkList.addListener((ListChangeListener<Link>) change -> {
            while (change.next()) {
                if (change.wasAdded()) {
                    if (!viewModel.getLinkPaintProperty()) {
                        for (int i = change.getFrom(); i < change.getTo(); i++) {
                            Link link = viewModel.linkList.get(i);
                            viewModel.nameManager.setLinkName(link);

                            LinkView linkView = new LinkView();

                            linkControl.put(link, linkView);
                            linkView.setContext(link, self);
                            link.setParent(viewModel);

                            linkView.initLink(new Pair<>(link.getSource().getXProperty(), link.getSource().getYProperty()));
                            for (Pair<DoubleProperty, DoubleProperty> node : link.linkDots) {
                                if (!node.equals(link.linkDots.get(0)) && !node.equals(link.linkDots.get(link.linkDots.size() - 1)))
                                    linkView.addLine(new Pair<>(node.getKey(), node.getValue()));
                            }

                            if (link.getTarget() != null) {
                                linkView.finishLink(new Pair<>(link.getTarget().getXProperty(), link.getTarget().getYProperty()));
                            } else {
                                linkView.addLine(new Pair<>(link.linkDots.get(link.linkDots.size() - 1).getKey(), link.linkDots.get(link.linkDots.size() - 1).getValue()));
                                currentLinkView = linkView;
                            }
                            getChildren().add(linkView);
                        }
                    }
                }

                if (change.wasRemoved()) {
                    for (Link link : change.getRemoved()) {
                        LinkView linkView = linkControl.get(link);
                        viewModel.nameManager.freeLinkName(link);

                        for (LinkNode i : link.linkNodeList) {
                            self.getChildren().remove(linkView.nodeControl.get(i));
                        }

                        getChildren().remove(linkView);

                        if (linkView.equals(currentLinkView)) {
                            currentLinkView = null;
                        }
                    }
                }
            }
        });
        viewModel.initialize();
        setWidth(viewModel.width.get());
        setHeight(viewModel.height.get());
        objectPane.setPrefSize(viewModel.width.get(), viewModel.height.get());
    }

    void updateObjects() {
        for (BaseObject obj : viewModel.objects) {
            viewModel.nameManager.setObjectName(obj);
            BaseObjectView objView = objectViewFactory.createObjectView(obj.type.toString());

            objView.parent = this;

            objView.setLayoutX(obj.x.get());
            objView.setLayoutY(obj.y.get());

            objectPane.getChildren().add(objView);
            objView.setContext(obj);
            objView.setPorts();
            objectControl.put(obj, objView);
        }
    }

    void updateLinks() {
        for (Link link : viewModel.linkList) {
            viewModel.nameManager.setLinkName(link);
            LinkView linkView = new LinkView();

            linkControl.put(link, linkView);
            linkView.setContext(link, this);
            link.setParent(viewModel);
            linkView.initLink(new Pair<>(link.getSource().getXProperty(), link.getSource().getYProperty()));

            for (Pair<DoubleProperty, DoubleProperty> node : link.linkDots) {
                if (!node.equals(link.linkDots.get(0)) && !node.equals(link.linkDots.get(link.linkDots.size() - 1))) {
                    linkView.addLine(new Pair<>(node.getKey(), node.getValue()));
                }
            }
            linkView.finishLink(new Pair<>(link.getTarget().getXProperty(), link.getTarget().getYProperty()));

            getChildren().add(linkView);
        }
    }
}