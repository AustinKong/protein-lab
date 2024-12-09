using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class Mixing : Draggable
{
  private Vector3 initialPosition;
  private bool canShake = true;

  private const float SHAKE_THRESHOLD = 20f;
  private const int SHAKES_REQUIRED = 2; // Tweaked, original is 8
  private const float SHAKES_COOLDOWN = 1.5f;

  private int shakesCount = 0;
  private float lastShakeTime;

  private void Start() {
    initialPosition = transform.position;
  }

  protected override void Update() {
    if (canShake) {
      base.Update();
      if (isDragging) {
        TrackShakeInput();
      } else {
        transform.position = Vector3.Lerp(transform.position, initialPosition, Time.deltaTime * 8f);
      }
    } else {
      transform.position = Vector3.Lerp(transform.position, initialPosition, Time.deltaTime * 8f);
      transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, Time.deltaTime * 10f);
    }
  }

  private void TrackShakeInput() {
    float magnitude = deltaAction.ReadValue<Vector2>().magnitude;
    if (magnitude > SHAKE_THRESHOLD && Time.time - lastShakeTime > SHAKES_COOLDOWN) {
      shakesCount++;
      lastShakeTime = Time.time;
      ParticlePoolManager.Instance.PlayParticle("RedBubblesLarge", transform.position + transform.up);

      if (shakesCount >= SHAKES_REQUIRED) {
        canShake = false;
        ParticlePoolManager.Instance.PlayParticle("StarLarge", Vector2.zero);
        StartCoroutine(SuccessRoutine());
      }
    }
  }

  private IEnumerator SuccessRoutine() {
    yield return new WaitForSeconds(1);
    SceneManager.Instance.ReturnToOriginalScene();
  }
}
