using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using System.Collections; // ✅ UI 이미지에 접근할 때 필요


public class WeaponShooterWithJoystick : MonoBehaviour
{
    [System.Serializable]
    public class SkillData
    {
        public GameObject skillPrefab;
        public Transform spawnPoint;
        public float force = 1000f;
        public float lifeTime = 3f;
        public AudioClip skillSound;
        public string skillAnimation;
    }


    [System.Serializable]
    public class WeaponData
    {
        public GameObject weaponPrefab; // 발사할 무기 프리팹
        public GameObject heldWeaponPrefab; // 손에 들고 있을 무기 프리팹
        public AudioClip fireSound;
        public float shootForce = 500f;
        public int damage = 10;

        public Sprite weaponIcon; // ✅ UI에 표시할 무기 아이콘

        public string fireAnimationName; // ✅ 무기를 발사할 때 실행할 애니메이션 이름

        public float fireRate = 0.5f; // ✅ 무기별 발사 간격

        public float spawnDelay = 0f; // ✅ 무기 생성 지연 시간 (단위: 초)

    }

    // public SkillData skill; // ✅ 단일 스킬 예시

    public List<WeaponData> weapons = new List<WeaponData>();
    private int currentWeaponIndex = 0;

    public Camera arCamera;
    public DynamicJoystick joystick;
    //public float fireRate = 0.5f;
    private float nextFire = 0f;

    public Transform weaponHoldPoint; // 들고 있을 무기를 붙일 위치
    private GameObject currentHeldWeapon; // 현재 들고 있는 무기 오브젝트

    public AudioSource audioSource;

    private List<GameObject> heldWeapons = new List<GameObject>(); // 손에 들고 있는 무기들을 관리
    private bool weaponsSpawned = false;

    public Image weaponUIImage; // 현재 무기 이미지 표시용

    public Transform upperBodyBone; // 예: 상체 본 (Spine 등)
    public Vector3 extraRotationEuler; // 추가로 회전하고 싶은 각도


    public Animator playerAnimator; // ✅ 플레이어 애니메이션 재생용

    public List<SkillData> skills = new List<SkillData>(); // ✅ 여러 스킬 등록 가능

    void Start()
    {
        Invoke(nameof(InitializeWeapons), 0.1f); // 살짝 딜레이 후 무기 생성
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
            nextFire = Time.time + currentWeapon.fireRate; // ✅ 무기별 발사 속도 적용
            StartCoroutine(ShootWeaponWithSpawnDelay(currentWeapon));
        }
    }

    IEnumerator ShootWeaponWithSpawnDelay(WeaponData currentWeapon)
    {
        // 애니메이션 및 상체 회전 (미리 실행)
        if (playerAnimator != null && !string.IsNullOrEmpty(currentWeapon.fireAnimationName))
        {
            playerAnimator.Play(currentWeapon.fireAnimationName);

            if (upperBodyBone != null)
            {
                StartCoroutine(TemporarilyRotateUpperBody(extraRotationEuler, 0.3f));
            }
        }

        // ✅ 생성 지연 시간만큼 기다림
        if (currentWeapon.spawnDelay > 0f)
            yield return new WaitForSeconds(currentWeapon.spawnDelay);

        // 발사 방향 계산
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



    void SpawnAllHeldWeapons()
    {
        heldWeapons.Clear(); // 중복 방지
        foreach (var weapon in weapons)
        {
          //  if (weapon.heldWeaponPrefab != null)
          //  {
                //GameObject heldWeapon = Instantiate(
                   /// weapon.heldWeaponPrefab,
                 //   weaponHoldPoint.position,
                  //  weaponHoldPoint.rotation,
                  //  weaponHoldPoint
                //);

                //weapon.heldWeaponPrefab.transform.localScale = Vector3.one; // ✅ 무조건 (1,1,1)로 고정
                weapon.heldWeaponPrefab.SetActive(false); // 기본 비활성화
                heldWeapons.Add(weapon.heldWeaponPrefab);
            
        }
        weaponsSpawned = true;
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

    public void PreviousWeapon()
    {
        currentWeaponIndex--;
        if (currentWeaponIndex < 0)
        {
            currentWeaponIndex = weapons.Count - 1;
        }
        Debug.Log("현재 무기: " + weapons[currentWeaponIndex].weaponPrefab.name);
        EquipCurrentWeapon();
    }

    void UpdateWeaponUI()
    {
        if (weaponUIImage != null && weapons.Count > 0)
        {
            Sprite icon = weapons[currentWeaponIndex].weaponIcon;
            weaponUIImage.sprite = icon;
        }
    }


    void EquipCurrentWeapon()
    {
        if (!weaponsSpawned) return; // 무기가 스폰되지 않았다면 실행하지 않음

        if (weaponHoldPoint == null)
        {
            Debug.LogWarning("Weapon Hold Point가 설정되지 않았습니다!");
            return;
        }

        // 모든 무기 끄기
        foreach (var weaponObj in heldWeapons)
        {
            if (weaponObj != null)
                weaponObj.SetActive(false);
        }

        // 현재 무기만 켜기
        if (currentWeaponIndex >= 0 && currentWeaponIndex < heldWeapons.Count)
        {
            currentHeldWeapon = heldWeapons[currentWeaponIndex];
            if (currentHeldWeapon != null)
            {
                currentHeldWeapon.SetActive(true);
            }
        }
        UpdateWeaponUI(); // ✅ 이미지 갱신

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

        if (playerAnimator != null && !string.IsNullOrEmpty(currentWeapon.fireAnimationName))
        {
            playerAnimator.Play(currentWeapon.fireAnimationName);

            if (upperBodyBone != null)
            {
                StartCoroutine(TemporarilyRotateUpperBody(extraRotationEuler, 0.3f)); // 애니메이션 길이만큼 유지
            }
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

        if (skill.skillPrefab == null || skill.spawnPoint == null) return;

        // 애니메이션
        if (!string.IsNullOrEmpty(skill.skillAnimation))
        {
            playerAnimator.Play(skill.skillAnimation);
        }

        // 스킬 생성
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

            rb.useGravity = false; // ✅ 직선 발사
            rb.AddForce(forceDir * skill.force, ForceMode.Impulse);
        }

        Destroy(skillObj, skill.lifeTime);
    }

}
