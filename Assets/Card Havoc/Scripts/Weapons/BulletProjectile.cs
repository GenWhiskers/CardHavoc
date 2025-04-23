using UnityEngine;
using FishNet.Object;
using UnityEditor;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class BulletProjectile : NetworkBehaviour
{
    public float speed = 50f;
    public float lifetime = 5f;
    public float damage = 25f;
    public int maxBounces = 0;

    private Rigidbody rb;
    private int bounceCount;
    private Vector3 lastVelocity;

    public override void OnStartServer()
    {
        base.OnStartServer();
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * speed;

        Destroy(gameObject, lifetime); // Server-only cleanup
    }

    void LateUpdate()
    {
        lastVelocity = rb.linearVelocity;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!IsServerStarted) return; // Only server handles collision logic

        GameObject hit = collision.gameObject;

        
        if (hit.CompareTag("Bullet")) {
            return;
        }

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
            // Bounces the bullet
            Vector3 reflectDir = Vector3.Reflect(lastVelocity.normalized, collision.contacts[0].normal);
            float newSpeed = lastVelocity.magnitude * 0.8f; //How much speed is retained
            newSpeed = Mathf.Max(newSpeed, 3f); //Minimum speed after bounce to stop sticking
            transform.position = collision.contacts[0].point + collision.contacts[0].normal * 0.1f;
            rb.linearVelocity = reflectDir * Mathf.Max(lastVelocity.magnitude, 0);
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