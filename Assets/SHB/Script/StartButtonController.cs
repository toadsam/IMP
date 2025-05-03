using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using UnityEngine.Events;

public class StartButtonController : MonoBehaviour
{
    public ARPlaneManager planeManager;      // Handles detection and management of AR planes
    //public GameObject scanInstructionUI;   // UI that shows during scanning (currently unused)
    public UnityEvent OnScanFinished;        // Event triggered after scan is completed
    public ARSession arSession;              // Used to reset the AR session

    private bool isScanning = false;         // Keeps track of scanning state

    void Start()
    {
        // Initialize: make sure plane detection is off at start
        //scanInstructionUI.SetActive(false);
        if (planeManager != null) planeManager.enabled = false;
    }

    public void StartScanRoutine()
    {
        // Toggles scanning on or off based on current state
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
        // Activate plane detection
        if (planeManager != null)
        {
            planeManager.enabled = true;

            // Make existing planes visible again
            foreach (var plane in planeManager.trackables)
            {
                plane.gameObject.SetActive(true);
            }
        }

        // Clean up previously generated objects (floor, walls, spawners)
        destroyAllPlane();  // Reset AR session to clear old data

        GameObject floor = GameObject.FindWithTag("floor");
        GameObject[] wall = GameObject.FindGameObjectsWithTag("wall");
        GameObject[] spawner = GameObject.FindGameObjectsWithTag("spawner");

        if (floor != null) Destroy(floor);

        if (wall != null)
        {
            foreach (var walls in wall)
            {
                Destroy(walls);
            }
        }

        if (spawner != null)
        {
            foreach (var spawners in spawner)
            {
                Destroy(spawners);
            }
        }

        Debug.Log("Scan started");
        isScanning = true;

        //if (scanInstructionUI != null) scanInstructionUI.SetActive(true);
    }

    private void StopScan()
    {
        // Ends the scanning process and locks the environment
        Debug.Log("Scan stopped & locked");
        isScanning = false;

        FinishScan();  // Notify other components that scanning is done

        //if (scanInstructionUI != null)
        //    scanInstructionUI.SetActive(false);

        if (planeManager != null)
        {
            // Deactivate all tracked planes
            foreach (var plane in planeManager.trackables)
            {
                plane.gameObject.SetActive(false);
            }
            planeManager.enabled = false;
        }
    }

    public void destroyAllPlane()
    {
        // Reset the ARSession, which clears all plane data
        arSession.Reset();
    }

    public void FinishScan()
    {
        // Triggers UnityEvent that notifies other systems to build the game environment
        Debug.Log("FinishScan() start");
        OnScanFinished?.Invoke();
    }
}
