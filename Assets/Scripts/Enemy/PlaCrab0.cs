using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class PlaCrab0 : MonoBehaviour
{
    [SerializeField, Tooltip("Throwing Object")] private GameObject bottle;
    [SerializeField, Tooltip("Target")] private GameObject targetObject;

    [SerializeField, Range(0F, 90F), Tooltip("Throwing Angle")] private float throwingAngle;
    [SerializeField] private float throwInterval = 6f;
    private float throwTime = 0f, t = 0f;

    [SerializeField] float moveSpeed = 1f;
    private bool isWalk = true, bossPlaying;
    public Animator anim;

    [SerializeField] AudioSource moveAudio;

    private void Start()
    {
        targetObject = GameObject.FindGameObjectWithTag("Player");
    }
    private void Update()
    {
        UIManager uIManager;
        GameObject uIObj = GameObject.Find("UI");
        uIManager = uIObj.GetComponent<UIManager>();
        bossPlaying = uIManager.bossPlaying;

        if (bossPlaying)
        {
            Eliminate();
            bossPlaying = false;
        }

        Move();
        throwTime += Time.deltaTime;

        if (throwTime > throwInterval)
        {
            isWalk = false;
            anim.SetTrigger("Throwing");
            StartCoroutine("Throw");
            throwTime = 0f;
        }




    }
    private void Move()
    {
        var direction = targetObject.transform.position - this.transform.position;
        this.transform.forward = new Vector3(direction.x, 0f, direction.z).normalized;

        Vector3 nextVec = this.transform.position;

        nextVec += transform.forward * Time.deltaTime * moveSpeed;

        var factor = direction.magnitude * Mathf.Sin(t) / 700f;
        Vector3 offset = transform.right * factor;
        nextVec += offset;

        if (!isWalk) nextVec = this.transform.position;
        else if (direction.magnitude < 2f)
        {
            nextVec = this.transform.position;
            throwInterval = 30f;
            Attack();
        }
        else t += Time.deltaTime;

        this.transform.position = Vector3.Lerp(this.transform.position, nextVec, 0.6f);
    }

    private IEnumerator Throw()
    {

        yield return new WaitForSeconds(0.4f);

        GameObject ball = Instantiate(bottle, this.transform.position, Quaternion.identity);
        Vector3 targetPosition = targetObject.transform.position;
        Vector3 offset = new Vector3(
            Random.Range(-0.5f, 0.5f),
            Random.Range(-0.25f, 0.25f),
            Random.Range(-0.5f, 0.5f)
        );

        targetPosition += offset;
        float angle = throwingAngle;
        Vector3 velocity = CalculateVelocity(this.transform.position, targetPosition, angle);

        Rigidbody rb = ball.GetComponent<Rigidbody>();
        rb.AddForce(velocity * rb.mass * 1.75f, ForceMode.Impulse);

        yield return new WaitForSeconds(1.5f);

        isWalk = true;
        anim.SetTrigger("Walking");
    }

    private Vector3 CalculateVelocity(Vector3 pointA, Vector3 pointB, float angle)
    {
        float rad = angle * Mathf.PI / 180;
        float x = Vector2.Distance(new Vector2(pointA.x, pointA.z), new Vector2(pointB.x, pointB.z));
        float y = pointA.y - pointB.y;
        float speed = Mathf.Sqrt(-Physics.gravity.y * Mathf.Pow(x, 2) / (2 * Mathf.Pow(Mathf.Cos(rad), 2) * (x * Mathf.Tan(rad) + y)));
        if (float.IsNaN(speed))
        {
            return Vector3.zero;
        }
        else
        {
            return (new Vector3(pointB.x - pointA.x, x * Mathf.Tan(rad), pointB.z - pointA.z).normalized * speed);
        }
    }

    void Attack()
    {
        moveAudio.Stop();
        anim.SetTrigger("Attacking");
        
        Vector3 destroyPos = this.transform.position;
        destroyPos.y -= 0.5f;

        float destroyDelay = 6f;
        this.transform.DOLocalMove(destroyPos, 1.5f).SetDelay(destroyDelay);
        this.transform.DORotate(new Vector3(0, 270, 0), 1.5f, RotateMode.LocalAxisAdd).SetDelay(destroyDelay);
        this.transform.DOScale(new Vector3(0.1f, 0.1f, 0.1f), 1.5f).SetDelay(destroyDelay);

        Destroy(this.gameObject, 2f + destroyDelay);

    }

    void Eliminate()
    {
        //Debug.Log("Emergency");
        Vector3 destroyPos = this.transform.position + new Vector3(0f, -0.5f, 0f);
        this.transform.DOScale(new Vector3(0.1f, 0.1f, 0.1f), 0.5f);

        Destroy(this.gameObject, 0.5f);

    }
}