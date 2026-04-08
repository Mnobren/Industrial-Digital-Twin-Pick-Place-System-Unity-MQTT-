using UnityEngine;
using System.Threading.Tasks;

public class DataLogger : MonoBehaviour
{
    [SerializeField] private MqttPublisher mqttPublisher;

    // Assíncrono
    public async Task Log(CellEvent cellEvent)
    {
        if (cellEvent == null) return;

        string json = JsonUtility.ToJson(cellEvent);

        // Debug no console
        Debug.Log($"Logging CellEvent: {json}");

        // Publica usando MqttPublisher
        if (mqttPublisher != null)
        {
            await mqttPublisher.PublishAsync(json);
        }
    }

    /// Síncrono
    public void LogSync(CellEvent cellEvent)
    {
        _ = Log(cellEvent); // dispara sem esperar
    }
}