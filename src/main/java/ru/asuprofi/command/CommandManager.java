package ru.asuprofi.command;

import javafx.scene.control.Menu;
import javafx.scene.control.MenuBar;
import javafx.scene.control.MenuItem;

import java.util.*;

public class CommandManager {

    private final List<CommandSource> srcList = new ArrayList<>();
    private final Map<CommandSource, Boolean> isEnabledMap = new HashMap<>();
    private MenuBar menuBar;

    public CommandManager() {
    }

    public void setMenuBar(MenuBar menuBar) {
        this.menuBar = menuBar;
    }

    public void register(CommandSource src) {
        src.setManager(this);
        if (!srcList.contains(src)) {
            srcList.add(src);
            isEnabledMap.put(src, false);
        }
    }

    public void unregister(CommandSource src) {
        srcList.remove(src);
    }

    String myCombine(List<String> nm, int n) {
        StringBuilder path = new StringBuilder();
        for (int i = 0; i < (n + 1); i++) {
            if (i > 0)
                path.append("/");
            path.append(nm.get(i));
        }
        return path.toString();
    }

    //menuBar problem is here
    void rebuild() {
        Menu menu;
        Map<String, Menu> menuMap = new HashMap<>();
        List<Command> cmdList = new ArrayList<>();

        for (CommandSource cs : srcList)
            if (isEnabledMap.get(cs))
                cmdList.addAll(cs.getCmdList());

        menuBar.getMenus().clear();

        for (Command cmd : cmdList) {
            List<String> nm = Arrays.asList(cmd.name.split("/"));
            Menu currLevel = null;

            for (int i = 0; i < nm.size() - 1; i++) {
                String path = myCombine(nm, i);
                if (!menuMap.containsKey(path)) {
                    menu = new Menu(nm.get(i));
                    menuMap.put(path, menu);
                    if (currLevel == null) {
                        menuBar.getMenus().add(menu);
                    } else {
                        currLevel.getItems().add(menu);
                    }
                    currLevel = menu;
                }
            }

            menu = menuMap.get(myCombine(nm, nm.size() - 2));

            boolean flag = false;

            MenuItem menuItem = null;

            for (MenuItem i : menu.getItems()) {
                if (i.getText().equals(nm.get(nm.size() - 1))) {
                    flag = true;
                    menuItem = i;
                }
            }

            if (!flag) {
                menuItem = new MenuItem(nm.get(nm.size() - 1));
                menu.getItems().add(menuItem);
            }

            menuItem.setAccelerator(cmd.shortcut);
            menuItem.setOnAction(cmd);
        }
    }

    public void enable(CommandSource src) {
        isEnabledMap.put(src, true);
        rebuild();
    }

    public void disable(CommandSource src) {
        isEnabledMap.put(src, false);
        rebuild();
    }
}