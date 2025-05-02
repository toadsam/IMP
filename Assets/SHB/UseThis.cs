using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UseThis : MonoBehaviour
{
    public string pleaseDontTouch = "public에 뭐 집어넣으면 안 돼!!";
    public GameObject floor;
    public Vector3[] floorCorners;

    public GameObject[] wall;
    public Vector3[] spawnerPoint;

    public Boolean isGameStart = false;
    public GameObject myCamera;
    public GameObject markerPrefab;
    public void getInfo()
    {
        floor = GameObject.FindWithTag("floor");
        if (floor == null) Debug.Log("in UseThis, can't find floor");

        floorCorners = GameObject.Find("AdjustmentSystem").GetComponent<AdjustmentSystem>().floorCornersFinal;
        Debug.Log(floorCorners[0]);  //좌측 하단
        Debug.Log(floorCorners[1]);  //우측 하단
        Debug.Log(floorCorners[2]);  //우측 상단
        Debug.Log(floorCorners[3]);  //좌측 하단

        wall = GameObject.FindGameObjectsWithTag("wall");
        if (wall == null) Debug.Log("in UseThis, can't find wall");

        float width = floorCorners[2].x - floorCorners[0].x;
        float length = floorCorners[2].z - floorCorners[0].z;
        setSpawnerPoint(floor.transform, width, length);
    }

    public void setSpawnerPoint(Transform floorTransform, float width, float length)
    {
        Vector3 floorForward = myCamera.transform.forward;
        floorForward.y = 0;
        floorForward.Normalize();

        int frontEdge = -1;
        float maxDot = float.MinValue;

        // Step 1: 정면 벽 탐색
        for (int i = 0; i < 4; i++)
        {
            Vector3 a = floorCorners[i];
            Vector3 b = floorCorners[(i + 1) % 4];
            Vector3 edgeCenter = (a + b) * 0.5f;
            Vector3 toEdge = (edgeCenter - floor.transform.position).normalized;

            float dot = Vector3.Dot(toEdge, floorForward);
            if (dot > maxDot)
            {
                maxDot = dot;
                frontEdge = i;
            }
        }

        int leftEdge = (frontEdge + 3) % 4;
        int rightEdge = (frontEdge + 1) % 4;

        List<Vector3> spawnList = new List<Vector3>();

        // Step 2: 카메라가 바라보는 벽의 좌우 끝점을 기준으로 6개 배치
        Vector3 fa = floorCorners[frontEdge];
        Vector3 fb = floorCorners[(frontEdge + 1) % 4];
        List<Vector3> createdMarkers = new List<Vector3>(); // 이미 생성된 마커들의 리스트

        for (int i = 0; i < 6; i++)
        {
            float t = i / 5.0f;
            Vector3 pos = Vector3.Lerp(fa, fb, t);
            createdMarkers.Add(pos);
            spawnList.Add(pos);
            Instantiate(markerPrefab, pos, Quaternion.identity);
        }

        // Step 3: 마커 간 간격 계산
        float markerSpacing = Vector3.Distance(createdMarkers[0], createdMarkers[1]);

        // 좌측 벽 방향
        int leftA = (frontEdge + 3) % 4;
        int leftB = frontEdge;
        Vector3 leftDir = (floorCorners[leftA] - floorCorners[leftB]).normalized;
        Vector3 leftPos = createdMarkers[0] + leftDir * markerSpacing;
        spawnList.Add(leftPos);
        Instantiate(markerPrefab, leftPos, Quaternion.identity);

        // 우측 벽 방향
        int rightA = (frontEdge + 2) % 4;
        int rightB = (frontEdge + 1) % 4;
        Vector3 rightDir = (floorCorners[rightA] - floorCorners[rightB]).normalized;
        Vector3 rightPos = createdMarkers[5] + rightDir * markerSpacing;
        spawnList.Add(rightPos);
        Instantiate(markerPrefab, rightPos, Quaternion.identity);

        spawnerPoint = spawnList.ToArray();
    }
}

// 예시: 임의의 위치로 이동
// int randIndex = Random.Range(0, 10);
// monsterSpawner.transform.position = spawnPositions[randIndex];