using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GetConnectionState : MonoBehaviour
{
    public TextMeshProUGUI LeftState;
    public TextMeshProUGUI RightState;
    public TextMeshProUGUI SolenoidState;

    // Update is called once per frame
    void Update()
    {
        BleConnectionState rightState = BleMultiDeviceManager.Instance.GetDeviceState("Syakote_Right");
        BleConnectionState leftState = BleMultiDeviceManager.Instance.GetDeviceState("Syakote_Left");
        BleConnectionState solenoidState = BleMultiDeviceManager.Instance.GetDeviceState("Syakote_Solenoid");

        LeftState.text = $"{leftState}";
        RightState.text = $"{rightState}";
        SolenoidState.text = $"{solenoidState}";
    }
}
