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

    public float spawnInterval = 3f;

    // 몬스터 드래그해서 던지는 코드를 위한 변수
    //private Enemy3 draggedEnemy;
    //private Vector3 dragStartPos;
    //private float dragStartTime;

    void Start()
    {
        enemy1Spawned = true;
        StartCoroutine(SpawnEnemy1());
    }

    void Update()
    {
        /*if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.TryGetComponent<Enemy3>(out var enemy))
                {
                    draggedEnemy = enemy;
                    draggedEnemy.state = Enemy3State.Dragged;

                    draggedEnemy.isBeingDragged = true;

                    // 코루틴 정지
                    if (draggedEnemy.cloakingCoroutine != null)
                    {
                        StopCoroutine(draggedEnemy.cloakingCoroutine);
                        draggedEnemy.cloakingCoroutine = null;
                    }

                    dragStartPos = Input.mousePosition;
                    dragStartTime = Time.time;
                }
            }
        }
        // 드래그 중 위치 업데이트
        else if (Input.GetMouseButton(0) && draggedEnemy != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                draggedEnemy.dragTargetPos = hit.point;
            }
        }
        // 드래그 종료 → 휘두르면 날림
        else if (Input.GetMouseButtonUp(0) && draggedEnemy != null)
        {
            Vector3 dragEndPos = Input.mousePosition;
            float dragDistance = Vector3.Distance(dragEndPos, dragStartPos);
            float dragTime = Time.time - dragStartTime;
            float dragSpeed = dragDistance / dragTime;

            Debug.Log($"DragDistance: {dragDistance}, DragSpeed: {dragSpeed}");

            if (dragDistance > 100f && dragSpeed > 500f)
            {
                // 드래그 끝나는 위치의 월드 방향 구하기
                Ray ray = Camera.main.ScreenPointToRay(dragEndPos);
                Vector3 worldDir = ray.direction;
                worldDir.y = 0; // 수평 방향으로만
                worldDir.Normalize();
                draggedEnemy.Throw(worldDir);
            }
            else
            {
                draggedEnemy.state = Enemy3State.Normal;
            }

            draggedEnemy = null;
        }*/
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
        spawnPos.y = player.position.y;
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
            //Coroutine cloaking = StartCoroutine(CloakingEnemy(monster));
            //enemy3.cloakingCoroutine = cloaking;
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
            //enemy3Spawned = false;
            boss3Spawned = true;
            StartCoroutine(SpawnBoss3());
            slainedMoster = 0;
        }
    }

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

    public void OnBoss3Slained()
    {
        Debug.Log("You Slained Boss3");

        boss3Spawned = false;
        enemy3Spawned = false;
        slainedMoster = 0;
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
