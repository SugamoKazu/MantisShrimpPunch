
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class EffectManager : MonoBehaviour
{
    [SerializeField] private GameObject cabEffect;
    [SerializeField] private GameObject CPL;
    [SerializeField] private Transform cabPos;
    [SerializeField] private GameObject sandSmoke;
    [SerializeField] private Transform sandSmokePos;
    [SerializeField] private GameObject shockWave;
    [SerializeField] private Transform shockWavePos;
    private Animator anim;
    private bool preState0 = false;
    private bool preState1 = false;
    private bool isHit = false;
    private GameObject obj;

    [SerializeField] Transform start;
    [SerializeField] Transform end;
    private Vector3 startPos, endPos;
    private bool waveStart = false;
    private float count = 0f;

    public AudioSource waveAudio;
    public AudioMixer mixer;
    private float pitch;

    private PunchAction script;
    private int power = 0;
    private bool triggerUp = false;
    [SerializeField] private float power0Time_ms = 500f;
    [SerializeField] private float power1Time_ms = 250f;
    [SerializeField] private float power2Time_ms = 100f;
    public float transmitTime_ms = 0f; // Transmission time in milliseconds

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();

        script = GetComponent<PunchAction>();
        power = script.punchPower;
        //triggerUp = script.TriggerUp;

        shockWave.SetActive(false);

        //GameObject obj = GameObject.Find("CPL");
        //isHit = obj.GetComponent<RayManager>().colorChange;
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update()
    {
        shockWave.SetActive(false);
        
        power = script.punchPower;
        triggerUp = script.TriggerUp;

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            switch (power)
            {
                case 0:
                    transmitTime_ms = power0Time_ms;
                    break;
                case 1:
                    transmitTime_ms = power1Time_ms;
                    break;
                case 2:
                    transmitTime_ms = power2Time_ms;
                    break;
            }
        }

        //transmitTimeMs = 500f;
        //transmitTimeMs = 250f;

        
        

        
        if (triggerUp)
        {
            RayColor rayColor; //呼ぶスクリプトにあだなつける
            rayColor = CPL.GetComponent<RayColor>(); //付いているスクリプトを取得
            isHit = rayColor.colorChange;
        }

        //Debug.Log(isHit);
        //if(isHit && triggerUp)
        EffectGenerate(transmitTime_ms);

        //ShockWave(transmitTimeMs);

        //Debug.Log("ShockWave Count: " + count);
    }

    void EffectGenerate(float Time_ms)
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Punch") && preState0)
        {
            sandSmoke = (GameObject)Resources.Load("SandSmoke" + power.ToString("0"));
            Instantiate(sandSmoke, sandSmokePos.position, this.transform.rotation);
        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("PunchHit"))
        {       
            if (preState1)
            {
                cabEffect = (GameObject)Resources.Load("cabEffect");
                //Instantiate(cabEffect, cabPos.position, this.transform.rotation, this.transform);//キャビテーション発生
                Sound();
                count = 0f;
                waveStart = true;
            }
            //if (waveStart)
            StartCoroutine(ShockWave(Time_ms));
        }

        preState0 = anim.GetCurrentAnimatorStateInfo(0).IsName("Idle");
        preState1 = anim.GetCurrentAnimatorStateInfo(0).IsName("Punch");
    }

    IEnumerator ShockWave(float Time_ms)
    {
        var speed = Time_ms / 500f;
        //衝撃波発生までのタイムラグ
        yield return new WaitForSeconds(0.5f * speed);
        shockWave.SetActive(true);

        startPos = start.position;
        endPos = end.position;
        Vector3 trail = endPos - startPos;

        if (count <= Time_ms / 1000f) count += Time.deltaTime;
        else
        {
            shockWave.SetActive(false);
            //countStart = false;

            waveStart = false;
            shockWave.transform.position = start.position;
            //return;
        }
        if (waveStart) shockWave.transform.position = start.position + trail * (count / (Time_ms / 1000f));
        shockWave.transform.forward = (end.position - start.position).normalized;
        shockWave.transform.LookAt(cabPos.position);


    }

    void Sound()
    {
        waveAudio.pitch = 1f + (float)(100f / transmitTime_ms);
        pitch = 1f / waveAudio.pitch;
        mixer.SetFloat("Shifter", pitch);
        waveAudio.Play();
    }
}
