using UnityEngine;
using System;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

[System.Serializable]
public class DeviceStatus { public string name; public string ip; }

public class MQTTController : MonoBehaviour
{
    [Header("MQTT Broker Settings")]
    public string brokerAddress = "192.168.137.1";
    public int brokerPort = 1883;

    [Header("MQTT Topics")]
    public string commandTopic = "devices/command";

    [Header("UDP Settings")]
    public int udpListenPort = 4210;

    private MqttClient client;
    private Dictionary<string, string> discoveredEsp32s = new Dictionary<string, string>();
    
    private UdpClient udpClient;
    private Thread receiveThread;
    public event Action<int> OnUdpDataReceived;

    void Start()
    {
        client = new MqttClient(brokerAddress, brokerPort, false, null, null, MqttSslProtocols.None);
        client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
        string clientId = Guid.NewGuid().ToString();
        client.Connect(clientId);

        string subscribeTopic = "devices/+/status";
        client.Subscribe(new string[] { subscribeTopic }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });

        Debug.Log($"<color=green>Connected to MQTT broker at {brokerAddress} and subscribed to '{subscribeTopic}'</color>");
        
        udpClient = new UdpClient(udpListenPort);
        receiveThread = new Thread(new ThreadStart(ReceiveUdpData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
        Debug.Log($"<color=orange>UDP listener started on port {udpListenPort}.</color>");
    }

    void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    {
        string message = Encoding.UTF8.GetString(e.Message);
        string topic = e.Topic;
        
        // ★★★ MqttClient.TopicMatches の行を削除 ★★★
        // このイベントで受け取るメッセージは購読したトピックのものだけなので、
        // 現状ではチェックは不要です。
        UnityMainThreadDispatcher.Instance().Enqueue(() => {
            DeviceStatus status = JsonUtility.FromJson<DeviceStatus>(message);
            if (status != null && !string.IsNullOrEmpty(status.name)) {
                Debug.Log($"<color=cyan>Discovered or updated device: {status.name} at IP {status.ip}</color>");
                discoveredEsp32s[status.name] = status.ip;
            }
        });
    }

    private void ReceiveUdpData()
    {
        while (true)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = udpClient.Receive(ref anyIP);
                if (data.Length >= 4)
                {
                    // バイト配列の先頭から4バイトを整数(int)に変換
                    int receivedValue = BitConverter.ToInt32(data, 0);

                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        OnUdpDataReceived?.Invoke(receivedValue);
                        // 変換した整数値をコンソールに表示
                        Debug.Log($"<color=yellow>UDP Received from {anyIP.Address}: Counter = {receivedValue}</color>");
                    });
                }
            }
            catch (Exception e) {
                 Debug.LogWarning(e.ToString());
            }
        }
    }

    /*
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S)) { SendCommand("START_UDP"); }
        if (Input.GetKeyDown(KeyCode.X)) { SendCommand("STOP_UDP"); }
    }
    */


// 自身のローカルIPアドレスを取得するヘルパー関数
public string GetLocalIPAddress()
{
    var host = Dns.GetHostEntry(Dns.GetHostName());
    foreach (var ip in host.AddressList)
    {
        if (ip.AddressFamily == AddressFamily.InterNetwork)
        {
            return ip.ToString();
        }
    }
    throw new Exception("No network adapters with an IPv4 address in the system!");
}

// JSON形式でコマンドを送信するメソッド
public void SendCommandWithIP(string command)
{
    if (client != null && client.IsConnected)
    {
        string localIP = GetLocalIPAddress();
        // ★ {"command":"START_UDP", "ip":"192.168.137.xx"} のようなJSONを作成
        string payload = $"{{\"command\":\"{command}\", \"ip\":\"{localIP}\"}}";

        client.Publish(commandTopic, Encoding.UTF8.GetBytes(payload), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
        Debug.Log($"Published to '{commandTopic}': {payload}");
    }
}

// Input Systemから呼び出すメソッドを修正
public void StartUdpStream()
{
    SendCommandWithIP("START_UDP");
}

public void StopUdpStream()
{
    // 停止コマンドはIP不要なので元のままでも良いが、形式を統一
    SendCommandWithIP("STOP_UDP");
}

    public void SendCommand(string command)
    {
        if (client != null && client.IsConnected) {
            client.Publish(commandTopic, Encoding.UTF8.GetBytes(command), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
            Debug.Log($"Published to '{commandTopic}': {command}");
        }
    }

    void OnApplicationQuit()
    {
        if (receiveThread != null && receiveThread.IsAlive) receiveThread.Abort();
        if (udpClient != null) udpClient.Close();
        if (client != null && client.IsConnected) client.Disconnect();
    }
}