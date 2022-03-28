package ru.asuprofi.view;

import javafx.collections.ListChangeListener;
import javafx.event.ActionEvent;
import javafx.geometry.Orientation;
import javafx.scene.control.*;
import javafx.scene.input.KeyCode;
import javafx.scene.input.KeyCodeCombination;
import javafx.scene.input.KeyCombination;
import javafx.scene.layout.VBox;
import javafx.stage.FileChooser;
import javafx.stage.Stage;
import ru.asuprofi.command.Command;
import ru.asuprofi.command.CommandManager;
import ru.asuprofi.command.CommandSource;
import ru.asuprofi.viewModel.BlockProperties;
import ru.asuprofi.viewModel.DocumentTree;
import ru.asuprofi.viewModel.FlowDiagram;
import ru.asuprofi.viewModel.MainWindow;

import java.io.File;

public class MainWindowView extends VBox {
    private static final KeyCombination newTabShortcut = new KeyCodeCombination(KeyCode.N, KeyCombination.SHORTCUT_DOWN);
    private static final KeyCombination nextTabShortcut = new KeyCodeCombination(KeyCode.TAB, KeyCombination.SHORTCUT_DOWN);
    private static final KeyCombination prevTabShortcut = new KeyCodeCombination(KeyCode.TAB, KeyCombination.SHIFT_DOWN, KeyCombination.SHORTCUT_DOWN);


    final DocumentTreeView treeView = new DocumentTreeView();
    final BlockPropertiesView blockView = new BlockPropertiesView();
    final ScrollPane scrollPane = new ScrollPane();
    final SplitPane splitPane = new SplitPane();
    final DocumentTree DTviewModel = new DocumentTree();
    final BlockProperties BPviewModel = new BlockProperties();
    final TabPane tabPane = new TabPane();
    private final CommandManager commandManager = new CommandManager();
    FlowDiagramView currentFlowDiagramView = new FlowDiagramView();

    private MainWindow viewModel;

    private Stage stage;

    public MainWindowView() {
        MenuBar menuBar = new MenuBar();

        commandManager.setMenuBar(menuBar);

        getChildren().add(menuBar);

        tabPane.setTabDragPolicy(TabPane.TabDragPolicy.REORDER);
        tabPane.setTabClosingPolicy(TabPane.TabClosingPolicy.UNAVAILABLE);

        tabPane.getSelectionModel().selectedItemProperty().addListener((observableValue, tab, t1) -> {
            if (!currentFlowDiagramView.equals(tabPane.getSelectionModel().getSelectedItem().getContent()))
                updateContext((FlowDiagramView) tabPane.getSelectionModel().getSelectedItem().getContent());
        });

        tabPane.getTabs().addListener((ListChangeListener<Tab>) change -> {
            if (tabPane.getTabs().size() == 1)
                tabPane.setTabClosingPolicy(TabPane.TabClosingPolicy.UNAVAILABLE);
            else
                tabPane.setTabClosingPolicy(TabPane.TabClosingPolicy.ALL_TABS);
        });

        tabPane.setMaxWidth(3000);
        tabPane.setMaxHeight(1500);

        scrollPane.setContent(tabPane);

        currentFlowDiagramView.getStyleClass().add("flowDiagramView");
        currentFlowDiagramView.setStage(stage);

        registerCommands(commandManager);

        this.splitPane.getItems().add(treeView);
        this.splitPane.getItems().add(scrollPane);
        this.splitPane.getItems().add(blockView);

        this.splitPane.setOrientation(Orientation.HORIZONTAL);

        splitPane.getDividers().get(0).positionProperty().set(0.15);
        splitPane.getDividers().get(1).positionProperty().set(0.75);

        this.getChildren().add(splitPane);
    }

    public void setStage(Stage stage) {
        this.stage = stage;
        treeView.prefHeightProperty().bind(stage.getScene().heightProperty());
        blockView.prefHeightProperty().bind(stage.getScene().heightProperty());
        blockView.prefWidthProperty().bind(stage.getScene().widthProperty().multiply(0.15));
    }

