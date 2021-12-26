package ru.asuprofi.view.links;

import javafx.beans.property.DoubleProperty;
import javafx.event.EventHandler;
import javafx.scene.input.MouseEvent;
import javafx.scene.paint.Color;
import javafx.util.Pair;
import ru.asuprofi.view.FlowDiagramView;
import ru.asuprofi.viewModel.links.LinkNode;

public class LinkNodeView extends LinkElementView {

    public Boolean toDrag = false;
    LinkNode viewModel;
    FlowDiagramView parent;
    EventHandler<MouseEvent> onClick;
    EventHandler<MouseEvent> onMouseMove;

    public LinkNodeView() {
        this.setVisible(false);
        this.setFill(Color.BLACK);
        this.setRadius(7.0);
    }

    public LinkNodeView(Pair<DoubleProperty, DoubleProperty> point) {
        this.setVisible(true);
        this.setFill(Color.BLACK);
        this.setRadius(10.0);
        this.centerXProperty().bind(point.getKey());
        this.centerYProperty().bind(point.getValue());
    }

    public void setContext(LinkNode viewModel, FlowDiagramView parent) {
        this.viewModel = viewModel;
        this.parent = parent;
        this.centerXProperty().bind(viewModel.getXProperty());
        this.centerYProperty().bind(viewModel.getYProperty());

        onClick = mouseEvent -> toDrag = !toDrag;

        onMouseMove = mouseEvent -> {
            if (toDrag) {
                if (!viewModel.getXProperty().isBound())
                    viewModel.getXProperty().set(mouseEvent.getX());
                if (!viewModel.getYProperty().isBound())
                    viewModel.getYProperty().set(mouseEvent.getY());
            }
        };

        setOnMouseClicked(onClick);

        setOnMouseMoved(onMouseMove);

    }

    public EventHandler<MouseEvent> getOnClick() {
        return onClick;
    }

    public EventHandler<MouseEvent> getOnMouseMove() {
        return onMouseMove;
    }
}
