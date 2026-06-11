using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchAction : MonoBehaviour
{
    //public GameObject joint;
    [SerializeField] AudioSource chargeAudio;
    private float punchSpeed;
    private Animator anim;
    private bool jointActive = true;
    public int punchPower = 0;
    public bool TriggerUp = false;

    [SerializeField] public bool cabGenerate = false; //Cabitationの生成フラグ
    public float cabPower = 0f;

    [SerializeField] Collider punchCollider;


    void Start()
    {
        anim = GetComponent<Animator>();

        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update()
    {
        cabGenerate = false;

        GetValue();
        if (punchPower >= 0) chargeAudio.Play();
        else chargeAudio.Stop();

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            //入力制限解除
            jointActive = true;

        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Punch")) punchCollider.enabled = true;
        else punchCollider.enabled = false;

        if (jointActive)
        {

            //入力の終了 
            if (TriggerUp)
            {
                //パンチの開始
                Punch(punchPower);
                cabGenerate = true; //Cabitationの生成フラグを立てる
                cabPower = punchPower;
            }
        }
        //anim.SetTrigger("HitTrigger");
    }

    void Punch(int punchPower)
    {

        //Debug.Log("Punch");

        punchSpeed = 1.5f + 1.1f * punchPower; //パワーに応じてパンチの速度を変化
        anim.SetFloat("Speed", punchSpeed);
        anim.SetTrigger("Punching");

        //連続入力の制限のため、入力を受け付けないようにするフラグ
        jointActive = false;        

    }



    /*
            IEnumerator Effect(float punchPower)
            {
                string num = punchPower.ToString("0");

                if (punchPower == 0) cabTime = 0.3f;
                else if (punchPower == 1) cabTime = 0.07f;
                else cabTime = 0.05f;

                //0.15秒停止
                yield return new WaitForSeconds(cabTime);
            }
        */
    void GetValue()
    {
        GetTriggerValue triggerValue = GetComponent<GetTriggerValue>();
        punchPower = (int)triggerValue.punchPower;
        TriggerUp = triggerValue.TriggerUp;
    }

}
