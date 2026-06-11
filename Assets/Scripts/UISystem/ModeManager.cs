using UnityEngine;

public class ModeManager : MonoBehaviour
{
    public static bool isMainMode = true;
    private GameObject chairObj, mainArm, subArm;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        chairObj = GameObject.Find("Chair");
        mainArm = GameObject.Find("ArmsMain");
        subArm = GameObject.Find("ArmsSub");
    }

    // Update is called once per frame
    void Update()
    {
        
        //chairObj = GameObject.Find("Chair");

        if (OVRInput.GetDown(OVRInput.Button.SecondaryThumbstick) || Input.GetKeyDown(KeyCode.LeftAlt)) //クエスト左コンのスティック押し込み
        {
            //Debug.Log("PrimaryThumbstick");
            isMainMode = !isMainMode;
        }

        if (isMainMode)
        {
            chairObj.SetActive(false);
            mainArm.SetActive(true);
            subArm.SetActive(false);

        }
        else
        {
            chairObj.SetActive(true);
            mainArm.SetActive(false);
            subArm.SetActive(true);
        }
    }
}
