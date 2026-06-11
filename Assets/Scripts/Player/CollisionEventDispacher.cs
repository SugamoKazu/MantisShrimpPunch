using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ColliderEvent : UnityEvent<Collider> { }


public class CollisionEventDispacher : MonoBehaviour
{
    public Animator anim;


    public ColliderEvent _OnColliderEvent;

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Target") && anim.GetCurrentAnimatorStateInfo(0).IsName("Punch"))
        {
            _OnColliderEvent.Invoke(other);
            //Debug.Log("Serial.Write()");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Target"))
        {
            _OnColliderEvent.Invoke(collision.collider);
            //Debug.Log("Serial.Write()");
        }
    }

}
