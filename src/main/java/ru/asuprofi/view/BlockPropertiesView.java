package ru.asuprofi.view;

import javafx.beans.property.BooleanProperty;
import javafx.beans.property.SimpleBooleanProperty;
import javafx.beans.property.SimpleStringProperty;
import javafx.beans.property.StringProperty;
import javafx.collections.FXCollections;
import javafx.collections.ObservableList;
import javafx.geometry.Point2D;
import javafx.scene.control.*;
import javafx.scene.input.MouseEvent;
import javafx.scene.layout.VBox;
import javafx.scene.text.Text;
import javafx.util.Pair;
import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.NamedNodeMap;
import org.w3c.dom.NodeList;
import org.xml.sax.SAXException;
import ru.asuprofi.viewModel.BlockProperties;
import ru.asuprofi.viewModel.MainWindow;
import ru.asuprofi.viewModel.links.Link;
import ru.asuprofi.viewModel.links.LinkType;
import ru.asuprofi.viewModel.objects.BaseObject;
import ru.asuprofi.viewModel.objects.Component;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;
import java.io.ByteArrayInputStream;
import java.io.IOException;
import java.util.ArrayList;
import java.util.List;


public class BlockPropertiesView extends VBox {

    //observableCollection of object field
    //editable special cells
    //systems
    //BP - is document, which describes a table
    //instant update - is update of document
    //current version of block properties is crashes MVVM logic
    //need to rebuild ViewModel of this entity, to save MVVM architecture

    final TreeTableColumn<Data, String> firstTableColumn = new TreeTableColumn<>("Property");
    final TreeTableColumn<Data, String> secondTableColumn = new TreeTableColumn<>("Value");
    private final TreeItem<Data> rootItem = new TreeItem<>(new Data("root", ""));
    private final TreeTableView<Data> body = new TreeTableView<>();
    private BlockProperties viewModel = null;
    private MainWindow parent = null;
    private BPmodes currentMode;
    private Component selectedToDelete = null;

