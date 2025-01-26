using UnityEngine;

public class Molecule : MonoBehaviour
{
  [SerializeField] private Sprite[] sprites;

  private bool isStatic = true;
  private const float VELOCITY = 16f;
  private int bounces = 0;

  private void Start() {
    GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Length)];
  }

  private void Update() {
    if (!isStatic) {
      transform.Translate(Vector3.up * VELOCITY * Time.deltaTime);
    }
  }

  private void OnTriggerEnter2D(Collider2D o) {
    if (isStatic) return;

    if (o.TryGetComponent<Molecule>(out Molecule other)) {
      isStatic = true;
      transform.position = MoleculeManager.Instance.AlignToGrid(transform.position);
      MoleculeManager.Instance.Merge(this, other);
    } else {
      if (bounces > 0) Destroy(gameObject);
      Vector3 currEulers = transform.rotation.eulerAngles;
      transform.rotation = Quaternion.Euler(0, 0, -currEulers.z);
      transform.Translate(Vector3.up * VELOCITY * Time.deltaTime);
      bounces++;
    }
  }

  private void OnBecameInvisible() {
    Destroy(gameObject);
  }

  public void Shoot() {
    isStatic = false;
  }
}
