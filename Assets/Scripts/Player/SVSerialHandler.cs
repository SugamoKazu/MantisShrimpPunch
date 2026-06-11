/*
using UnityEngine;
using System;
using System.Collections;
using System.IO.Ports;
using System.Threading;

public class SVSerialHandler : MonoBehaviour
{
    public delegate void SerialDataReceivedEventHandler(string message);
    public event SerialDataReceivedEventHandler OnDataReceived;

    public string portName = "COM14";
    public int baudRate    = 115200;

    private SerialPort serialPort_;
    private Thread thread_;
    private bool isRunning_ = false;

    private string message_;
    private bool isNewMessageReceived_ = false;

    void Awake()
    {
        Open();
    }

    // ワンチャンいらない
    void Update()
    {
        
        if (isNewMessageReceived_){
            OnDataReceived(message_);
        }
        isNewMessageReceived_ = false;
    }
    

    void OnDestroy()
    {
        Close();
    }

    private void Open()
    {
        serialPort_ = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One);
        serialPort_.Open();

        isRunning_ = true;

        thread_ = new Thread(Read);
        thread_.Start();
    }

    private void Close()
    {
        isRunning_ = false;

        if (thread_ != null && thread_.IsAlive) {
            thread_.Join();
        }

        if (serialPort_ != null && serialPort_.IsOpen) {
            serialPort_.Close();
            serialPort_.Dispose();
        }
    }

    private void Read()
    {
        while (isRunning_ && serialPort_ != null && serialPort_.IsOpen) {
            try {
                if (serialPort_.BytesToRead > 0) {
                    message_ = serialPort_.ReadLine();
                    isNewMessageReceived_ = true;
                } 
            }catch (System.Exception e) {
                Debug.LogWarning(e.Message);
            }
        }
    }

    public void Write(string message)
    {
        Debug.Log("Serial.Write: " + message);
        try
        {
            serialPort_.Write(message);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e.Message);
        }
    }
}
*/

using UnityEngine;
using System;
using System.Collections;
using System.IO.Ports;
using System.Threading;

public class SVSerialHandler : MonoBehaviour
{
    public delegate void SerialDataReceivedEventHandler(string message);
    public event SerialDataReceivedEventHandler OnDataReceived;

    public string portName = "COM14";
    public int baudRate    = 115200;

    private SerialPort serialPort_;
    private Thread thread_;
    private volatile bool isRunning_ = false;          // ★ volatile

    private string message_;
    private volatile bool isNewMessageReceived_ = false; // ★ volatile

    void Awake()
    {
        Open();
    }

    void Update()
    {
        if (isNewMessageReceived_) {
            OnDataReceived?.Invoke(message_);          // ★ null安全
            isNewMessageReceived_ = false;
        }
    }

    void OnDestroy()
    {
        Close();
    }

    void OnApplicationQuit()                            // ★ 追加
    {
        Close();
    }

    private void Open()
 {
     serialPort_ = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One);
     serialPort_.ReadTimeout = 50;     // 任意（保険）
     serialPort_.NewLine     = "\n";   // 送信側に合わせる
     serialPort_.Open();

     isRunning_ = true;

    thread_ = new Thread(Read);
    thread_ = new Thread(Read) { IsBackground = true }; // ★ バックグラウンド
    thread_.Start();
 }

 private void Close()
 {
     isRunning_ = false;

    if (thread_ != null && thread_.IsAlive) {
        thread_.Join();
    }
    // ★ 待たない。Editor停止時のデッドロック回避

     if (serialPort_ != null && serialPort_.IsOpen) {
         serialPort_.Close();
         serialPort_.Dispose();
     }
 }

    private void Read()
    {
        while (isRunning_ && serialPort_ != null && serialPort_.IsOpen) {
            try {
                // ★ BytesToReadガードは外す（行読みと相性悪い）
                message_ = serialPort_.ReadLine();     // 改行 or タイムアウト or Close() で戻る
                isNewMessageReceived_ = true;
            }
            catch (TimeoutException) {
                // タイムアウト：isRunning_ 再評価のため続行
            }
            catch (Exception e) {
                // Close() での解除時はここに来る（ObjectDisposed等）
                Debug.LogWarning(e.Message);
                Thread.Sleep(10);                      // ノイズ多発時のスロットリング
            }
        }
    }

    public void Write(string message)
    {
        Debug.Log("Serial.Write: " + message);
        try {
            if (serialPort_ != null && serialPort_.IsOpen) {
                serialPort_.Write(message);
            }
        } catch (Exception e) {
            Debug.LogWarning(e.Message);
        }
    }
}
