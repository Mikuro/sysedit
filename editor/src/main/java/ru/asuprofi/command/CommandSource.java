package ru.asuprofi.command;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

public class CommandSource {

    private final List<Command> cmdList = new ArrayList<>();
    private CommandManager manager;

    public CommandSource(Command... list) {
        cmdList.addAll(Arrays.asList(list));
    }

    public List<Command> getCmdList() {
        return cmdList;
    }

    public void setManager(CommandManager manager) {
        this.manager = manager;
    }

    public void enable() {
        manager.enable(this);
    }

    public void disable() {
        manager.disable(this);
    }
}
