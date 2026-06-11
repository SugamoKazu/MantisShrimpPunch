using UnityEngine;

public class SandSmoke : MonoBehaviour
{
    public GameObject arm;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // this.transform.position.y = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        // Ensure the y position remains at 0
        Vector3 position = arm.transform.position;
        position.y = 0f;
        this.transform.position = position;

        //SandSmoke();
        
    }
}
