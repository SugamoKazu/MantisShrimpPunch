using UnityEngine;
using UnityEngine.SceneManagement;

public class ModeManager : MonoBehaviour
{
    public static bool isVRDevice = false;

    public static bool isMainMode = true;
    public static bool isConnectionMode = false; // true:マイコン接続モード、false:マイコン非接続モード
    public static bool isTutorialMode = true;

    [SerializeField] private bool defaultConnectionMode = false;   // インスペクターで変更可能

    private GameObject chairObj, mainArm, subArm;


    private void Awake()
    {
        isVRDevice = XRSettings.isDeviceActive;
        isConnectionMode = defaultConnectionMode;
    }

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
        if (isVRDevice)
        {
            if (SceneManager.GetActiveScene().name == "GameTitle")
            {
                if (Input.GetKeyDown(KeyCode.V)) isConnectionMode = !isConnectionMode;
                if (Input.GetKeyDown(KeyCode.T)) isTutorialMode = !isTutorialMode;
            }

            if (OVRInput.GetDown(OVRInput.Button.SecondaryThumbstick) || Input.GetKeyDown(KeyCode.LeftAlt))
            {
                isMainMode = !isMainMode;
            }
        }

        // 一旦保留
        if (SceneManager.GetActiveScene().name == "GameTitle" && Input.GetKeyDown(KeyCode.T))
        {
            isTutorialMode = !isTutorialMode;
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
