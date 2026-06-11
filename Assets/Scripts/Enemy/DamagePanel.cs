using UnityEngine;

public class DamagePanel : MonoBehaviour
{
    [SerializeField] private GameObject DamageImage;
    [SerializeField] private float damageDelay;
    private GameObject Panel;
    private Animator anim;
    private bool Trigger;

    [SerializeField] AudioSource damageAudio;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Panel = GameObject.Find("DamagePanel");
        anim = GetComponent<Animator>();
        if(anim == null) anim = transform.Find("Crab").GetComponent<Animator>();
        //Debug.Log("anim : " + anim);
        Trigger = true;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("AnimeAttack : " + anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack"));
        if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack") && Trigger)
        {
            Debug.Log("Crab Damage");
            Invoke(nameof(Damage), damageDelay);
            damageAudio.PlayDelayed(damageDelay-0.1f);
            Trigger = false;
        }
    }
    
    void Damage()
    {
        GameObject iconClone = Instantiate(DamageImage, Panel.transform);
        iconClone.GetComponent<RectTransform>().anchoredPosition = new Vector2(Random.Range(-100f, 100f), Random.Range(-100f, 100f));
        iconClone.GetComponent<RectTransform>().rotation = Panel.transform.rotation;
        iconClone.GetComponent<RectTransform>().localEulerAngles = new Vector3(0, 0, Random.Range(-60f, 60f));
        iconClone.GetComponent<RectTransform>().localScale = Vector3.one * Random.Range(1.3f, 1.8f);
        Destroy(iconClone, 1.5f);
    }
}
