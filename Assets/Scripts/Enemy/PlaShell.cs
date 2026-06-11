using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class PlaShell : MonoBehaviour
{
    public Transform target;

    private GameObject sandSmoke;
    public float speed = 1f;           // 移動速度
    private float T = 0; // 後退距離
    private Animator anim;
    private bool bossPlaying, destroying = false;
    public bool tentacle = false;

    private GameObject Panel;
    [SerializeField] private bool isTutorial= false;

    void Start()
    {
        anim = GetComponent<Animator>();
        sandSmoke = (GameObject)Resources.Load("SandSmoke");


        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }

        transform.position = new Vector3(transform.position.x, -0.097f, transform.position.z);



        if(isTutorial)
        {
            anim.SetTrigger("Idling");
            GetComponent<PlaShell>().enabled = false;
        }
        else
        {
            Panel = GameObject.Find("DangerPanel");

            GameObject dangerIcon = (GameObject)Resources.Load("DangerIcon");

            GameObject iconClone = Instantiate(dangerIcon, Panel.transform);

            GameObject targetAttach = transform.Find("target").gameObject;
            iconClone.GetComponent<TargetNavigation>().targetTransform = targetAttach.transform;
            iconClone.GetComponent<TargetNavigation>().targetClone = this.gameObject;
        }
        

    }


    void Update()
    {
        UIManager uIManager;
        GameObject uIObj = GameObject.Find("UI");
        uIManager = uIObj.GetComponent<UIManager>();
        bossPlaying = uIManager.bossPlaying;

        bool broken = GetComponent<BreakableObject>().isHit;

        if (broken)
        {

            GetComponent<PlaShell>().enabled = false;
            anim.enabled = false;
        }

        if (bossPlaying && !destroying)
        {
            //Debug.Log("Emergency");
            destroying = true;
            if (!tentacle) Eliminate();
            //else
            //{
              //  if (Boss.currentHP == 0) Eliminate();
            //}
        }
        if (!bossPlaying && tentacle ) Eliminate();

        Vector3 dir = target.position - transform.position; 
        dir.y = 0f;
        this.transform.forward = dir.normalized;

        if (dir.magnitude < 2f)
        {
            if(anim != null) anim.SetTrigger("Idling");

            if (T < 2f)
            {
                T += Time.deltaTime;
            }
            else
            {
                if(anim != null) anim.SetTrigger("Attacking");

                Vector3 destroyPos = this.transform.position + new Vector3(0f, -1f, 0f);
                this.transform.DOLocalMove(destroyPos, 3f).SetDelay(5f);
                this.transform.DOScale(new Vector3(0.1f, 0.1f, 0.1f), 3f).SetDelay(5f);
                Destroy(this.gameObject, 9f);
                
            }

        }

        else this.transform.position = transform.position + speed * dir.normalized / 500f;
    }
    
    void Eliminate()
    {
        Vector3 destroyPos = this.transform.position + new Vector3(0f, -0.5f, 0f);
        this.transform.DOScale(new Vector3(0.1f, 0.1f, 0.1f), 0.5f);

        Destroy(this.gameObject, 0.5f);
        
    }

}