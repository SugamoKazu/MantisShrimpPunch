using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LObjectController : MonoBehaviour
{
    public LSerialHandler LserialHandler;
    public float value;
    private static float sensor_offset = 4095;
    void Start()
    {
        LserialHandler.OnDataReceived += OnDataReceived;
    }

    // Update is called once per frame
    void OnDataReceived(string message)
    {
        
        LSerialManager LserialManager;
        GameObject obj = GameObject.Find("ActionManager");
        LserialManager = obj.GetComponent<LSerialManager>();

        //Debug.Log(LserialManager.data);

        value = 2f - 2 * LserialManager.data / sensor_offset;

        //Debug.Log("計算後データ: " + value);
        
        
    }
}