    public void setContext(MainWindow viewModel) {
        this.viewModel = viewModel;

        currentFlowDiagramView.setContext(viewModel.flowDiagram);
        currentFlowDiagramView.registerCommands(commandManager);
        currentFlowDiagramView.enableCommands();
        Tab firstTab = new Tab(viewModel.flowDiagram.docName.get(), currentFlowDiagramView);
        firstTab.textProperty().bind(viewModel.flowDiagram.docName);
        firstTab.setOnClosed(event -> {
            if (currentFlowDiagramView.viewModel.dirtyFlag.get()) {
                File file = currentFlowDiagramView.viewModel.getCurrentSaveDirectory();
                if (file == null) {
                    FileChooser fc = new FileChooser();
                    fc.getExtensionFilters().addAll(new FileChooser.ExtensionFilter("All Files", "*.*"));
                    fc.setInitialFileName(currentFlowDiagramView.viewModel.docName.get() + ".xml");
                    file = fc.showSaveDialog(stage);
                    currentFlowDiagramView.viewModel.setCurrentSaveDirectory(file);
                }
                try {
                    currentFlowDiagramView.viewModel.saveFile(file);
                } catch (NullPointerException e) {
                    System.out.println("File wasn't found");
                }
            }
            viewModel.sheetList.remove(currentFlowDiagramView.viewModel);
        });
        tabPane.getTabs().add(firstTab);
        treeView.setContext(DTviewModel, viewModel);
        blockView.setContext(BPviewModel, viewModel);

        this.viewModel.sheetList.addListener((ListChangeListener<FlowDiagram>) change -> {
            if (change.next()) {
                if (change.wasAdded()) {
                    for (FlowDiagram i : change.getAddedSubList()) {
                        FlowDiagramView flowView = new FlowDiagramView();
                        flowView.getStyleClass().add("flowDiagramView");
                        flowView.setStage(stage);
                        flowView.setContext(i);
                        flowView.registerCommands(commandManager);
                        flowView.updateObjects();
                        flowView.updateLinks();
                        currentFlowDiagramView = flowView;
                        currentFlowDiagramView.enableCommands();
                        blockView.setContext(BPviewModel, viewModel);
                        Tab newTab = new Tab(flowView.viewModel.docName.get(), flowView);
                        newTab.textProperty().bind(flowView.viewModel.docName);
                        newTab.setOnClosed(event -> {
                            if (flowView.viewModel.dirtyFlag.get()) {
                                File file = flowView.viewModel.getCurrentSaveDirectory();
                                if (file == null) {
                                    FileChooser fc = new FileChooser();
                                    fc.getExtensionFilters().addAll(new FileChooser.ExtensionFilter("All Files", "*.*"));
                                    fc.setInitialFileName(flowView.viewModel.docName.get() + ".xml");
                                    file = fc.showSaveDialog(stage);
                                    flowView.viewModel.setCurrentSaveDirectory(file);
                                }
                                try {
                                    flowView.viewModel.saveFile(file);
                                } catch (NullPointerException e) {
                                    System.out.println("File wasn't found");
                                }
                            }
                            viewModel.sheetList.remove(flowView.viewModel);
                        });
                        tabPane.getTabs().add(newTab);
                        tabPane.getSelectionModel().select(newTab);
                    }
                }
            }
        });
    }

    public void updateContext(FlowDiagramView flowDiagramView) {
        currentFlowDiagramView.disableCommands();
        currentFlowDiagramView = flowDiagramView;
        currentFlowDiagramView.enableCommands();
        this.viewModel.setCurrentFlowDiagram(currentFlowDiagramView.viewModel);
        blockView.setContext(BPviewModel, viewModel);
    }

