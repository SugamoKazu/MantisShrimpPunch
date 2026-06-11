using UnityEngine;

public class AudioPunch : MonoBehaviour
{
    private Animator anim;
    private AudioSource[] sources;
    private bool isPunch = true, isHit = true;
    private float punchPower = 0;

    private GameObject joint;
    //private AudioSource chargeAudio;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        sources = gameObject.GetComponents<AudioSource>();
        joint = transform.Find("Joint").gameObject;
        //chargeAudio = joint.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        GetValue();
        if (punchPower > 0f)
        {
            sources[1].Play();
            //Debug.Log("ChargeSound");
        }
        else sources[1].Stop();

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Punch") && isPunch)
        {
            //Debug.Log("PunchSound");

            sources[0].Play();
            //sources[1].PlayDelayed(0.1f);
            isPunch = false;
        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("PunchIdle") && isHit)//仮の状態検知
        {
           // Debug.Log("PunchHitSound");
            isHit = false;
        }






        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            isPunch = true;
            isHit = true;
        }

        punchPower = 0f;
    }
    
    void GetValue()
    {
        GetTriggerValue triggerValue = GetComponent<GetTriggerValue>();
        punchPower = triggerValue.punchPower;
        //TriggerUp = triggerValue.TriggerUp;
    }


}