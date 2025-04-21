using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class WeaponHandler : NetworkBehaviour
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
        AimGunTowardCrosshair();
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

        if (hipPosition == null || adsPosition == null)
        {
            Debug.LogWarning("Weapon is missing HipPosition or ADSPosition!");
            return;
        }

        // Snap immediately to hip
        currentWeapon.transform.localPosition = hipPosition.localPosition;
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
        // Camera cam = Camera.main;
        // Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        // Vector3 targetPoint;

        // if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        //     targetPoint = hit.point;
        // else
        //     targetPoint = ray.origin + ray.direction * 100f;

        // Vector3 shootDirection = (targetPoint - firePoint.position).normalized;
        // Quaternion rotation = Quaternion.LookRotation(shootDirection);

        // Send to server for spawning
        ShootServerRpc(firePoint.position, firePoint.rotation);
    }

    [ServerRpc]
    void ShootServerRpc(Vector3 position, Quaternion rotation)
    {
        if (bulletPrefab == null)
        {
            Debug.LogWarning("Bullet prefab not assigned.");
            return;
        }

        GameObject bullet = Instantiate(bulletPrefab, position, rotation);
        Spawn(bullet); // FishNet spawns and syncs it
    }

    void AimGunTowardCrosshair()
    {
        if (currentWeapon == null) return;

        Camera cam = Camera.main;
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            targetPoint = hit.point;
        else
            targetPoint = ray.origin + ray.direction * 100f;

        // Rotate weapon root toward the aim point
        Vector3 lookDirection = targetPoint - currentWeapon.transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
        lookRotation *= Quaternion.Euler(180f, 0f, 180f); //TODO: Get the gun to rotate properly 

        currentWeapon.transform.rotation = Quaternion.Slerp(
            currentWeapon.transform.rotation,
            lookRotation,
            Time.deltaTime * aimSpeed
        );
    }
}