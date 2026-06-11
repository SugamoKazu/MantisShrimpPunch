using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class Boss : MonoBehaviour
{
    [SerializeField] private Animator anim;
    public static int currentHP = 8;
    public bool defeated = false;
    public static bool damaged = false;

    private AudioSource audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHP = 8;
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("currentHP: " + currentHP);
        if (damaged)//if (Input.GetKeyDown("space"))
        {
            //currentHP--;
            //if (currentHP >= 1) anim.SetTrigger("isDamage");
            //Damage();
        }
        if (currentHP <= 0)
        {
            //Debug.Log("Boss Defeated!");
            audioSource.Play();

            anim.SetTrigger("isDefeated");
            defeated = true;
            StartCoroutine(Defeat());
            currentHP = 8;
        }
        
        damaged = false;
    }
    IEnumerator Defeat()
    {
        // ボスが倒されたときの処理
        // 例: アニメーション再生、エフェクト表示、ゲームクリアなど
        //Debug.Log("Boss Defeated! Game Clear!");
        // ここにゲームクリアの処理を追加


        yield return new WaitForSeconds(1.9f); // 1.9秒待つ

        this.gameObject.SetActive(false);
        defeated = false;
    }
}
