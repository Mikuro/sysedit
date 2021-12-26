package ru.asuprofi.viewModel;

import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.Node;
import org.w3c.dom.NodeList;
import org.xml.sax.SAXException;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;
import java.io.ByteArrayInputStream;
import java.io.IOException;
import java.util.ArrayList;
import java.util.List;
import java.util.Objects;

public class Item {
    final List<String> paths = new ArrayList<>();
    final List<String> glyphs = new ArrayList<>();
    double width;
    double height;

    public Item(String objectName) {
        byte[] arr = new byte[0];
        try {
            arr = Objects.requireNonNull(Item.class.getResourceAsStream("/objects/" + objectName + ".xml")).readAllBytes();
        } catch (IOException e) {
            e.printStackTrace();
        }

        ByteArrayInputStream data = new ByteArrayInputStream(arr);
        try {
            DocumentBuilder documentBuilder = DocumentBuilderFactory.newInstance().newDocumentBuilder();
            Document document = documentBuilder.parse(data);
            Element head = document.getDocumentElement();

            head.normalize();

            NodeList headNodes = head.getChildNodes();

            Element canvas = null;

            for (int i = 0; canvas == null; i++) {
                Node tmp = headNodes.item(i);
                if (tmp instanceof Element)
                    canvas = (Element) tmp;
            }

            height = Double.parseDouble(canvas.getAttribute("Height"));
            width = Double.parseDouble(canvas.getAttribute("Width"));

            NodeList pathList = canvas.getElementsByTagName("Path");
            for (int i = 0; i < pathList.getLength(); i++) {
                Element path = (Element) pathList.item(i);
                Element pathData = (Element) path.getChildNodes().item(1);
                Element pathGeometry = (Element) pathData.getChildNodes().item(1);
                paths.add(pathGeometry.getAttribute("Figures") + " " + (!path.getAttribute("Fill").isEmpty() ? path.getAttribute("Fill") : null) + " " + (!path.getAttribute("Stroke").isEmpty() ? path.getAttribute("Stroke") : null));
            }

            NodeList glyphsList = canvas.getElementsByTagName("Glyphs");
            for (int i = 0; i < glyphsList.getLength(); i++) {
                Element glyph = (Element) glyphsList.item(i);
                glyphs.add(glyph.getAttribute("Fill") + " " + glyph.getAttribute("FontRenderingEmSize") + " " + glyph.getAttribute("FontUrl") + " "
                        + glyph.getAttribute("OriginX") + " " + glyph.getAttribute("OriginY") + " " + glyph.getAttribute("UnicodeString"));
            }
        } catch (ParserConfigurationException | IOException | SAXException e) {
            System.out.println(e.getMessage());
        }

    }

    public double getWidth() {
        return width;
    }

    public double getHeight() {
        return height;
    }

    public List<String> getPaths() {
        return paths;
    }

    public List<String> getGlyphs() {
        return glyphs;
    }
}
