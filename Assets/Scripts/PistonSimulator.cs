using System;
using UnityEngine;

[Serializable]
public enum CurrentGameState
{
    Initial = 0,
    Final = 1,
    Simulating = 2
}
public class PistonSimulator : MonoBehaviour
{
    [SerializeField] private float lineThickness;
    [SerializeField] private Color lineColor;
    [SerializeField] private GameObject piston;
    [SerializeField] private SpriteRenderer[] lines;
    [SerializeField] private GasParticles particles;
    [SerializeField] private ThermoState defaultInitialThermoState;
    [SerializeField] private float animationDuration = 5.0f;

    [SerializeField] private StateUIInput[] inputs;

    [Header("Volume")]
    [Tooltip("ALL UNITS ARE SI")]
    [SerializeField] private MinMax pistonHeightRange;
    [SerializeField] private float volumeUnitsRange;

    [Header("Temperature")]
    [SerializeField] private float temperatureUnitsRange;

    [Header("Pressure")]
    [SerializeField] private Color lowPressureColor;
    [SerializeField] private Color highPressureColor;
    [SerializeField] private float pressureUnitsRange;

    public ThermoState initialState;
    [SerializeField] private ThermoState currentState;
    public ThermoState finalState;

    public static PistonSimulator instance { get; private set; }
    public CurrentGameState currentGameState;
    private bool isSimulating;
    private float simulationT = 0.0f;
    public StateProperty unknownProperty = StateProperty.Volume;

    private void OnValidate()
    {
        foreach (var line in lines)
        {
            var scale = line.transform.localScale;
            scale.y = lineThickness;
            line.transform.localScale = scale;
            line.color = lineColor;
        }
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        SetInitialState(defaultInitialThermoState);
    }

    private void Update()
    {
        if (isSimulating)
        {
            currentState = initialState;
            currentState.volume = Mathf.SmoothStep(initialState.volume, finalState.volume, simulationT);
            currentState.temperature = Mathf.SmoothStep(initialState.temperature, finalState.temperature, simulationT);
            currentState.pressure = Mathf.SmoothStep(initialState.pressure, finalState.pressure, simulationT);
            currentState.moles = Mathf.SmoothStep(initialState.moles, finalState.moles, simulationT);
            inputs[0].SetValue(currentState.volume);
            inputs[1].SetValue(currentState.temperature);
            inputs[2].SetValue(currentState.pressure);
            inputs[3].SetValue(currentState.moles);
            SetThermoState(currentState);
            simulationT += Time.deltaTime / animationDuration;
        }
    }

    public void SetGameState(int state)
    {
        currentGameState = (CurrentGameState)state;
        if (currentGameState == CurrentGameState.Final)
        {
            finalState = initialState;
            SetUnknown(0);
        }
    }

    public void SetInitialState(ThermoState state)
    {
        initialState = state;
        SetThermoState(state);
    }

    private void SetThermoState(ThermoState state)
    {
        SetVolume(state.volume);
        SetTemperature(state.temperature);
        SetPressure(state.pressure);
        SetMoles(state.moles);
    }

    private void SetVolume(float volume)
    {
        var t = GetLerpT(volume, initialState.volume, volumeUnitsRange);
        var height = pistonHeightRange.Lerp(t);
        var pos = piston.transform.localPosition;
        pos.y = height;
        piston.transform.localPosition = pos;
    }

    private void SetTemperature(float temperature)
    {
        var t = GetLerpT(temperature, initialState.temperature, temperatureUnitsRange);
        particles.SetParticleSpeedPercent(t);
    }

    private void SetPressure(float pressure)
    {
        var t = GetLerpT(pressure, initialState.pressure, pressureUnitsRange);
        var color = Color.Lerp(lowPressureColor, highPressureColor, t);
        foreach(var line in lines)
        {
            line.color = color;
        }
    }

    private void SetMoles(float moles)
    {
        particles.SetMoles(moles);
    }

    private float GetLerpT(float currentValue, float initialValue, float unitsRange)
    {
        if (currentValue <= Mathf.Epsilon) { return 0.0f; }
        var delta = ((currentValue - initialValue) / initialValue) / unitsRange;
        return Mathf.Clamp01(0.5f + delta);
    }

    public void StartSimulating()
    {
        isSimulating = true;
        currentGameState = CurrentGameState.Simulating;
        var p = finalState.pressure;
        var t = finalState.temperature;
        var v = finalState.volume;
        var n = finalState.moles;
        var r = (initialState.pressure * initialState.volume) / (initialState.moles * initialState.temperature);
        switch (unknownProperty)
        {
            case StateProperty.Volume:
                finalState.volume = n * r * t / p;
                break;
            case StateProperty.Temperature:
                finalState.temperature = p * v / (n * r);
                break;
            case StateProperty.Pressure:
                finalState.pressure = n * r * t / v;
                break;
            case StateProperty.Moles:
                finalState.moles = p * v / (r * t);
                break;
        }
    }

    public void SetUnknown(int stateProperty)
    {
        unknownProperty = (StateProperty)stateProperty;
        for (int i = 0; i < 4; i++)
        {
            if (i == stateProperty)
                inputs[i].MarkUnkown();
            else inputs[i].SetValue(initialState.GetProperty((StateProperty)i));
        }

    }

    public void SetInstant(bool instant)
    {
        animationDuration = instant ? 1.0f : 10.0f;
    }
}
