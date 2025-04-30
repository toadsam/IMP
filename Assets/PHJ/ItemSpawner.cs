using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImageManager))]
public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private ARTrackedImageManager trackedImageManager;
    [SerializeField] private GameObject flowerPrefab;
    [SerializeField] private GameObject sosPrefab;
    [SerializeField] private GameObject anotherPrefab;

    private Player player;
    private Dictionary<string, GameObject> spawnedObjects = new Dictionary<string, GameObject>();
    private HashSet<string> collectedItems = new HashSet<string>();

    void Awake()
    {
        // 씬에서 Player 스크립트를 찾아 자동으로 할당
        player = Object.FindFirstObjectByType<Player>();
    }

    void OnEnable()
    {
        if (trackedImageManager != null)
            trackedImageManager.trackablesChanged.AddListener(OnTrackedImagesChanged);
    }

    void OnDisable()
    {
        if (trackedImageManager != null)
            trackedImageManager.trackablesChanged.RemoveListener(OnTrackedImagesChanged);
    }

    // AR Foundation 6.x용 이벤트 핸들러
    private void OnTrackedImagesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        foreach (var addedImage in eventArgs.added)
            HandleTrackedImage(addedImage);

        foreach (var updatedImage in eventArgs.updated)
            HandleTrackedImage(updatedImage);

        foreach (var removedImage in eventArgs.removed)
            HandleRemovedImage(removedImage.Value);
    }

    private void HandleTrackedImage(ARTrackedImage trackedImage)
    {
        // 추적이 중지된 경우, 해당 오브젝트 숨기기
        if (trackedImage.trackingState != TrackingState.Tracking)
        {
            if (spawnedObjects.TryGetValue(trackedImage.referenceImage.name, out var obj))
                obj.SetActive(false);
            return;
        }

        var key = trackedImage.referenceImage.name;
        if (collectedItems.Contains(key))
            return;

        // 이미지 이름별로 올바른 프리팹 생성/갱신
        switch (key)
        {
            case "Flower":
                SpawnOrUpdate(key, flowerPrefab, trackedImage.transform.position, trackedImage.transform.rotation);
                break;
            case "sos":
                SpawnOrUpdate(key, sosPrefab, trackedImage.transform.position, trackedImage.transform.rotation);
                break;
            case "AnotherImage":
                SpawnOrUpdate(key, anotherPrefab, trackedImage.transform.position, trackedImage.transform.rotation);
                break;
            default:
                Debug.LogWarning($"Unhandled tracked image: {key}");
                break;
        }
    }

    private void HandleRemovedImage(ARTrackedImage trackedImage)
    {
        var key = trackedImage.referenceImage.name;
        if (spawnedObjects.TryGetValue(key, out var obj))
        {
            Destroy(obj);
            spawnedObjects.Remove(key);
        }
    }

    private void SpawnOrUpdate(string key, GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (!spawnedObjects.ContainsKey(key))
            spawnedObjects[key] = Instantiate(prefab, position, rotation);

        var obj = spawnedObjects[key];
        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);
    }

    void Update()
    {
        if (Input.touchCount == 0) return;

        var touch = Input.GetTouch(0);
        if (touch.phase != TouchPhase.Began) return;

        Ray ray = Camera.main.ScreenPointToRay(touch.position);
        if (!Physics.Raycast(ray, out var hit)) return;

        foreach (var kvp in spawnedObjects)
        {
            if (hit.collider.gameObject == kvp.Value)
            {
                HandleObjectClick(kvp.Key);
                break;
            }
        }
    }

    private void HandleObjectClick(string imageName)
    {
        switch (imageName)
        {
            case "Flower":
                Debug.Log("Flower object clicked!");
                player?.AddHealth(10);
                Debug.Log("Player's health increased by 10.");
                collectedItems.Add("Flower");
                Destroy(spawnedObjects["Flower"]);
                spawnedObjects.Remove("Flower");
                break;

            case "sos":
                Debug.Log("SOS object clicked!");
                // SOS 전용 동작 추가 가능
                collectedItems.Add("sos");
                Destroy(spawnedObjects["sos"]);
                spawnedObjects.Remove("sos");
                break;

            case "AnotherImage":
                Debug.Log("AnotherImage object clicked!");
                // 다른 이미지 전용 동작 추가 가능
                collectedItems.Add("AnotherImage");
                Destroy(spawnedObjects["AnotherImage"]);
                spawnedObjects.Remove("AnotherImage");
                break;

            default:
                Debug.LogWarning($"Unhandled object clicked: {imageName}");
                break;
        }
    }
}
