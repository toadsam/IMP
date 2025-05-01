using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using UnityEngine.Events;

public class StartButtonController : MonoBehaviour
{
    public ARPlaneManager planeManager;
    public GameObject scanInstructionUI; // "환경 스캔 중" UI
    public UnityEvent OnScanFinished;
    public ARSession arSession;

    private bool isScanning = false;  // 스캔 중 상태

    void Start()
    {
        scanInstructionUI.SetActive(false);
        if (planeManager != null) planeManager.enabled = false;
    }

    public void StartScanRoutine()
    {
        if (!isScanning)
        {
            StartScan();
        }
        else
        {
            StopScan();
        }
    }

    private void StartScan()
    {
        if (planeManager != null)
        {
            planeManager.enabled = true;

            foreach (var plane in planeManager.trackables)
            {
                plane.gameObject.SetActive(true);
            }
        }

        destroyAllPlane();
        GameObject floor = GameObject.FindWithTag("floor");
        GameObject[] wall = GameObject.FindGameObjectsWithTag("wall");
        GameObject[] spawner = GameObject.FindGameObjectsWithTag("spawner");

        if(floor != null) Destroy(floor);
        if(wall != null){
            foreach(var walls in wall){
                Destroy(walls);
            }
        }

        if(spawner != null){
            foreach(var spawners in spawner){
                Destroy(spawners);
            }
        }

        Debug.Log("Scan started");
        isScanning = true;

        if (scanInstructionUI != null) scanInstructionUI.SetActive(true);
    }

    private void StopScan()
    {
        Debug.Log("Scan stopped & locked");
        isScanning = false;

        // 필요한 처리 호출 (ex. 평면 고정, 바닥 및 벽 생성 등)
        FinishScan();

        if (scanInstructionUI != null)
            scanInstructionUI.SetActive(false);

        if (planeManager != null)
        {
            foreach (var plane in planeManager.trackables)
            {
                plane.gameObject.SetActive(false);
            }
            planeManager.enabled = false;
        }
    }

    public void destroyAllPlane()
    {
        arSession.Reset();
    }

    public void FinishScan()
    {
        Debug.Log("FinishScan() start");
        OnScanFinished?.Invoke();
    }
}
