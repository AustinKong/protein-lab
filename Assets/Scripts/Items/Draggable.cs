using UnityEngine;

/// <summary>
/// Base class for interactables that support being clicked and dragged. Also provides semi-realistic inertia-based rotation.
/// </summary>
public class Draggable : Interactable 
{
  private const float ROTATION_THRESHOLD = 5f; // Threshold in pixels travelled by pointer in 1 frame to trigger rotation
  private const float ROTATION_SPEED = 5f;

  private Vector3 dragOffset;

  protected virtual void Update() {
    DoRotation();
    DoMovement();
  }

  protected override void OnDragStart() {
    base.OnDragStart();
    dragOffset = transform.position - GetPointerWorldPosition();
  }

  private void DoMovement() {
    if (isDragging) {
      transform.position = GetPointerWorldPosition() + transform.rotation * dragOffset;
    }
  }

  private void DoRotation() {
    Vector2 delta = GetPointerDelta();
    float deltaSign = Mathf.Sign(delta.x);

    if (isDragging && Mathf.Abs(delta.x) > ROTATION_THRESHOLD) {
      transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, -45f * deltaSign), Time.deltaTime * ROTATION_SPEED);
    } else {
      transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, Time.deltaTime * ROTATION_SPEED * 1.5f);
    }
  }
}
