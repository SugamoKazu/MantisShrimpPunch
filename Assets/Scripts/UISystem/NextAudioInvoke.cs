using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextAudioInvoke : MonoBehaviour
{
    [SerializeField] private GameObject nextAudio;

    [SerializeField] private GameObject activateObject;
    [SerializeField] private float interval = 0;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("CallObject", interval);

        if (activateObject != null) activateObject.SetActive(true);
    }
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One,OVRInput.Controller.RTouch) || Input.GetKeyDown("s"))//仮
        {
            CallObject();

        }
    }
    
    void CallObject()
    {
        nextAudio.SetActive(true);
        gameObject.SetActive(false);
    }

}
