using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using UnityEngine.Events;

public class AdjustmentSystem : MonoBehaviour
{
    public ARPlaneManager planeManager;
    public GameObject floorPrefab; // Prefab to instantiate the rectangular floor
    public GameObject wallPrefab;  // Prefab to instantiate the surrounding walls

    public GameObject StartButtonController;
    public GameObject myCamera;

    public GameObject monsterSpawner;

    public UnityEvent allSetEnd;

    public Vector3[] floorCornersFinal = new Vector3[4]; // Stores final floor corners for reference

    public void HandleScanFinished()
    {
        Debug.Log("Start HandleScanFinished()");

        List<Vector3> allWorldPoints = new List<Vector3>();

        // 1. Collect only horizontal floor planes and convert boundary points to world space
        foreach (var plane in planeManager.trackables)
        {
            if (plane.alignment == PlaneAlignment.HorizontalUp)
            {
                foreach (var point in plane.boundary)
                {
                    Vector3 worldPoint = plane.transform.TransformPoint(new Vector3(point.x, 0f, point.y));
                    allWorldPoints.Add(worldPoint);
                }
            }

            plane.gameObject.SetActive(true);
        }

        // 2. If no detected points, exit early
        if (allWorldPoints.Count == 0)
        {
            Debug.LogWarning("No detection plane");
            return;
        }

        // 3. Find min/max X, Z, and min Y to define the rectangular floor boundary
        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minZ = float.MaxValue;
        float maxZ = float.MinValue;
        float minY = float.MaxValue;

        foreach (var point in allWorldPoints)
        {
            if (point.x < minX) minX = point.x;
            if (point.x > maxX) maxX = point.x;
            if (point.z < minZ) minZ = point.z;
            if (point.z > maxZ) maxZ = point.z;
            if (point.y < minY) minY = point.y;
        }

        // Include camera position to prevent floor from being too small
        Vector3 cameraPos = myCamera.transform.position;

        if (cameraPos.x + 1f > maxX) maxX = cameraPos.x + 1f;
        if (cameraPos.x - 1f < minX) minX = cameraPos.x - 1f;

        if (cameraPos.z + 1f > maxZ) maxZ = cameraPos.z + 1f;
        if (cameraPos.z - 1f < minZ) minZ = cameraPos.z - 1f;

        // 4. Calculate center and size of the rectangular floor
        float centerX = (minX + maxX) / 2f;
        float centerZ = (minZ + maxZ) / 2f;
        float width = maxX - minX;
        float length = maxZ - minZ;

        Vector3 center = new Vector3(centerX, minY, centerZ);

        // 5. Instantiate the floor using the prefab
        GameObject floor = Instantiate(floorPrefab, center, Quaternion.identity);

        // Scale the floor: default Plane in Unity is 10x10 units
        floor.transform.localScale = new Vector3(width / 10f, 1f, length / 10f);

        // Calculate the 4 corners of the floor
        Vector3[] floorCorners = new Vector3[4];
        floorCorners[0] = new Vector3(minX, minY, minZ);  // Bottom-left
        floorCorners[1] = new Vector3(maxX, minY, minZ);  // Bottom-right
        floorCorners[2] = new Vector3(maxX, minY, maxZ);  // Top-right
        floorCorners[3] = new Vector3(minX, minY, maxZ);  // Top-left

        for (int i = 0; i < floorCorners.Length; i++)
        {
            Debug.Log($"Corner {i}: {floorCorners[i]}");
        }

        // 6. Create 4 walls along the edges of the floor
        CreateWall(floorCorners[0], floorCorners[1]);  // Bottom edge
        CreateWall(floorCorners[1], floorCorners[2]);  // Right edge
        CreateWall(floorCorners[2], floorCorners[3]);  // Top edge
        CreateWall(floorCorners[3], floorCorners[0]);  // Left edge

        Debug.Log($"End make plane. Center: {center}, size: {width} x {length}");

        // Store the final corner positions for external use
        floorCornersFinal = floorCorners;

        // 7. Monster spawner instantiation is handled by UseThis.cs
        // SpawnMonsterSpawners(floor.transform, width, length);

        // Optional: Scale the floor 1.5x (as final touch)
        floor.transform.localScale *= 1.5f;

        sendAllSetEnd();
    }

    // Creates a vertical wall between two floor corner points
    void CreateWall(Vector3 start, Vector3 end)
    {
        Vector3 direction = end - start;
        float distance = direction.magnitude;
        Vector3 midPoint = (start + end) / 2;

        // Instantiate the wall prefab at midpoint
        GameObject wall = Instantiate(wallPrefab, midPoint, Quaternion.identity);

        // Set wall size: thin in X, fixed height in Y, and length in Z
        wall.transform.localScale = new Vector3(0.01f, 1f, distance);

        // Raise wall's Y so it's placed above floor
        wall.transform.position = new Vector3(midPoint.x, Mathf.Min(start.y, end.y) + wall.transform.localScale.y / 2f, midPoint.z);

        // Rotate wall to face the end point (horizontal rotation only)
        Vector3 directionToLook = end - start;
        directionToLook.y = 0;
        if (directionToLook != Vector3.zero)
        {
            wall.transform.rotation = Quaternion.LookRotation(directionToLook);
        }
    }

    public void sendAllSetEnd()
    {
        Debug.Log("All setting End!!!!!");
        allSetEnd?.Invoke(); // Notify others that setup is complete
    }
}
