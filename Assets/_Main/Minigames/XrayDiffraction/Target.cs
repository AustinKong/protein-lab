using UnityEngine;

public class Target : MonoBehaviour
{
  private float minY;
  private float maxY;
  private float speed;
  private bool movingUp = true;

  private float hp = 30;
  private SpriteRenderer spriteRenderer;

  public void Start()
  {
    spriteRenderer = GetComponent<SpriteRenderer>();
  }

    public void Initialize(float minY, float maxY, float speed) {
    this.minY = minY;
    this.maxY = maxY;
    this.speed = speed;
  }

  private void Update() {
    if (movingUp) {
      transform.position += Vector3.up * speed * Time.deltaTime;
      if (transform.position.y >= maxY) {
        movingUp = false;
      }
    } else {
      transform.position += Vector3.down * speed * Time.deltaTime;
      if (transform.position.y <= minY) {
        movingUp = true;
      }
    }

    if (Time.time - lastHitTime < 0.1f) {
      spriteRenderer.color = Color.red;
    } else {
      spriteRenderer.color = Color.white;
    }
  }

  private float lastHitTime = 0;

  public void Hit() {
    hp -= Time.deltaTime * 10f;
    lastHitTime = Time.time;
    if (hp <= 0) {
      Destroy(gameObject);
    }
  }
}