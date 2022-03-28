package ru.asuprofi.view.objects;

import javafx.geometry.Insets;
import javafx.scene.control.Label;
import javafx.scene.paint.Color;
import javafx.scene.shape.*;
import javafx.scene.text.Font;

public class LiquidTankView extends BaseObjectView {

    public LiquidTankView() {
        this.setWidth(80.0);
        this.setHeight(168.0);
        Path objectPathFirst = new Path(
                new MoveTo(0.000000, 44.000000),
                new CubicCurveTo(0.000000, 24.000000, 28.000000, 24.000000, 33.000000, 24.000000),
                new LineTo(47.000000, 24.000000),
                new CubicCurveTo(52.000000, 24.000000, 80.000000, 24.000000, 80.000000, 44.000000),
                new LineTo(80.000000, 64.000000),
                new LineTo(0.000000, 64.000000),
                new LineTo(0.000000, 44.000000),
                new MoveTo(0.000000, 68.000000),
                new LineTo(80.000000, 68.000000),
                new LineTo(80.000000, 124.000000),
                new CubicCurveTo(80.000000, 144.000000, 45.000000, 144.000000, 40.000000, 144.000000),
                new CubicCurveTo(35.000000, 144.000000, 0.000000, 144.000000, 0.000000, 124.000000),
                new ClosePath()
        );
        objectPathFirst.setFill(Color.DARKGRAY);

        Path objectPathSecond = new Path(
                new MoveTo(31.000000, 24.000000),
                new LineTo(31.000000, 18.000000),
                new LineTo(25.000000, 18.000000),
                new LineTo(25.000000, 6.000000),
                new LineTo(37.000000, 6.000000),
                new LineTo(37.000000, 18.000000),
                new LineTo(31.000000, 18.000000),
                new LineTo(31.000000, 24.000000),
                new MoveTo(31.000000, 24.000000),
                new ClosePath()
        );
        objectPathSecond.setFill(Color.LIGHTGRAY);
        objectPathSecond.setStroke(Color.BLACK);

        Path objectPathThird = new Path(
                new MoveTo(49.000000, 24.000000),
                new LineTo(49.000000, 12.000000),
                new LineTo(43.000000, 12.000000),
                new LineTo(43.000000, 0.000000),
                new LineTo(55.000000, 0.000000),
                new LineTo(55.000000, 12.000000),
                new LineTo(49.000000, 12.000000),
                new LineTo(49.000000, 24.000000),
                new MoveTo(49.000000, 24.000000),
                new ClosePath()
        );
        objectPathThird.setFill(Color.LIGHTGRAY);
        objectPathThird.setStroke(Color.BLACK);

        Path objectPathFourth = new Path(
                new MoveTo(40.000000, 144.000000),
                new LineTo(40.000000, 156.000000),
                new LineTo(46.000000, 156.000000),
                new LineTo(46.000000, 168.000000),
                new LineTo(34.000000, 168.000000),
                new LineTo(34.000000, 156.000000),
                new LineTo(40.000000, 156.000000),
                new LineTo(40.000000, 144.000000),
                new MoveTo(40.000000, 144.000000),
                new ClosePath()
        );
        objectPathFourth.setFill(Color.LIGHTGRAY);
        objectPathFourth.setStroke(Color.BLACK);

        this.content.getChildren().addAll(objectPathFirst, objectPathSecond, objectPathThird, objectPathFourth);

        {
            Font newFont = Font.font("Times New Roman", 10);
            Label l1 = new Label("e");
            l1.setFont(newFont);
            l1.setPadding(Insets.EMPTY);
            this.content.getChildren().add(l1);
            l1.setLayoutX(29);
            l1.setLayoutY(15 - newFont.getSize());
        }
        {
            Font newFont = Font.font("Times New Roman", 10);
            Label l1 = new Label("1");
            l1.setPadding(Insets.EMPTY);
            l1.setFont(newFont);
            this.content.getChildren().add(l1);
            l1.setLayoutX(46.5);
            l1.setLayoutY(9.5 - newFont.getSize());
        }
        {
            Font newFont = Font.font("Times New Roman", 10);
            Label l1 = new Label("1");
            l1.setPadding(Insets.EMPTY);
            l1.setFont(newFont);
            this.content.getChildren().add(l1);
            l1.setLayoutX(37.5);
            l1.setLayoutY(165.5 - newFont.getSize());
        }
    }
}
