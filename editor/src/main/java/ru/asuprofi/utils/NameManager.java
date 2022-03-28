package ru.asuprofi.utils;

import ru.asuprofi.viewModel.FlowDiagram;
import ru.asuprofi.viewModel.links.Link;
import ru.asuprofi.viewModel.objects.BaseObject;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

public class NameManager {

    private final Map<BaseObject, String> standardProcessUnitNames;// "c" template
    private final Map<BaseObject, String> standardInstrumentUnitNames;// "i" template
    private final Map<BaseObject, String> freeUserNames;// other templates
    private final Map<Link, String> standardStreamLinkNames;// "s" template
    private final Map<Link, String> standardSignalLinkNames;// "w" template
    FlowDiagram parent;

    public NameManager() {
        standardProcessUnitNames = new HashMap<>();
        standardInstrumentUnitNames = new HashMap<>();
        freeUserNames = new HashMap<>();

        standardStreamLinkNames = new HashMap<>();
        standardSignalLinkNames = new HashMap<>();
    }

    private String nextLexicographicalName(String name) {
        String res;
        String nameString = name.replaceAll("\\d+", "");
        int nameCode = Integer.parseInt(name.replaceAll("\\D+", ""));
        nameCode++;
        res = nameString + nameCode;
        return res;
    }

    private void setFreeProcessUnitName(BaseObject obj) {
        if (standardProcessUnitNames.containsValue(obj.getId()) || obj.getIdProperty().get() == null || obj.getIdProperty().get().equals("")) {
            String res = "c1000";
            if (obj.getId() == null) {
                obj.setId(res);
            }

            boolean flag = true;

            while (flag) {
                if (standardProcessUnitNames.containsValue(res)) {
                    res = nextLexicographicalName(res);
                } else {
                    flag = false;
                }
            }
            obj.setId(res);
        }
        standardProcessUnitNames.put(obj, obj.getId());
    }

    private void setFreeInstrumentUnitName(BaseObject obj) {
        if (standardInstrumentUnitNames.containsValue(obj.getId()) || obj.getIdProperty().get() == null || obj.getIdProperty().get().equals("")) {
            String res = "i1000";

            if (obj.getId() == null) {
                obj.setId(res);
            }

            boolean flag = true;
            while (flag) {
                if (standardInstrumentUnitNames.containsValue(res)) {
                    res = nextLexicographicalName(res);
                } else {
                    flag = false;
                }
            }
            obj.setId(res);
        }
        standardInstrumentUnitNames.put(obj, obj.getId());
    }

    private void setFreeUserName(BaseObject obj) {
        switch (obj.objectClass) {
            case ProcessUnit -> {
                if (freeUserNames.containsValue(obj.getId()) || obj.getIdProperty().get() == null || obj.getIdProperty().get().equals("")) {
                    obj.setId("c1000");
                    setFreeProcessUnitName(obj);
                } else {
                    freeUserNames.put(obj, obj.getId());
                }
            }
            case InstrumentUnit -> {
                if (freeUserNames.containsValue(obj.getId()) || obj.getIdProperty().get() == null || obj.getIdProperty().get().equals("")) {
                    obj.setId("i1000");
                    setFreeInstrumentUnitName(obj);
                } else {
                    freeUserNames.put(obj, obj.getId());
                }
            }
            default -> System.out.println("Object is unknown!");
        }
    }

    private Boolean isUserName(BaseObject obj) {
        String nameString = obj.getId().replaceAll("\\d+", "");
        return (!nameString.equals("c") && !nameString.equals("i")) && obj.getId() != null && !obj.getId().equals("");
    }

    private void setStreamLinkNames(Link link) {
        if (standardStreamLinkNames.containsValue(link.getId()) || link.getId() == null || link.getId().equals("")) {
            String res = "s1000";
            if (link.getId() == null) {
                link.setId(res);
            }

            boolean flag = true;

            while (flag) {
                if (standardStreamLinkNames.containsValue(res)) {
                    res = nextLexicographicalName(res);
                } else {
                    flag = false;
                }
            }
            link.setId(res);
        }
        standardStreamLinkNames.put(link, link.getId());
    }

    private void setSignalLinkNames(Link link) {
        if (standardSignalLinkNames.containsValue(link.getId()) || link.getId() == null || link.getId().equals("")) {
            String res = "w1000";
            if (link.getId() == null) {
                link.setId(res);
            }

            boolean flag = true;

            while (flag) {
                if (standardSignalLinkNames.containsValue(res)) {
                    res = nextLexicographicalName(res);
                } else {
                    flag = false;
                }
            }
            link.setId(res);
        }
        standardSignalLinkNames.put(link, link.getId());
    }

    public void setObjectName(BaseObject obj) {
        if (isUserName(obj)) {
            setFreeUserName(obj);
        } else {
            switch (obj.objectClass) {
                case ProcessUnit -> setFreeProcessUnitName(obj);
                case InstrumentUnit -> setFreeInstrumentUnitName(obj);
                default -> System.out.println("Some error occurred in naming object");
            }
        }
    }

    public void setLinkName(Link link) {
        switch (link.getLinkClass()) {
            case Stream -> setStreamLinkNames(link);
            case Signal -> setSignalLinkNames(link);
            default -> System.out.println("Some error occurred in naming link");
        }
    }

    public void freeObjectName(BaseObject obj) {
        if (isUserName(obj)) {
            freeUserNames.remove(obj);
        } else {
            switch (obj.objectClass) {
                case ProcessUnit -> standardProcessUnitNames.remove(obj, obj.getId());
                case InstrumentUnit -> standardInstrumentUnitNames.remove(obj, obj.getId());
                default -> System.out.println("Some error occurred in naming objects");
            }
        }
    }

    public void freeLinkName(Link link) {
        switch (link.getLinkClass()) {
            case Stream -> standardStreamLinkNames.remove(link, link.getId());
            case Signal -> standardSignalLinkNames.remove(link, link.getId());
            default -> System.out.println("Some error occurred in naming objects");
        }
    }

    public void freeObjectsNames(List<BaseObject> objectList) {
        for (BaseObject i : objectList) {
            freeObjectName(i);
        }
    }

    public void freeLinksNames(List<Link> linkList) {
        for (Link i : linkList) {
            freeLinkName(i);
        }
    }

    public void setContext(FlowDiagram parent) {
        this.parent = parent;
    }
}