using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImageManager))]
public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private ARTrackedImageManager trackedImageManager; // AR image tracking manager
    [SerializeField] private GameObject flowerPrefab; // Flower item prefab
    [SerializeField] private GameObject sosPrefab; // SOS item prefab
    [SerializeField] private GameObject shieldPrefab; // Shield item prefab
    [SerializeField] private AudioSource heartAudioSource; // Audio source for heart sound
    [SerializeField] private AudioSource weaponSolved; // Audio source for weapon unlock sound

    private Player player; // Player script reference
    private Dictionary<string, GameObject> spawnedObjects = new Dictionary<string, GameObject>(); // Spawned objects dictionary
    private HashSet<string> collectedItems = new HashSet<string>(); // Collected items list
    private HashSet<string> processedImages = new HashSet<string>(); // Processed images list
    private GameObject selectedObject; // Currently dragged object
    private Vector3 dragOffset; // Drag offset
    private Plane dragPlane; // Drag plane
    public int health = 100; // Player health
    private bool isDragging = false; // Dragging state
    private bool isSosDragged = false; // SOS dragging state

    public WeaponShooterWithJoystick weaponshooterwithJoystick; // Weapon shooter controller
    public PlayerHealth playerHealth; // Player health controller

    // Called when the script is initialized
    void Awake()
    {
        player = Object.FindFirstObjectByType<Player>();
    }

    // Called when the object is enabled
    // Registers to AR tracked image event
    void OnEnable()
    {
        if (trackedImageManager != null)
            trackedImageManager.trackablesChanged.AddListener(OnTrackedImagesChanged);
    }

    // Called when the object is disabled
    // Unregisters from AR tracked image event
    void OnDisable()
    {
        if (trackedImageManager != null)
            trackedImageManager.trackablesChanged.RemoveListener(OnTrackedImagesChanged);
    }

    // Called when tracked images are added, updated, or removed
    void OnTrackedImagesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> eventArgs)
    {
        foreach (var addedImage in eventArgs.added)
            HandleTrackedImage(addedImage);

        foreach (var updatedImage in eventArgs.updated)
            HandleTrackedImage(updatedImage);

        foreach (var removedImage in eventArgs.removed)
            HandleRemovedImage(removedImage.Value);
    }

    // Handles logic when an image is tracked
    void HandleTrackedImage(ARTrackedImage trackedImage)
    {
        // If not tracked, disable object (except SOS and shield)
        if (trackedImage.trackingState != TrackingState.Tracking)
        {
            var trackedKey = trackedImage.referenceImage.name;

            if (trackedKey == "sos" || trackedKey == "shield")
                return;

            if (spawnedObjects.TryGetValue(trackedKey, out var obj))
                obj.SetActive(false);
            return;
        }

        var key = trackedImage.referenceImage.name;

        // Skip if already collected
        if (collectedItems.Contains(key))
            return;

        // Skip updating if dragging
        if ((key == "sos" && (isDragging || isSosDragged)) || (key == "shield" && isDragging))
            return;

        // Spawn or update based on item type
        switch (key)
        {
            case "Flower":
                SpawnOrUpdate(key, flowerPrefab, trackedImage.transform.position, trackedImage.transform.rotation);
                break;
            case "sos":
                SpawnOrUpdate(key, sosPrefab, trackedImage.transform.position, trackedImage.transform.rotation);
                break;
            case "magic":
                if (!processedImages.Contains(key))
                {
                    processedImages.Add(key);
                    weaponSolved?.Play();
                }
                weaponshooterwithJoystick.UnlockWeapon(1);
                break;
            case "sword":
                if (!processedImages.Contains(key))
                {
                    processedImages.Add(key);
                    weaponSolved?.Play();
                }
                weaponshooterwithJoystick.UnlockWeapon(2);
                break;
            case "gun":
                if (!processedImages.Contains(key))
                {
                    processedImages.Add(key);
                    weaponSolved?.Play();
                }
                weaponshooterwithJoystick.UnlockWeapon(3);
                break;
            case "shield":
                SpawnOrUpdate(key, shieldPrefab, trackedImage.transform.position, trackedImage.transform.rotation);
                break;
            default:
                Debug.LogWarning($"Unhandled tracked image: {key}");
                break;
        }
    }

    // Called when a tracked image is removed
    void HandleRemovedImage(ARTrackedImage trackedImage)
    {
        var key = trackedImage.referenceImage.name;

        if (spawnedObjects.TryGetValue(key, out var obj))
        {
            if (key == "sos")
            {
                StartCoroutine(DestroyAfterDelay(obj, 7f));
            }
            else
            {
                Destroy(obj);
                spawnedObjects.Remove(key);
            }
        }
    }

    // Coroutine to destroy object after delay
    private IEnumerator DestroyAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (obj != null)
        {
            Destroy(obj);
            Debug.Log($"SOS prefab destroyed after {delay} seconds.");
        }
    }

    // Spawns a new object or updates existing one
    private void SpawnOrUpdate(string key, GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (!spawnedObjects.ContainsKey(key))
            spawnedObjects[key] = Instantiate(prefab, position, Quaternion.Euler(0, 180, 0));

        var obj = spawnedObjects[key];
        obj.transform.SetPositionAndRotation(position, Quaternion.Euler(0, 180, 0));
        obj.SetActive(true);
    }

    // Called every frame to handle touch input and dragging
    void Update()
    {
        if (Input.touchCount == 0) return;

        var touch = Input.GetTouch(0);

        // On touch begin - select object
        if (touch.phase == TouchPhase.Began)
        {
            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                foreach (var kvp in spawnedObjects)
                {
                    if (hit.collider.gameObject == kvp.Value)
                    {
                        // Only SOS and shield can be dragged
                        if (kvp.Key == "sos" || kvp.Key == "shield")
                        {
                            selectedObject = kvp.Value;
                            float fixedZ = selectedObject.transform.position.z;
                            dragPlane = new Plane(Vector3.forward, new Vector3(0, 0, fixedZ));
                            dragOffset = selectedObject.transform.position - hit.point;
                            isDragging = true;
                            if (kvp.Key == "sos")
                                isSosDragged = true;
                            HandleObjectClick(kvp.Key);
                            break;
                        }

                        HandleObjectClick(kvp.Key);
                        break;
                    }
                }
            }
        }

        // On touch move - drag object
        if (touch.phase == TouchPhase.Moved && selectedObject != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            if (dragPlane.Raycast(ray, out float enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                selectedObject.transform.position = hitPoint + dragOffset;
            }
        }

        // On touch end - release object
        if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            selectedObject = null;
            isDragging = false;
        }
    }

    // Handles object click actions
    private void HandleObjectClick(string imageName)
    {
        switch (imageName)
        {
            case "Flower":
                Debug.Log("Flower object clicked!");
                playerHealth.Heal(50);
                collectedItems.Add("Flower");
                if (heartAudioSource != null)
                {
                    weaponshooterwithJoystick.UnlockWeapon(3);
                    heartAudioSource.Play();
                }
                else
                {
                    Debug.LogWarning("Heart AudioSource not assigned.");
                }
                Destroy(spawnedObjects["Flower"]);
                spawnedObjects.Remove("Flower");
                break;

            case "sos":
                Debug.Log("SOS object clicked!");
                // Additional SOS behavior here
                break;

            case "shield":
                Debug.Log("Shield object clicked!");
                // Additional shield behavior here
                break;

            default:
                Debug.LogWarning($"Unhandled object clicked: {imageName}");
                break;
        }
    }
}
