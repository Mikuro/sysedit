package ru.asuprofi.view.objects;

import javafx.scene.control.Label;
import javafx.scene.paint.Color;
import javafx.scene.shape.*;
import javafx.scene.text.Font;

public class PressureGaugeView extends BaseObjectView {


    public PressureGaugeView() {
        this.setWidth(24.0);
        this.setHeight(41.0);

        Path objectPath = new Path(
                new MoveTo(12.000000, 24.000000),
                new ArcTo(12.000000, 12.000000, 180.000000, 12.000000, 0.000000, false, true),
                new ArcTo(12.000000, 12.000000, 180.000000, 12.000000, 24.000000, false, true),
                new LineTo(12.000000, 31.000000),
                new ArcTo(5.000000, 5.000000, 36.869900, 15.000000, 32.000000, false, true),
                new LineTo(9.000000, 40.000000),
                new ArcTo(5.000000, 5.000000, 106.260200, 9.000000, 32.000000, false, true),
                new LineTo(15.000000, 40.000000),
                new ArcTo(5.000000, 5.000000, 106.260200, 15.000000, 32.000000, false, false),
                new LineTo(9.000000, 40.000000),
                new ArcTo(5.000000, 5.000000, 73.739800, 15.000000, 40.000000, false, false),
                new LineTo(9.000000, 32.000000),
                new ArcTo(5.000000, 5.000000, 36.869900, 12.000000, 31.000000, false, true),
                new ClosePath()
        );

        objectPath.setFill(Color.LIGHTGRAY);
        objectPath.setStroke(Color.BLACK);
        this.content.getChildren().addAll(objectPath);

        {
            Font newFont = Font.font("Times New Roman", 12);
            Label l1 = new Label("PT");
            l1.setFont(newFont);
            this.content.getChildren().add(l1);
            l1.setLayoutX(4.5);
            l1.setLayoutY(16.5 - newFont.getSize());
        }
    }
}
