using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSpeedControl : MonoBehaviour
{
    public ParticleSystem particleSystem; // Inspectorで指定
    public float newSpeed = 1.0f; // 再生速度（デフォルト: 1）

    void Start()
    {
        if (particleSystem == null) return;

        GameObject obj = transform.parent.gameObject;  //タグによる左右の判別とが必要かも
        EffectManager script;
        script = obj.GetComponent<EffectManager>();
        //newSpeed = script.transmitTime_ms;



        // mainモジュールの参照を取得してから simulationSpeed を変更
        var main = particleSystem.main;
        main.simulationSpeed = 500f / newSpeed;
    }
}