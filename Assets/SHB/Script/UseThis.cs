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

    public bool isGameStart = false;
    public GameObject myCamera;
    public GameObject markerPrefab;

    public void getInfo()
    {
        floor = GameObject.FindWithTag("floor");
        if (floor == null) Debug.Log("in UseThis, can't find floor");

        floorCorners = GameObject.Find("AdjustmentSystem").GetComponent<AdjustmentSystem>().floorCornersFinal;
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

        // Step 1: 정면 벽 탐색
        int frontEdge = -1;
        float maxDot = float.MinValue;

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

        List<Vector3> spawnList = new List<Vector3>();

        // Step 2: 정면 벽 기준 마커 6개 배치 (좌우 경계에서 약간 안쪽으로)
        Vector3 fa = floorCorners[frontEdge];
        Vector3 fb = floorCorners[(frontEdge + 1) % 4];
        Vector3 edgeDir = (fb - fa).normalized;
        float fullLength = Vector3.Distance(fa, fb);
        float spacing = fullLength / 5f;

        float sideTrim = spacing * 0.5f; // 좌우 가장자리 마커 안쪽으로 이동
        Vector3 faAdjusted = fa + edgeDir * sideTrim;
        Vector3 fbAdjusted = fb - edgeDir * sideTrim;

        Vector3 forwardDir = (fa + fb) * 0.5f - floor.transform.position;
        forwardDir.y = 0;
        forwardDir.Normalize();
        float inwardOffset = spacing * 0.2f;

        List<Vector3> createdMarkers = new List<Vector3>();

        for (int i = 0; i < 6; i++)
        {
            float t = i / 5.0f;
            Vector3 basePos = Vector3.Lerp(faAdjusted, fbAdjusted, t);
            Vector3 inwardPos = basePos - forwardDir * inwardOffset;

            inwardPos.y += 1f;
            createdMarkers.Add(inwardPos);
            spawnList.Add(inwardPos);
            // Instantiate(markerPrefab, inwardPos, Quaternion.identity);
        }

        // Step 3: 좌우 벽에 마커 하나씩 추가 (중앙 방향으로 살짝 당김)
        float sideOffset = spacing * 2f;

        // 좌측
        int leftA = (frontEdge + 3) % 4;
        int leftB = frontEdge;
        Vector3 leftDir = (floorCorners[leftA] - floorCorners[leftB]).normalized;
        Vector3 leftPos = createdMarkers[0] + leftDir * sideOffset;
        Vector3 centerDirLeft = (floor.transform.position - leftPos).normalized;
        leftPos += centerDirLeft * spacing * 0.2f;
        leftPos.y += 1f;
        spawnList.Add(leftPos);
        // Instantiate(markerPrefab, leftPos, Quaternion.identity);

        // 우측
        int rightA = (frontEdge + 2) % 4;
        int rightB = (frontEdge + 1) % 4;
        Vector3 rightDir = (floorCorners[rightA] - floorCorners[rightB]).normalized;
        Vector3 rightPos = createdMarkers[5] + rightDir * sideOffset;
        Vector3 centerDirRight = (floor.transform.position - rightPos).normalized;
        rightPos += centerDirRight * spacing * 0.2f;
        rightPos.y += 1f;
        spawnList.Add(rightPos);
        // Instantiate(markerPrefab, rightPos, Quaternion.identity);

        spawnerPoint = spawnList.ToArray();
    }
}
