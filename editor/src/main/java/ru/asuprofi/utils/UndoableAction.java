package ru.asuprofi.utils;

public abstract class UndoableAction {
    public abstract void Do();

    public abstract void Undo();

    public abstract void Redo();

}
