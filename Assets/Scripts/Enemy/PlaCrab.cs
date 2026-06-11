using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class PlaCrab : MonoBehaviour
{
    public Transform target;
    public float speed = 1f;           // 移動速度
    private int d = 1;                 // 横移動方向
    private float interval = 0f, vec = 3.75f; // 後退距離
    public Animator anim;
    private bool bossPlaying, destroying = false;
    public bool tentacle = false;

    private GameObject Panel;
    [SerializeField] private bool isTutorial= false;
    void Start()
    {
        d = Random.Range(0, 2) * 2 - 1;
        if (isTutorial) d = -1;

        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }

        transform.position = new Vector3(transform.position.x, 0.016f, transform.position.z);
        Panel = GameObject.Find("DangerPanel");

        GameObject dangerIcon = (GameObject)Resources.Load("DangerIcon");

        GameObject iconClone = Instantiate(dangerIcon, Panel.transform);

        GameObject targetAttach = transform.Find("target").gameObject;
        iconClone.GetComponent<TargetNavigation>().targetTransform = targetAttach.transform;
        iconClone.GetComponent<TargetNavigation>().targetClone = this.gameObject;
    

    }


    void Update()
    {
        //GetComponent<PlaCrab>().enabled = false;
        UIManager uIManager;
        GameObject uIObj = GameObject.Find("UI");
        uIManager = uIObj.GetComponent<UIManager>();
        bossPlaying = uIManager.bossPlaying;

        bool broken = GetComponent<BreakableObject>().isHit;

        if (broken)
        {
            GetComponent<PlaCrab>().enabled = false;
            anim.enabled = false;
        }

        if (bossPlaying && !destroying)
        {
            //Debug.Log("Emergency");
            destroying = true;
            if (!tentacle) Eliminate();
            else destroying = false;
            //else
            //{
              //  if (Boss.currentHP == 0) Eliminate();
            //}
        }
        if (!bossPlaying && tentacle && !destroying) Eliminate();

        Vector3 dir = target.position - transform.position; 
        dir.y = 0f;
        this.transform.forward = dir.normalized;

        if (dir.magnitude < 2f)
        {

            
            if (anim != null && !isTutorial) anim.SetTrigger("Attacking");

            Vector3 destroyPos = this.transform.position + new Vector3(0f, -1f, 0f);
            this.transform.DOLocalMove(destroyPos, 3f).SetDelay(5f);
            this.transform.DOScale(new Vector3(0.1f, 0.1f, 0.1f), 3f).SetDelay(5f);
            Destroy(this.gameObject, 9f);



        }

        else
        {
            interval += Time.deltaTime;
            if (dir.magnitude < 3.5f)
            {
                this.transform.position = transform.position + speed * dir.normalized / 500f;
                interval = 0f;
            }

            if (interval > 0.4f)
            {
                Vector3 nextPosition = transform.position + vec * transform.forward + d * vec * Mathf.Sqrt(3) * transform.right;
                transform.DOMove(nextPosition, 0.25f).SetEase(Ease.OutBack);
                interval = 0f;
                vec *= Mathf.Sqrt(3) / 2;
                d *= -1;
            }
            
            
            
            
        }
            
    }
    
    void Eliminate()
    {
        Vector3 destroyPos = this.transform.position + new Vector3(0f, -0.5f, 0f);
        this.transform.DOScale(new Vector3(0.1f, 0.1f, 0.1f), 0.5f);

        Destroy(this.gameObject, 0.5f);
        
    }

}