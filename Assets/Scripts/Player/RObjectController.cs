using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RObjectController : MonoBehaviour
{
    public RSerialHandler RserialHandler;
    public float value = 0f;
    private static float sensor_offset = 4095;
    void Start()
    {
        RserialHandler.OnDataReceived += OnDataReceived;
    }

    /*
    void Update()
    {
        RserialHandler.Write(value.ToString());
        Debug.Log("送信データ: " + value);
    }*/

    // Update is called once per frame
    void OnDataReceived(string message)
    {
        
        RSerialManager RserialManager;
        GameObject obj = GameObject.Find("ActionManager");
        RserialManager = obj.GetComponent<RSerialManager>();

        //Debug.Log(LserialManager.data);

        value = 2f - 2 * RserialManager.data / sensor_offset;

        //Debug.Log("計算後データ: " + value);
        
        
        
    }
}
