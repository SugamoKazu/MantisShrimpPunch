using UnityEngine;

public class BossHit : MonoBehaviour
{
    //private Script boss;
    private Animator anim;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject bossObject = GameObject.Find("Kraken");
        anim = bossObject.GetComponent<Animator>();

        Boss.currentHP--;
        anim.SetTrigger("isDamage");
        //boss = bossObject.GetComponent<Boss>();
    }

    // Update is called once per frame
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LeftArm") || other.CompareTag("RightArm"))
        {
            //Boss.damaged = true;
            Debug.Log("Boss Damaged!");
        }
    }
}
