package ru.asuprofi.utils;

import javafx.beans.property.IntegerProperty;
import javafx.beans.property.SimpleIntegerProperty;
import javafx.collections.FXCollections;
import javafx.collections.ObservableList;
import ru.asuprofi.viewModel.FlowDiagram;

import java.util.LinkedList;
import java.util.List;

public class SystemsManager {

    UpperTriangularMatrix systemsMatrix = null;
    FlowDiagram parent = null;

    public SystemsManager() {
        //when component added from BP - correct systems
        //when component removed from BP - correct systems
    }

    public void registerSystems(ObservableList<ObservableList<Double>> systems) {
        systemsMatrix = new UpperTriangularMatrix(systems);
    }

    public void updateSystems(ObservableList<ObservableList<Double>> systems) {
        systems.clear();

        for (int i = 0; i < systemsMatrix.matrix.size(); i++) {
            ObservableList<Double> temp = FXCollections.observableArrayList();
            for (int j = i; j < systemsMatrix.matrix.size(); j++) {
                temp.add(systemsMatrix.matrix.get(i).get(j));
            }
            systems.add(temp);
        }
    }

    public void addComponent() {
        systemsMatrix.addRowAndColumn();
        systemsMatrix.size.set(systemsMatrix.size.get() + 1);
    }

    public void removeComponent(int componentPos) {
        systemsMatrix.deleteRowAndColumn(componentPos);
    }

    public void setContext(FlowDiagram flowDiagram) {
        this.parent = flowDiagram;
    }

    private static class UpperTriangularMatrix {
        List<List<Double>> matrix = new LinkedList<>();
        IntegerProperty size = new SimpleIntegerProperty(0);

        public UpperTriangularMatrix(int size) {
            for (int i = 0; i < size; i++) {
                matrix.add(new LinkedList<>());
            }
        }

        public UpperTriangularMatrix(ObservableList<ObservableList<Double>> dataArray) {
            size.set(dataArray.size());
            for (List<Double> i : dataArray) {
                if (i.size() > size.get()) {
                    size.set(i.size());
                }
            }

            for (List<Double> doubles : dataArray) {
                LinkedList<Double> temp = copyToLinkedList(doubles);
                correctLineFirst(temp);
                matrix.add(temp);
            }
        }

        private void correctLineFirst(List<Double> line) {
            int oldSize = line.size();
            for (int i = 0; i < size.get() - oldSize; i++) {
                ((LinkedList<Double>) line).addFirst(0.0);
            }
        }

        private void correctLineLast(List<Double> line) {
            int oldSize = line.size();
            for (int i = 0; i < size.get() - oldSize; i++) {
                ((LinkedList<Double>) line).addLast(0.0);
            }
        }

        public void deleteRowAndColumn(int pos) {
            if (pos == 0) {
                matrix.remove(pos);
                size.set(size.get() - 1);
            } else if (pos < size.get()) {
                for (List<Double> i : matrix) {
                    i.remove(pos - 1);
                }
                matrix.remove(pos - 1);
                size.set(size.get() - 1);
            }
        }

        public void addRowAndColumn() {
            if (size.get() > 0) {
                LinkedList<Double> newArray = new LinkedList<>();
                matrix.add(newArray);
                for (List<Double> i : matrix) {
                    correctLineLast(i);
                }
            }
        }

        //created, if usage will need use DoubleProperty in systems, then just change this method and fix template class
        private LinkedList<Double> copyToLinkedList(List<Double> array) {
            return new LinkedList<>(array);
        }

        @Override
        public String toString() {
            String result = "";
            for (List<Double> i : matrix) {
                result += "| ";
                for (Double j : i) {
                    result += j + " ";
                }
                result += "|\n";
            }
            return result;
        }
    }
}