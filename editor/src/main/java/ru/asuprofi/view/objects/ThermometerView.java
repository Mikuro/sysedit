package ru.asuprofi.view.objects;

import ru.asuprofi.view.ItemView;
import ru.asuprofi.viewModel.ObjectType;

public class ThermometerView extends BaseObjectView {
    public ThermometerView() {
        this.setWidth(24.0);
        this.setHeight(41.0);

        ItemView thermometerView = new ItemView(ObjectType.Thermometer.toString());

        this.content.getChildren().add(thermometerView);
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
