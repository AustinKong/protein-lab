using System.Collections;
using UnityEngine;

public class Molecule : MonoBehaviour
{
  private bool isStatic = true;
  private const float VELOCITY = 16f;
  private int bounces = 0;

  private void Update() {
    if (!isStatic) {
      transform.Translate(Vector3.up * VELOCITY * Time.deltaTime);
    }
  }

  private void OnTriggerEnter2D(Collider2D o) {
    if (isStatic) return;

    if (o.TryGetComponent<Molecule>(out Molecule other)) {
      isStatic = true;
      MoleculeManager.Instance.Merge(this, other);
      StartCoroutine(LerpToPositionRoutine());
    }
  }

  private IEnumerator LerpToPositionRoutine() {
    float timer = 0;
    Vector3 startingPosition = transform.position;
    Vector3 targetPosition = MoleculeManager.Instance.AlignToGrid(transform.position);
    while (timer < 0.5f) {
      timer += Time.deltaTime;
      yield return null;
      transform.position = Vector3.Slerp(startingPosition, targetPosition, timer / 0.5f);
    }
  }

  private void OnBecameInvisible() {
    if (transform.position.y > 20f) {
      Destroy(gameObject);
    } else {
      if (bounces > 5) Destroy(gameObject);
      else {
        Vector3 currEulers = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(0, 0, -currEulers.z);
        transform.Translate(Vector3.up * VELOCITY * Time.deltaTime);
        bounces++;
      }
    }
  }

  public void Shoot() {
    isStatic = false;
  }
}
