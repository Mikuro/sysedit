package ru.asuprofi.view.objects;

import javafx.beans.property.DoubleProperty;
import javafx.geometry.Point2D;
import javafx.scene.input.ClipboardContent;
import javafx.scene.input.Dragboard;
import javafx.scene.input.MouseButton;
import javafx.scene.input.TransferMode;
import javafx.scene.layout.Pane;
import javafx.scene.paint.Color;
import javafx.scene.shape.Rectangle;
import javafx.scene.text.Text;
import javafx.util.Pair;
import ru.asuprofi.view.FlowDiagramView;
import ru.asuprofi.view.links.LinkNodeView;
import ru.asuprofi.view.links.PortView;
import ru.asuprofi.viewModel.FormatObject;
import ru.asuprofi.viewModel.links.LinkClass;
import ru.asuprofi.viewModel.links.LinkNode;
import ru.asuprofi.viewModel.links.Port;
import ru.asuprofi.viewModel.objects.BaseObject;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

public class BaseObjectView extends Pane {
    final public Pane content = new Pane();
    public final Text textID = new Text();
    final Rectangle selectionBorder = new Rectangle();
    final Rectangle errorBorder = new Rectangle();
    final Map<Port, PortView> portControl;
    public FlowDiagramView parent;
    public BaseObject viewModel;
    List<BaseObject> selectedObjects;

    public BaseObjectView() {
        this.getChildren().add(content);
        setWidth(54);
        setHeight(20);

        portControl = new HashMap<>();

        getChildren().add(errorBorder);
        getChildren().add(selectionBorder);

        BaseObjectView self = this;

        this.setOnMouseClicked(mouseEvent -> {
            if (parent.viewModel.getLinkPaintProperty() && !parent.viewModel.getLinkFromSourceProperty()) {
                if (parent.currentLinkView.viewModel.getLinkClass() == LinkClass.Signal) {
                    LinkNode target = new LinkNode(parent.viewModel);

                    target.getXPadding().set(mouseEvent.getX());
                    target.getYPadding().set(mouseEvent.getY());
                    target.getXProperty().bind(self.viewModel.x.add(target.getXPadding()));
                    target.getYProperty().bind(self.viewModel.y.add(target.getYPadding()));

                    target.objectFinishOutPort = self.viewModel;
                    target.setIsOnLinkFinish(false);

                    switch (parent.currentLinkView.viewModel.getSource().getPortDirection()) {
                        case Input -> target.pseudoPortName = viewModel.getMainVariable();
                        case Output -> target.pseudoPortName = parent.currentLinkView.viewModel.getSource().getTargetName();
                    }

                    LinkNodeView nodeView = new LinkNodeView();
                    nodeView.setContext(target, parent);
                    parent.currentLinkView.nodeControl.put(target, nodeView);
                    parent.getChildren().add(nodeView);

                    parent.currentLinkView.viewModel.setTarget(target);
                    target.padding = 0.0;
                    Pair<DoubleProperty, DoubleProperty> targetDot = new Pair<>(target.getXProperty(), target.getYProperty());

                    parent.currentLinkView.viewModel.linkDots.add(targetDot);

                    parent.currentLinkView.finishLink(targetDot);
                    parent.viewModel.setLinkPaintProperty(false);
                }
            } else if (mouseEvent.getButton() == MouseButton.PRIMARY) {
                if (!mouseEvent.isControlDown() && !mouseEvent.isShiftDown()) {
                    if (viewModel.isSelected.get()) {
                        viewModel.parent.amountSelected.set(0);
                        viewModel.parent.setSelectedObjects(new ArrayList<>());//clear selection
                        viewModel.parent.setSelectedLinks(new ArrayList<>());
                    } else {
                        viewModel.parent.amountSelected.set(0);
                        viewModel.parent.setSelectedObjects(new ArrayList<>());//clear selection
                        viewModel.parent.setSelectedLinks(new ArrayList<>());
                        viewModel.isSelected.set(!viewModel.isSelected.get());//select/unselect single object
                    }
                    viewModel.parent.amountSelected.set(viewModel.parent.getSelectedObjects().size() + viewModel.parent.getSelectedLinks().size());
                }
                if (mouseEvent.isControlDown() && !mouseEvent.isShiftDown()) {
                    viewModel.parent.amountSelected.set(0);
                    viewModel.isSelected.set(!viewModel.isSelected.get());
                    viewModel.parent.setSelectedLinks(new ArrayList<>());
                    viewModel.parent.amountSelected.set(viewModel.parent.getSelectedObjects().size() + viewModel.parent.getSelectedLinks().size());
                }
                if (!mouseEvent.isControlDown() && mouseEvent.isShiftDown()) {
                    viewModel.parent.amountSelected.set(0);
                    viewModel.isSelected.set(true);
                    viewModel.parent.setSelectedLinks(new ArrayList<>());
                    viewModel.parent.amountSelected.set(viewModel.parent.getSelectedObjects().size() + viewModel.parent.getSelectedLinks().size());
                }
            }
        });

        selectionBorder.setOnDragDetected(mouseEvent -> {
            Point2D startPoint = new Point2D(mouseEvent.getX() + viewModel.x.get(),
                    mouseEvent.getY() + viewModel.y.get()
            );

            if (mouseEvent.isPrimaryButtonDown()) {
                selectedObjects = new ArrayList<>();

                selectedObjects.addAll(viewModel.parent.getSelectedObjects());

                Dragboard dragBoard;
                if (mouseEvent.isControlDown())
                    dragBoard = selectionBorder.startDragAndDrop(TransferMode.COPY);
                else
                    dragBoard = selectionBorder.startDragAndDrop(TransferMode.MOVE);


                ClipboardContent content = new ClipboardContent();
                byte[] dataArray = viewModel.parent.prepareDataDrag(startPoint, parent.viewDragID);
                content.put(FormatObject.fmtDrag, dataArray);

                dragBoard.setContent(content);
            }
            mouseEvent.consume();
        });

        selectionBorder.setOnDragDone(dragEvent -> {
            if (dragEvent.getTransferMode() == TransferMode.MOVE) {
                viewModel.parent.nameManager.freeLinksNames(viewModel.parent.getDeletingLinks());
                viewModel.parent.deleteLinks(viewModel.parent.getDeletingLinks());
                viewModel.parent.nameManager.freeObjectsNames(selectedObjects);
                viewModel.parent.deleteObjects(selectedObjects);
                selectedObjects.clear();
            }
            dragEvent.consume();
        });

        errorBorder.visibleProperty();//!!!
        errorBorder.setVisible(false);
        errorBorder.setFill(Color.color(1, 0, 0, 0.5));
        errorBorder.setStroke(Color.color(1, 0, 0, 0.5));

        selectionBorder.setVisible(false);
        selectionBorder.setFill(Color.color(0, 0, 1, 0.5));
        selectionBorder.setStroke(Color.color(0, 0, 1, 0.5));
    }

