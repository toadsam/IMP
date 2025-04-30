using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform player;
    public GameObject smallMonsterPrefab;
    public GameObject bossMonsterPrefab;

    public int spawnCountBeforeBoss = 10;
    private int smallMonstersKilled = 0;
    private bool bossSpawned = false;

    public float spawnInterval = 3f;

    void Start()
    {
        StartCoroutine(SpawnSmallMonsters());
    }

    IEnumerator SpawnSmallMonsters()
    {
        while (!bossSpawned)
        {
            SpawnMonster(smallMonsterPrefab);
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnMonster(GameObject prefab)
    {
        Vector3 spawnPos = transform.position + new Vector3(player.position.x + Random.Range(-0.1f,0.1f), player.position.y, player.position.z);  
        spawnPos.y = player.position.y;
        GameObject monster = Instantiate(prefab, spawnPos, Quaternion.identity);
        monster.GetComponent<Monster>().Init(player, this);
    }

    public void OnSmallMonsterKilled()
    {
        smallMonstersKilled++;
        Debug.Log("Enemy:" +  smallMonstersKilled);
        if (smallMonstersKilled >= spawnCountBeforeBoss && !bossSpawned)
        {
            bossSpawned = true;
            StartCoroutine(SpawnBoss());
        }
    }

    IEnumerator SpawnBoss()
    {
        yield return new WaitForSeconds(1.5f);
        Debug.Log("Warning");
        SpawnMonster(bossMonsterPrefab);
    }

}
