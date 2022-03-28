package ru.asuprofi.viewModel;

import javafx.geometry.Point2D;
import javafx.geometry.Rectangle2D;
import ru.asuprofi.viewModel.links.Link;
import ru.asuprofi.viewModel.objects.BaseObject;

import java.util.List;

public class DragDTO {
    public String viewDragID;
    public Rectangle2D BR;
    public Point2D offset;
    public List<BaseObject> objectList;
    public List<Link> linkList;
}
