using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    [Header("References")]
    public GameObject currentWeapon; // Assigned manually or default weapon at start

    private Transform hipPosition;
    private Transform adsPosition;
    private bool isAiming;

    [Header("ADS Settings")]
    public float aimSpeed = 10f;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.2f;

    private float nextFireTime;

    void Start()
    {
        EquipWeapon(currentWeapon);
    }

    void Update()
    {
        HandleADS();
        HandleShoot();
    }

    void HandleADS()
    {
        if (Input.GetMouseButtonDown(1))
            isAiming = true;
        if (Input.GetMouseButtonUp(1))
            isAiming = false;

        if (currentWeapon == null || hipPosition == null || adsPosition == null)
            return;

        Transform target = isAiming ? adsPosition : hipPosition;

        currentWeapon.transform.localPosition = Vector3.Lerp(
            currentWeapon.transform.localPosition,
            target.localPosition,
            Time.deltaTime * aimSpeed
        );

        currentWeapon.transform.localRotation = Quaternion.Lerp(
            currentWeapon.transform.localRotation,
            target.localRotation,
            Time.deltaTime * aimSpeed
        );
    }

    public void EquipWeapon(GameObject newWeapon)
    {
        // Disable old weapon
        if (currentWeapon != null)
            currentWeapon.SetActive(false);

        currentWeapon = newWeapon;
        currentWeapon.SetActive(true);

        hipPosition = currentWeapon.transform.Find("HipPosition");
        adsPosition = currentWeapon.transform.Find("ADSPosition");

        if (hipPosition == null || adsPosition == null)
        {
            Debug.LogWarning("Weapon is missing HipPosition or ADSPosition!");
            return;
        }

        // Snap immediately to hip
        currentWeapon.transform.localPosition = hipPosition.localPosition;
        currentWeapon.transform.localRotation = hipPosition.localRotation;
    }

    void HandleShoot()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Fire()
    {
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning("Missing bulletPrefab or firePoint reference.");
            return;
        }

        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }
}