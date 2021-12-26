package ru.asuprofi.view.objects;

import ru.asuprofi.view.ItemView;
import ru.asuprofi.viewModel.ObjectType;

public class PipeView extends BaseObjectView {
    public PipeView() {
        this.setWidth(12.0);
        this.setHeight(136.0);

        ItemView pipeView = new ItemView(ObjectType.Pipe.toString());

        this.content.getChildren().add(pipeView);
    }

    @Override
    public void setWidth(double width) {
        super.setWidth(width);
        this.content.setPrefWidth(width);
    }

    @Override
    public void setHeight(double height) {
        super.setHeight(height);
        this.content.setPrefHeight(height);
    }
}
