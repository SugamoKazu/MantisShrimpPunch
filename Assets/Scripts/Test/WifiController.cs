using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using TMPro;
using UnityEngine.XR;

public class WifiController : MonoBehaviour
{
    [Header("Network Settings")]
    [Tooltip("圧力センサーESP1のIPアドレス")]
    public string pressureSensor1_ip = "192.168.1.101";
    [Tooltip("圧力センサーESP2のIPアドレス")]
    public string pressureSensor2_ip = "192.168.1.102";
    [Tooltip("受信専用ESP3のIPアドレス")]
    public string receiver3_ip = "192.168.1.103";
    [Tooltip("ESPで設定したポート番号")]
    public int esp32_port = 4210;

    [Header("UI Settings")]
    public TMP_Text displayText1; // ESP1からのデータを表示
    public TMP_Text displayText2; // ESP2からのデータを表示
    public TMP_Text displayText3; // ESP3への送信ログを表示

    private UdpClient client;
    private Thread receiveThread;
    private readonly Queue<(string ip, string message)> messageQueue = new Queue<(string, string)>();

    void Start()
    {
        client = new UdpClient(esp32_port);
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    void Update()
    {
        // キューにメッセージがあれば、全て処理する
        while (messageQueue.Count > 0)
        {
            (string ip, string message) receivedData;
            lock (messageQueue)
            {
                receivedData = messageQueue.Dequeue();
            }
            // 受信したデータをIPアドレスに基づいて処理する
            ProcessReceivedData(receivedData.ip, receivedData.message);
        }
    }

    /// <summary>
    /// 受信したデータをIPアドレスによって振り分ける司令塔
    /// </summary>
    private void ProcessReceivedData(string senderIp, string message)
    {
        if (senderIp == pressureSensor1_ip)
        {
            // --- 圧力センサー1からの処理 ---
            displayText1.text = $"[ESP1]: {message}";
            
            // 例：圧力値が500を超えたら特定のデータを送り返す
            if (float.TryParse(message, out float pressureValue) && pressureValue > 500)
            {
                SendMessageToESP32(pressureSensor1_ip, "HIGH_PRESSURE_DETECTED");
            }
        }
        else if (senderIp == pressureSensor2_ip)
        {
            // --- 圧力センサー2からの処理 ---
            displayText2.text = $"[ESP2]: {message}";
            // こちらも同様に、値に応じた処理を記述
        }
        // 受信専用ESP3からのデータは想定しないので、ここでは何もしない
    }
    
    /// <summary>
    /// 特定のタイミングで受信専用ESP3にデータを送信する（コントローラー等から呼び出す）
    /// </summary>
    public void SendCommandToReceiver3(string command)
    {
        SendMessageToESP32(receiver3_ip, command);
        displayText3.text = $"Sent to [ESP3]: {command}";
    }

    public void SendMessageToESP32(string ipAddress, string message)
    {
        try {
            byte[] data = Encoding.UTF8.GetBytes(message);
            client.Send(data, data.Length, ipAddress, esp32_port);
            Debug.Log($"Sent message to {ipAddress}: {message}");
        } catch (Exception err) {
            Debug.LogError(err.ToString());
        }
    }

    private void ReceiveData()
    {
        while (true) {
            try {
                IPEndPoint senderEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref senderEndPoint);
                string senderIp = senderEndPoint.Address.ToString();
                string text = Encoding.UTF8.GetString(data);

                // IPアドレスとメッセージをペアでキューに追加
                lock (messageQueue)
                {
                    messageQueue.Enqueue((senderIp, text));
                }
            } catch { break; }
        }
    }

    void OnApplicationQuit()
    {
        if (receiveThread != null) receiveThread.Abort();
        if (client != null) client.Close();
    }
}