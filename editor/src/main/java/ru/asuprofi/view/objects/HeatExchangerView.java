package ru.asuprofi.view.objects;

import ru.asuprofi.view.ItemView;
import ru.asuprofi.viewModel.ObjectType;

public class HeatExchangerView extends BaseObjectView {

    public HeatExchangerView() {
        this.setWidth(98.0);
        this.setHeight(98.0);

        ItemView heatExchangerView = new ItemView(ObjectType.HeatExchanger.toString());

        this.content.getChildren().add(heatExchangerView);
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