    public BlockPropertiesView() {
        body.setRoot(rootItem);
        body.setShowRoot(false);

        rootItem.setExpanded(true);

        firstTableColumn.minWidthProperty().set(this.prefWidthProperty().get() / 2);
        secondTableColumn.minWidthProperty().set(this.prefWidthProperty().get() / 2);

        firstTableColumn.setSortable(false);
        secondTableColumn.setSortable(false);

        firstTableColumn.setCellValueFactory(
                dataStringCellDataFeatures -> dataStringCellDataFeatures.getValue().getValue().getProperty());
        secondTableColumn.setCellValueFactory(
                dataStringCellDataFeatures -> dataStringCellDataFeatures.getValue().getValue().getValueProperty());

        secondTableColumn.setCellFactory(column -> new TreeTableCell<>() {
            private final TextField textField = new TextField();
            private final ChoiceBox<String> choice = new ChoiceBox<>();

            private boolean ignoreChoiceBoxChange = false;

            {
                choice.valueProperty().addListener((obs, oldValue, newValue) -> {
                    if (!ignoreChoiceBoxChange) {
                        commitEdit(newValue);
                    }
                });

                choice.focusedProperty().addListener((obs, wasFocused, isNowFocused) -> {
                    if (!isNowFocused) {
                        cancelEdit();
                    }
                });
                choice.showingProperty().addListener((obs, wasShowing, isNowShowing) -> {
                    if (!isNowShowing) {
                        cancelEdit();
                    }
                });
                textField.setOnAction(e -> commitEdit(textField.getText()));
                textField.focusedProperty().addListener((obs, wasFocused, isNowFocused) -> {
                    if (!isNowFocused) {
                        cancelEdit();
                    }
                });
            }

            @Override
            public void updateItem(String item, boolean empty) {
                super.updateItem(item, empty);
                if (isEditing()) {
                    updateEditor();
                } else {
                    updateTextUnselected(item);
                }
            }

            @Override
            public void startEdit() {
                Data item = getTreeTableView().getTreeItem(getIndex()).getValue();
                if (item.isEditable() && getTreeTableView().getTreeItem(getIndex()).getChildren().size() == 0) {
                    super.startEdit();
                    updateEditor();
                } else {
                    editableProperty().unbind();
                }
            }

            @Override
            public void cancelEdit() {
                super.cancelEdit();
                setContentDisplay(ContentDisplay.TEXT_ONLY);
                setText(getText());
            }

            @Override
            public void commitEdit(String item) {
                super.commitEdit(item);
                updateText(item);
            }

            private void updateEditor() {
                setContentDisplay(ContentDisplay.GRAPHIC_ONLY);
                int index = getIndex();
                Data item = getTreeTableView().getTreeItem(index).getValue();
                if (item.getChoice()) {
                    ignoreChoiceBoxChange = true;
                    choice.getItems().setAll(item.choices);//rule
                    choice.getSelectionModel().select(getItem());
                    setGraphic(choice);
                    choice.show();
                    ignoreChoiceBoxChange = false;
                } else {
                    textField.setText(getItem());
                    setGraphic(textField);
                }
            }

            private void updateTextUnselected(String str) {
                if (isEmpty()) {
                    setText(null);
                } else {
                    setText(str);
                }
            }

            private void updateText(String str) {
                setContentDisplay(ContentDisplay.TEXT_ONLY);
                if (isEmpty()) {
                    setText(null);
                } else {
                    switch (currentMode) {
                        case Link -> {
                            TreeItem<Data> currentEditing = getTreeTableView().getTreeItem(getIndex());
                            currentEditing.getValue().setValueProperty(str);

                            ByteArrayInputStream data = new ByteArrayInputStream(parent.flowDiagram.prepareForBlockProperties());

                            DocumentBuilder documentBuilder = null;
                            try {
                                documentBuilder = DocumentBuilderFactory.newInstance().newDocumentBuilder();
                            } catch (ParserConfigurationException e) {
                                e.printStackTrace();
                            }

                            Document document = null;
                            try {
                                assert documentBuilder != null;
                                document = documentBuilder.parse(data);
                            } catch (SAXException | IOException e) {
                                e.printStackTrace();
                            }

                            assert document != null;
                            Element newData = document.createElement(viewModel.getInfoFromLink().getType().toString());

                            TreeItem<Data> rootNode = currentEditing.getParent();
                            while (!rootNode.getValue().property.get().equals("root")) {
                                rootNode = rootNode.getParent();
                            }

                            List<TreeItem<Data>> children = rootNode.getChildren();
                            for (TreeItem<Data> i : children) {
                                if (i.getChildren().size() == 0) {
                                    newData.setAttribute(i.getValue().property.get(), i.getValue().value.get());
                                } else {
                                    Element compoundAttribute = document.createElement(i.getValue().property.get());
                                    for (TreeItem<Data> j : i.getChildren()) {
                                        Element compoundParam = document.createElement("CompoundProperty");
                                        compoundParam.setTextContent(j.getValue().value.get());
                                        compoundAttribute.appendChild(compoundParam);
                                    }
                                    newData.appendChild(compoundAttribute);
                                }
                            }
                            if (!viewModel.getInfoFromLink().getId().equals(newData.getAttribute("Id"))) {
                                parent.flowDiagram.nameManager.freeLinkName(viewModel.getInfoFromLink());
                                viewModel.getInfoFromLink().XMLimport(document, newData);
                                parent.flowDiagram.nameManager.setLinkName(viewModel.getInfoFromLink());
                            } else {
                                viewModel.getInfoFromLink().XMLimport(document, newData);
                            }
                        }
                        case BaseObject -> {
                            TreeItem<Data> currentEditing = getTreeTableView().getTreeItem(getIndex());
                            currentEditing.getValue().setValueProperty(str);

                            ByteArrayInputStream data = new ByteArrayInputStream(parent.flowDiagram.prepareForBlockProperties());

                            DocumentBuilder documentBuilder = null;
                            try {
                                documentBuilder = DocumentBuilderFactory.newInstance().newDocumentBuilder();
                            } catch (ParserConfigurationException e) {
                                e.printStackTrace();
                            }

                            Document document = null;
                            try {
                                assert documentBuilder != null;
                                document = documentBuilder.parse(data);
                            } catch (SAXException | IOException e) {
                                e.printStackTrace();
                            }

                            assert document != null;
                            Element newData = document.createElement(viewModel.getInfoFromObject().type.toString());

                            TreeItem<Data> rootNode = currentEditing.getParent();
                            while (!rootNode.getValue().property.get().equals("root")) {
                                rootNode = rootNode.getParent();
                            }

                            List<TreeItem<Data>> children = rootNode.getChildren();
                            for (TreeItem<Data> i : children) {
                                if (i.getChildren().size() == 0) {
                                    if (i.getValue().property.get().equals("X") && Math.abs(viewModel.getInfoFromObject().x.get() - Double.parseDouble(i.getValue().value.get())) > 0.5) {
                                        parent.flowDiagram.shiftObject(viewModel.getInfoFromObject(), new Point2D(Double.parseDouble(i.getValue().value.get()) - viewModel.getInfoFromObject().x.get(), 0.0));
                                    } else if (i.getValue().property.get().equals("Y") && Math.abs(viewModel.getInfoFromObject().y.get() - Double.parseDouble(i.getValue().value.get())) > 0.5) {
                                        parent.flowDiagram.shiftObject(viewModel.getInfoFromObject(), new Point2D(0.0, Double.parseDouble(i.getValue().value.get()) - viewModel.getInfoFromObject().y.get()));
                                    } else {
                                        if (i.getValue().editable.get())
                                            newData.setAttribute(i.getValue().property.get(), i.getValue().value.get());
                                    }
                                } else {
                                    Element compoundAttribute = document.createElement(i.getValue().property.get());
                                    for (TreeItem<Data> j : i.getChildren()) {
                                        Element compoundParam = document.createElement("CompoundProperty");
                                        compoundParam.setTextContent(j.getValue().value.get());
                                        compoundAttribute.appendChild(compoundParam);
                                    }
                                    newData.appendChild(compoundAttribute);
                                }
                            }
                            if (!viewModel.getInfoFromObject().getId().equals(newData.getAttribute("Id"))) {
                                parent.flowDiagram.nameManager.freeObjectName(viewModel.getInfoFromObject());
                                viewModel.getInfoFromObject().XMLimport(document, newData);
                                String objectCommitName = viewModel.getInfoFromObject().getId();
                                parent.flowDiagram.nameManager.setObjectName(viewModel.getInfoFromObject());
                                String objectManagedName = viewModel.getInfoFromObject().getId();
                                if (!objectManagedName.equals(objectCommitName)) {
                                    str = objectManagedName;
                                    Alert alert = new Alert(Alert.AlertType.WARNING);
                                    alert.setTitle("Warning");
                                    alert.setHeaderText(null);
                                    alert.setContentText("This name is already taken");
                                    alert.showAndWait();
                                }
                            } else {
                                viewModel.getInfoFromObject().XMLimport(document, newData);
                            }
                        }
                        case FlowDiagram -> {
                            TreeItem<Data> currentEditing = getTreeTableView().getTreeItem(getIndex());
                            TreeItem<Data> components = null;

                            if (currentEditing.getValue().getProperty().get().equals("Document name")) {
                                currentEditing.getValue().getValueProperty().set(str);
                            }

                            if (currentEditing.getValue().getProperty().get().equals("Option")) {
                                for (Component comp : Component.consts) {
                                    if (comp.getId().equals(str)) {
                                        parent.flowDiagram.components.add(comp);
                                        currentEditing.getValue().choices.remove(comp.getId());
                                    }
                                }
                            }

                            if (currentEditing.getParent().getValue().getProperty().get().equals("Array")) {
                                currentEditing.getValue().getValueProperty().set(str);
                            }

                            if (!currentEditing.getParent().equals(rootItem)) {
                                components = currentEditing.getParent();
                            }

                            if (components != null) {
                                for (Component i : Component.consts) {
                                    if (i.getId().equals(str)) {
                                        CompoundBlock componentCompoundBlock = new CompoundBlock(i.getId(), i.getAllFields());
                                        components.getChildren().remove(currentEditing);
                                        components.getChildren().add(componentCompoundBlock);
                                        components.getChildren().add(currentEditing);
                                        body.getSelectionModel().select(currentEditing);
                                    }
                                }
                            }
                        }
                    }
                    setText(str);
                }
            }
        });

        body.getColumns().add(firstTableColumn);
        body.getColumns().add(secondTableColumn);

        //
        ContextMenu contextMenu = new ContextMenu();
        this.setOnContextMenuRequested(event -> {
            if (event.getTarget() instanceof TreeTableCell) {
                @SuppressWarnings("unchecked")// if you have problems - check this first
                TreeItem<Data> selectedItem = ((TreeTableCell<Data, StringProperty>) event.getTarget()).getTreeTableRow().getTreeItem();

                if (selectedItem != null) {
                    for (Component comp : parent.flowDiagram.components) {
                        if (comp.getId().equals(selectedItem.getValue().property.get())) {
                            selectedToDelete = comp;
                            contextMenu.show(body, event.getScreenX(), event.getScreenY());
                            break;
                        }
                    }
                }
            }
            if (event.getTarget() instanceof Text) {
                boolean flag = false;
                for (Component comp : parent.flowDiagram.components) {
                    if (comp.getId().equals(((Text) event.getTarget()).getText())) {
                        flag = true;
                        selectedToDelete = comp;
                        break;
                    }
                }
                if (flag) {
                    contextMenu.show(body, event.getScreenX(), event.getScreenY());
                }
            }
        });

        body.addEventHandler(MouseEvent.MOUSE_CLICKED, e -> contextMenu.hide());

        MenuItem delete = new MenuItem("Delete");
        delete.setOnAction(actionEvent -> {
            parent.flowDiagram.compToDelete = parent.flowDiagram.components.indexOf(selectedToDelete);
            parent.flowDiagram.components.remove(selectedToDelete);
            selectedToDelete = null;
            actionEvent.consume();
        });
        contextMenu.getItems().addAll(delete);

        this.getChildren().add(body);
    }

