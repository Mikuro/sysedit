package ru.asuprofi.view;

import javafx.geometry.Insets;
import javafx.scene.control.Label;
import javafx.scene.layout.BorderPane;
import javafx.scene.layout.Pane;
import javafx.scene.paint.Color;
import javafx.scene.shape.*;
import javafx.scene.text.Font;
import ru.asuprofi.viewModel.Item;

import java.util.Arrays;
import java.util.List;
import java.util.Scanner;

public class ItemView extends BorderPane {

    final Pane content = new Pane();
    Item viewModel;

    public ItemView(String objectName) {
        setContext(new Item(objectName));
        this.setCenter(content);
        this.setPrefWidth(viewModel.getWidth());
        this.setPrefHeight(viewModel.getHeight());
        this.parsePaths(viewModel.getPaths());
        this.parseGlyphs(viewModel.getGlyphs());
    }

    void parsePaths(List<String> paths) {
        for (String i : paths) {
            Path path = new Path();
            Scanner sc = new Scanner(i);
            sc.useDelimiter(" ");
            char action = 0;
            while (action != 'Z') {
                action = sc.next().charAt(0);
                List<String> args;
                switch (action) {
                    case 'M' -> {
                        args = Arrays.asList(sc.next().split(","));
                        path.getElements().add(new MoveTo(
                                        Double.parseDouble(args.get(0)),
                                        Double.parseDouble(args.get(1))
                                )
                        );
                    }
                    case 'L' -> {
                        args = Arrays.asList(sc.next().split(","));
                        path.getElements().add(new LineTo(
                                        Double.parseDouble(args.get(0)),
                                        Double.parseDouble(args.get(1))
                                )
                        );
                    }
                    case 'C' -> {
                        args = Arrays.asList(sc.next().split(","));
                        path.getElements().add(new CubicCurveTo(
                                        Double.parseDouble(args.get(0)), Double.parseDouble(args.get(1)),
                                        Double.parseDouble(args.get(2)), Double.parseDouble(args.get(3)),
                                        Double.parseDouble(args.get(4)), Double.parseDouble(args.get(5))
                                )
                        );
                    }
                    case 'A' -> {
                        args = Arrays.asList(sc.next().split(","));
                        path.getElements().add(new ArcTo(
                                        Double.parseDouble(args.get(0)),
                                        Double.parseDouble(args.get(1)),
                                        Double.parseDouble(args.get(2)),
                                        Double.parseDouble(args.get(5)),
                                        Double.parseDouble(args.get(6)),
                                        Integer.parseInt(args.get(3)) == 1,
                                        Integer.parseInt(args.get(4)) == 1
                                )
                        );
                    }
                    case 'Z' -> path.getElements().add(new ClosePath());
                    default -> {
                    }
                }
            }
            String fillColor = sc.next();
            if (!fillColor.equals("null"))
                path.setFill(Color.valueOf(fillColor));
            String strokeColor = sc.next();
            if (!strokeColor.equals("null"))
                path.setStroke(Color.valueOf((strokeColor)));
            sc.close();
            content.getChildren().add(path);
        }
    }

    void parseGlyphs(List<String> glyphs) {
        for (String i : glyphs) {
            List<String> args = Arrays.asList(i.split(" "));
            Font newFont = Font.font("Times New Roman", Integer.parseInt(args.get(1)));
            Label label = new Label(args.get(5));
            label.setFont(newFont);
            content.getChildren().add(label);
            label.setPadding(Insets.EMPTY);
            label.setLayoutX(Double.parseDouble(args.get(3)));
            label.setLayoutY(Double.parseDouble(args.get(4)) - newFont.getSize());
        }
    }

    void setContext(Item viewModel) {
        this.viewModel = viewModel;
    }
}
