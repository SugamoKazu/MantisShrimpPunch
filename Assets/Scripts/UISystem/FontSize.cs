using UnityEngine;
using TMPro;

public class FontSize : MonoBehaviour
{
    private TextMeshProUGUI text;
    private float size;

    [SerializeField, Range(0f, 1f)] private float range = 0.3f;

    [SerializeField, Range(0f, 10f)] private float freq = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        size = text.fontSize;
    }

    // Update is called once per frame
    void Update()
    {
        var goalSize = size * (1 + range * Mathf.Cos(Mathf.PI * (1 - freq * Time.time % 1) % Mathf.PI)); // カウントダウンに応じてフォントサイズを大きくする
        text.fontSize = Mathf.Lerp(text.fontSize, goalSize, 0.1f);
    }
}