    public void setContext(BlockProperties viewModel, MainWindow parent) {
        this.viewModel = viewModel;
        this.parent = parent;

        showFlowDiagramBlock();

        this.parent.flowDiagram.amountSelected.addListener((observableValue, number, t1) -> {
            switch (observableValue.getValue().intValue()) {
                case 0 -> showFlowDiagramBlock();
                case 1 -> {
                    showSelectedBaseObject();
                    showSelectedLink();
                }
                default -> {
                    rootItem.getChildren().clear();
                    currentMode = BPmodes.None;
                }
            }
        });
    }

    public void updateBlockProperties() {
        switch (this.parent.flowDiagram.amountSelected.getValue()) {
            case 0 -> showFlowDiagramBlock();
            case 1 -> {
                showSelectedBaseObject();
                showSelectedLink();
            }
            default -> {
                rootItem.getChildren().clear();
                currentMode = BPmodes.None;
            }
        }
    }

    private void showFlowDiagramBlock() {
        body.setEditable(true);
        currentMode = BPmodes.FlowDiagram;

        secondTableColumn.setOnEditCommit(event -> event.getRowValue().getValue());

        SimpleBlock docName = new SimpleBlock(new Pair<>("Document name", parent.flowDiagram.docName.get()));
        docName.data.editable.set(false);
        docName.data.value.bind(parent.flowDiagram.docName);
        SimpleBlock amount = new SimpleBlock(new Pair<>("Amount of objects", Integer.toString(this.parent.flowDiagram.objects.size())));

        amount.data.editable.set(false);

        List<Pair<String, String>> argArray = new ArrayList<>();

        argArray.add(new Pair<>("Width", Double.toString(parent.flowDiagram.width.get())));
        argArray.add(new Pair<>("Height", Double.toString(parent.flowDiagram.height.get())));

        CompoundBlock size = new CompoundBlock("Size", argArray);

        for (Data i : size.dataList) {
            i.setEditable(false);
        }

        TreeItem<Data> componentsBlock = new TreeItem<>(new Data("Components", ""));
        TreeItem<Data> systemsBlock = new TreeItem<>(new Data("Calculation System", ""));

        for (Component i : parent.flowDiagram.components) {
            CompoundBlock componentCompoundBlock = new CompoundBlock(i.getId(), i.getAllFields());
            componentsBlock.getChildren().add(componentCompoundBlock);
        }

        TreeItem<Data> functionalComponentBlock = new TreeItem<>(new Data("Option", "New Component"));

        functionalComponentBlock.getValue().choice.set(true);

        for (Component comp : Component.consts) {
            if (!parent.flowDiagram.components.contains(comp))
                functionalComponentBlock.getValue().choices.add(comp.getId());
        }
        functionalComponentBlock.getValue().choices.add("New Component");//feature for adding new component in list
        componentsBlock.getChildren().add(functionalComponentBlock);

        for (List<Double> j : parent.flowDiagram.systems) {
            TreeItem<Data> array = new TreeItem<>(new Data("Array", ""));
            for (int k = 0; k < j.size(); k++) {
                SimpleBlock value = new SimpleBlock(new Pair<>(Integer.toString(k), j.get(k).toString()));
                int finalK = k;
                value.data.value.addListener((observable, oldValue, newValue) -> {
                    try {
                        Double val = Double.parseDouble(newValue);
                        j.remove(finalK);
                        j.add(finalK, val);
                    } catch (NumberFormatException | NullPointerException e) {
                        Alert alert = new Alert(Alert.AlertType.ERROR);
                        alert.setTitle("ERROR");
                        alert.setHeaderText(null);
                        alert.setContentText("You must enter a number!");
                        alert.showAndWait();
                    }
                });
                array.getChildren().add(value);
            }
            systemsBlock.getChildren().add(array);
        }

        rootItem.getChildren().clear();

        rootItem.getChildren().add(docName);

        rootItem.getChildren().add(componentsBlock);
        if (systemsBlock.getChildren().size() > 0)
            rootItem.getChildren().add(systemsBlock);

        rootItem.getChildren().add(amount);
        rootItem.getChildren().add(size);
    }

