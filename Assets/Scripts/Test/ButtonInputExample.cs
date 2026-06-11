using UnityEngine;
public class ButtonInputExample : MonoBehaviour
{
   void Update()
   {
       // ボタンが押されているか確認
       if (OVRInput.Get(OVRInput.Button.One))
       {
           Debug.Log("Button One is being pressed.");
       }
       // ボタンが押された瞬間を検出
       if (OVRInput.GetDown(OVRInput.Button.One))
       {
           Debug.Log("Button One was just pressed.");
       }
       // ボタンが離された瞬間を検出
       if (OVRInput.GetUp(OVRInput.Button.One))
       {
           Debug.Log("Button One was just released.");
       }
   }
}