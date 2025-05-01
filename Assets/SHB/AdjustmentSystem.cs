using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using UnityEngine.Events;

public class AdjustmentSystem : MonoBehaviour
{
    public ARPlaneManager planeManager;
    public GameObject floorPrefab; // 직사각형 바닥을 만들 프리팹 (ex: Plane)
    public GameObject wallPrefab;  // 벽을 만들 프리팹 (ex: Wall)

    public GameObject StartButtonController;
    public GameObject myCamera;

    public UnityEvent allSetEnd;

    public Vector3[] floorCornersFinal = new Vector3[4];

    public void HandleScanFinished()
    {
        Debug.Log("Start HandleScanFinished()");

        List<Vector3> allWorldPoints = new List<Vector3>();

        // 1. 바닥 평면들만 모으고, 각 boundary 경계점을 월드 좌표로 변환
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

        // 2. 좌표가 하나도 없다면 종료
        if (allWorldPoints.Count == 0)
        {
            Debug.LogWarning("No detection plane");
            return;
        }

        // 3. 최소/최대 X, Z값 계산
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

        // 추가: 카메라 좌표 포함
        Vector3 cameraPos = myCamera.transform.position;

        if (cameraPos.x + 1f > maxX) maxX = cameraPos.x + 1f;
        if (cameraPos.x - 1f < minX) minX = cameraPos.x - 1f;

        if (cameraPos.z + 1f > maxZ) maxZ = cameraPos.z + 1f;
        if (cameraPos.z - 1f < minZ) minZ = cameraPos.z - 1f;

        // 4. 중심과 크기 계산
        float centerX = (minX + maxX) / 2f;
        float centerZ = (minZ + maxZ) / 2f;
        float width = maxX - minX;
        float length = maxZ - minZ;

        Vector3 center = new Vector3(centerX, minY, centerZ);

        // 5. 바닥 GameObject 생성 (프리팹 활용)
        GameObject floor = Instantiate(floorPrefab, center, Quaternion.identity);

        // Plane은 기본적으로 10x10 단위라, 실제 크기에 맞게 스케일 조정 필요
        floor.transform.localScale = new Vector3(width / 10f, 1f, length / 10f);

        // 바닥의 각 꼭짓점 좌표 출력
        Vector3[] floorCorners = new Vector3[4];
        floorCorners[0] = new Vector3(minX, minY, minZ);  // 좌측 하단
        floorCorners[1] = new Vector3(maxX, minY, minZ);  // 우측 하단
        floorCorners[2] = new Vector3(maxX, minY, maxZ);  // 우측 상단
        floorCorners[3] = new Vector3(minX, minY, maxZ);  // 좌측 상단

        for (int i = 0; i < floorCorners.Length; i++)
        {
            Debug.Log($"Corner {i}: {floorCorners[i]}");
        }

        // 6. 벽 만들기
        CreateWall(floorCorners[0], floorCorners[1]);  // 바닥의 좌측 하단과 우측 하단
        CreateWall(floorCorners[1], floorCorners[2]);  // 바닥의 우측 하단과 우측 상단
        CreateWall(floorCorners[2], floorCorners[3]);  // 바닥의 우측 상단과 좌측 상단
        CreateWall(floorCorners[3], floorCorners[0]);  // 바닥의 좌측 상단과 좌측 하단

        Debug.Log($"End make plane. Center: {center}, size: {width} x {length}");

        //StartButtonController.GetComponent<StartButtonController>().destroyAllPlane();
        floorCornersFinal = floorCorners;

        sendAllSetEnd();
    }

    // 벽을 생성하는 함수
    void CreateWall(Vector3 start, Vector3 end)
    {
        // 벽의 크기와 위치 계산
        Vector3 direction = end - start;
        float distance = direction.magnitude;
        Vector3 midPoint = (start + end) / 2;

        // 벽 프리팹 인스턴스화
        GameObject wall = Instantiate(wallPrefab, midPoint, Quaternion.identity);

        // 벽의 크기 설정
        wall.transform.localScale = new Vector3(0.01f, 1f, distance); // 벽 높이는 1, 길이는 start와 end 사이의 거리

        // 벽의 Y 위치 설정 (바닥 위에 정확히 배치)
        wall.transform.position = new Vector3(midPoint.x, Mathf.Min(start.y, end.y) + wall.transform.localScale.y / 2f, midPoint.z);

        // 벽이 시작점에서 끝점을 향하도록 회전 (y축 회전만 적용)
        Vector3 directionToLook = end - start;
        directionToLook.y = 0;  // y축 회전만 고려
        if (directionToLook != Vector3.zero)  // 방향이 0벡터가 아닐 때만 회전
        {
            wall.transform.rotation = Quaternion.LookRotation(directionToLook);
        }
    }


    public void sendAllSetEnd()
    {
        Debug.Log("All setting End!!!!!");
        allSetEnd?.Invoke();
    }
}