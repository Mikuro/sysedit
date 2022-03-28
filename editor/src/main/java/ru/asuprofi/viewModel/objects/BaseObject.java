package ru.asuprofi.viewModel.objects;

import javafx.beans.property.*;
import org.w3c.dom.Document;
import org.w3c.dom.Element;
import ru.asuprofi.viewModel.FlowDiagram;
import ru.asuprofi.viewModel.ObjectClass;
import ru.asuprofi.viewModel.ObjectType;
import ru.asuprofi.viewModel.links.Port;

import java.util.ArrayList;
import java.util.List;

public abstract class BaseObject {

    public final List<Port> portsList = new ArrayList<>();
    public final DoubleProperty x = new SimpleDoubleProperty();
    public final DoubleProperty y = new SimpleDoubleProperty();
    public final DoubleProperty h = new SimpleDoubleProperty();
    public final DoubleProperty w = new SimpleDoubleProperty();
    public final BooleanProperty isSelected = new SimpleBooleanProperty();
    public final BooleanProperty isErrorOccurred = new SimpleBooleanProperty();
    private final StringProperty Id = new SimpleStringProperty();
    public FlowDiagram parent;
    public ObjectType type;
    public ObjectClass objectClass;
    private String mainVariable = null;

    public String getId() {
        return this.Id.get();
    }

    public void setId(String Id) {
        this.Id.set(Id);
    }

    public StringProperty getIdProperty() {
        return this.Id;
    }

    public void setParent(FlowDiagram parent) {
        this.parent = parent;
    }

    public abstract Element XMLexport(Document document);

    public abstract void XMLimport(Document document, Element elem);

    abstract void initialize();

    public String getMainVariable() {
        return mainVariable;
    }

    public void setMainVariable(String mainVariable) {
        this.mainVariable = mainVariable;
    }

    @Override
    public String toString() {
        return "BaseObject{" +
                "Id=" + Id +
                ", type=" + type +
                '}';
    }
}