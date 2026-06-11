using UnityEngine;
using System.Collections;

public class AudioShell : MonoBehaviour
{
    private AudioSource[] sources;
    private bool isHit = false;

    void Start()
    {
        sources = gameObject.GetComponents<AudioSource>();
        sources[0].PlayDelayed(0.36f);
    }

    void Update()
    {
        if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("ShellIdle"))
        {
            sources[0].Stop();
            //Debug.Log("Idle");
            sources[1].Play();
            //StartCoroutine(StartDelay(sources[1], 0f));
            //sources[2].PlayDelayed(0.35f);
            //StartCoroutine(StartDelay(sources[2], 0.35f));
        }

        if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("ShellRemove"))
        {
            //StartCoroutine(StartDelay(sources[3], 0.36f));
        }
        //Debug.Log(sources[3].clip.name);
    }
}