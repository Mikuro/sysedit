package ru.asuprofi.view.objects;

import ru.asuprofi.view.ItemView;
import ru.asuprofi.viewModel.ObjectType;

public class PumpView extends BaseObjectView {
    public PumpView() {
        this.setWidth(78.0);
        this.setHeight(33.0);

        ItemView pumpView = new ItemView(ObjectType.Pump.toString());

        this.content.getChildren().add(pumpView);
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
