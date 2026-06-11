using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITutrial : MonoBehaviour
{
    [SerializeField] private GameObject[] audios;

    [SerializeField] private GameObject[] panels;
    



    // Start is called before the first frame update
    void Start()
    {

        DataSendManager.Instance.SendPassive("Syakote_Right");
        DataSendManager.Instance.SendPassive("Syakote_Left");
        
        
    }

    // Update is called once per frame
    void Update()
    {
        //if()
        //StartCoroutine(Delay());
        //「君にはモンハナシャコになってもらう」
        //「シャコになりデコピンを使って、シャコパンチを打つんだ」
        //「シャコのもつ最強のパンチとキャビテーションを体験してみよう」
        //「操作説明」
        audios[0].SetActive(true);
        //「まずは右のほうを見るんだ」
        //「こんなみためのプラクリーチャーが襲い掛かってくる」
        panels[0].SetActive(true);
        DataSendManager.Instance.SendActive("Syakote_Right");
        //「デコピンで力をためるんだ」
        //「指に力を入れるとパンチのパワーがたまるぞ」
        //「射程範囲に入るとここの色が変わるぞ」

        //「デコピンを開放してパンチを放て！」


        //audios[].SetActive(true);
        if (panels[0] == null) DataSendManager.Instance.SendPassive("Syakote_Right"); //「そうだ」


        panels[1].SetActive(true);
        //「見えないとこの敵には警告アイコンが表示されるぞ」

        //「左に体をひねって」
        DataSendManager.Instance.SendActive("Syakote_Left");
        //「次は左手で打ってみよう」

        if (panels[1] == null)
        {
            //「残りの一つもぶち壊すんだ」
            panels[2].SetActive(true);
            //audios[].SetActive(true);
            DataSendManager.Instance.SendActive("Syakote_Right");
        }

        //「さあプラクリーチャーで汚れた海をきれいにするんだ」
        //「襲い掛かる強敵を迎え撃て」
        //「準備ができたら両方の手でパンチを打つんだ」



        // if()



        //state++; //ゲーム開始の状態遷移
    }
    
    //IEnumerator(float seconds)
    //{
    //    //yield return new WaitForSeconds(seconds);

    //}

    void ActiveChange(GameObject obj)
    {
        obj.SetActive(false);
    }
}
