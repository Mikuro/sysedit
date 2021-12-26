package ru.asuprofi.view.objects;

import ru.asuprofi.view.ItemView;
import ru.asuprofi.viewModel.ObjectType;

public class StrainerView extends BaseObjectView {
    public StrainerView() {
        this.setWidth(78.0);
        this.setHeight(30.0);

        ItemView strainerView = new ItemView(ObjectType.Strainer.toString());

        this.content.getChildren().add(strainerView);
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
