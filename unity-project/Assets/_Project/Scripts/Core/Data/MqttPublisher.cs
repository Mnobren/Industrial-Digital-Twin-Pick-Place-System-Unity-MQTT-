using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;

public class MqttPublisher : MonoBehaviour
{
    private IMqttClient client;
    private IMqttClientOptions options;

    [Header("MQTT Config")]
    public string brokerAddress = "localhost";
    public int brokerPort = 1883;
    public string topic = "robot/data";

    [Header("Settings")]
    public bool enableEventFiltering = true;
    public float reconnectDelay = 5f;

    public bool IsConnected { get; private set; } = false;

    private string lastJson = null;
    private CancellationTokenSource reconnectCts;

    async void Start()
    {
        await SetupMqttClientAsync();
    }

    private void OnEnable()
    {
        reconnectCts = new CancellationTokenSource();
    }

    private void OnDisable()
    {
        reconnectCts?.Cancel();
        reconnectCts = null;
    }

    private async Task SetupMqttClientAsync()
    {
        var factory = new MqttFactory();
        client = factory.CreateMqttClient();

        client.UseConnectedHandler(OnConnected);
        client.UseDisconnectedHandler(OnDisconnected);

        options = new MqttClientOptionsBuilder()
            .WithTcpServer(brokerAddress, brokerPort)
            .Build();

        await ConnectAsync();
    }

    private async Task ConnectAsync()
    {
        try
        {
            await client.ConnectAsync(options);
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"MQTT connection failed: {ex.Message} (fallback enabled)");
            IsConnected = false;

            // só reconectar se estiver em Play Mode
            if (Application.isPlaying)
                StartReconnectLoop();
        }
    }

    private Task OnConnected(MqttClientConnectedEventArgs e)
    {
        Debug.Log("MQTT Connected");
        IsConnected = true;

        // cancela reconexão se estava rodando
        reconnectCts?.Cancel();
        reconnectCts = null;

        return Task.CompletedTask;
    }

    private Task OnDisconnected(MqttClientDisconnectedEventArgs e)
    {
        Debug.LogWarning("MQTT Disconnected");
        IsConnected = false;

        if (Application.isPlaying)
            StartReconnectLoop();

        return Task.CompletedTask;
    }

    private void StartReconnectLoop()
    {
        if (reconnectCts != null) return; // já está rodando

        reconnectCts = new CancellationTokenSource();
        _ = ReconnectLoopAsync(reconnectCts.Token);
    }

    private async Task ReconnectLoopAsync(CancellationToken token)
    {
        while (!IsConnected && !token.IsCancellationRequested)
        {
            Debug.Log($"Attempting MQTT reconnect in {reconnectDelay} seconds...");
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(reconnectDelay), token);
                if (!token.IsCancellationRequested)
                    await ConnectAsync();
            }
            catch (TaskCanceledException) { /* cancelado */ }
            catch (Exception ex)
            {
                Debug.LogWarning($"Reconnect loop error: {ex.Message}");
            }
        }
    }

    public async Task PublishAsync(string json)
    {
        if (enableEventFiltering && json == lastJson) return;
        lastJson = json;

        if (!IsConnected || client == null)
        {
            Debug.Log("MQTT fallback → " + json);
            return;
        }

        var message = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(Encoding.UTF8.GetBytes(json))
            .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
            .Build();

        try
        {
            await client.PublishAsync(message);
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"MQTT publish failed: {ex.Message}");
        }
    }
}