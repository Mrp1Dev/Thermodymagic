using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum StateProperty
{
    Volume,
    Temperature,
    Pressure,
    Moles
}

public enum VolumeUnits
{
    L,
    ml,
    m3
}

public enum TemperatureUnits
{
    K,
    C
}

public enum PressureUnits
{
    Pa,
    atm,
    mmHg
}

public class StateUIInput : MonoBehaviour
{
    public StateProperty stateProperty;
    private TMP_Dropdown unitDropdown;
    private TMP_InputField inputField;
    private void Awake()
    {
        inputField = GetComponentInChildren<TMP_InputField>();
        inputField.onEndEdit.AddListener(OnValueChanged);
        unitDropdown = GetComponentInChildren<TMP_Dropdown>();
    }

    private void OnValueChanged(string value)
    {
        if (float.TryParse(value, out float numericalValue) == false) return;
        var piston = PistonSimulator.instance;
        if (piston.currentGameState == CurrentGameState.Initial)
        {
            float siValue = 0;
            switch (stateProperty)
            {
                case StateProperty.Volume:
                    var volUnit = (VolumeUnits)unitDropdown.value;
                    siValue = volUnit switch
                    {
                        VolumeUnits.L => numericalValue / 1000f,
                        VolumeUnits.ml => (numericalValue / 1000f) / 1000f,
                        VolumeUnits.m3 => numericalValue,
                    };
                    piston.initialState.volume = siValue;
                    piston.SetInitialState(piston.initialState);
                    break;
                case StateProperty.Temperature:
                    var tempUnit = (TemperatureUnits)unitDropdown.value;
                    siValue = tempUnit switch
                    {
                        TemperatureUnits.K => numericalValue,
                        TemperatureUnits.C => numericalValue + 273f,
                    };
                    piston.initialState.temperature = siValue;
                    piston.SetInitialState(piston.initialState);
                    break;
                case StateProperty.Pressure:
                    var pressureUnit = (PressureUnits)unitDropdown.value;
                    siValue = pressureUnit switch
                    {
                        PressureUnits.Pa => numericalValue,
                        PressureUnits.atm => numericalValue * 100000f,
                        PressureUnits.mmHg => numericalValue * 133.322f,
                    };
                    piston.initialState.pressure = siValue;
                    piston.SetInitialState(piston.initialState);
                    break;
                case StateProperty.Moles:
                    siValue = numericalValue;
                    piston.initialState.moles = siValue;
                    piston.SetInitialState(piston.initialState);
                    break;
            }
        }
        else if (piston.currentGameState == CurrentGameState.Final)
        {
            float siValue = 0;
            switch (stateProperty)
            {
                case StateProperty.Volume:
                    var volUnit = (VolumeUnits)unitDropdown.value;
                    siValue = volUnit switch
                    {
                        VolumeUnits.L => numericalValue / 1000f,
                        VolumeUnits.ml => (numericalValue / 1000f) / 1000f,
                        VolumeUnits.m3 => numericalValue,
                    };
                    piston.finalState.volume = siValue;
                    break;
                case StateProperty.Temperature:
                    var tempUnit = (TemperatureUnits)unitDropdown.value;
                    siValue = tempUnit switch
                    {
                        TemperatureUnits.K => numericalValue,
                        TemperatureUnits.C => numericalValue + 273f,
                    };
                    piston.finalState.temperature = siValue;
                    break;
                case StateProperty.Pressure:
                    var pressureUnit = (PressureUnits)unitDropdown.value;
                    siValue = pressureUnit switch
                    {
                        PressureUnits.Pa => numericalValue,
                        PressureUnits.atm => numericalValue * 100000f,
                        PressureUnits.mmHg => numericalValue * 133.322f,
                    };
                    piston.finalState.pressure = siValue;
                    break;
                case StateProperty.Moles:
                    siValue = numericalValue;
                    piston.finalState.moles = siValue;
                    break;
            }
        }
    }

    public void SetValue(float siValue)
    {
        var numericalValue = 0.0f;
        switch (stateProperty)
        {
            case StateProperty.Volume:
                var volUnit = (VolumeUnits)unitDropdown.value;
                numericalValue = volUnit switch
                {
                    VolumeUnits.L => siValue * 1000.0f,
                    VolumeUnits.ml => siValue * 1000.0f * 1000.0f,
                    VolumeUnits.m3 => siValue,
                };
                break;
            case StateProperty.Temperature:
                var tempUnit = (TemperatureUnits)unitDropdown.value;
                numericalValue = tempUnit switch
                {
                    TemperatureUnits.K => siValue,
                    TemperatureUnits.C => siValue - 273.0f,
                };
                break;
            case StateProperty.Pressure:
                var pUnit = (PressureUnits)unitDropdown.value;
                numericalValue = pUnit switch
                {
                    PressureUnits.Pa => siValue,
                    PressureUnits.atm => siValue / 100000.0f,
                    PressureUnits.mmHg => siValue / 133.322f,
                };
                break;
            case StateProperty.Moles:
                numericalValue = siValue;
                break;
        }
        inputField.text = numericalValue.ToString("N3");
    }

    public void MarkUnkown()
    {
        inputField.text = "----";
    }

    public void ReApplyValue()
    {
        OnValueChanged(inputField.text);
    }
}
