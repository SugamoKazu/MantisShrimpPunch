using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UITitle : MonoBehaviour
{


    [SerializeField] Image connectionLeft;
    [SerializeField] Image connectionRight;
    [SerializeField] Image connectionCenter;

    [SerializeField] Image chargeCircleLeft;
    [SerializeField] Image chargeCircleRight;
    private Vector4 nowColor = new Vector4(0f, 1f, 0f, 1f);

    [SerializeField] GameObject SettingPanel;

    private int pushcount = 0;

    private bool colorWhite = false;
    private float flashTime = 0f;

    [SerializeField] LSerialHandler LserialHandler;
    [SerializeField] RSerialHandler RserialHandler;

    private string usedArmLeft, usedArmRight;
    //SettingsUI settingsUI;


    // Update is called once per frame
    void Update()
    {
        Mode();

        ChargeGauge(usedArmLeft, chargeCircleLeft);
        ChargeGauge(usedArmRight, chargeCircleRight);


        if (ConnectionChecker("Syakote_Right")) connectionRight.enabled = false;
        if (ConnectionChecker("Syakote_Left")) connectionLeft.enabled = false;
        if (ConnectionChecker("Syakote_Solenoid")) connectionCenter.enabled = false;

        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger,OVRInput.Controller.RTouch) || Input.GetKeyDown(KeyCode.P))
        {
            if (pushcount % 2 == 0) SettingPanel.SetActive(true);
            else SettingPanel.SetActive(false);
            pushcount++;
        }

        if (OVRInput.GetDown(OVRInput.Button.One,OVRInput.Controller.RTouch) || Input.GetKeyDown(KeyCode.Space))
        {
            //DataSendManager.Instance.SendDefault("Syakote_Right");
            //DataSendManager.Instance.SendDefault("Syakote_Left");
            DataSendManager.Instance.SendPassive("Syakote_Right");
            DataSendManager.Instance.SendPassive("Syakote_Left");
            pushcount = 0;
            UnityEngine.SceneManagement.SceneManager.LoadScene("Game");

        }
        
        /*
        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            pushcount = 0;
            DataSendManager.Instance.SendPassive("Syakote_Right");
            DataSendManager.Instance.SendPassive("Syakote_Left");
            UnityEngine.SceneManagement.SceneManager.LoadScene("Initializer");
        }
        */
        

        

        
    }

    void Mode()
    {
        if (ModeManager.isMainMode)
        {
            usedArmLeft = "PunchArmLeftMain";
            usedArmRight = "PunchArmRightMain";
        }
        else
        {
            usedArmLeft = "PunchArmLeftSub";
            usedArmRight = "PunchArmRightSub";
        }
    }

    void FilledCircle(Image circle, float fill, float total)
    {
        circle.fillAmount = fill / total;
    }

    void ChargeGauge(string punchSide, Image chargeCircle)
    {
        flashTime += Time.deltaTime;

        //if (gamePlaying)
        //{
        float punchPower = GameObject.Find(punchSide).GetComponent<GetTriggerValue>().punchPower;
        float flashInterval = 0.8f; // Default flash interval

        chargeCircle.fillAmount = Mathf.Lerp(chargeCircle.fillAmount, 0.3f*punchPower / 2f, 0.2f);

        if (punchPower >= 2f)
        {
            chargeCircle.fillAmount = 0.3f;
            nowColor = new Vector4(1f, 0.2f, 0.2f, 0.8f); // Set color to red
            //chargeCircle.color = nowColor;
            flashInterval = 0.8f; // Reset flash interval for yellow
        }
        else if (punchPower < 1f)
        {
            nowColor = new Vector4(0.2f, 1f, 0.2f, 0.8f); // Set color to green
            chargeCircle.color = nowColor;
            flashInterval = 0.6f; // Reset flash interval for green
        }
        else
        {
            nowColor = new Vector4(1f, 1f, 0.2f, 0.8f);
            chargeCircle.color = nowColor;
            flashInterval = 0.4f; // Reset flash interval for yellow
        }


        if (flashTime > flashInterval)
        {
            flashTime = 0;
            colorWhite = !colorWhite;
        }

        if (colorWhite) chargeCircle.color = Color.Lerp(chargeCircle.color, Color.white, 0.5f);
        else chargeCircle.color = Color.Lerp(chargeCircle.color, nowColor, 0.5f);

        //}
    }

    bool ConnectionChecker(string devicename)
    {
        // IsDeviceConnectedがtrueならtrueを、falseならfalseを返す
        //return BleMultiDeviceManager.Instance.IsDeviceConnected(devicename);
        return true;
    }
    
}
