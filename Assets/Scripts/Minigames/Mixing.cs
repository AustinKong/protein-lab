using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class Mixing : Draggable
{
  private InputAction deltaAction;

  private Vector3 initialPosition;
  private bool canShake = true;

  private const float SHAKE_THRESHOLD = 20f;
  private const int SHAKES_REQUIRED = 8;
  private const float SHAKES_COOLDOWN = 1.5f;

  private int shakesCount = 0;
  private float lastShakeTime;

  protected override void Awake() {
    base.Awake();
    deltaAction = inputActions.FindAction("Game/Delta");
  }

  private void Start() {
    initialPosition = transform.position;
  }

  protected override void Update() {
    if (canShake) {
      base.Update();
      if (isDragging) {
        TrackShakeInput();
        DoRotation();
      } else {
        transform.position = Vector3.Lerp(transform.position, initialPosition, Time.deltaTime * 8f);
      }
    } else {
      transform.position = Vector3.Lerp(transform.position, initialPosition, Time.deltaTime * 8f);
      transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, Time.deltaTime * 10f);
    }
  }

    protected override void OnDragStart(InputAction.CallbackContext context) {
      base.OnDragStart(context);
      // Overwritten for better rotation effect
      dragOffset = Vector2.zero;
    }

    private void TrackShakeInput() {
    float magnitude = deltaAction.ReadValue<Vector2>().magnitude;
    if (magnitude > SHAKE_THRESHOLD && Time.time - lastShakeTime > SHAKES_COOLDOWN) {
      shakesCount++;
      lastShakeTime = Time.time;
      ParticlePoolManager.Instance.PlayParticle("RedBubblesLarge", transform.position + transform.up);

      if (shakesCount >= SHAKES_REQUIRED) {
        canShake = false;
        ParticlePoolManager.Instance.PlayParticle("StarLarge", transform.position);
      }
    }
  }

  private void DoRotation() {
    float magnitude = deltaAction.ReadValue<Vector2>().magnitude;
    if (magnitude > 1f) {
      float angle = Mathf.Atan2(deltaAction.ReadValue<Vector2>().y, deltaAction.ReadValue<Vector2>().x) * Mathf.Rad2Deg - 90f;
      transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angle), Time.deltaTime * 6f);
    } else {
      transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, Time.deltaTime * 10f);
    }
  }
}
