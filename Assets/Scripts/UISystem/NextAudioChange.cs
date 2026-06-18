using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextAudioChange : MonoBehaviour
{
    [SerializeField] private GameObject nextAudio;


    [SerializeField] private bool rightActive, leftActive, powerTrigger,turnRightTrigger,turnLeftTrigger, colorTrigger, objectTrigger;
    [SerializeField] private GameObject obj;
    [SerializeField] private GameObject rightArm, rightCPL, leftArm, leftCPL;
    //[SerializeField] public GameObject rightWall, leftWall;
    [SerializeField] private GameObject arrowMain;

    [SerializeField] private GameObject arrowSub;
    private float armPower;
    private bool cPLChange, audioChange = true;
    private Collider objCol;
    private bool turnRight = false, turnLeft = false;
    [SerializeField] private GameObject activateObject;
    // Start is called before the first frame update
    void Start()
    {
        if (activateObject != null)
        {
            activateObject.SetActive(true);
            //if(activateObject.name != "CPLArrow") activateObject.SetActive(true);
            //else Invoke("SetArrow", 1.3f);
        }


        if (rightActive) DataSendManager.Instance.SendActive("Syakote_Right");
        else DataSendManager.Instance.SendPassive("Syakote_Right");

        if (leftActive) DataSendManager.Instance.SendActive("Syakote_Left");
        else DataSendManager.Instance.SendPassive("Syakote_Left");


        if (ModeManager.isMainMode)
        {
            rightArm = GameObject.Find("PunchArmRightMain");
            rightCPL = rightArm.transform.GetChild(0).gameObject;
            leftArm = GameObject.Find("PunchArmLeftMain");
            leftCPL = leftArm.transform.GetChild(0).gameObject;

            activateObject = arrowMain;


        }
        else
        {
            rightArm = GameObject.Find("PunchArmRightSub");
            rightCPL = rightArm.transform.GetChild(0).gameObject;
            leftArm = GameObject.Find("PunchArmLeftSub");
            leftCPL = leftArm.transform.GetChild(0).gameObject;

            activateObject = arrowSub;
        }
        
        Invoke("SetArrow", 1.3f);

        if (obj != null) objCol = obj.GetComponent<Collider>();

       // rightWall = GameObject.Find("TurnRightWall");
       // leftWall = GameObject.Find("TurnLeftWall");
    }

    // Update is called once per frame
    void Update()
    {
        cPLChange = rightCPL.GetComponent<RayColor>().colorChange;
        turnRight = rightCPL.GetComponent<RayColor>().turnRight;
        turnLeft = leftCPL.GetComponent<RayColor>().turnLeft;

        Debug.Log("right : "+turnRight+" / left : "+turnLeft);

        armPower = rightArm.GetComponent<GetTriggerValue>().punchPower;
        Debug.Log("PunchPower @ AudioChanger : " +armPower);
        Debug.Log("CPL @ AudioCanger : "+cPLChange);
        
        if (OVRInput.GetDown(OVRInput.Button.One,OVRInput.Controller.RTouch) || Input.GetKeyDown("s"))//仮
        {
            ChangeNext();

        }

        if (objectTrigger)
        {
            if(obj != null) objCol.enabled = true;
            else Invoke("ChangeNext", 0.7f);
        }

        if (powerTrigger && armPower > 1.5f && audioChange)
        {
            audioChange = false;
            Debug.Log("PowerEnough");
            Invoke("ChangeNext", 1.0f);
        }
        //if (gameObject.name == "TutorialVoice4_2") rightWall.SetActive(true);
        //if (gameObject.name == "TutorialVoice12") leftWall.SetActive(true);


        if (turnRightTrigger && turnRight)
        {
            //rightWall.SetActive(false);
            //turnRight = false;
            ChangeNext();
        }
        if (turnLeftTrigger && turnLeft) {
            //leftWall.SetActive(false);
            //turnLeft = false;
            ChangeNext();
        }

        if (colorTrigger && cPLChange) Invoke("ChangeNext", 4.0f);


    }

    void ChangeNext()
    {
        nextAudio.SetActive(true);
        gameObject.SetActive(false);
        Debug.Log("SetActiveFalse" + gameObject.name);
    }
    
    void SetArrow()
    {
        activateObject.SetActive(true);
    }
    
}
