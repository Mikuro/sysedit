package ru.asuprofi.viewModel;

import javafx.collections.FXCollections;
import javafx.collections.ObservableList;
import javafx.scene.control.Alert;
import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.NodeList;
import org.xml.sax.SAXException;
import ru.asuprofi.utils.ActionManager;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;
import javax.xml.transform.OutputKeys;
import javax.xml.transform.Transformer;
import javax.xml.transform.TransformerException;
import javax.xml.transform.TransformerFactory;
import javax.xml.transform.dom.DOMSource;
import javax.xml.transform.stream.StreamResult;
import java.io.*;
import java.nio.file.Files;
import java.nio.file.Path;


public class MainWindow {
    public final ObservableList<FlowDiagram> sheetList = FXCollections.observableArrayList();
    public FlowDiagram flowDiagram;

    public MainWindow() {
        createNewFlowDiagram();
    }

    public void setCurrentFlowDiagram(FlowDiagram flowDiagram) {
        this.flowDiagram = flowDiagram;
    }

    public void createNewFlowDiagram() {
        flowDiagram = new FlowDiagram();
        flowDiagram.connectSystems();
        sheetList.add(flowDiagram);
    }

    public void clearSheetList() {
        this.sheetList.clear();
    }

    public void deleteFlowDiagramFromSheetList(FlowDiagram toDelete) {
        this.sheetList.remove(toDelete);
    }

    public ActionManager getManager() {
        return flowDiagram.getActionManager();
    }

    public void saveFile(File saveFile) {
        OutputStream outputStream;
        ByteArrayOutputStream byteOutputStream = new ByteArrayOutputStream();
        try {
            DocumentBuilder documentBuilder = DocumentBuilderFactory.newInstance().newDocumentBuilder();
            Document document = documentBuilder.newDocument();
            outputStream = new FileOutputStream(saveFile);
            Element rootObject = document.createElement("Sheets");
            document.appendChild(rootObject);
            for (FlowDiagram diagram : sheetList) {
                Element diagramNode = document.createElement("FlowDiagram");
                diagramNode.setAttribute("DocName", diagram.docName.get());
                diagramNode.setAttribute("Width", String.valueOf(diagram.width.get()));
                diagramNode.setAttribute("Height", String.valueOf(diagram.height.get()));
                diagram.prepareFlowDiagramComponents(document, diagramNode);
                diagram.prepareFlowDiagramSystems(document, diagramNode);
                if (diagram.prepareDataSave(document, diagramNode)) {
                    Alert alert = new Alert(Alert.AlertType.ERROR);
                    alert.setTitle("Error");
                    alert.setHeaderText(null);
                    alert.setContentText("Contains object with errors in " + diagram.docName.get());
                    alert.showAndWait();
                }
                diagram.prepareLinksOnFlowDiagram(document, diagramNode);
                rootObject.appendChild(diagramNode);
            }
            Transformer tr = TransformerFactory.newInstance().newTransformer();
            tr.setOutputProperty(OutputKeys.OMIT_XML_DECLARATION, "yes");
            tr.setOutputProperty(OutputKeys.ENCODING, "UTF-8");
            tr.setOutputProperty(OutputKeys.INDENT, "yes");
            DOMSource source = new DOMSource(document);
            StreamResult result = new StreamResult(byteOutputStream);
            tr.transform(source, result);
            byteOutputStream.writeTo(outputStream);
        } catch (TransformerException | IOException | ParserConfigurationException e) {
            e.printStackTrace();
        }
    }

    public void loadFile(File saveFile) {
        byte[] arr = new byte[0];
        try {
            arr = Files.readAllBytes(Path.of(saveFile.getPath()));
        } catch (IOException e) {
            e.printStackTrace();
        }
        clearSheetList();
        ByteArrayInputStream data = new ByteArrayInputStream(arr);

        DocumentBuilder documentBuilder;
        try {
            documentBuilder = DocumentBuilderFactory.newInstance().newDocumentBuilder();
            Document document = documentBuilder.parse(data);
            Element rootObject = document.getDocumentElement();

            if (rootObject.getTagName().equals("FlowDiagram")) {
                FlowDiagram diagram = new FlowDiagram();
                diagram.parseFlowDiagram(document, rootObject);
                diagram.connectSystems();
                sheetList.add(diagram);
                this.flowDiagram = diagram;
                diagram.setCurrentSaveDirectory(saveFile);
            } else {
                NodeList flowDiagramList = rootObject.getElementsByTagName("FlowDiagram");

                for (int i = 0; i < flowDiagramList.getLength(); i++) {
                    Element child = (Element) flowDiagramList.item(i);
                    FlowDiagram diagram = new FlowDiagram();
                    diagram.parseFlowDiagram(document, child);
                    diagram.connectSystems();
                    sheetList.add(diagram);
                    this.flowDiagram = diagram;
                }
            }

        } catch (ParserConfigurationException | SAXException | IOException e) {
            e.printStackTrace();
        }
    }

}