using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWorldSpace : MonoBehaviour
{
    public float amplitude = 0.01f; // 振幅

    public GameObject player; // プレイヤーのGameObject

    void Start()
    {
        Forward();
        if(player == null) player = GameObject.FindWithTag("Player");

    }

    // Update is called once per frame
    void Update()
    {
        Forward();
        UpDown();
    }

    void UpDown()
    {

        float T = 3.0f;
        float F = 1.0f / T;

        // 上下に振動させる
        float posYSin = Mathf.Sin(2.0f * Mathf.PI * F * Time.time);
        this.transform.position += new Vector3(0f, posYSin * amplitude, 0f);
    }

    void Forward()
    {
        Vector3 dir = player.transform.position - this.transform.position;
        this.transform.forward = Vector3.Lerp(this.transform.forward, -dir, 0.01f);
    }
}
