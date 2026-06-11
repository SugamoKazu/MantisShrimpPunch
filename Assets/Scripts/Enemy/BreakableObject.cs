using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

[System.Serializable]
public class CabitationEvent : UnityEvent { }
public class BreakableObject : MonoBehaviour
{
    private GameObject brokenObject;
    [SerializeField] private GameObject brokenPrefab;
    private GameObject cabEffect;
    [SerializeField] private Transform cabPos;
    private Transform brokenTransform;
    private Rigidbody rb;
    private Vector3 hitVec;
    private float forcePower; // 衝突時の力の強さ
    private Transform player; // プレイヤーのTransformを参照するための変数
    public bool isHit = false; // 衝突フラグ

    private UIManager uIManager;
    public int enemyNum;     //敵の種類
    public int breakScore; //点数

    [SerializeField] private AudioSource audioSource;

    //public CabitationEvent onCabitationEvent;
    public bool isRight = false;
    public bool isLeft = false;

    private bool ColliderEnabled;

    private Collider Collider;

    private SVSerialHandler SVserialHandler;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        
            
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        GameObject UI = GameObject.Find("UI");
        uIManager = UI.GetComponent<UIManager>();
        Collider = GetComponent<Collider>();
        ColliderEnabled = true;

        //GameObject serialObj = GameObject.Find("ActionManager");
        //SVserialHandler = serialObj.GetComponent<SVSerialHandler>();

    }
    
    // Update is called once per frame
    void Update()
    {
        isLeft = false;
        isRight = false;
        
        if (isHit)//break
        {
            //onCabitationEvent.Invoke();


            audioSource.Play();
            rb.AddForce(0.75f * hitVec * forcePower * forcePower, ForceMode.Impulse);
            rb.AddTorque(new Vector3(forcePower * forcePower *150f, 0f, 0f)); // y軸方向に回転を加える
            


            StartCoroutine(Smash());

        }
        isHit = false; // 衝突フラグをリセット
    }

    IEnumerator Smash()
    {
        uIManager.scores[enemyNum] += breakScore;
        //Debug.Log("+ " + breakScore);

        yield return new WaitForSeconds(0.3f);

        brokenObject = Instantiate(brokenPrefab, transform.position, transform.rotation);
        brokenTransform = brokenObject.transform;
        brokenTransform.localScale = transform.localScale;

        foreach (Rigidbody rb in brokenObject.GetComponentsInChildren<Rigidbody>())
        {
            rb.AddExplosionForce(550f, transform.position - Vector3.up, 3f);
        }

        foreach (Transform child in brokenTransform.GetComponentsInChildren<Transform>())
        {
            child.DOScale(0.01f * child.localScale, 0.6f).SetDelay(0.8f);
        }

        Destroy(this.gameObject);
        Destroy(brokenObject, 1.3f);
    }


    void OnTriggerEnter(Collider other)
    {
 

        if ((other.CompareTag("LeftArm") || other.CompareTag("RightArm")) && ColliderEnabled)
        {
            Transform punchTransform = other.transform.parent.parent;
            GameObject punchArm = punchTransform.gameObject; //腕の取得
            ColliderEnabled = false;

            forcePower = punchArm.GetComponent<PunchAction>().cabPower;



            isHit = true; // 衝突フラグを立てる
            hitVec = punchTransform.forward + 0.5f * Vector3.up;
            Debug.Log("Vectol : " + hitVec);

            cabEffect = (GameObject)Resources.Load("cabEffect");
            Transform cabPos = other.transform.GetChild(1);
            //Debug.Log(cabPos.name);
            Instantiate(cabEffect, cabPos.position, this.transform.rotation);//, other.transform);//キャビテーション発生

            Animator punchAnim = punchArm.GetComponent<Animator>();
            punchAnim.SetTrigger("HitTrigger"); //ヒット時の腕のアニメーション再生

            if (other.CompareTag("LeftArm"))
            {
                if (ModeManager.isMainMode)
                {
                    DataSendManager.Instance.SendLeft("Syakote_Solenoid");
                }
                else
                {
                    DataSendManager.Instance.SendMiss("Syakote_Solenoid");
                }

                DataSendManager.Instance.SendHit("Syakote_Left");

                //SVserialHandler.Write("L"); 
            }
            else if (other.CompareTag("RightArm"))
            {
                if (ModeManager.isMainMode)
                {
                    DataSendManager.Instance.SendRight("Syakote_Solenoid");
                }
                else
                {
                    DataSendManager.Instance.SendHit("Syakote_Solenoid");
                }

                DataSendManager.Instance.SendHit("Syakote_Right");
                //SVserialHandler.Write("R");
            }
            // プレイヤーがオブジェクトに触れたときの処理
            //Debug.Log("Arm touched the breakable object!");
        }
    }
    void OnCollisionEnter(Collision collider)
    {
        if (collider.gameObject.CompareTag("LeftArm") || collider.gameObject.CompareTag("RightArm"))
        {
            isHit = true; // 衝突フラグを立てる
            //hitVec = (this.transform.position - player.position).normalized + Vector3.up;

            if (collider.gameObject.CompareTag("LeftArm"))
            {
                isLeft = true;
            }
            else if (collider.gameObject.CompareTag("RightArm"))
            {
                isRight = true;
            }
            // プレイヤーがオブジェクトに触れたときの処理
            //Debug.Log("Arm touched the breakable object!");
        }
    }
}
