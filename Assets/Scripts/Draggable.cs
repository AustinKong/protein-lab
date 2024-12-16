using UnityEngine;
using UnityEngine.InputSystem;

public class Draggable : SimpleDraggable 
{
  protected InputAction deltaAction;

  private const float ROTATION_THRESHOLD = 5f; // Threshold in pixels travelled by pointer in 1 frame to trigger rotation
  private const float ROTATION_SPEED = 5f;

  protected override void Awake() {
    base.Awake();
    deltaAction = inputActions.FindAction("Game/Delta");
  }

  protected override void Update() {
    base.Update();
    DoRotation();
  }

  private void DoRotation() {
    Vector2 delta = deltaAction.ReadValue<Vector2>();
    float deltaSign = Mathf.Sign(delta.x);

    if (isDragging && Mathf.Abs(delta.x) > ROTATION_THRESHOLD) {
      transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, -45f * deltaSign), Time.deltaTime * ROTATION_SPEED);
    } else {
      transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, Time.deltaTime * ROTATION_SPEED * 1.5f);
    }
  }
}
