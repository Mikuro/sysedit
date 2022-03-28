package ru.asuprofi.view.objects;

import javafx.scene.control.Label;
import javafx.scene.paint.Color;
import javafx.scene.shape.*;
import javafx.scene.text.Font;

public class PIDControllerView extends BaseObjectView {

    public PIDControllerView() {
        this.setWidth(78.0);
        this.setHeight(30.0);

        Path objectPath = new Path(
                new MoveTo(3.000000, 9.000000),
                new LineTo(15.000000, 9.000000),
                new LineTo(15.000000, 15.000000),
                new LineTo(27.000000, 15.000000),
                new ArcTo(15.000000, 15.000000, 143.130100, 54.000000, 6.000000, false, true),
                new LineTo(66.000000, 6.000000),
                new LineTo(66.000000, 0.000000),
                new LineTo(78.000000, 0.000000),
                new LineTo(78.000000, 12.000000),
                new LineTo(66.000000, 12.000000),
                new LineTo(66.000000, 6.000000),
                new LineTo(54.000000, 6.000000),
                new ArcTo(15.000000, 15.000000, 73.739800, 54.000000, 24.000000, false, true),
                new LineTo(66.000000, 24.000000),
                new LineTo(66.000000, 18.000000),
                new LineTo(78.000000, 18.000000),
                new LineTo(78.000000, 30.000000),
                new LineTo(66.000000, 30.000000),
                new LineTo(66.000000, 24.000000),
                new LineTo(54.000000, 24.000000),
                new ArcTo(15.000000, 15.000000, 143.130100, 27.000000, 15.000000, false, true),
                new LineTo(15.000000, 15.000000),
                new LineTo(15.000000, 21.000000),
                new LineTo(3.000000, 21.000000),
                new ClosePath()
        );

        objectPath.setFill(Color.LIGHTGRAY);
        objectPath.setStroke(Color.BLACK);
        this.content.getChildren().addAll(objectPath);

        {
            Font newFont = Font.font("Times New Roman", 12);
            Label l1 = new Label("PID");
            l1.setFont(newFont);
            this.content.getChildren().add(l1);
            l1.setLayoutX(32.0);
            l1.setLayoutY(19.0 - newFont.getSize());

        }
        {
            Font newFont = Font.font("Times New Roman", 10);
            Label l1 = new Label("P");
            l1.setFont(newFont);
            this.content.getChildren().add(l1);
            l1.setLayoutX(6.0);
            l1.setLayoutY(18.0 - newFont.getSize());
        }
        {
            Font newFont = Font.font("Times New Roman", 10);
            Label l1 = new Label("M");
            l1.setFont(newFont);
            this.content.getChildren().add(l1);
            l1.setLayoutX(68.0);
            l1.setLayoutY(9.0 - newFont.getSize());
        }
        {
            Font newFont = Font.font("Times New Roman", 10);
            Label l1 = new Label("C");
            l1.setFont(newFont);
            this.content.getChildren().add(l1);
            l1.setLayoutX(68.0);
            l1.setLayoutY(28.0 - newFont.getSize());
        }
    }
}
