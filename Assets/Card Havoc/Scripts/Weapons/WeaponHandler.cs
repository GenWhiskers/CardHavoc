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

    [SerializeField] public Transform debugTransform;
    [SerializeField] public LayerMask aimColliderLayerMask = new LayerMask();

    private float nextFireTime;

    void Start()
    {
        EquipWeapon(currentWeapon);
    }

    void Update()
    {
        HandleShoot();
        AimGunTowardCrosshair();
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

        Vector3 targetPoint = Vector3.zero;

        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        if (Physics.Raycast(ray, out RaycastHit hit, 999f, aimColliderLayerMask))
            targetPoint = hit.point;
            debugTransform.position = hit.point; //Turn on when you need to debug where player is looking


        // Rotates gun to crosshair
        Vector3 lookDirection = targetPoint - currentWeapon.transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
        lookRotation *= Quaternion.Euler(0f, 90f, 0f); 
        currentWeapon.transform.rotation = Quaternion.Slerp(
            currentWeapon.transform.rotation,
            lookRotation,
            Time.deltaTime * aimSpeed
        );
    }
}