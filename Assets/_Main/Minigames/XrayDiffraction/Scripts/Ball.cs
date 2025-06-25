using UnityEngine;

public class Ball : MonoBehaviour
{
    private Vector2 velocity;
    private bool hasBounced = false;
    public bool isTopBall;

    [SerializeField] private GameObject plusOnePrefab;
    [SerializeField] private GameObject missedPrefab;

    /// <summary>
    /// Call this right after Instantiate:
    /// direction → who to shoot toward
    /// speed     → initial speed
    /// isTop     → true if this is the “top” ball of the pair
    /// </summary>
    public void Initialize(Vector2 direction, float speed, bool isTop)
    {
        velocity = direction.normalized * speed;
        isTopBall = isTop;
    }

    void Update()
    {
        transform.Translate(velocity * Time.deltaTime);

        if (transform.position.x > 10f || transform.position.y < -5.5f)
        {
            if (transform.position.y < -5.5f && !isTopBall)
            {
                PaddleController.Instance.RegisterMiss();
                Instantiate(missedPrefab, transform.position + Vector3.up, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // only bounce once, and only on the matching paddle
        if (hasBounced) return;

        if ((isTopBall && other.CompareTag("PaddleTop")) || (!isTopBall && other.CompareTag("PaddleBottom")))
        {
            if (!isTopBall)
            {
                Instantiate(plusOnePrefab, transform.position, Quaternion.identity);
            }

            HandlePaddleHit();
            hasBounced = true;
            velocity.y *= -1;
        }
    }

    protected virtual void HandlePaddleHit()
    {
        if (!isTopBall)
        {
            SoundManager.Instance.PlaySFX($"SFX-impact-simple-0{Random.Range(1, 4)}_wav");
        }
        PaddleController.Instance.RegisterHit();
    }
}
