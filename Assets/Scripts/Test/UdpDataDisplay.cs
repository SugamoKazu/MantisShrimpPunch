using UnityEngine;
using TMPro; // TextMeshProを扱うために必要

// TextMeshProUGUIコンポーネントがアタッチされていることを保証する
[RequireComponent(typeof(TextMeshProUGUI))]
public class UdpDataDisplay : MonoBehaviour
{
    // 表示用テキストコンポーネントの参照
    private TextMeshProUGUI textElement;
    
    // ネットワーク処理を行うコントローラーの参照
    private MQTTController mqttController;

    void Awake()
    {
        // 自身にアタッチされているTextMeshProUGUIコンポーネントを取得
        textElement = GetComponent<TextMeshProUGUI>();
    }

    // オブジェクトが有効になったときに呼ばれる
    void OnEnable()
    {
        // シーン内からMQTTControllerを探して参照を取得
        mqttController = FindObjectOfType<MQTTController>();
        if (mqttController != null)
        {
            // MQTTControllerのOnUdpDataReceivedイベントに、
            // 自身のUpdateDisplayTextメソッドを登録（購読）する
            mqttController.OnUdpDataReceived += UpdateDisplayText;
        }
        else
        {
            Debug.LogError("MQTTControllerがシーン内に見つかりません。");
        }
    }

    // オブジェクトが無効になったときに呼ばれる
    void OnDisable()
    {
        // メモリリークを防ぐため、登録したイベントを必ず解除する
        if (mqttController != null)
        {
            mqttController.OnUdpDataReceived -= UpdateDisplayText;
        }
    }

    // イベント経由で呼び出されるメソッド
    // UDPデータが受信されるたびに、このメソッドが実行される
    private void UpdateDisplayText(int value)
    {
        // 受け取った整数値を文字列に変換してテキストに設定
        textElement.text = value.ToString();
    }
}