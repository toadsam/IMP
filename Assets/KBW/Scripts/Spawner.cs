using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform player;
    public GameObject enemy1Prefab;
    public GameObject boss1Prefab;
    public GameObject enemy2Prefab;
    public GameObject boss2Prefab;
    public GameObject enemy3Prefab;
    public GameObject boss3Prefab;

    public int counterBoss = 10;
    private int slainedMoster = 0;

    private bool enemy1Spawned = false;
    private bool enemy2Spawned = false;
    private bool enemy3Spawned = false;

    private bool boss1Spawned = false;
    private bool boss2Spawned = false;
    private bool boss3Spawned = false;

    private bool isEnd = false;


    public float spawnInterval = 3f;

    public EndUI endUI;

   
    void Start()
    {
        enemy1Spawned = true;
        StartCoroutine(SpawnEnemy1());
        player = GameObject.Find("Main Camera").transform;
        if (endUI == null)
        {
            GameObject obj = GameObject.Find("EndUI"); // 오브젝트 이름 정확히 입력
            if (obj != null)
               endUI = obj.GetComponent<EndUI>();
            //obj.SetActive(false);
            
        }
    }

    void Update()
    {
        IsEnd();
    }
    void IsEnd() 
    {
        if (isEnd) 
        {
            endUI.WinEnd();
        }
    }

    IEnumerator SpawnEnemy1()
    {
        while (enemy1Spawned)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnMonster(enemy1Prefab);            
        }
    }

    IEnumerator SpawnEnemy2()
    {
        while (enemy2Spawned)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnMonster(enemy2Prefab);            
        }
    }

    IEnumerator SpawnEnemy3()
    {
        while (enemy3Spawned)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnMonster(enemy3Prefab);            
        }
    }

    void SpawnMonster(GameObject prefab)
    {
        Vector3 spawnPos = transform.position;  
        GameObject monster = Instantiate(prefab, spawnPos, Quaternion.identity);
        if (monster.TryGetComponent<Enemy1>(out var enemy1))
        {
            enemy1.Init(player, this);
        }

        if (monster.TryGetComponent<Boss1>(out var boss1))
        {
            boss1.Init(player, this);
        }

        if (monster.TryGetComponent<Enemy2>(out var enemy2))
        {
            enemy2.Init(player, this);
        }

        if (monster.TryGetComponent<Boss2>(out var boss2))
        {
            boss2.Init(player, this);
        }
        
        if (monster.TryGetComponent<Enemy3>(out var enemy3))
        {
            enemy3.Init(player, this);
            StartCoroutine(CloakingEnemy(monster));
        }

        if (monster.TryGetComponent<Boss3>(out var boss3))
        {
            boss3.Init(player, this);
        }
    }

    public void OnEnemy1Slained()
    {
        slainedMoster ++;
        Debug.Log("Enemy1:" + slainedMoster);
        if (slainedMoster >= counterBoss && !boss1Spawned)
        {
            enemy1Spawned = false;
            boss1Spawned = true;
            StartCoroutine(SpawnBoss1());
            slainedMoster = 0;
        }
    }

    public void OnEnemy2Slained()
    {
        slainedMoster++;
        Debug.Log("Enemy2:" + slainedMoster);
        if (slainedMoster >= counterBoss && !boss2Spawned)
        {
            enemy2Spawned = false;
            boss2Spawned = true;
            StartCoroutine(SpawnBoss2());
            slainedMoster = 0;
        }
    }

    public void OnEnemy3Slained()
    {
        slainedMoster++;
        Debug.Log("Enemy3:" + slainedMoster);
        if (slainedMoster >= counterBoss && !boss3Spawned)
        {
            boss3Spawned = true;
            StartCoroutine(SpawnBoss3());
            slainedMoster = 0;
        }
    }

    public void OnBoss1Slained()
    {
        Debug.Log("You Slained Boss1");
        isEnd = true;
        if (!enemy2Spawned)
        {
            boss1Spawned = false;
            enemy2Spawned = true;
            StartCoroutine(SpawnEnemy2());
            slainedMoster = 0;
        }
    }

    public void OnBoss2Slained()
    {
        Debug.Log("You Slained Boss2");
        endUI.WinEnd();
        if (!enemy3Spawned)
        {
            boss2Spawned = false;
            enemy3Spawned = true;
            StartCoroutine(SpawnEnemy3());            
            slainedMoster = 0;
        }
    }

    public void OnBoss3Slained()
    {
        Debug.Log("You Slained Boss3");
        
        //게임 종료 기능 여기서 시작
        boss3Spawned = false;
        enemy3Spawned = false;
        slainedMoster = 0;

        endUI.WinEnd();
    }

    IEnumerator SpawnBoss1()
    {
        yield return new WaitForSeconds(5f);
        if(boss1Spawned)
        {
            Debug.Log("Warning1");
            SpawnMonster(boss1Prefab);
        }        
    }

    IEnumerator SpawnBoss2()
    {
        yield return new WaitForSeconds(5f);
        if (boss2Spawned)
        {
            Debug.Log("Warning2");
            SpawnMonster(boss2Prefab);
        }        
    }

    IEnumerator SpawnBoss3()
    {
        yield return new WaitForSeconds(5f);
        if(boss3Spawned)
        {
            Debug.Log("Warning3");
            SpawnMonster(boss3Prefab);
        }        
    }

    IEnumerator CloakingEnemy(GameObject cloakingEnemy)
    {
        while (cloakingEnemy != null)
        {
            yield return new WaitForSeconds(2f);
            cloakingEnemy.SetActive(false);
            yield return new WaitForSeconds(1f + Random.Range(-0.5f, 0.5f));
            cloakingEnemy.SetActive(true);
        }        
    }
}
