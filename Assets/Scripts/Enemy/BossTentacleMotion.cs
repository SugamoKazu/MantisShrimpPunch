using UnityEngine;

public class BossTentacleMotion : MonoBehaviour
{
    private float dir = 1f;
    private float t = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime;
        //this.transform.rotation = Quaternion.Lerp(this.transform.rotation, this.transform.rotation * Quaternion.Euler(0f, dir * 1f, 0f), 0.01f);
        this.transform.Rotate(new Vector3(0f, dir * 1f, 0f), Space.World);
        if (t > 0.25f)
        {
            dir *= -1f;
            t = 0f;
        }
    }
}
