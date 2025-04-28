using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private ARTrackedImageManager trackedImageManager;
    [SerializeField] private GameObject flowerPrefab;

    private Player player;
    private GameObject spawnedFlower;
    private bool flowerCollected;

    void Awake()
    {
        player = FindFirstObjectByType<Player>();        
    }

    void OnEnable()
    {
        trackedImageManager.trackablesChanged.AddListener(OnImageChanged);
    }

    void OnDisable()
    {
        trackedImageManager.trackablesChanged.RemoveListener(OnImageChanged);
    }

    private void OnImageChanged(ARTrackablesChangedEventArgs<ARTrackedImage> args)
    {
        foreach (var img in args.added)
        {
            HandleFlower(img);
        }

        foreach (var img in args.updated)
        {
            HandleFlower(img);
        }

        foreach (var img in args.removed)
        {
            if (spawnedFlower != null)
            {
                Destroy(spawnedFlower);
                spawnedFlower = null;
                flowerCollected = false;
            }
        }
    }

    private void HandleFlower(ARTrackedImage img)
    {
        if (img.referenceImage.name != "Flower" || flowerCollected)
            return;

        if (img.trackingState == TrackingState.Tracking)
        {
            if (spawnedFlower == null)
            {
                spawnedFlower = Instantiate(flowerPrefab, img.transform.position, Quaternion.identity);
            }
            spawnedFlower.transform.position = img.transform.position;
            spawnedFlower.SetActive(true);
        }
        else if (spawnedFlower != null)
        {
            spawnedFlower.SetActive(false);
        }
    }

    void Update()
    {        
        if (spawnedFlower == null || Input.touchCount == 0) return;

        var touch = Input.GetTouch(0);
        if (touch.phase != TouchPhase.Began) return;

        Ray ray = Camera.main.ScreenPointToRay(touch.position);
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.gameObject == spawnedFlower)
        {
            Debug.Log("Raycast hit: " + hit.collider.gameObject.name);
            if (player != null)
            {
                player.AddHealth(10);
                print("Item collected: +10 health");
                Debug.Log("Player's remaining health: " + player.Health);
                flowerCollected = true;
                Destroy(spawnedFlower);
                spawnedFlower = null;
            }
            else
            {
                Debug.LogError("Player is null!");
            }
        }
    }
}
