package ru.asuprofi.view.objects;


import ru.asuprofi.view.ItemView;
import ru.asuprofi.viewModel.ObjectType;

public class ControlValveView extends BaseObjectView {

    public ControlValveView() {
        this.setWidth(78.0);
        this.setHeight(35.0);

        ItemView controlValveView = new ItemView(ObjectType.ControlValve.toString());

        this.content.getChildren().add(controlValveView);
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
