using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UseThis : MonoBehaviour
{
    public string pleaseDontTouch = "Do NOT assign anything here manually!";
    public GameObject floor;                     // The generated floor object
    public Vector3[] floorCorners;               // Four corners of the generated floor

    public GameObject[] wall;                    // Array of generated wall objects
    public Vector3[] spawnerPoint;               // Points where the monster spawner can move

    public bool isGameStart = false;             // True when the game starts
    public GameObject myCamera;                  // Reference to the main camera (player view)
    public GameObject markerPrefab;              // Marker prefab used for visualizing spawn points

    public void getInfo()
    {
        // Find and assign the floor object using its tag
        floor = GameObject.FindWithTag("floor");
        if (floor == null) Debug.Log("in UseThis, can't find floor");

        // Get the final floor corner positions from AdjustmentSystem
        floorCorners = GameObject.Find("AdjustmentSystem").GetComponent<AdjustmentSystem>().floorCornersFinal;

        // Find and assign all wall objects using their tag
        wall = GameObject.FindGameObjectsWithTag("wall");
        if (wall == null) Debug.Log("in UseThis, can't find wall");

        // Calculate width and length based on corners
        float width = floorCorners[2].x - floorCorners[0].x;
        float length = floorCorners[2].z - floorCorners[0].z;

        // Call function to calculate and store spawner points
        setSpawnerPoint(floor.transform, width, length);
    }

    public void setSpawnerPoint(Transform floorTransform, float width, float length)
    {
        // Get the forward direction from the camera (horizontal only)
        Vector3 floorForward = myCamera.transform.forward;
        floorForward.y = 0;
        floorForward.Normalize();

        // Step 1: Find which edge of the floor is most facing the camera
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

        // Step 2: Place 6 markers along the front wall (slightly inset from ends)
        Vector3 fa = floorCorners[frontEdge];
        Vector3 fb = floorCorners[(frontEdge + 1) % 4];
        Vector3 edgeDir = (fb - fa).normalized;
        float fullLength = Vector3.Distance(fa, fb);
        float spacing = fullLength / 5f;

        float sideTrim = spacing * 0.5f; // Offset from both ends
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

            inwardPos.y += 1f; // Raise slightly above the floor
            createdMarkers.Add(inwardPos);
            spawnList.Add(inwardPos);
            // Instantiate(markerPrefab, inwardPos, Quaternion.identity);
        }

        // Step 3: Add 1 marker on each side wall, slightly pulled toward center
        float sideOffset = spacing * 2f;

        // Left side
        int leftA = (frontEdge + 3) % 4;
        int leftB = frontEdge;
        Vector3 leftDir = (floorCorners[leftA] - floorCorners[leftB]).normalized;
        Vector3 leftPos = createdMarkers[0] + leftDir * sideOffset;
        Vector3 centerDirLeft = (floor.transform.position - leftPos).normalized;
        leftPos += centerDirLeft * spacing * 0.2f;
        leftPos.y += 1f;
        spawnList.Add(leftPos);
        // Instantiate(markerPrefab, leftPos, Quaternion.identity);

        // Right side
        int rightA = (frontEdge + 2) % 4;
        int rightB = (frontEdge + 1) % 4;
        Vector3 rightDir = (floorCorners[rightA] - floorCorners[rightB]).normalized;
        Vector3 rightPos = createdMarkers[5] + rightDir * sideOffset;
        Vector3 centerDirRight = (floor.transform.position - rightPos).normalized;
        rightPos += centerDirRight * spacing * 0.2f;
        rightPos.y += 1f;
        spawnList.Add(rightPos);
        // Instantiate(markerPrefab, rightPos, Quaternion.identity);

        // Final result assigned to public array
        spawnerPoint = spawnList.ToArray();
    }
}