    private void showSelectedBaseObject() {
        body.setEditable(true);

        ByteArrayInputStream data = new ByteArrayInputStream(parent.flowDiagram.prepareForBlockProperties());
        DocumentBuilder documentBuilder = null;

        try {
            documentBuilder = DocumentBuilderFactory.newInstance().newDocumentBuilder();
        } catch (ParserConfigurationException e) {
            e.printStackTrace();
        }

        Document document = null;

        try {
            assert documentBuilder != null;
            document = documentBuilder.parse(data);
        } catch (SAXException | IOException e) {
            e.printStackTrace();
        }
        if (parent.flowDiagram.getSelectedObjects().size() > 0) {
            currentMode = BPmodes.BaseObject;
            BaseObject object = parent.flowDiagram.getSelectedObjects().get(0);

            viewModel.setInfoFrom(object);

            Element obj = object.XMLexport(document);

            NamedNodeMap attributes = obj.getAttributes();

            rootItem.getChildren().clear();//WARNING

            SimpleBlock objectClass = new SimpleBlock(new Pair<>("Object class", object.type.toString()));
            rootItem.getChildren().add(objectClass);
            objectClass.getValue().editable.set(false);

            for (int i = 0; i < attributes.getLength(); i++) {
                SimpleBlock attribute = new SimpleBlock(new Pair<>(attributes.item(i).getNodeName(), attributes.item(i).getNodeValue()));
                rootItem.getChildren().add(attribute);
            }

            NodeList compoundAttributes = obj.getChildNodes();

            for (int i = 0; i < compoundAttributes.getLength(); i++) {
                CompoundBlock compoundAttribute;
                NodeList params = compoundAttributes.item(i).getChildNodes();
                List<Pair<String, String>> propertyValueList = new ArrayList<>();
                for (int j = 0; j < params.getLength(); j++) {
                    propertyValueList.add(new Pair<>("[" + params.item(j).getNodeName().split(":")[1] + "]", params.item(j).getTextContent()));
                }
                compoundAttribute = new CompoundBlock(compoundAttributes.item(i).getNodeName(), propertyValueList);

                rootItem.getChildren().add(compoundAttribute);
            }
        }
    }

