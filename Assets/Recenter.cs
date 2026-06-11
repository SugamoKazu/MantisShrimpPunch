using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Recenter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        OVRManager.display.RecenterPose();
        
    }

    // Update is called once per frame
    void Update()
    {
        // 例：AボタンまたはRキーでリセット
        if (OVRInput.GetDown(OVRInput.Button.One) || Input.GetKeyDown(KeyCode.R))
        {
            RecenterXR();
        }
    }

    void RecenterXR()
    {
        var subsystems = new System.Collections.Generic.List<XRInputSubsystem>();
        SubsystemManager.GetInstances(subsystems);

        foreach (var subsystem in subsystems)
        {
            subsystem.TryRecenter();
        }

        Debug.Log("[QuestRecenter] View recentered");
    }
}
