package ru.asuprofi.view.links;

import javafx.scene.paint.Color;
import ru.asuprofi.viewModel.links.Port;


public class PortView extends LinkElementView {

    public Port viewModel = null;


    public PortView() {
        this.setFill(new Color(0, 0, 0, 0));
    }

    public void setContext(Port viewModel) {
        this.viewModel = viewModel;
        this.setCenterX(viewModel.x.get() + 2);
        this.setCenterY(viewModel.y.get() - 2.5);
        this.setRadius(viewModel.padding);
        this.setOnMouseClicked(mouseEvent -> {
            if (!viewModel.isBusy.get()) {
                if (!viewModel.getParent().parent.getLinkPaintProperty()) {
                    viewModel.getParent().parent.setLinkFromSourceProperty(true);
                    viewModel.getParent().parent.setLinkPaintProperty(true);
                    viewModel.isBusy.set(true);
                }
                viewModel.getXPadding().unbind();
                viewModel.getYPadding().unbind();
                viewModel.getXPadding().set(0);
                viewModel.getYPadding().set(0);
            }
        });
    }


}
