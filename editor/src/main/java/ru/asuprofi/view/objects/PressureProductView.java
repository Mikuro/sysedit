package ru.asuprofi.view.objects;

import javafx.scene.control.Label;
import javafx.scene.paint.Color;
import javafx.scene.shape.ClosePath;
import javafx.scene.shape.LineTo;
import javafx.scene.shape.MoveTo;
import javafx.scene.shape.Path;
import javafx.scene.text.Font;

public class PressureProductView extends BaseObjectView {


    public PressureProductView() {
        this.setWidth(56.0);
        this.setHeight(20.0);

        Path objectPathFirst = new Path(
                new MoveTo(36.000000, 10.000000),
                new LineTo(26.000000, 0.000000),
                new LineTo(56.000000, 0.000000),
                new LineTo(56.000000, 20.000000),
                new LineTo(26.000000, 20.000000),
                new ClosePath()
        );

        objectPathFirst.setFill(Color.DARKGRAY);

        Path objectPathSecond = new Path(
                new MoveTo(36.000000, 10.000000),
                new LineTo(12.000000, 10.000000),
                new LineTo(12.000000, 4.000000),
                new LineTo(0.000000, 4.000000),
                new LineTo(0.000000, 16.000000),
                new LineTo(12.000000, 16.000000),
                new LineTo(12.000000, 10.000000),
                new MoveTo(36.000000, 10.000000),
                new ClosePath()
        );

        objectPathSecond.setFill(Color.LIGHTGRAY);
        objectPathSecond.setStroke(Color.BLACK);
        this.content.getChildren().addAll(objectPathFirst, objectPathSecond);

        {
            Font newFont = Font.font("Times New Roman", 14);
            Label l1 = new Label("BL");
            l1.setFont(newFont);
            this.content.getChildren().add(l1);
            l1.setLayoutX(36.0);
            l1.setLayoutY(14.0 - newFont.getSize());
        }
        {
            Font newFont = Font.font("Times New Roman", 10);
            Label l1 = new Label("1");
            l1.setFont(newFont);
            this.content.getChildren().add(l1);
            l1.setLayoutX(3.5);
            l1.setLayoutY(13.5 - newFont.getSize());
        }
    }
}
