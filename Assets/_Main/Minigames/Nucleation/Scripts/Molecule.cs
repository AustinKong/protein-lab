using UnityEngine;

public enum MoleculeState { Free, Clustered, Falling }

public class Molecule : MonoBehaviour
{
    private float SPEED = 3f, UPWARDS_BOUNCE_FORCE = 5f, SIDEWAYS_BOUNCE_FORCE = 1f, GRAVITY = -9.8f;

    private Vector2 velocity = Vector2.zero;
    public MoleculeState state = MoleculeState.Free;
    public bool isMainMoleculeType = false;

    public void InitializeMovement(Vector2 direction) => velocity = direction.normalized * SPEED;

    void Update()
    {
        switch (state)
        {
            case MoleculeState.Clustered:
                // Handle clustered state if needed
                break;
            case MoleculeState.Falling:
                velocity.y += GRAVITY * Time.deltaTime;
                transform.Translate(velocity * Time.deltaTime);
                break;
            case MoleculeState.Free:
                transform.Translate(velocity * Time.deltaTime);
                break;
        }

        TryDespawn();
    }

    public void TriggerFall(Molecule other)
    {
        state = MoleculeState.Falling;
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.7f);
        GetComponent<Collider2D>().enabled = false;
        velocity = new Vector2(
            (transform.position.x > other.transform.position.x ? 1 : -1) * SIDEWAYS_BOUNCE_FORCE,
            UPWARDS_BOUNCE_FORCE
        ) * Random.Range(0.9f, 1.1f);
    }

    public void TriggerBounce(Molecule other)
    {
        Vector2 normal = (other.transform.position - transform.position).normalized;
        velocity = Vector2.Reflect(velocity, normal);
    }

    private void TryDespawn()
    {
        if (Vector3.Distance(transform.position, Vector3.zero) > 15f)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (state != MoleculeState.Free) return;

        if (collision.TryGetComponent(out Molecule other))
        {
            // Only smaller ID triggers collision OR other is clustered and won't trigger
            if (this.GetInstanceID() < other.GetInstanceID() || other.state == MoleculeState.Clustered) ClusterManager.Instance.HandleCollision(this, other);
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (state != MoleculeState.Free) return;

        if (collision.CompareTag("Indicator"))
        {
            ClusterManager.Instance.HandleIndicatorCollision(this);
        }
    }
}
