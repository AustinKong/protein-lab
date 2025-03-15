using UnityEngine;

public class Xray : MonoBehaviour
{
  [SerializeField] private Transform trail;

  private const float SPEED = 8f;
  private Vector2 direction;
  private float offset;

  private void Start() {
    direction = Vector2.right;
    offset = Random.Range(0f, 360f);
  }

  private void Update() {
    transform.Translate(direction * Time.deltaTime * SPEED);
    trail.localPosition = new Vector3(0, Mathf.Sin(Time.time * 80f + offset) * 0.1f, 0);
  }

  public void SetDirection(Vector2 direction) {
    this.direction = direction;
  }

  void OnBecameInvisible() {
    Invoke(nameof(Destroy), 3f);   
  }

  private void Destroy() {
    Destroy(gameObject);
  }

  private void OnTriggerEnter2D(Collider2D collision) {
    if (collision.CompareTag("XrayReflective"))
    {
      Transform reflectiveSurface = collision.transform;
      SpriteRenderer spriteRenderer = collision.GetComponent<SpriteRenderer>();

      if (spriteRenderer != null)
      {
        float spriteHeight = spriteRenderer.bounds.size.y; // Get actual height of sprite

        // Find contact point's relative Y position
        float hitY = transform.position.y; // Y of the X-ray beam at collision
        float centerY = reflectiveSurface.position.y; // Center Y of the reflective object

        // Normalize to range [-1, 1] (top of sprite = 1, bottom = -1)
        float normalizedY = Mathf.Clamp((hitY - centerY) / (spriteHeight / 2f), -1f, 1f);

        // Map to refraction range [-80, 80] using Lerp
        float angleOffset = Mathf.Lerp(-30f, 30f, (normalizedY + 1f) / 2f);

        // Rotate the direction by the calculated angle
        transform.rotation = Quaternion.Euler(0, 0, angleOffset);
        // direction = Quaternion.Euler(0, 0, angleOffset) * direction;
      }
    }
  }
}
