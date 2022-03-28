package ru.asuprofi.view.objects.factory;

import ru.asuprofi.view.objects.*;

public class ObjectViewFactory {
    public BaseObjectView createObjectView(String name) {
        return switch (name) {
            case "ControlValve" -> new ControlValveView();
            case "FlowMeter" -> new FlowMeterView();
            case "LiquidLevelMeter" -> new LiquidLevelMeterView();
            case "LiquidTank" -> new LiquidTankView();
            case "PIDController" -> new PIDControllerView();
            case "PressureFeed" -> new PressureFeedView();
            case "PressureGauge" -> new PressureGaugeView();
            case "PressureProduct" -> new PressureProductView();
            case "HeatExchanger" -> new HeatExchangerView();
            case "Mixer" -> new MixerView();
            case "Pipe" -> new PipeView();
            case "Pump" -> new PumpView();
            case "Splitter" -> new SplitterView();
            case "Strainer" -> new StrainerView();
            case "Thermometer" -> new ThermometerView();
            default -> null;
        };
    }
}
