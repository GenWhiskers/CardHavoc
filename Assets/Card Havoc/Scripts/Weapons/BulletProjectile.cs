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
    public LayerMask ignoreLayers;                // Optional: layers to ignore

    [Header("Hit Detection")]
    public string playerTag = "Player";           // Tag used to identify player targets

    private Rigidbody rb;
    private int bounceCount = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * speed;
        Destroy(gameObject, lifetime); // Just in case it never hits anything
    }

    void OnCollisionEnter(Collision collision)
    {
        GameObject hitObject = collision.gameObject;
        // Ignore gun by layer
        if (((1 << hitObject.layer) & ignoreLayers) != 0)
            return;

        // If hit a player → despawn
        if (hitObject.CompareTag(playerTag))
        {
            PlayerHealth health = hitObject.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(25f);
            }

            Destroy(gameObject);
            return;
        }

        // Bounce logic
        if (bounceCount < maxBounces)
        {
            ContactPoint contact = collision.contacts[0];
            Vector3 reflectDir = Vector3.Reflect(rb.linearVelocity.normalized, collision.contacts[0].normal);
            transform.position = contact.point + contact.normal * 0.01f;
            rb.linearVelocity = reflectDir * speed;
            bounceCount++;
            return;
        }

        // Default: despawn on non-player impact
        Destroy(gameObject);
    }
}