    public void createNewContext() {
        FlowDiagramView newFlowDiagram = new FlowDiagramView();
        newFlowDiagram.getStyleClass().add("flowDiagramView");
        newFlowDiagram.setStage(stage);
        viewModel.createNewFlowDiagram();
        newFlowDiagram.setContext(viewModel.flowDiagram);
        newFlowDiagram.registerCommands(commandManager);
        updateContext(newFlowDiagram);
    }

    public void registerCommands(CommandManager commandManager) {
        Command newTabCommand = new Command("File/New tab", newTabShortcut) {
            @Override
            public void handle(ActionEvent event) {
                try {
                    viewModel.createNewFlowDiagram();
                    event.consume();
                } catch (NullPointerException e) {
                    System.out.println(e.getMessage());
                }
            }
        };

        Command nextTab = new Command("Goto/Switch File/Next tab", nextTabShortcut) {
            @Override
            public void handle(ActionEvent event) {
                Tab curTab = tabPane.getSelectionModel().getSelectedItem();
                tabPane.getSelectionModel().selectNext();
                if (curTab.equals(tabPane.getSelectionModel().getSelectedItem()))
                    tabPane.getSelectionModel().selectFirst();
                event.consume();
            }
        };

        Command prevTab = new Command("Goto/Switch File/Previous tab", prevTabShortcut) {
            @Override
            public void handle(ActionEvent event) {
                Tab curTab = tabPane.getSelectionModel().getSelectedItem();
                tabPane.getSelectionModel().selectPrevious();
                if (curTab.equals(tabPane.getSelectionModel().getSelectedItem()))
                    tabPane.getSelectionModel().selectLast();
                event.consume();
            }
        };

        Command saveProjectCommand = new Command("File/Save Project", new KeyCodeCombination(KeyCode.S, KeyCombination.SHIFT_DOWN, KeyCombination.ALT_DOWN)) {
            @Override
            public void handle(ActionEvent actionEvent) {
                FileChooser fc = new FileChooser();
                fc.getExtensionFilters().addAll(new FileChooser.ExtensionFilter("All Files", "*.*"));
                File file = fc.showSaveDialog(stage);
                if (file != null) {
                    viewModel.saveFile(file);
                } else {
                    Alert alert = new Alert(Alert.AlertType.ERROR);
                    alert.setTitle("Error");
                    alert.setHeaderText(null);
                    alert.setContentText("This file doesn't exist");
                    alert.showAndWait();
                }
            }
        };

        Command loadCommand = new Command("File/Load", new KeyCodeCombination(KeyCode.O, KeyCombination.SHORTCUT_DOWN)) {
            @Override
            public void handle(ActionEvent actionEvent) {
                FileChooser fc = new FileChooser();
                fc.getExtensionFilters().addAll(new FileChooser.ExtensionFilter("All Files", "*.*"));
                File file = fc.showOpenDialog(stage);

                if (file != null) {
                    viewModel.loadFile(file);
                    viewModel.flowDiagram.docName.set(file.getName().replaceFirst("[.][^.]+$", ""));
                    updateContext(currentFlowDiagramView);
                } else {
                    Alert alert = new Alert(Alert.AlertType.ERROR);
                    alert.setTitle("Error");
                    alert.setHeaderText(null);
                    alert.setContentText("This file doesn't exist");
                    alert.showAndWait();
                }
            }
        };

        CommandSource commandSource = new CommandSource(loadCommand, newTabCommand, nextTab, prevTab);

        commandManager.register(commandSource);

        commandSource.enable();
    }

    public boolean finishApplication() {
        for (FlowDiagram i : viewModel.sheetList) {
            File file;
            if (i.dirtyFlag.get()) {
                FileChooser fc = new FileChooser();
                fc.setInitialFileName(i.docName.get());
                fc.getExtensionFilters().addAll(new FileChooser.ExtensionFilter("All Files", "*.*"));
                file = fc.showSaveDialog(stage);
                if(file == null)
                    return false;//
            } else {
                file = i.getCurrentSaveDirectory();
            }
            if (file != null) {
                viewModel.saveFile(file);
            }
        }
        return true;
    }
}