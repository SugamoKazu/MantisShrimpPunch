using UnityEngine;
using UnityEngine.SceneManagement;

public class Load : MonoBehaviour
{
    void Start()
    {
        DataSendManager.Instance.SendPassive("Syakote_Right");
        DataSendManager.Instance.SendPassive("Syakote_Left");
        // このシーンが起動したら、すぐに次のシーン（GameTitle）に移動する
        SceneManager.LoadScene("GameTitle");

    }
}