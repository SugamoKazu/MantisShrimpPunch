using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using System.Runtime.InteropServices;

public enum BleConnectionState
{
    Discovered, Connecting, Connected, Subscribing, Ready, Failed
}

public class BleDevice
{
    public string Name;
    public string Address;
    public BleConnectionState State = BleConnectionState.Discovered;
}

public class BleMultiDeviceManager : MonoBehaviour
{
    public static BleMultiDeviceManager Instance { get; private set; }

    public event Action<BleDevice> OnDeviceDiscovered;
    public event Action<BleDevice> OnDeviceConnected;
    public event Action<string> OnDeviceDisconnected;
    public event Action<string, byte[]> OnDataReceived;

    [Header("BLE Settings")]
    public string ServiceUUID = "4fafc201-1fb5-459e-8fcc-c5c9-c331914b";
    public string DataCharacteristicUUID = "beb5483e-36e1-4688-b7f5-ea07361b26a8";
    public string CommandCharacteristicUUID = "026186d9-4f21-42ef-9b94-2ffb2ead7a58";
    
    [Header("Target Devices")]
    public List<string> TargetDeviceNames = new List<string> { "Syakote_Right", "Syakote_Left", "Syakote_Solenoid" };

    private Dictionary<string, BleDevice> _devices = new Dictionary<string, BleDevice>();

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    void Start()
    {
        BluetoothLEHardwareInterface.Initialize(true, false, () => StartScan(), (error) => Debug.LogError($"BLE Error: {error}"));
    }

    private void StartScan()
    {
        BluetoothLEHardwareInterface.ScanForPeripheralsWithServices(new string[] { ServiceUUID }, (address, name) =>
        {
            if (TargetDeviceNames.Contains(name) && !_devices.ContainsKey(address))
            {
                var newDevice = new BleDevice { Name = name, Address = address };
                _devices.Add(address, newDevice);
                OnDeviceDiscovered?.Invoke(newDevice);

                newDevice.State = BleConnectionState.Connecting;
                BluetoothLEHardwareInterface.ConnectToPeripheral(address, null, null,
                    (addr_char, service, characteristic) =>
                    {
                        var foundDevice = _devices[addr_char];
                        if (foundDevice.State < BleConnectionState.Connected)
                        {
                            foundDevice.State = BleConnectionState.Connected;
                            OnDeviceConnected?.Invoke(foundDevice);
                        }

                        if (IsEqual(characteristic, DataCharacteristicUUID) && name != "Syakote_Solenoid")
                        {
                            StartCoroutine(SubscribeWithRetry(foundDevice, service, characteristic));
                        }
                    },
                    (addr_disconnect) =>
                    {
                        if (_devices.ContainsKey(addr_disconnect))
                        {
                            OnDeviceDisconnected?.Invoke(addr_disconnect);
                            _devices.Remove(addr_disconnect);
                        }
                    });
            }
        });
    }

    // ★★★ エラーコールバックなしのAPIに合わせて修正 ★★★
    private IEnumerator SubscribeWithRetry(BleDevice device, string serviceUUID, string characteristicUUID)
    {
        device.State = BleConnectionState.Subscribing;
        bool subscribed = false;
        int attempts = 0;
        const int maxAttempts = 3;

        while (!subscribed && attempts < maxAttempts)
        {
            attempts++;
            Debug.Log($"[{device.Name}] Attempting to subscribe... (Attempt #{attempts})");
            
            BluetoothLEHardwareInterface.SubscribeCharacteristicWithDeviceAddress(device.Address, serviceUUID, characteristicUUID,
                // OnSubscriptionStart Callback
                (deviceAddress, notificationCharacteristicUUID) => {
                    // このコールバックが呼ばれたら、購読成功とみなす
                    subscribed = true;
                },
                // OnDataReceived Callback
                (addr_notify, char_notify, bytes) => {
                    OnDataReceived?.Invoke(addr_notify, bytes);
                }
            );

            // 1秒待って、その間にsubscribedフラグがtrueになったか確認する
            yield return new WaitForSeconds(3.0f);
        }

        if (subscribed)
        {
            device.State = BleConnectionState.Ready;
            Debug.Log($"<color=green>[{device.Name}] Successfully subscribed!</color>");
        }
        else
        {
            device.State = BleConnectionState.Failed;
            Debug.LogError($"[{device.Name}] Failed to subscribe after {maxAttempts} attempts. The device might be unresponsive.");
        }
    }

    public void SendCommand(string deviceName, char command)
    {
        string targetAddress = GetAddressFromName(deviceName);
        if (targetAddress != null)
        {
            byte[] data = { Convert.ToByte(command) };
            BluetoothLEHardwareInterface.WriteCharacteristic(targetAddress, ServiceUUID, CommandCharacteristicUUID, data, data.Length, true, null);
        }
    }

    public bool IsDeviceConnected(string deviceName)
    {
        BleDevice device = _devices.Values.FirstOrDefault(d => d.Name == deviceName);
        return device != null && device.State == BleConnectionState.Ready;
    }
    
    public BleConnectionState GetDeviceState(string deviceName)
    {
        // 管理下のデバイスリストから、指定された名前のデバイスを探す
        BleDevice device = _devices.Values.FirstOrDefault(d => d.Name == deviceName);

        // デバイスが見つかれば、その現在の状態を返す
        if (device != null)
        {
            return device.State;
        }

        // デバイスが見つからない場合（まだ発見されていない、または切断済み）は、Disconnectedを返す
        return BleConnectionState.Failed;
    }
    
    public BleDevice GetDevice(string address) => _devices.ContainsKey(address) ? _devices[address] : null;
    public string GetAddressFromName(string name) => _devices.Values.FirstOrDefault(d => d.Name == name)?.Address;
    private bool IsEqual(string uuid1, string uuid2) => uuid1.ToUpper().Equals(uuid2.ToUpper());

    void OnDestroy()
    {
        BluetoothLEHardwareInterface.StopScan();
        if (_devices != null)
        {
            foreach (var address in _devices.Keys.ToList()) BluetoothLEHardwareInterface.DisconnectPeripheral(address, null);
        }
        BluetoothLEHardwareInterface.DeInitialize(null);
    }
}