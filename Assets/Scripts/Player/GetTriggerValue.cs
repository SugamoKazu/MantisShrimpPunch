using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetTriggerValue : MonoBehaviour
{
    [SerializeField] LSerialHandler LserialHandler;
    [SerializeField] RSerialHandler RserialHandler;
    public LRSide LR;
    public enum LRSide { Left, Right }
    public float punchPower = 0f;
    public bool TriggerUp = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public LObjectController LobjectController;
    public RObjectController RobjectController;
    public float Lvalue;
    public float Rvalue;
    private static float sensor_offset = 4095;
    private bool LChargeFlag = false;
    private bool RChargeFlag = false;

    public Animator animL;
    public Animator animR;
    private bool isSent = false;

    [SerializeField] private AudioSource chargeAudio;

    //[SerializeField] private AudioSource chargeAudio;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (TriggerUp) punchPower = 0f;
        TriggerUp = false; // Reset TriggerUp each frame
        

        /*
        if (ModeManager.isMainMode)
        {
            Lvalue = LobjectController.value;
            Rvalue = RobjectController.value;
        }
        else
        {
            Lvalue = RobjectController.value;
            Rvalue = LobjectController.value;
        } */

        if (LR == LRSide.Left)
        {
            if (ModeManager.isConnectionMode)
            {
                Lvalue = 2f - 2 * DeviceDataManager.Instance.LeftData.Value / sensor_offset;
            }
            else // PCモードはマウス操作
            {
                if (OVRInput.Get(OVRInput.RawButton.LIndexTrigger) || Input.GetMouseButton(0))
                {
                    Lvalue = Mathf.Min(Lvalue + Time.deltaTime * 2f, 2f);
                }
                else
                {
                    Lvalue = 0f;
                }
            }

            //Lvalue = LobjectController.value;
            GetLeftTriggerValue();

            //punchPower = Lvalue;

            if (animL.GetCurrentAnimatorStateInfo(0).IsName("PunchBack") && !isSent)
            {
                DataSendManager.Instance.SendMiss("Syakote_Left");
                //LserialHandler.Write("M"); //PunchMiss
                //Debug.Log("PunchMiss / 'M' sent");
                isSent = true;
            }
            if (animL.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                isSent = false;
            }

        }
        else if (LR == LRSide.Right)
        {
            if (ModeManager.isConnectionMode)
            {
                Rvalue = 2f - 2 * DeviceDataManager.Instance.RightData.Value / sensor_offset;
            }
            else // PCモードはマウス操作
            {
                if (OVRInput.Get(OVRInput.RawButton.RIndexTrigger) || Input.GetMouseButton(1))
                {
                    Rvalue = Mathf.Min(Rvalue + Time.deltaTime * 2f, 2f);
                }
                else
                {
                    Rvalue = 0f;
                }
            }

            //Rvalue = RobjectController.value;
            GetRightTriggerValue();
            //punchPower = Rvalue;

            if (animR.GetCurrentAnimatorStateInfo(0).IsName("PunchBack") && !isSent)
            {
                DataSendManager.Instance.SendMiss("Syakote_Right");
                //RserialHandler.Write("M"); //PunchMiss
                //Debug.Log("PunchMiss / 'M' sent");
                isSent = true;
            }
            if (animR.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                isSent = false;
            }
        }
    }

    void GetLeftTriggerValue()
    {
        if (LChargeFlag)
        {
            if (punchPower < 2f)
            {
                if(punchPower < Lvalue) punchPower = Lvalue;
                chargeAudio.Play();
                chargeAudio.pitch = 0.5f + punchPower / 4f;
                //chargeAudio.enabled = true;
                //Debug.Log("ChargeSound");
            }

            //Debug.Log(punchPower);
            if (Lvalue == 0)
            {
                TriggerUp = true;
                LChargeFlag = false;    
                chargeAudio.Stop();
                //chargeAudio.enabled = false;
            }
        }
        else
        {
            if (Lvalue != 0)
            {
                LChargeFlag = true;
            }
        }

        bool isTriggerPressed = false;
        bool isTriggerReleased = false;

        isTriggerPressed = OVRInput.Get(OVRInput.RawButton.LIndexTrigger) || Input.GetMouseButton(0);
        isTriggerReleased = OVRInput.GetUp(OVRInput.RawButton.LIndexTrigger) || Input.GetMouseButtonUp(0);

        if (isTriggerPressed)
        {
            if (punchPower < 2f) punchPower = Lvalue;
        }
        if (isTriggerReleased)
        {
            TriggerUp = true;
        }
    }

    void GetRightTriggerValue()
    {
        if (RChargeFlag)
        {
            if (punchPower < 2f)
            {
                if(punchPower < Rvalue) punchPower = Rvalue;
                chargeAudio.Play();
                chargeAudio.pitch = 0.5f + punchPower / 4f;
                //chargeAudio.enabled = true;
                //Debug.Log("ChargeSound");
            }
            //punchPower = Rvalue;
            //Debug.Log(punchPower);
            if (Rvalue == 0)
            {
                TriggerUp = true;
                RChargeFlag = false;
                chargeAudio.Stop();
            }
        }
        else
        {
            if (Rvalue != 0)
            {
                RChargeFlag = true;
            }
        }

        bool isTriggerPressed = false;
        bool isTriggerReleased = false;

        isTriggerPressed = OVRInput.Get(OVRInput.RawButton.RIndexTrigger) || Input.GetMouseButton(1);
        isTriggerReleased = OVRInput.GetUp(OVRInput.RawButton.RIndexTrigger) || Input.GetMouseButtonUp(1);

        if (isTriggerPressed)
        {
            if (punchPower < 2f) punchPower = Rvalue;
        }
        if (isTriggerReleased)
        {
            TriggerUp = true;
        }
    }
}
