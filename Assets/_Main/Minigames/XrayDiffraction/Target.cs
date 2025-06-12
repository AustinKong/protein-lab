using UnityEngine;

public class Target : MonoBehaviour
{
  private float minY;
  private float maxY;
  private float speed;
  private bool movingUp = true;

  private float hp = 3;
  private SpriteRenderer spriteRenderer;

  private float lastHitTime = 0;
  private float rockingDuration = 0.5f; // Duration of rocking effect
  private float rockingFrequency = 10f; // Frequency of sine wave rocking
  private float rockingAmplitude = 10f; // Amplitude of rocking (degrees)

  public void Start()
  {
    spriteRenderer = GetComponent<SpriteRenderer>();
    InvokeRepeating(nameof(SwitchDirection), 0, Random.Range(3f, 6f));
  }

  public void Initialize(float minY, float maxY, float speed)
  {
    this.minY = minY;
    this.maxY = maxY;
    this.speed = speed;
  }

  private void SwitchDirection()
  {
    movingUp = !movingUp;
  }

  private void Update()
  {
    // Vertical movement
    if (movingUp)
    {
      transform.position += Vector3.up * speed * Time.deltaTime;
      if (transform.position.y >= maxY)
      {
        movingUp = false;
      }
    }
    else
    {
      transform.position += Vector3.down * speed * Time.deltaTime;
      if (transform.position.y <= minY)
      {
        movingUp = true;
      }
    }

    // Rocking effect when hit
    if (Time.time - lastHitTime < 0.02f)
    {
      float rockingAngle = Mathf.Sin(Time.time * rockingFrequency) * rockingAmplitude;
      transform.rotation = Quaternion.Euler(0, 0, rockingAngle);
      spriteRenderer.color = Color.red;
    }
    else
    {
      transform.rotation = Quaternion.identity; // Reset rotation
      spriteRenderer.color = Color.white;
    }
  }

  public void Hit()
  {
    hp -= Time.deltaTime;
    lastHitTime = Time.time;
    if (hp <= 0)
    {
      XrayDiffractionManager.Instance.Score();
      ParticlePoolManager.Instance.PlayParticle("Star", transform.position);
      Destroy(gameObject);
    }
  }
}