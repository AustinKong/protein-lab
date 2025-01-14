using UnityEngine;

public class Molecule : MonoBehaviour
{
  private bool isStatic = true;
  private const float VELOCITY = 16f;

  private void Update() {
    if (!isStatic) {
      transform.Translate(Vector3.up * VELOCITY * Time.deltaTime);
    }
  }

  private void OnTriggerEnter2D(Collider2D o) {
    if (o.TryGetComponent<Molecule>(out Molecule other)) {
      isStatic = true;
      transform.position = MoleculeManager.Instance.AlignToGrid(transform.position);
      MoleculeManager.Instance.Merge(this, other);
    }
  }

  private void OnBecameInvisible() {
    Destroy(gameObject);
  }

  public void Shoot() => isStatic = false;
}
