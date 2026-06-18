using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnPlayerMove : MonoBehaviour
{
    [SerializeField] private PlayerMove playerMove;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (playerMove != null && !ModeManager.isVRDevice)
        {
            playerMove.enabled = true;
        }
    }
}
