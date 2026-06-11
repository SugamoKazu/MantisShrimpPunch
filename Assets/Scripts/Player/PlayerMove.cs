using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKey("right"))
        {
            transform.Rotate(Vector3.up);
        }
        if (Input.GetKey("left"))
        {
            transform.Rotate(Vector3.down);
        }
    }
}
