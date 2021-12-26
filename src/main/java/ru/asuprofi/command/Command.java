package ru.asuprofi.command;

import javafx.event.ActionEvent;
import javafx.event.EventHandler;
import javafx.scene.input.KeyCombination;

public abstract class Command implements EventHandler<ActionEvent> {

    public final String name;
    public final KeyCombination shortcut;

    public Command(String name, KeyCombination shortcut) {
        this.name = name;
        this.shortcut = shortcut;
    }
}
