package ru.asuprofi.utils;

import java.util.ArrayList;
import java.util.List;

public class CompositeUndoableAction extends UndoableAction {

    List<UndoableAction> actions = new ArrayList<>();

    @Override
    public void Do() {
        for (UndoableAction action : actions) {
            action.Do();
        }
    }

    @Override
    public void Undo() {
        for (UndoableAction action : actions) {
            action.Undo();
        }
    }

    @Override
    public void Redo() {
        for (UndoableAction action : actions) {
            action.Redo();
        }
    }

    public void put(UndoableAction action) {
        actions.add(action);
    }
}
