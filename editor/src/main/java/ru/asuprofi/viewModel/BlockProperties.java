package ru.asuprofi.viewModel;

import ru.asuprofi.viewModel.links.Link;
import ru.asuprofi.viewModel.objects.BaseObject;

import java.io.ByteArrayOutputStream;

// methods for serialization of document
// method to import from serialization
// method to update field??
//Object currentBPobject
//(FlowDiagram) currentBPoject
//(Link)currentBPobject
//(BaseObject)currentBPobject
//currentBPobject = null(to show message of nothing to show)

public class BlockProperties {
    private BaseObject currentObject = null;
    private Link currentLink = null;

    public BaseObject getInfoFromObject() {
        return currentObject;
    }

    public Link getInfoFromLink() {
        return currentLink;
    }

    public void setInfoFrom(BaseObject obj) {
        this.currentObject = obj;
        this.currentLink = null;
    }

    public void setInfoFrom(Link link) {
        this.currentLink = link;
        this.currentObject = null;
    }

    public void clear() {
        this.currentObject = null;
        this.currentLink = null;
    }

    public byte[] prepareInfoObject() {
        ByteArrayOutputStream data = new ByteArrayOutputStream();

        return data.toByteArray();
    }

    public void parseInfoObject() {

    }

    public byte[] prepareInfoLink() {
        ByteArrayOutputStream data = new ByteArrayOutputStream();

        return data.toByteArray();
    }

    public void parseInfoLink() {

    }
}