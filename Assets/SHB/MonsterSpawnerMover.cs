using System.Collections;
using UnityEngine;

public class MonsterSpawnerMover : MonoBehaviour
{
    public UseThis useThisScript;  // 인스펙터에서 UseThis가 붙은 오브젝트 Drag&Drop
    public GameObject monsterSpawner;

    void Start()
    {
        monsterSpawner = Instantiate(monsterSpawner, Vector3.zero, Quaternion.identity);
        StartCoroutine(MoveWhenGameStarts());
    }

    IEnumerator MoveWhenGameStarts()
    {
        while (true)
        {
            if (useThisScript != null && useThisScript.isGameStart)
            {
                Debug.Log("move spawner!");

                Vector3[] points = useThisScript.spawnerPoint;

                if (points != null && points.Length > 0)
                {
                    int randomIndex = Random.Range(0, points.Length);
                    Vector3 targetPos = points[randomIndex];

                    Debug.Log($"Moving to: {targetPos}");
                    monsterSpawner.transform.position = targetPos;
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
