using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // Reference to the player (in this case, the camera)
    public Transform player;

    // Prefabs for enemies and bosses
    public GameObject enemy1Prefab;
    public GameObject boss1Prefab;
    public GameObject enemy2Prefab;
    public GameObject boss2Prefab;
    public GameObject enemy3Prefab;
    public GameObject boss3Prefab;

    // Number of enemies to defeat before boss appears
    public int counterBoss = 10;

    // Internal kill counter
    private int slainedMoster = 0;

    // Spawn state flags for each enemy/boss
    private bool enemy1Spawned = false;
    private bool enemy2Spawned = false;
    private bool enemy3Spawned = false;

    private bool boss1Spawned = false;
    private bool boss2Spawned = false;
    private bool boss3Spawned = false;

    // Flags for game end conditions
    private bool isEnd = false;
    private bool isLoss = false;

    // Time between each spawn
    public float spawnInterval = 3f;

    // UI component for handling win/loss screens
    public EndUI endUI;

    void Start()
    {
        // Start by spawning Enemy1
        enemy1Spawned = true;
        StartCoroutine(SpawnEnemy1());

        // Locate the player (camera)
        player = GameObject.Find("Main Camera").transform;

        // Initialize EndUI if not assigned
        if (endUI == null)
        {
            GameObject obj = GameObject.Find("EndUI");
            if (obj != null)
                endUI = obj.GetComponent<EndUI>();
        }
    }

    void Update()
    {
        IsEnd();   // Check for win
        IsLoss();  // Check for loss
    }

    void IsEnd()
    {
        if (isEnd)
        {
            endUI.WinEnd();  // Show win screen
        }
    }

    void IsLoss()
    {
        if (isLoss)
        {
            endUI.LossEnd();  // Show loss screen
        }
    }

    // Coroutine to continuously spawn Enemy1
    IEnumerator SpawnEnemy1()
    {
        while (enemy1Spawned)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnMonster(enemy1Prefab);
        }
    }

    // Coroutine to continuously spawn Enemy2
    IEnumerator SpawnEnemy2()
    {
        while (enemy2Spawned)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnMonster(enemy2Prefab);
        }
    }

    // Coroutine to continuously spawn Enemy3
    IEnumerator SpawnEnemy3()
    {
        while (enemy3Spawned)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnMonster(enemy3Prefab);
        }
    }

    // Handles the actual monster instantiation and initialization
    void SpawnMonster(GameObject prefab)
    {
        Vector3 spawnPos = transform.position;
        GameObject monster = Instantiate(prefab, spawnPos, Quaternion.identity);

        // Initialize respective components if present
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
            StartCoroutine(CloakingEnemy(monster));  // Enable cloaking behavior
        }

        if (monster.TryGetComponent<Boss3>(out var boss3))
        {
            boss3.Init(player, this);
        }
    }

    // Called when Enemy1 is defeated
    public void OnEnemy1Slained()
    {
        slainedMoster++;
        Debug.Log("Enemy1:" + slainedMoster);
        if (slainedMoster >= counterBoss && !boss1Spawned)
        {
            enemy1Spawned = false;
            boss1Spawned = true;
            StartCoroutine(SpawnBoss1());
            slainedMoster = 0;
        }
    }

    // Called when Enemy2 is defeated
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

    // Called when Enemy3 is defeated
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

    // Called when Boss1 is defeated
    public void OnBoss1Slained()
    {
        Debug.Log("You Slained Boss1");
        if (!enemy2Spawned)
        {
            boss1Spawned = false;
            enemy2Spawned = true;
            StartCoroutine(SpawnEnemy2());
            slainedMoster = 0;
        }
    }

    // Called when Boss2 is defeated
    public void OnBoss2Slained()
    {
        Debug.Log("You Slained Boss2");
        if (!enemy3Spawned)
        {
            boss2Spawned = false;
            enemy3Spawned = true;
            StartCoroutine(SpawnEnemy3());
            slainedMoster = 0;
        }
    }

    // Called when Boss3 is defeated (end of the game)
    public void OnBoss3Slained()
    {
        Debug.Log("You Slained Boss3");

        boss3Spawned = false;
        enemy3Spawned = false;
        slainedMoster = 0;

        isEnd = true;  // Trigger win state
    }

    // Coroutine to spawn Boss1 with delay
    IEnumerator SpawnBoss1()
    {
        yield return new WaitForSeconds(5f);
        if (boss1Spawned)
        {
            Debug.Log("Warning1");
            SpawnMonster(boss1Prefab);
        }
    }

    // Coroutine to spawn Boss2 with delay
    IEnumerator SpawnBoss2()
    {
        yield return new WaitForSeconds(5f);
        if (boss2Spawned)
        {
            Debug.Log("Warning2");
            SpawnMonster(boss2Prefab);
        }
    }

    // Coroutine to spawn Boss3 with delay
    IEnumerator SpawnBoss3()
    {
        yield return new WaitForSeconds(5f);
        if (boss3Spawned)
        {
            Debug.Log("Warning3");
            SpawnMonster(boss3Prefab);
        }
    }

    // Cloaking behavior: turn invisible and visible at intervals
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
