package ru.asuprofi.utils;

import javafx.beans.property.BooleanProperty;
import javafx.beans.property.SimpleBooleanProperty;

import java.util.Stack;

public class ActionManager {
    public final BooleanProperty canUndo = new SimpleBooleanProperty();
    public final BooleanProperty canRedo = new SimpleBooleanProperty();
    private final Stack<UndoableAction> undoStack = new Stack<>();
    private final Stack<UndoableAction> redoStack = new Stack<>();

    public void clear() {
        undoStack.clear();
        redoStack.clear();
    }

    public void doAction(UndoableAction act) {
        undoStack.push(act);
        redoStack.clear();
        act.Do();
        canUndo.set(true);
        canRedo.set(false);
    }

    public void undo() {
        if (!undoStack.empty()) {
            UndoableAction act = undoStack.pop();
            redoStack.push(act);
            act.Undo();
            if (undoStack.empty()) canUndo.set(false);
            canRedo.set(true);
        }
    }

    public void redo() {
        if (!redoStack.empty()) {
            UndoableAction act = redoStack.pop();
            undoStack.push(act);
            act.Redo();
            if (redoStack.empty()) canRedo.set(false);
            canUndo.set(true);
        }
    }
}