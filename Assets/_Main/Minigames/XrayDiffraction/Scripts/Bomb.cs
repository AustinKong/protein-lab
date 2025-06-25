using UnityEngine;

public class Bomb : MonoBehaviour
{
    private Vector2 velocity;
    private float gravity = -9.8f;
    private bool hasExploded = false;
    private float rotationSpeed;
    private int rotationDirection; // -1 or +1

    [SerializeField] private ParticleSystem explosionEffect;

    public void Initialize(Vector2 initialVelocity)
    {
        velocity = initialVelocity;

        // Random spin direction and speed
        rotationDirection = Random.value < 0.5f ? -1 : 1;
        rotationSpeed = Random.Range(180f, 360f); // degrees per second
    }

    void Update()
    {
        if (hasExploded) return;

        // Simulate gravity
        velocity.y += gravity * Time.deltaTime;
        transform.position += (Vector3)(velocity * Time.deltaTime);

        // Apply visual rotation
        transform.Rotate(0f, 0f, rotationSpeed * rotationDirection * Time.deltaTime);

        // Destroy offscreen
        if (transform.position.x > 12f || transform.position.y < -6f)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasExploded) return;

        if (other.CompareTag("PaddleTop") || other.CompareTag("PaddleBottom"))
        {
            hasExploded = true;

            if (explosionEffect != null)
                Instantiate(explosionEffect, transform.position, Quaternion.identity);

            PaddleController.Instance.RegisterMiss();       // Reset combo
            PaddleController.Instance.LockPaddle(1f);       // 🔐 Lock for 1 second
            SoundManager.Instance.PlaySFX("DM-CGS-17");

            Destroy(gameObject);
        }
    }
}
