using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class SettingsUI : MonoBehaviour
{
    //[Header("参照するUIManager")]
    // 既存のUIManagerをインスペクターから設定する
    //public UIManager uiManager;

    [SerializeField] LSerialHandler LserialHandler;
    [SerializeField] RSerialHandler RserialHandler;
    [SerializeField] SVSerialHandler SVserialHandler;

    [Header("UIコンポーネント")]
    public TMP_InputField playTimeInputField;
    public TMP_InputField bossTimeInputField;
    public Button saveButton;
    public Button closeButton;

    // このUIパネル自身
    private GameObject settingsPanel;

    void Awake()
    {
        settingsPanel = this.gameObject;

        /*
        if (uiManager == null)
        {
            Debug.LogError("UIManagerが設定されていません！インスペクターを確認してください。");
            return;
        }
        */

        // ボタンが押された時の処理を登録
        saveButton.onClick.AddListener(OnSaveButtonClicked);
        closeButton.onClick.AddListener(OnCloseButtonClicked);

        settingsPanel.SetActive(false);
    }
    void Start()
    {
        LserialHandler.Write("S");
        RserialHandler.Write("S");
        Debug.Log("Sent S");
    }
    void OnEnable()
    {
        // ① UIManagerから現在の値を取得し、InputFieldに反映
    /*if (uiManager != null)
    {*/
    playTimeInputField.text = UIManager.sPlayTime.ToString();
            bossTimeInputField.text = UIManager.sBossTime.ToString();
        /*}*/
    }

    /// <summary>
    /// ③ Saveボタンが押された時の処理
    /// </summary>
    private void OnSaveButtonClicked()
    {
        // ② InputFieldの値をfloat型に変換
        float.TryParse(playTimeInputField.text, out float newPlayTime);
        float.TryParse(bossTimeInputField.text, out float newBossTime);

        UIManager.sPlayTime = newPlayTime;
        UIManager.sBossTime = newBossTime;

        // UIManagerに用意した更新メソッドを呼び出す
        //UIManager.UpdateGameTimes(newPlayTime, newBossTime);
    }

    /// <summary>
    /// ④ Closeボタンが押された時の処理
    /// </summary>
    private void OnCloseButtonClicked()
{
    settingsPanel.SetActive(false);
}

    /// <summary>
    /// 外部（例：タイトル画面の「設定」ボタン）からこのパネルを開くためのメソッド
    /// </summary>

    /*
    public void OpenPanel()
    {
        settingsPanel.SetActive(true);
    }
    */
}