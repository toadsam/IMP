using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;

public class WeaponShooterWithJoystick : MonoBehaviour
{
    [System.Serializable]
    public class WeaponData
    {
        public GameObject weaponPrefab; // 발사할 무기 프리팹
        public GameObject heldWeaponPrefab; // 손에 들고 있을 무기 프리팹
        public AudioClip fireSound;
        public float shootForce = 500f;
        public int damage = 10;
    }

    public List<WeaponData> weapons = new List<WeaponData>();
    private int currentWeaponIndex = 0;

    public Camera arCamera;
    public bl_Joystick joystick;
    public float fireRate = 0.5f;
    private float nextFire = 0f;

    public Transform weaponHoldPoint; // 들고 있을 무기를 붙일 위치
    private GameObject currentHeldWeapon; // 현재 들고 있는 무기 오브젝트

    public AudioSource audioSource;

    void Start()
    {
        EquipCurrentWeapon();
    }

    void Update()
    {
        if (Time.time > nextFire && (joystick.Horizontal != 0 || joystick.Vertical != 0))
        {
            nextFire = Time.time + fireRate;
            ShootWeapon();
        }
    }

    public void NextWeapon()
    {
        currentWeaponIndex++;
        if (currentWeaponIndex >= weapons.Count)
        {
            currentWeaponIndex = 0;
        }
        Debug.Log("현재 무기: " + weapons[currentWeaponIndex].weaponPrefab.name);

        EquipCurrentWeapon(); // 무기 전환 시 들고 있는 무기 변경
    }

    void EquipCurrentWeapon()
    {
        if (weaponHoldPoint == null)
        {
            Debug.LogWarning("Weapon Hold Point가 설정되지 않았습니다!");
            return;
        }

        // 이전 무기 삭제
        if (currentHeldWeapon != null)
        {
            Destroy(currentHeldWeapon);
        }

        // 새 무기 생성
        GameObject heldPrefab = weapons[currentWeaponIndex].heldWeaponPrefab;
        if (heldPrefab != null)
        {
            currentHeldWeapon = Instantiate(heldPrefab, weaponHoldPoint.position, weaponHoldPoint.rotation, weaponHoldPoint);
        }
    }

    void ShootWeapon()
    {
        if (weapons.Count == 0 || arCamera == null || joystick == null)
        {
            Debug.LogWarning("Weapon 리스트나 AR Camera, Joystick 연결이 필요합니다.");
            return;
        }

        WeaponData currentWeapon = weapons[currentWeaponIndex];

        if (currentWeapon.weaponPrefab == null)
        {
            Debug.LogWarning("현재 선택된 무기의 발사용 Prefab이 없습니다.");
            return;
        }

        // 발사 방향 계산
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

        // 2초 후 자동 삭제 추가
        Destroy(spawnedWeapon, 2f);
    }

}
