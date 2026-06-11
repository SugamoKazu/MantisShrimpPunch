using UnityEngine;

public class MoveAround : MonoBehaviour
{
    public Transform player;
    public GameObject timer;
    public float rotateSpeed = 30f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        if(timer == null) timer = GameObject.Find("Timer");
        

        rotateSpeed *= Random.Range(0.7f, 1f);
        rotateSpeed *= Random.Range(-1f, -0.7f);
        
    }

    // Update is called once per frame
    void Update()
    {
        //if(timer != null && timer.activeSelf) transform.RotateAround(player.position, Vector3.up, (2+Mathf.Cos(Time.time))*rotateSpeed*Time.deltaTime);
        //else if(timer == null)    transform.RotateAround(player.position, Vector3.up, (2+Mathf.Cos(Time.time))*rotateSpeed*Time.deltaTime);
        transform.RotateAround(player.position, Vector3.up, (2+Mathf.Cos(Time.time))*rotateSpeed*Time.deltaTime);
    }
}
