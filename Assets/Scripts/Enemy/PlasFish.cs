using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class PlasFish : MonoBehaviour
{
    private GameObject target;
    private Vector3 dir;
    private int RL;

    [SerializeField] float moveSpeed = 1f;

    public Animator anim;
    private bool approaching, attacking, bossPlaying, destroying = false;
    public bool tentacle = false;
    private GameObject Panel;

    [SerializeField] private bool isTutorial= false;
    void Start()
    {
        if (Random.Range(0, 2) == 0) RL = 1;
        else RL = -1;

        if (isTutorial) RL = 1;

        target = GameObject.FindGameObjectWithTag("Player");
        dir = target.transform.position - this.transform.position;
        dir.y = 0f;

        transform.position = new Vector3(transform.position.x, 0.9f, transform.position.z);
        this.transform.forward = dir;
        this.transform.Rotate(0f, RL * 90f, 0f);

        approaching = true;

        Panel = GameObject.Find("DangerPanel");

        GameObject dangerIcon = (GameObject)Resources.Load("DangerIcon");

        GameObject iconClone = Instantiate(dangerIcon, Panel.transform);

        GameObject targetAttach = transform.Find("target").gameObject;
        iconClone.GetComponent<TargetNavigation>().targetTransform = targetAttach.transform;
        iconClone.GetComponent<TargetNavigation>().targetClone = this.gameObject;
    }
    void Update()
    {
        dir = target.transform.position - this.transform.position;

        UIManager uIManager;
        GameObject uIObj = GameObject.Find("UI");
        uIManager = uIObj.GetComponent<UIManager>();
        bossPlaying = uIManager.bossPlaying;

        bool broken = GetComponent<BreakableObject>().isHit;

        if (broken)
        {

            GetComponent<PlasFish>().enabled = false;
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

        if (approaching)
        {
            if (dir.magnitude < 2f)
            {
                approaching = false;
                attacking = true;
            }
            else Move();
        }
        else if (attacking == true)
        {
            attacking = false;
            if(!isTutorial) StartCoroutine("Attack");
        }

    }

    void Move()
    {
        this.transform.RotateAround(target.transform.position, Vector3.up, -RL * moveSpeed / 1.85f);

        Vector3 nextPos = this.transform.position - RL * transform.right * moveSpeed / 45f;
        this.transform.position = nextPos;
    }

    private IEnumerator Attack()
    {
        anim.SetTrigger("Attacking");
        Tween lookAtTween = transform.DOLookAt(target.transform.position, 1f);

        var destroyDelay = 6f;

        yield return new WaitForSeconds(1.5f);

        lookAtTween.Kill();

        Vector3 destroyPos = this.transform.position - 4 * dir.normalized;
        this.transform.DOLocalMove(destroyPos, 1.5f).SetDelay(destroyDelay);
        this.transform.DORotate(new Vector3(0f, 180f, 0f), 1.5f, RotateMode.WorldAxisAdd).SetDelay(destroyDelay - 1f);
        this.transform.DOScale(new Vector3(0.1f, 0.1f, 0.1f), 1.5f).SetDelay(destroyDelay);

        Destroy(this.gameObject, 7f);

    }

    void Eliminate()
    {
        //Debug.Log("Emergency");
        Vector3 destroyPos = this.transform.position + new Vector3(0f, -1.5f, 0f);
        this.transform.DOScale(new Vector3(0.1f, 0.1f, 0.1f), 0.5f);

        Destroy(this.gameObject, 0.5f);

    }
}