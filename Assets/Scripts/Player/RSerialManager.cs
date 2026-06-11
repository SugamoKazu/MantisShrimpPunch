using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RSerialManager : MonoBehaviour
{
    public RSerialHandler RserialHandler;
    public BreakableObject breakableObject;


    //受信用変数
    public float data;              //受信データのfloat型版変数
    string receive_data;            //受信した生データを入れる変数

    //送信用変数
    //bool onoff = true;              //オンオフどちらにするかを決定する変数（今回はオンで固定）
    //bool cansend = true;            //送信するかどうかを判断する変数


    void Start()
    {
        RserialHandler.OnDataReceived += OnDataReceived;
    }

    //データを受信したら


    void OnDataReceived(string message)
    {
        receive_data = (message);           //受信データをreceive_dataに入れる
        data = float.Parse(receive_data);   //float型に変換してdataに入れる
        //Debug.Log("受信データ: " + data);
    }

    private void Update()
    {
        //if (breakableObject.isHit && breakableObject.isRight)
        //{
          //  RserialHandler.Write("H");
            //Debug.Log("Hを送信しました");
        //}
    }
    
    
}
