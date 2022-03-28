package ru.asuprofi.view;

import javafx.geometry.Point2D;
import javafx.scene.Node;
import javafx.scene.control.Label;
import javafx.scene.control.ScrollPane;
import javafx.scene.control.TreeItem;
import javafx.scene.control.TreeView;
import javafx.scene.input.ClipboardContent;
import javafx.scene.input.Dragboard;
import javafx.scene.input.TransferMode;
import javafx.scene.transform.Scale;
import ru.asuprofi.viewModel.*;
import ru.asuprofi.viewModel.objects.BaseObject;
import ru.asuprofi.viewModel.objects.Component;
import ru.asuprofi.viewModel.objects.LiquidTank;
import ru.asuprofi.viewModel.objects.PressureFeed;

import java.util.ArrayList;
import java.util.List;
import java.util.UUID;

public class DocumentTreeView extends ScrollPane {

    final String viewDragID = UUID.randomUUID().toString();
    private final String title = "Objects";
    private final TreeItem<Node> rootItem = new TreeItem<>(new Label(title));
    private final TreeView<Node> treeView = new TreeView<>(rootItem);
    private DocumentTree viewModel;
    private MainWindow parent;

    public DocumentTreeView() {
        rootItem.setExpanded(true);
        treeView.setShowRoot(false);

        treeView.prefHeightProperty().bind(this.heightProperty().multiply(0.4));
        treeView.prefWidthProperty().bind(this.widthProperty());

        for (ObjectType i : ObjectType.values()) {
            ItemView objectView = new ItemView(i.toString());
            Scale scale = new Scale(0.6, 0.6);
            objectView.getTransforms().add(scale);

//            objectView.setScaleX(0.6);
//            objectView.setScaleY(0.6);

            objectView.setOnDragDetected(mouseEvent -> {
                if (mouseEvent.isPrimaryButtonDown()) {
                    BaseObject object = parent.flowDiagram.objectFactory.createObject(i.toString());

                    if (object instanceof PressureFeed) {
                        for (Component ignored : parent.flowDiagram.components) {
                            ((PressureFeed) object).getArr().add(0.0);
                        }
                    }

                    if (object instanceof LiquidTank) {
                        for (Component ignored : parent.flowDiagram.components) {
                            ((LiquidTank) object).getU().add(0.0);
                        }
                    }

                    List<BaseObject> arrayObjects = new ArrayList<>();
                    arrayObjects.add(object);
                    object.isSelected.set(true);

                    Dragboard dragBoard = startDragAndDrop(TransferMode.COPY_OR_MOVE);
                    ClipboardContent content = new ClipboardContent();

                    byte[] dataArray = parent.flowDiagram.prepareDataDragHelper(new Point2D(mouseEvent.getX(), mouseEvent.getY()), viewDragID, arrayObjects, new RectCords(object.x.get(), object.y.get(), object.w.get(), object.h.get()));

                    content.put(FormatObject.fmtDrag, dataArray);

                    dragBoard.setContent(content);
                }
                mouseEvent.consume();
            });

            TreeItem<Node> objectItem = new TreeItem<>(objectView);
            rootItem.getChildren().add(objectItem);
        }
        this.setContent(treeView);
    }

    public void setContext(DocumentTree viewModel, MainWindow parent) {
        this.viewModel = viewModel;
        this.parent = parent;
    }

}