    private void showSelectedLink() {
        body.setEditable(true);

        ByteArrayInputStream data = new ByteArrayInputStream(parent.flowDiagram.prepareForBlockProperties());
        DocumentBuilder documentBuilder = null;

        try {
            documentBuilder = DocumentBuilderFactory.newInstance().newDocumentBuilder();
        } catch (ParserConfigurationException e) {
            e.printStackTrace();
        }

        Document document = null;

        try {
            assert documentBuilder != null;
            document = documentBuilder.parse(data);
        } catch (SAXException | IOException e) {
            e.printStackTrace();
        }

        if (parent.flowDiagram.getSelectedLinks().size() > 0) {
            currentMode = BPmodes.Link;
            Link link = parent.flowDiagram.getSelectedLinks().get(0);

            viewModel.setInfoFrom(link);

            assert document != null;
            Element lnk = link.XMLexport(document);

            NamedNodeMap attributes = lnk.getAttributes();

            rootItem.getChildren().clear();

            for (int i = 0; i < attributes.getLength(); i++) {
                SimpleBlock attribute = new SimpleBlock(new Pair<>(attributes.item(i).getNodeName(), attributes.item(i).getNodeValue()));

                if (attributes.item(i).getNodeName().equals("Type")) {
                    attribute.data.choice.set(true);
                    for (LinkType type : LinkType.values())
                        attribute.data.choices.add(type.toString());
                } else {
                    attribute.data.editable.set(false);
                }
                rootItem.getChildren().add(attribute);
            }
        }
    }

