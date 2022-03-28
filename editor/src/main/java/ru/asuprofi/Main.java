package ru.asuprofi;

import javafx.application.Application;
import javafx.scene.Scene;
import javafx.stage.Stage;
import ru.asuprofi.view.MainWindowView;
import ru.asuprofi.viewModel.MainWindow;

import java.util.Objects;

public class Main extends Application {

    MainWindowView mainView = null;

    public static void main(String[] args) {
        launch(args);
    }

    @Override
    public void start(Stage stage) {

        mainView = new MainWindowView();
        MainWindow viewModel = new MainWindow();

        Scene scene = new Scene(mainView, 1280, 720);
        mainView.setMaxSize(1280, 720);

        String stylesheet = Objects.requireNonNull(getClass().getResource("/css/stylesheet.css")).toExternalForm();

        scene.getStylesheets().add(stylesheet);

        stage.setTitle("Sysedit");
        stage.setScene(scene);
        mainView.setStage(stage);
        mainView.setContext(viewModel);
        stage.setOnCloseRequest(event -> {
            if (!mainView.finishApplication())
                event.consume();
        });
        stage.show();
    }
}