    public void setPorts() {
        for (Port i : viewModel.portsList) {
            PortView portView = new PortView();
            portView.setContext(i);
            this.content.getChildren().add(portView);
            portControl.put(i, portView);
        }
    }

    public void setWidth(double width) {
        this.setMaxWidth(width);
        selectionBorder.setWidth(width);
        errorBorder.setWidth(width);
    }

    public void setHeight(double height) {
        this.setMaxHeight(height);
        selectionBorder.setHeight(height);
        errorBorder.setHeight(height);
    }

    public void setContext(BaseObject viewModel) {
        this.viewModel = viewModel;

        textID.textProperty().bind(viewModel.getIdProperty());
        parent.getChildren().add(textID);


        textID.setX(this.viewModel.x.get() + this.viewModel.w.get() / 2 - (textID.getText().length() - 1f) / 2f * textID.getFont().getSize());
        textID.setY(this.viewModel.y.get() + this.viewModel.h.get() + 12);

        errorBorder.visibleProperty().bind(viewModel.isErrorOccurred);

        viewModel.isSelected.addListener((observableValue, aBoolean, t1) -> {
            if (viewModel.parent.getLinkPaintProperty()) {
                viewModel.parent.setSelectedObjects(new ArrayList<>());
            } else {
                selectionBorder.setVisible(t1);

                if (selectionBorder.isVisible() && errorBorder.isVisible()) {
                    selectionBorder.setOpacity(0.0);
                } else {
                    selectionBorder.setOpacity(1.0);
                }

                for (Port i : viewModel.portsList) {
                    i.isSelected.bind(viewModel.isSelected);
                }
            }
        });

        errorBorder.visibleProperty().addListener((observable, oldValue, newValue) -> {
            if (selectionBorder.isVisible() && newValue) {
                selectionBorder.setOpacity(0.0);
            } else {
                selectionBorder.setOpacity(1.0);
            }
        });

        viewModel.x.addListener((observableValue, number, t1) -> {
            setLayoutX(t1.doubleValue());
            textID.setX(viewModel.x.get() + viewModel.w.get() / 2 - (textID.getText().length() - 1f) / 2f * textID.getFont().getSize());
            viewModel.isErrorOccurred.set(t1.doubleValue() < 0.0 || t1.doubleValue() + viewModel.w.get() > parent.viewModel.width.get());
        });
        viewModel.y.addListener((observableValue, number, t1) -> {
            setLayoutY(t1.doubleValue());
            textID.setY(viewModel.y.get() + viewModel.h.get() + 12);
            viewModel.isErrorOccurred.set(t1.doubleValue() < 0.0 || t1.doubleValue() + viewModel.h.get() > parent.viewModel.height.get());
        });

        viewModel.getIdProperty().addListener((observableValue, s, t1) -> textID.setX(viewModel.x.get() + viewModel.w.get() / 2 - (textID.getText().length() - 1f) / 2f * textID.getFont().getSize()));
    }
}