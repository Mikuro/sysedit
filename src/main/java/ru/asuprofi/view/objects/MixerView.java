package ru.asuprofi.view.objects;

import ru.asuprofi.view.ItemView;
import ru.asuprofi.viewModel.ObjectType;

public class MixerView extends BaseObjectView {
    public MixerView() {
        this.setWidth(58.0);
        this.setHeight(58.0);

        ItemView mixerView = new ItemView(ObjectType.Mixer.toString());

        this.content.getChildren().add(mixerView);
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
