using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameStarter : MonoBehaviour
{
    [SerializeField] Animator rightAnim, leftAnim;

    private bool rightReady, leftReady;
    [SerializeField] private GameObject UI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DataSendManager.Instance.SendActive("Syakote_Right");
        DataSendManager.Instance.SendActive("Syakote_Left");

        if (leftAnim.GetCurrentAnimatorStateInfo(0).IsName("Punch")) leftReady = true;
        if (rightAnim.GetCurrentAnimatorStateInfo(0).IsName("Punch")) rightReady = true;

        if (OVRInput.GetDown(OVRInput.Button.One) || Input.GetKeyDown("s"))//仮 両手パンチでゲーム開始
        {
            //UI.GetComponent<UIManager>().state++;
        }
        if (leftReady && rightReady)
        {
            // UI.GetComponent<UIManager>().state++;
            UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
            gameObject.SetActive(false);
        }

    }
}
