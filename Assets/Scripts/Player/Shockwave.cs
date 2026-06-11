using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shockwave : MonoBehaviour
{
    private Material mat;
    private float phase = 1f; // Phase of the shockwave effect
    public float period = 1f; // Period of the shockwave effect
    private bool waveStop = true, cabGenerate = false; // Flag to control the wave generation
    public float punchPower = 0f; // Power of the punch, can
    private float delay = 0.3f; // Delay before starting the wave
    // Start is called before the first frame update
    void Start()
    {
        //Id = Shader.PropertyToID("_MainTex");
        mat = GetComponent<Renderer>().material;
        mat.SetFloat("_Phase", phase);
    }
    // Update is called once per frame
    void Update()
    {
        PunchAction punchAction; //呼ぶスクリプトにあだなつける
        GameObject obj = GameObject.FindWithTag("Player"); //Playerっていうオブジェクトを探す
        punchAction = obj.GetComponent<PunchAction>(); //付いているスクリプトを取得
        cabGenerate = punchAction.cabGenerate; //Cabitationの生成フラグを取得
        punchPower = punchAction.cabPower; //パンチのパワーを取得

        if (!waveStop)
        {
            mat.SetFloat("_Phase", phase);
            if (phase < 0.05f)
            {
                //phase = 1f;
                waveStop = true;
                //Destroy(gameObject);
            }
            phase -= Time.deltaTime / period; // Decrease phase over time
        }

        if (cabGenerate)
        {
            Debug.Log("Shockwave Update: " + cabGenerate + ", Power: " + punchPower);
            cabGenerate = false; // Reset the flag
            if(punchPower == 0f) delay = 0.3f;
            else if(punchPower == 1f) delay = 0.07f;
            else delay = 0.04f;

            StartCoroutine(StartWave(delay, 1f + punchPower)); //punchPower)); // Start a new wave with a period of 1 second

            //waveStop = false;
            //period = 1f; // Reset period if needed
        }
        
    }

    IEnumerator StartWave(float delay,float wait)
    {
        yield return new WaitForSeconds(delay + 1.5f -0.25f*wait); // Wait for a short duration before starting the wave
        period = 1.25f - 0.25f * wait; // Set the period based on the punch power
        waveStop = false;
        phase = 1f; // Reset phase to start the shockwave effect
    }
}
