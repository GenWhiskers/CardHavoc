using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class BulletProjectile : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 50f;
    public float lifetime = 5f;
    public int maxBounces = 0;

    [Header("Ignore Collision")]
    public string ignoreTag = "Gun";              // Optional: tag to ignore
    public LayerMask ignoreLayers;                // Optional: layer mask to ignore

    private Rigidbody rb;
    private int bounceCount = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * speed;

        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check for tag
        if (!string.IsNullOrEmpty(ignoreTag) && collision.gameObject.CompareTag(ignoreTag))
            return;

        // Check for layer
        if (((1 << collision.gameObject.layer) & ignoreLayers) != 0)
            return;

        // Bounce logic
        if (bounceCount < maxBounces)
        {
            Vector3 reflectDir = Vector3.Reflect(rb.linearVelocity.normalized, collision.contacts[0].normal);
            rb.linearVelocity = reflectDir * speed;
            bounceCount++;
            return;
        }

        // Despawn
        Destroy(gameObject);
    }
}