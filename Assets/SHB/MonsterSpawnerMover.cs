using System.Collections;
using UnityEngine;

public class MonsterSpawnerMover : MonoBehaviour
{
    public UseThis useThisScript;  // 인스펙터에서 UseThis가 붙은 오브젝트 Drag&Drop
    public GameObject monsterSpawner;
    public GameObject monsterSpawnerInstance;
    void Start()
    {
        StartCoroutine(MoveWhenGameStarts());
    }

    IEnumerator MoveWhenGameStarts()
    {
        while (true)
        {
            if (useThisScript != null && useThisScript.isGameStart)
            {
                if (monsterSpawnerInstance == null) monsterSpawnerInstance = Instantiate(monsterSpawner, Vector3.zero, Quaternion.identity);

                Debug.Log("move spawner!");

                Vector3[] points = useThisScript.spawnerPoint;

                if (points != null && points.Length > 0)
                {
                    int randomIndex = Random.Range(0, points.Length);
                    Vector3 targetPos = points[randomIndex];

                    Debug.Log($"Moving to: {targetPos}");
                    monsterSpawnerInstance.transform.position = targetPos;
                }
                else
                {
                    Debug.LogWarning("Spawner point array is empty or null.");
                }
            }

            yield return new WaitForSeconds(1f);
        }
    }

}
