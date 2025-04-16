using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class BulletProjectile : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 50f;
    public float lifetime = 5f;
    public int maxBounces = 0;

    private Rigidbody rb;
    private int bounceCount = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * speed;
        Destroy(gameObject, lifetime); // safety cleanup
    }

    void OnCollisionEnter(Collision collision)
    {
        if (bounceCount < maxBounces)
        {
            // Bounce logic (can be replaced with physics material too)
            Vector3 reflectDir = Vector3.Reflect(rb.linearVelocity.normalized, collision.contacts[0].normal);
            rb.linearVelocity = reflectDir * speed;
            bounceCount++;
            return;
        }

        // Despawn logic
        Destroy(gameObject);
    }
}