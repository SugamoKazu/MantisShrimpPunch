using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using TMPro;

public class WifiClient : MonoBehaviour
{
    [Header("Network Settings")]
    [Tooltip("ESP32のシリアルモニタに表示されたIPアドレスを入力します")]
    public string esp32_ip = "192.168.1.10"; // 例：実行前に手動で設定
    public int esp32_port = 4210;

    [Header("UI Settings")]
    public TMP_Text displayText;
    public TMP_InputField messageInputField;

    private UdpClient client;
    private Thread receiveThread;
    
    // スレッド間のデータ受け渡しにはキューを使用するのが最も安全です
    private readonly Queue<string> messageQueue = new Queue<string>();

    void Start()
    {
        // 指定したポートでデータを受信できるように初期化
        client = new UdpClient(esp32_port);

        // 受信をバックグラウンドスレッドで開始
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();

        Debug.Log("UDP Client started. Listening on port " + esp32_port);
    }

    void Update()
    {
        // メインスレッドでキューをチェックし、メッセージがあればUIを更新
        while (messageQueue.Count > 0)
        {
            string message;
            // 複数のスレッドから同時にアクセスされるのを防ぐ
            lock (messageQueue)
            {
                message = messageQueue.Dequeue();
            }
            displayText.text = message;
            Debug.Log("UI Updated: " + message);
        }
    }

    /// <summary>
    /// InputFieldの内容をESP32に送信する（UIのButtonから呼び出す）
    /// </summary>
    public void SendFromInputField()
    {
        if (messageInputField != null && !string.IsNullOrEmpty(messageInputField.text))
        {
            SendMessageToESP32(messageInputField.text);
        }
    }

    /// <summary>
    /// 指定されたメッセージをESP32に送信する
    /// </summary>
    public void SendMessageToESP32(string message)
    {
        // IPアドレスが有効か簡易チェック
        if (!IPAddress.TryParse(esp32_ip, out _))
        {
            Debug.LogError("無効なIPアドレスです: " + esp32_ip);
            return;
        }

        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            client.Send(data, data.Length, esp32_ip, esp32_port);
            Debug.Log($"Sent message to {esp32_ip}: {message}");
        }
        catch (Exception err)
        {
            Debug.LogError(err.ToString());
        }
    }

    // データ受信を行うバックグラウンドスレッド
    private void ReceiveData()
    {
        while (true)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref anyIP);
                string text = Encoding.UTF8.GetString(data);

                // 受信したメッセージをキューに追加
                lock (messageQueue)
                {
                    messageQueue.Enqueue(text);
                }
            }
            catch (ThreadAbortException)
            {
                // アプリ終了時にスレッドが中断されるのは正常
                break;
            }
            catch (Exception err)
            {
                Debug.LogError(err.ToString());
            }
        }
    }

    // アプリケーション終了時にリソースを解放
    void OnApplicationQuit()
    {
        if (receiveThread != null && receiveThread.IsAlive)
        {
            receiveThread.Abort();
        }
        if (client != null)
        {
            client.Close();
        }
    }
}