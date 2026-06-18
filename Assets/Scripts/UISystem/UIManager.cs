using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private float startCount = 3;
    public static float sPlayTime = 0;
    public static float sBossTime = 0;
    [SerializeField] public float playTime = 0f;
    [SerializeField] public float bossTime = 0f;
    private float totalPlayTime, totalBossTime, punchPowerLeft = 0, punchPowerRight = 0;
    [SerializeField] GameObject tutorial;
    private int childCount = 3;
    [SerializeField] GameObject tutorialVoiceObjForVR;
    [SerializeField] GameObject tutorialVoiceObjForPC;
    [SerializeField] GameObject pauseObj;
    [SerializeField] GameObject startObj;
    public TextMeshProUGUI startCountText;
    [SerializeField] Image startCountCircle;
    
    [SerializeField] GameObject gameObj;
    [SerializeField] Image chargeCircleLeft;
    [SerializeField] Image chargeCircleRight;
    private Vector4 nowColor = new Vector4(0f, 1f, 0f, 1f);
    public TextMeshProUGUI playTimeText;
    public TextMeshProUGUI bossTimeText;
    [SerializeField] Image gameTimeCircle;
    [SerializeField] GameObject bossObject;
    [SerializeField] GameObject bossHP;
    [SerializeField] GameObject resultObj;
    [SerializeField] TextMeshProUGUI scoreAText, scoreBText, scoreCText, scoreDText, RankText;
    public int[] scores = { 0, 0, 0, 0 };

    public bool gamePlaying, bossPlaying;

    private bool colorWhite = false, bossDefeated = false;
    private float flashTime = 0f;
    public int state = 0;

    [SerializeField] private AudioSource audioSource;

    [SerializeField] LSerialHandler LserialHandler;
    [SerializeField] RSerialHandler RserialHandler;

    private string usedArmLeft, usedArmRight;
    //SettingsUI settingsUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //SerialWrite("A");

        //playTime = sPlayTime;
        //bossTime = sBossTime;

        startCount++;

        playTime++;
        totalPlayTime = playTime;//回りくどくね //FilledCircle()でカウントゲージをつくるために割合をとるから全体の秒数が必要

        bossTime++;
        totalBossTime = bossTime;//回りくどくね

        gamePlaying = false;
        bossPlaying = false;
        bossObject.SetActive(false);
        bossHP.SetActive(false);

        if (tutorial != null) tutorial.SetActive(state == 0);
        if (ModeManager.isVRDevice)
        {
            if (tutorialVoiceObjForVR != null) tutorialVoiceObjForVR.SetActive(state == 0);
        }
        else
        {
            if (tutorialVoiceObjForPC != null) tutorialVoiceObjForPC.SetActive(state == 0);
        }
        
        pauseObj.SetActive(false);
        startObj.SetActive(false);
        gameObj.SetActive(false);
        resultObj.SetActive(false);

        audioSource.pitch = 0.7f;

        Time.fixedDeltaTime = 0.016666667f;

        // state = 0;
        // state = 1;

        //audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        Mode();

        if (state == 0)//チュートリアル
        {
            //RenderSettings.fogDensity = 0.5f;
            

            pauseObj.SetActive(false);

            if (tutorial != null) tutorial.SetActive(true);
            gameObj.SetActive(true);

            startCount = 4f;
            playTime = totalPlayTime;
            bossTime = totalBossTime;

            FilledCircle(gameTimeCircle, 0f, 1f);
            playTimeText.text = "";

            if (OVRInput.GetDown(OVRInput.Button.One) /* || Input.GetKeyDown("space") */)
            {
                if (tutorialVoiceObjForVR != null) tutorialVoiceObjForVR.SetActive(false);
                if (tutorialVoiceObjForPC != null) tutorialVoiceObjForPC.SetActive(false);
                state++;
                UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
            }



        }
        if (state == 1)//ゲーム開始前カウントダウン
        {
            if (tutorial != null) tutorial.SetActive(false);
            if (tutorialVoiceObjForVR != null) tutorialVoiceObjForVR.SetActive(false);
            if (tutorialVoiceObjForPC != null) tutorialVoiceObjForPC.SetActive(false);
            if (startObj != null) startObj.SetActive(true);
            if (gameObj != null) gameObj.SetActive(true);

            FilledCircle(gameTimeCircle, 0f, 1f);
            playTimeText.text = "";

            // audioSource.pitch = 0.7f;
            //audioSources[0].Play();
            if (tutorial != null)
            {
                foreach (Rigidbody rb in tutorial.GetComponentsInChildren<Rigidbody>())
                {
                    if (rb != null) Destroy(rb.gameObject);
                }
            }

            startCount = CountDown(startCount, startCountText, state);
            if (startCount > 1) FilledCircle(startCountCircle, startCount % 1, 1f);
            else FilledCircle(startCountCircle, 0f, 1f);

            //audioSources[1].PlayDelayed(totalPlayTime + totalBossTime - 3.5f); //ゲーム終了時にホイッスル
        }
        if (state == 2)//ゲーム中
        {
            gamePlaying = true;

            startObj.SetActive(false);
            //gameObj.SetActive(true);

            playTime = CountDown(playTime, playTimeText, state);
            if (playTime > 1) FilledCircle(gameTimeCircle, playTime - 1, totalPlayTime);
            else FilledCircle(gameTimeCircle, 0f, totalPlayTime);


        }

        if (state == 3)//ボス戦
        {
            audioSource.pitch = 1.3f;
            bossPlaying = true;
            bossObject.SetActive(true);
            bossHP.SetActive(true);

            bossTime = CountDown(bossTime, bossTimeText, state);
            FilledCircle(gameTimeCircle, bossTime - 1, totalBossTime);

            bossDefeated = bossObject.GetComponent<Boss>().defeated;

            if (bossDefeated)
            {
                DataSendManager.Instance.SendStop("Syakote_Right");
                DataSendManager.Instance.SendStop("Syakote_Left");
                state++;
            }
            //enemy destroy
                //no spawn
            }
        if (state == 4)//ゲーム終了
        {
            //Time.timeScale = 0;
            //RenderSettings.fogDensity = 0.5f;

            gamePlaying = false;
            bossPlaying = false;

            gameObj.SetActive(false);
            bossHP.SetActive(false);
            resultObj.SetActive(true);

            if (OVRInput.Get(OVRInput.Button.One) || Input.GetKeyDown("space"))
            {
                //SerialWrite("S");
                DataSendManager.Instance.SendPassive("Syakote_Right");
                DataSendManager.Instance.SendPassive("Syakote_Left");
                LoadScene("GameTitle");


                /*
                for (int i = 0; i < 3; i++)
                {
                    scores[i] = 0;

                    var Obj = (GameObject)Instantiate(panels[i], panelPositions[i].position, Quaternion.identity);
                    Obj.transform.parent = tutorial.transform;
                }

                if (bossObject.activeSelf) bossObject.SetActive(false);
                bossHP.SetActive(false);


                state = -1;
                */
            }

        }


        ChargeGauge(usedArmLeft, chargeCircleLeft);
        ChargeGauge(usedArmRight, chargeCircleRight);
        Score();

        if (OVRInput.Get(OVRInput.Button.Two,OVRInput.Controller.RTouch) /* || Input.GetKeyDown("b") */)
        {
            DataSendManager.Instance.SendPassive("Syakote_Left");
            DataSendManager.Instance.SendPassive("Syakote_Right");
            LoadScene("GameTitle");
        }
    
    }

    void Mode()
    {
        if (ModeManager.isMainMode)
        {
            usedArmLeft = "PunchArmLeftMain";
            usedArmRight = "PunchArmRightMain";
        }
        else
        {
            usedArmLeft = "PunchArmLeftSub";
            usedArmRight = "PunchArmRightSub";
        }
    }

    float CountDown(float count, TextMeshProUGUI countText, int nowState)
    {
        string[] timeupText = { "START", "EMERGENCY", "FINISH" };

        if (nowState == 1) count -= 1.5f * Time.fixedDeltaTime;
        else count -= 1.5f * Time.deltaTime;

        if (count >= 1) countText.text = "" + (int)count;
        else if (0 < count && count < 1)
        {
            countText.text = timeupText[state - 1];

        }
        //if (count < 0)
        else state++;


        return count;

        /*
                if (count < 4f)
                {
                    var goalSize = size * (1 + fontSizeRange * Mathf.Cos(Mathf.PI * (1 - count % 1) % Mathf.PI)); // カウントダウンに応じてフォントサイズを大きくする
                    text.fontSize = Mathf.Lerp(text.fontSize, goalSize, 0.1f);
                }
                */
    }

    void FilledCircle(Image circle, float fill, float total)
    {
        circle.fillAmount = fill / total;
    }

    void ChargeGauge(string punchSide, Image chargeCircle)
    {
        flashTime += Time.deltaTime;

        //if (gamePlaying)
        //{
        float punchPower = GameObject.Find(punchSide).GetComponent<GetTriggerValue>().punchPower;
        float flashInterval = 0.7f; // Default flash interval

        chargeCircle.fillAmount = Mathf.Lerp(chargeCircle.fillAmount, 0.3f * punchPower / 2f, 0.2f);

        if (punchPower >= 2f)
        {
            chargeCircle.fillAmount = 0.3f;
            nowColor = new Vector4(1f, 0.2f, 0.2f, 0.8f); // Set color to red
            chargeCircle.color = nowColor;
            flashInterval = 0.2f; // Reset flash interval for yellow
        }
        else if (punchPower < 1f)
        {
            nowColor = new Vector4(0.2f, 1f, 0.2f, 0.8f); // Set color to green
            chargeCircle.color = nowColor;
            flashInterval = 0.6f; // Reset flash interval for green
        }
        else
        {
            nowColor = new Vector4(1f, 1f, 0.2f, 0.8f);
            chargeCircle.color = nowColor;
            flashInterval = 0.4f; // Reset flash interval for yellow
        }


        if (flashTime > flashInterval)
        {
            flashTime = 0;
            colorWhite = !colorWhite;
        }

        if (colorWhite) chargeCircle.color = nowColor * 3f;

        //}
    }
    void Score()
    {
        float scoreA = scores[0];
        float scoreB = scores[1];
        float scoreC = scores[2];

        float scoreD = 0; //scores[3];
        int bossScore = 8 - Boss.currentHP;
        if (bossDefeated)
        {
            scoreD = 1;
            bossScore = 8;
        }
        

        scoreAText.text = "" + scoreA + "/6";
        scoreBText.text = "" + scoreB + "/5";
        scoreCText.text = "" + scoreC + "/4";
        scoreDText.text = "" + scoreD + "/1";

        char scoreRank = 'B';
        float scoreTotal = scoreA + scoreB + scoreC;
        if (scoreTotal >= 11) scoreRank = 'A';

        if (scoreRank == 'A' && scoreD == 1) scoreRank = 'S';

        RankText.text = "" + scoreRank;


        RenderSettings.fogDensity = 0.6f - (scoreTotal + bossScore) * 0.023f;
        
        //int trashChild = (scoreTotal + bossScore) / 2;
        //trashObject
        //Debug.Log(RenderSettings.fogDensity);

    }

    void SerialWrite(string c)
    {
        LserialHandler.Write(c);
        RserialHandler.Write(c);
    }
    
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
