using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public class WeaponHandler : NetworkBehaviour
{
    [Header("References")]
    public GameObject currentWeapon; // Assigned manually or default weapon at start

    [Header("ADS Settings")]
    public float aimSpeed = 10f;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.2f;

    [SerializeField] public Transform debugTransform;
    [SerializeField] public LayerMask aimColliderLayerMask = new LayerMask();

    private Quaternion targetRotation;

    private float nextFireTime;

    void Update()
    {
        if (IsOwner)
        {
            // Local player calculates aim
            HandleShoot();
            AimGunTowardCrosshair();
        }
        else
        {
            // Remote players smoothly move to the target rotation
            currentWeapon.transform.rotation = Quaternion.Slerp(
                currentWeapon.transform.rotation,
                targetRotation,
                Time.deltaTime * aimSpeed
            );
        }
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

        if (Time.frameCount % 3 == 0) // Every 3rd frame
        {
            // First send to server
            UpdateWeaponRotationServerRpc(lookRotation);
        }
    }

     // Server RPC
    [ServerRpc]
    private void UpdateWeaponRotationServerRpc(Quaternion rotation)
    {
        // Then broadcast to all clients
        UpdateWeaponRotationObserversRpc(rotation);
    }

    [ObserversRpc(ExcludeOwner = true)]
    private void UpdateWeaponRotationObserversRpc(Quaternion rotation)
    {
        targetRotation = rotation;
    }
}