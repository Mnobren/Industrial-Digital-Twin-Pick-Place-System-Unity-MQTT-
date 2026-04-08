using UnityEngine;
using Project.Sensors;
using UnityEngine.Rendering;

public class SensorLEDVisual : MonoBehaviour
{
    [SerializeField] private PresenceSensor sensor;
    [SerializeField] private Renderer rend;
    private bool current = false;

    private void OnEnable()
    {
        sensor.OnSensorStateChanged += HandleChange;
        UpdateColor(current);
    }

    private void OnDisable()
    {
        sensor.OnSensorStateChanged -= HandleChange;
    }

    private void HandleChange(bool b)
    {
        current = !current;
        UpdateColor(current);
    }

    private void UpdateColor(bool isActive)
    {
        rend.material.color = isActive ? Color.green : Color.gray;
    }
}

internal class SensorController
{
}