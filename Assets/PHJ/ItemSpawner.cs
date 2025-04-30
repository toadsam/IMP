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
    private GameObject selectedObject; // Dragged Object
    private Vector3 dragOffset; // Offset for dragging
    private Plane dragPlane; // Plane for dragging
    public int health = 100; // Player's Health
    private bool isDragging = false; // 드래그 상태를 추적하는 변수

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
            var trackedKey = trackedImage.referenceImage.name;

            if (trackedKey == "sos") {
                return;
            }
            if (spawnedObjects.TryGetValue(trackedImage.referenceImage.name, out var obj))
                obj.SetActive(false);
            return;
        }

        var key = trackedImage.referenceImage.name;
        if (collectedItems.Contains(key))
            return;

        // 드래그 중이거나 드래그가 완료된 상태에서는 위치를 갱신하지 않음
        if (key == "sos" && isDragging)
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
            if (key == "sos")
            {
                // Sos Prefab은 7초 뒤에만 삭제
                StartCoroutine(DestroyAfterDelay(obj, 7f));
            }
            else
            {
                // 다른 프리팹은 즉시 삭제
                Destroy(obj);
                spawnedObjects.Remove(key);
            }
        }
    }

    private IEnumerator DestroyAfterDelay(GameObject obj, float delay)
{
    yield return new WaitForSeconds(delay);

    // 오브젝트가 여전히 존재하고 활성화되어 있다면 삭제
    if (obj != null)
    {
        Destroy(obj);
        Debug.Log($"Sos Prefab has been destroyed after {delay} seconds.");
    }
}

    private void SpawnOrUpdate(string key, GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (!spawnedObjects.ContainsKey(key))
            spawnedObjects[key] = Instantiate(prefab, position, Quaternion.Euler(0, 90f, 0));

        var obj = spawnedObjects[key];
        obj.transform.SetPositionAndRotation(position, Quaternion.Euler(0, 90f, 0));
        obj.SetActive(true);
    }

    void Update()
    {
        if (Input.touchCount == 0) return;

        var touch = Input.GetTouch(0);

        // 터치 시작 시 오브젝트 선택
        if (touch.phase == TouchPhase.Began)
        {
            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                foreach (var kvp in spawnedObjects)
                {
                    if (hit.collider.gameObject == kvp.Value)
                    {
                        // 'sos' 키에 해당하는 오브젝트만 드래그 가능
                        if (kvp.Key == "sos" && hit.collider.gameObject == kvp.Value)
                        {
                            selectedObject = kvp.Value; // Sos Prefab 저장
                            dragPlane = new Plane(Vector3.up, selectedObject.transform.position);
                            dragOffset = selectedObject.transform.position - hit.point;
                            isDragging = true; // 드래그 시작
                            HandleObjectClick(kvp.Key);
                            break;
                        }

                        // 다른 오브젝트는 클릭 동작 처리
                        HandleObjectClick(kvp.Key);
                        break;
                    }
                }
            }
        }

        // 터치 이동 시 오브젝트 드래그
        if (touch.phase == TouchPhase.Moved && selectedObject != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            if (dragPlane.Raycast(ray, out float enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter); // 평면과의 교차점
                selectedObject.transform.position = hitPoint + dragOffset; // 오프셋 유지하며 이동
            }
        }

        // 터치 종료 시 선택 해제 (오브젝트는 사라지지 않음)
        if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            selectedObject = null; // 선택 해제
            //isDragging = false; // 드래그 종료
        }
    }

    private void HandleObjectClick(string imageName)
    {
        switch (imageName)
        {
            case "Flower":
                Debug.Log("Flower object clicked!");
                player?.AddHealth(health);
                Debug.Log("Player's health increased by 10.");
                collectedItems.Add("Flower");
                Destroy(spawnedObjects["Flower"]);
                spawnedObjects.Remove("Flower");
                break;

            case "sos":
                Debug.Log("SOS object clicked!");
                // SOS 전용 동작 추가 가능
                //collectedItems.Add("sos");
                //Destroy(spawnedObjects["sos"]);
                //spawnedObjects.Remove("sos");
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
