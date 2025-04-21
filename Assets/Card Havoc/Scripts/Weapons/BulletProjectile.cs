using UnityEngine;
using FishNet.Object;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class BulletProjectile : NetworkBehaviour
{
    public float speed = 50f;
    public float lifetime = 5f;
    public float damage = 25f;
    public int maxBounces = 0;

    private Rigidbody rb;
    private int bounceCount;

    public override void OnStartServer()
    {
        base.OnStartServer();
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * speed;

        Destroy(gameObject, lifetime); // Server-only cleanup
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!IsServerStarted) return; // Only server handles collision logic

        GameObject hit = collision.gameObject;

        if (hit.CompareTag("Player"))
        {
            PlayerHealth health = hit.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(damage); // Server-authoritative
            }

            Despawn(); // Despawn bullet
            return;
        }

        if (bounceCount < maxBounces)
        {
            Vector3 reflectDir = Vector3.Reflect(rb.linearVelocity.normalized, collision.contacts[0].normal);
            transform.position = collision.contacts[0].point + collision.contacts[0].normal * 0.01f;
            rb.linearVelocity = reflectDir * speed;
            bounceCount++;
            return;
        }

        Despawn();
    }

    void Despawn()
    {
        Despawn(gameObject); // FishNet-safe destroy
    }
}