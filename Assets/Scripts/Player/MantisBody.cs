using UnityEngine;

public class MantisBody : MonoBehaviour
{
    [SerializeField] private Transform HMD;
    private Vector3 offset;

    [SerializeField] private Transform[] armTransforms;
    [SerializeField] private float armSpeed = 1f;
    [SerializeField] private int min = -30;
    [SerializeField] private int max = -150;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        offset = HMD.position - this.transform.position;
        transform.rotation = this.transform.rotation;
        transform.up = Vector3.up;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = HMD.position;
        
        var direction = HMD.forward;
        direction.y = 0;

        var lookRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, 0.1f);
    

        //this.transform.rotation = Quaternion.Euler(new Vector3( 0f, HMD.eulerAngles.y, 0f ));
        

        for (int i = 0; i < armTransforms.Length; i++)
        {
            float rnd = Random.Range(0f, 2f);
            float armPos = min + rnd * max;
            armTransforms[i].rotation = Quaternion.Lerp(armTransforms[i].rotation, this.transform.rotation*Quaternion.Euler(armPos, 0f, 0f), armSpeed / 100f);

        }
    }
}