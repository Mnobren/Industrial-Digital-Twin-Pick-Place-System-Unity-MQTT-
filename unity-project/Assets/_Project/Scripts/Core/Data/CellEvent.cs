using System;

[Serializable]
public class CellEvent
{
    public string cellId = "cell_1";

    // Estados
    public string cellState;
    public string robotState;
    public string conveyorState;

    // KPIs
    public int pieceCount;
    public float cycleTime;

    // Sensores
    public bool pickSensorActive;

    // Status
    public bool isError;

    // Metadata
    public string eventType;
    public string source;
    public string timestamp;
}