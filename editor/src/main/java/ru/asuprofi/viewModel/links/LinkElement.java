package ru.asuprofi.viewModel.links;

import javafx.beans.property.*;
import javafx.util.Pair;


public abstract class LinkElement {

    public final BooleanProperty isSelected = new SimpleBooleanProperty(false);
    protected final SimpleDoubleProperty absoluteX = new SimpleDoubleProperty(0.0);
    protected final SimpleDoubleProperty absoluteY = new SimpleDoubleProperty(0.0);
    private final SimpleObjectProperty<Pair<DoubleProperty, DoubleProperty>> paddingSimpleObjectProperty = new SimpleObjectProperty<>(new Pair<>(new SimpleDoubleProperty(0.0), new SimpleDoubleProperty(0.0)));
    public Double padding;

    public abstract String getParentId();

    public abstract String getName();

    public abstract DoubleProperty getXProperty();

    public abstract DoubleProperty getYProperty();

    public DoubleProperty getXPadding() {
        return paddingSimpleObjectProperty.get().getKey();
    }

    public DoubleProperty getYPadding() {
        return paddingSimpleObjectProperty.get().getValue();
    }

}
