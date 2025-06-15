using UnityEngine;

public class Ball : MonoBehaviour
{
    private Vector2 velocity;
    private bool hasBounced = false;
    private bool isTopBall;
    [SerializeField] private GameObject plusOnePrefab;
    [SerializeField] private GameObject missedPrefab;
    [SerializeField] private GameObject wave;

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
        Vector2 perp = new Vector2(-velocity.y, velocity.x).normalized;
        wave.transform.position = transform.position + (Vector3)(Mathf.Sin(Time.time * 20f) * perp * 0.1f);
        if (transform.position.x > 10f || transform.position.y < -5.5f)
        {
            if (transform.position.y < -5.5f && !isTopBall) Instantiate(missedPrefab, transform.position + Vector3.up, Quaternion.identity);
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
                SoundManager.Instance.PlaySFX($"SFX-impact-simple-0{Random.Range(1, 4)}_wav");
            }
            MinigameManager.Instance.Score(1);
            hasBounced = true;
            velocity.y *= -1;
        }
    }
}
