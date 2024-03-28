using System;

[Serializable]
public struct ThermoState
{
    public float volume;
    public float temperature;
    public float pressure;
    public float moles;

    public float GetProperty(StateProperty prop)
    {
        return prop switch
        {
            StateProperty.Volume => volume,
            StateProperty.Temperature => temperature,
            StateProperty.Pressure => pressure,
            StateProperty.Moles => moles,
        };
    }
}
