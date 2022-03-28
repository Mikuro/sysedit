package ru.asuprofi.viewModel.objects.factory;

import ru.asuprofi.viewModel.objects.*;

public class ObjectFactory {
    public BaseObject createObject(String name) {
        return switch (name) {
            case "ControlValve" -> new ControlValve();
            case "FlowMeter" -> new FlowMeter();
            case "LiquidLevelMeter" -> new LiquidLevelMeter();
            case "LiquidTank" -> new LiquidTank();
            case "PIDController" -> new PIDController();
            case "PressureFeed" -> new PressureFeed();
            case "PressureGauge" -> new PressureGauge();
            case "PressureProduct" -> new PressureProduct();
            case "HeatExchanger" -> new HeatExchanger();
            case "Mixer" -> new Mixer();
            case "Pipe" -> new Pipe();
            case "Pump" -> new Pump();
            case "Splitter" -> new Splitter();
            case "Strainer" -> new Strainer();
            case "Thermometer" -> new Thermometer();
            default -> null;
        };
    }
}
