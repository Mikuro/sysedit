package ru.asuprofi.view.objects;

import ru.asuprofi.view.ItemView;
import ru.asuprofi.viewModel.ObjectType;

public class SplitterView extends BaseObjectView {
    public SplitterView() {
        this.setWidth(58.0);
        this.setHeight(58.0);

        ItemView splitterView = new ItemView(ObjectType.Splitter.toString());

        this.content.getChildren().add(splitterView);
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
