using UnityEngine;

public class DataSendManager : MonoBehaviour
{
    // --- シングルトン化 ---
    public static DataSendManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    public void SendHit(string deviceName)
    {
        SendCommandToDevice(deviceName, 'H');
    }

    public void SendMiss(string deviceName)
    {
        SendCommandToDevice(deviceName, 'M');
    }
    public void SendDefault(string deviceName)
    {
        SendCommandToDevice(deviceName, 'D');
    }
    public void SendStop(string deviceName)
    {
        SendCommandToDevice(deviceName, 'S');
    }
    public void SendActive(string deviceName)
    {
        SendCommandToDevice(deviceName, 'A');
    }
    public void SendPassive(string deviceName)
    {
        SendCommandToDevice(deviceName, 'P');
    }
    public void SendRight(string deviceName)
    {
        SendCommandToDevice(deviceName, 'R');
    }
    public void SendLeft(string deviceName)
    {
        SendCommandToDevice(deviceName, 'L');
    }


    

    // --- 内部的な送信処理 ---
    private void SendCommandToDevice(string deviceName, char command)
    {
        if (BleMultiDeviceManager.Instance != null)
        {
            BleMultiDeviceManager.Instance.SendCommand(deviceName, command);
        }
        else
        {
            if(ModeManager.isConnectionMode) Debug.Log("BleMultiDeviceManager is not available.");
        }
    }
}