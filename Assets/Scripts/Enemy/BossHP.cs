using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHP : MonoBehaviour
{
    [SerializeField] private Image hpBar;
    [SerializeField] private int maxHP = 8;
    [SerializeField] private int currentHP = 8;
    public bool isDefeated = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isDefeated = false;
        currentHP = maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (isDefeated)
        {
            hpBar.fillAmount = 0;
            return;
        } */

        // Update the health bar
        hpBar.color = Color.green;
        currentHP = Boss.currentHP;

        hpBar.fillAmount = 0.64f * currentHP / maxHP;
        if (currentHP <= 0)
        {
            isDefeated = true;
           //Debug.Log("Boss Defeated!");
            currentHP = maxHP;
        }


        if(currentHP == 2)
        {
            hpBar.color = Color.yellow;
        }
        else if(currentHP == 1)
        {
            hpBar.color = Color.red;
        }
    }
}
