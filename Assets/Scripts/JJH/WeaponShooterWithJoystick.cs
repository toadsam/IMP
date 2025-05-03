using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class WeaponShooterWithJoystick : MonoBehaviour
{
    // Data structure for skill configuration
    [System.Serializable]
    public class SkillData
    {
        public GameObject skillPrefab;
        public Transform spawnPoint;
        public float force = 1000f;
        public float lifeTime = 3f;
        public AudioClip skillSound;
        public string skillAnimation;
        public float cooldown = 3f;
        [HideInInspector] public float lastUsedTime = -Mathf.Infinity;
    }

    // Data structure for weapon configuration
    [System.Serializable]
    public class WeaponData
    {
        public GameObject weaponPrefab;
        public GameObject heldWeaponPrefab;
        public AudioClip fireSound;
        public float shootForce = 500f;
        public int damage = 10;
        public Sprite weaponIcon;
        public string fireAnimationName;
        public float fireRate = 0.5f;
        public float spawnDelay = 0f;
        public bool isUnlocked = false;
    }

    public List<WeaponData> weapons = new List<WeaponData>();
    private int currentWeaponIndex = 0;

    public Camera arCamera;
    public DynamicJoystick joystick;
    private float nextFire = 0f;

    public Transform weaponHoldPoint;
    private GameObject currentHeldWeapon;
    private List<GameObject> heldWeapons = new List<GameObject>();
    private bool weaponsSpawned = false;

    public Image weaponUIImage;
    public Transform upperBodyBone;
    public Vector3 extraRotationEuler;
    public Animator playerAnimator;
    public GameObject lockImage;
    public AudioSource audioSource;
    public List<SkillData> skills = new List<SkillData>();

    void Start()
    {
        Invoke(nameof(InitializeWeapons), 0.1f);

        foreach (var skill in skills)
        {
            skill.lastUsedTime = Time.time;
        }

        UnlockWeapon(0);
    }

    void InitializeWeapons()
    {
        SpawnAllHeldWeapons();
        EquipCurrentWeapon();
    }

    void Update()
    {
        WeaponData currentWeapon = weapons[currentWeaponIndex];

        if (Time.time > nextFire && (joystick.Horizontal != 0 || joystick.Vertical != 0))
        {
            nextFire = Time.time + currentWeapon.fireRate;
            StartCoroutine(ShootWeaponWithSpawnDelay(currentWeapon));
        }
    }

    IEnumerator ShootWeaponWithSpawnDelay(WeaponData currentWeapon)
    {
        if (!currentWeapon.isUnlocked)
        {
            Debug.Log("Weapon is locked.");
            yield break;
        }

        // Play animation and rotate upper body
        if (playerAnimator != null && !string.IsNullOrEmpty(currentWeapon.fireAnimationName))
        {
            playerAnimator.Play(currentWeapon.fireAnimationName);
            if (upperBodyBone != null)
                StartCoroutine(TemporarilyRotateUpperBody(extraRotationEuler, 0.15f));
        }

        if (currentWeapon.spawnDelay > 0f)
            yield return new WaitForSeconds(currentWeapon.spawnDelay);

        Vector3 direction = new Vector3(joystick.Horizontal, 0, joystick.Vertical).normalized;
        if (direction.magnitude < 0.1f)
            yield break;

        direction = arCamera.transform.TransformDirection(direction);
        direction.y = 0;

        GameObject spawnedWeapon = Instantiate(
            currentWeapon.weaponPrefab,
            arCamera.transform.position + arCamera.transform.forward * 0.5f,
            Quaternion.LookRotation(direction)
        );

        Rigidbody rb = spawnedWeapon.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = true;
            rb.AddForce(direction * currentWeapon.shootForce, ForceMode.Impulse);
        }

        if (audioSource != null && currentWeapon.fireSound != null)
        {
            audioSource.PlayOneShot(currentWeapon.fireSound);
        }

        Destroy(spawnedWeapon, 2f);
    }

    public void UnlockWeapon(int index)
    {
        if (index < 0 || index >= weapons.Count) return;

        weapons[index].isUnlocked = true;

        if (weapons[index].isUnlocked == false)
        {
            lockImage.SetActive(true);
        }

        Debug.Log($"Weapon {index} unlocked!");
    }

    void SpawnAllHeldWeapons()
    {
        heldWeapons.Clear();
        foreach (var weapon in weapons)
        {
            weapon.heldWeaponPrefab.SetActive(false);
            heldWeapons.Add(weapon.heldWeaponPrefab);
        }
        weaponsSpawned = true;
    }

    public void NextWeapon()
    {
        currentWeaponIndex = (currentWeaponIndex + 1) % weapons.Count;
        Debug.Log("Current weapon: " + weapons[currentWeaponIndex].weaponPrefab.name);
        EquipCurrentWeapon();
    }

    public void PreviousWeapon()
    {
        currentWeaponIndex--;
        if (currentWeaponIndex < 0)
        {
            currentWeaponIndex = weapons.Count - 1;
        }
        Debug.Log("Current weapon: " + weapons[currentWeaponIndex].weaponPrefab.name);
        EquipCurrentWeapon();
    }

    void UpdateWeaponUI()
    {
        if (weaponUIImage != null && weapons.Count > 0)
        {
            weaponUIImage.sprite = weapons[currentWeaponIndex].weaponIcon;
        }
    }

    void EquipCurrentWeapon()
    {
        if (!weaponsSpawned) return;

        foreach (var weaponObj in heldWeapons)
        {
            if (weaponObj != null)
                weaponObj.SetActive(false);
        }

        if (currentWeaponIndex >= 0 && currentWeaponIndex < heldWeapons.Count)
        {
            currentHeldWeapon = heldWeapons[currentWeaponIndex];
            if (currentHeldWeapon != null)
                currentHeldWeapon.SetActive(true);
        }

        WeaponData currentWeapon = weapons[currentWeaponIndex];
        if (lockImage != null)
            lockImage.SetActive(!currentWeapon.isUnlocked);

        UpdateWeaponUI();
    }

    void ShootWeapon()
    {
        if (weapons.Count == 0 || arCamera == null || joystick == null)
        {
            Debug.LogWarning("Missing components.");
            return;
        }

        WeaponData currentWeapon = weapons[currentWeaponIndex];

        if (currentWeapon.weaponPrefab == null)
        {
            Debug.LogWarning("No prefab assigned.");
            return;
        }

        if (playerAnimator != null && !string.IsNullOrEmpty(currentWeapon.fireAnimationName))
        {
            playerAnimator.Play(currentWeapon.fireAnimationName);

            if (upperBodyBone != null)
                StartCoroutine(TemporarilyRotateUpperBody(extraRotationEuler, 0.3f));
        }

        Vector3 direction = new Vector3(joystick.Horizontal, 0, joystick.Vertical).normalized;
        if (direction.magnitude < 0.1f)
            return;

        direction = arCamera.transform.TransformDirection(direction);
        direction.y = 0;

        GameObject spawnedWeapon = Instantiate(
            currentWeapon.weaponPrefab,
            arCamera.transform.position + arCamera.transform.forward * 0.5f,
            Quaternion.LookRotation(direction)
        );

        Rigidbody rb = spawnedWeapon.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = true;
            rb.AddForce(direction * currentWeapon.shootForce, ForceMode.Impulse);
        }

        if (audioSource != null && currentWeapon.fireSound != null)
        {
            audioSource.PlayOneShot(currentWeapon.fireSound);
        }

        Destroy(spawnedWeapon, 2f);
    }

    private IEnumerator TemporarilyRotateUpperBody(Vector3 rotationEuler, float duration)
    {
        Quaternion originalRotation = upperBodyBone.localRotation;
        upperBodyBone.localRotation *= Quaternion.Euler(rotationEuler);
        yield return new WaitForSeconds(duration);
        upperBodyBone.localRotation = originalRotation;
    }

    public void UseSkillByIndex(int index)
    {
        if (index < 0 || index >= skills.Count) return;

        SkillData skill = skills[index];

        if (Time.time < skill.lastUsedTime + skill.cooldown)
        {
            Debug.Log($"Skill {index} is still on cooldown!");
            return;
        }

        skill.lastUsedTime = Time.time;

        if (skill.skillPrefab == null || skill.spawnPoint == null) return;

        if (!string.IsNullOrEmpty(skill.skillAnimation))
        {
            playerAnimator.Play(skill.skillAnimation);
        }

        GameObject skillObj = Instantiate(
            skill.skillPrefab,
            skill.spawnPoint.position,
            skill.spawnPoint.rotation
        );

        Rigidbody rb = skillObj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 forceDir = skill.spawnPoint.forward;
            forceDir.y = 0;
            forceDir.Normalize();

            rb.useGravity = false;
            rb.AddForce(forceDir * skill.force, ForceMode.Impulse);
        }

        Destroy(skillObj, skill.lifeTime);
    }
}