    public static class SimpleBlock extends TreeItem<Data> {
        final public Data data;

        public SimpleBlock(Pair<String, String> propertyValuePair) {
            super();
            data = new Data(propertyValuePair.getKey(), propertyValuePair.getValue());
            this.setValue(data);
        }
    }

    public static class CompoundBlock extends TreeItem<Data> {
        final public List<Data> dataList = new ArrayList<>();
        final SimpleStringProperty property;
        final SimpleStringProperty value;

        public CompoundBlock(String propertyName, List<Pair<String, String>> propertyValueList) {

            this.property = new SimpleStringProperty(propertyName);
            this.value = new SimpleStringProperty(propertyValueList.toString());

            this.setValue(new Data(property.get(), value.get()));

            for (Pair<String, String> item : propertyValueList) {
                Data data = new Data(item.getKey(), item.getValue());
                dataList.add(data);
                this.getChildren().add(new TreeItem<>(data));
            }
        }

        public SimpleStringProperty getProperty() {
            return property;
        }

        public SimpleStringProperty getValueProperty() {
            return value;
        }
    }

    public static class Data {
        public final ObservableList<String> choices = FXCollections.observableArrayList();
        final SimpleStringProperty property;
        final SimpleStringProperty value;
        private final BooleanProperty editable = new SimpleBooleanProperty(true);
        private final BooleanProperty choice = new SimpleBooleanProperty();

        public Data(String property, String value) {
            this.property = new SimpleStringProperty(property);
            this.value = new SimpleStringProperty(value);
        }

        public SimpleStringProperty getProperty() {
            return property;
        }

        public void setProperty(String str) {
            this.property.set(str);
        }

        public SimpleStringProperty getValueProperty() {
            return value;
        }

        public void setValueProperty(String str) {
            this.value.set(str);
        }

        public boolean isEditable() {
            return editable.get();
        }

        public void setEditable(boolean editable) {
            this.editable.set(editable);
        }

        public BooleanProperty editableProperty() {
            return editable;
        }

        public boolean getChoice() {
            return choice.get();
        }

        public void setChoice(boolean choice) {
            this.choice.set(choice);
        }

        public BooleanProperty choiceProperty() {
            return choice;
        }
    }
}