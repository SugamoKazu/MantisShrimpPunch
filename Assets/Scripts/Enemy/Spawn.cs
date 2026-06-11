using UnityEngine;

public class Spawn : MonoBehaviour
{
    [SerializeField, Tooltip("Spawn Point")] private GameObject[] spawnPoints;

    [SerializeField] private GameObject[] enemies;
    [SerializeField] private GameObject[] bossEnemies;


    [SerializeField] private int[] spawnPointNum = new int[15];
    private int i = 0;//, j = 0;

    //[SerializeField] GameObject Panel;

    private float interval = 0f, time = 10f;

    private bool gamePlaying, bossPlaying;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    // Update is called once per frame
    void Update()
    {
        UIManager uIManager;
        GameObject uIObj = GameObject.Find("UI");
        uIManager = uIObj.GetComponent<UIManager>();
        gamePlaying = uIManager.gamePlaying;
        bossPlaying = uIManager.bossPlaying;


        if (gamePlaying || bossPlaying)
        {
            time += Time.deltaTime;

            if (time > interval)
            {
                if (i < spawnPointNum.Length)
                {
                    int num = spawnPointNum[i];
                    SpawnEnemy(num);
                }

                time = 0f;

                interval = 3.8f;

                if (bossPlaying) interval = 3f;
                i++;
            }
            //if (i == 14) time = 0f;

        }
        else
        {
            i = 0;
            time = 10f;
        }
    }
    private void SpawnEnemy(int n)
    {
        int nowEnemy = i;


        GameObject obj;
        if (bossPlaying)
        {
            nowEnemy -= 15;
            obj = bossEnemies[nowEnemy];
        }
        else
        {
            if (enemies[i] == null) i--;
            obj = enemies[nowEnemy];
        }

        //Debug.Log("Spawn " + name + " at " + n);

        // name = "Tentacle";

        //name = "PlaCrab";

        // プレハブをGameObject型で取得
        //GameObject obj = (GameObject)Resources.Load(name);
        // プレハブを元に、インスタンスを生成、
        //Debug.Log(i + ": " + enemies[i] + " = " + obj);
        Vector3 SpawnPos = spawnPoints[n].transform.position;// + new Vector3(Random.Range(-20f, 20f), 0, Random.Range(-20f, 20f));
        GameObject enemyClone = Instantiate(obj, SpawnPos, Quaternion.identity);

        //GameObject dangerIcon = (GameObject)Resources.Load("DangerIcon");

        //GameObject iconClone = Instantiate(dangerIcon, Panel.transform);

        //GameObject target = enemyClone.transform.Find("target").gameObject;
        //iconClone.GetComponent<TargetNavigation>().targetTransform = target.transform;
        //iconClone.GetComponent<TargetNavigation>().targetClone = enemyClone;




    }
}
