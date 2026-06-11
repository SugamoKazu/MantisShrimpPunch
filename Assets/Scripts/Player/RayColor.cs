using UnityEngine;

public class RayColor : MonoBehaviour
{
    public LRSide LR;
    public enum LRSide { Left, Right }
    [SerializeField] private Material CplL; // レイの色

    [SerializeField] private Material CplR; // レイの色
    [SerializeField] Vector4 defaultColor;
    [SerializeField] Vector4 changeColor;
    public bool colorChange = false, turnRight = false, turnLeft = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        defaultColor /= 255f;
        changeColor /= 255f;

    }

    // Update is called once per frame

    /*
    void FixedUpdate()
    {
        colorChange = false;
    }
    */

    void Update()
    {

        this.transform.Rotate(0f, 0f, 10f);

        if (LR == LRSide.Left)
        {
            if (colorChange)
            {
                //Debug.Log("Color Left Change Active");
                CplL.color = Vector4.Lerp(CplL.color, changeColor, Time.deltaTime * 3f); // レイの色を赤に変更
            }
            else
            {

                CplL.color = Vector4.Lerp(CplL.color, defaultColor, Time.deltaTime * 3f); // レイの色を元に戻す
            }
        }
        else if (LR == LRSide.Right)
        {
            if (colorChange)
            {
                //Debug.Log("Color Right Change Active");
                CplR.color = Vector4.Lerp(CplR.color, changeColor, Time.deltaTime * 3f); // レイの色を赤に変更
            }
            else
            {
                CplR.color = Vector4.Lerp(CplR.color, defaultColor, Time.deltaTime * 3f); // レイの色を元に戻す
            }
        }

        //Debug.Log(colorChange);
    }

    void OnTrigger(Collider collision)
    {
        
        
    }
    
    void OnTriggerStay(Collider collision)
    {
        Debug.Log(collision);
        
        if (collision.gameObject.CompareTag("Target"))
        {
            // プレイヤーがUIに触れたときの処理
            //Debug.Log("Player touched the UI!");
            colorChange = true;
            //CPL.color = new Color(1f, 0f, 0f, 0.27f); // レイの色を赤に変更
        }
        if (collision.gameObject.CompareTag("RightWall"))
        {
            turnRight = true;

        }
        if (collision.gameObject.CompareTag("LeftWall"))
        {
            turnLeft = true;
        }

    }
    void OnTriggerExit(Collider collision)
    {
        
        colorChange = false;
        if (collision.gameObject.CompareTag("Target"))
        {
            // プレイヤーがUIに触れたときの処理
            //Debug.Log("Player touched the UI!");
            colorChange = false;
            //CPL.color = new Color(1f, 0f, 0f, 0.27f); // レイの色を赤に変更

        }
    }
}
