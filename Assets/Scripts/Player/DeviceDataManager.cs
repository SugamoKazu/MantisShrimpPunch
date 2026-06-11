using UnityEngine;
using System;

// このスクリプトで管理したいデータ構造を定義
public struct DeviceData
{
    public int Value;
    // 必要なら、ここに bool isConnected; などを追加しても良い
}

public class DeviceDataManager : MonoBehaviour
{
    // --- シングルトン化 ---
    public static DeviceDataManager Instance { get; private set; }

    // --- publicなプロパティ ---
    // 他のスクリプトは、ここから最新データを読み取る
    public DeviceData RightData { get; private set; }
    public DeviceData LeftData { get; private set; }
    // Solenoidはデータを受信しないので、値は常に0

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    // BleManagerのイベントを購読
    void OnEnable()
    {
        // BleManagerが準備できるのを待ってから購読する
        if (BleMultiDeviceManager.Instance != null)
        {
            BleMultiDeviceManager.Instance.OnDataReceived += HandleDataReceived;
            //DataSendManager.Instance.SendStop("Syakote_Right");
            //DataSendManager.Instance.SendStop("Syakote_Left");
        }
            
    }

    // 購読を解除
    void OnDisable()
    {
        if (BleMultiDeviceManager.Instance != null)
            BleMultiDeviceManager.Instance.OnDataReceived -= HandleDataReceived;
    }

    // BleManagerからデータが届いたら、このメソッドが呼ばれる
    private void HandleDataReceived(string address, byte[] data)
    {
        Debug.Log($"DeviceDataManager: Event received from {address}.");
        BleDevice device = BleMultiDeviceManager.Instance.GetDevice(address);
        if (device == null) return;
        
        // バイトデータを整数に変換
        int receivedValue = (data.Length >= 4) ? BitConverter.ToInt32(data, 0) : 0;

        // デバイス名に応じて、適切なプロパティに値を格納する
        switch (device.Name)
        {
            case "Syakote_Right":
                RightData = new DeviceData { Value = receivedValue };
                Debug.Log($"DeviceDataManager: Updated RightData with value {receivedValue}.");
                break;
            case "Syakote_Left":
                LeftData = new DeviceData { Value = receivedValue };
                Debug.Log($"DeviceDataManager: Updated LeftData with value {receivedValue}.");
                break;
        }
    }
}