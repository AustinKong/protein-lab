using UnityEngine;
using UnityEngine.InputSystem;

public class Minigame_Draggable : MonoBehaviour
{
  private InputActionAsset inputActions;
  private Camera mainCamera;
  protected InputAction clickAction, pointAction, deltaAction;

  private Vector3 dragOffset;
  protected bool isDragging = false;

  private const float ROTATION_THRESHOLD = 5f; // Threshold in pixels travelled by pointer in 1 frame to trigger rotation
  private const float ROTATION_SPEED = 5f;
  protected bool DO_ROTATION = false;

  protected virtual void Awake() {
    mainCamera = Camera.main;
    inputActions = InputManager.Instance.inputActions;
    
    pointAction = inputActions.FindAction("Game/Point");
    clickAction = inputActions.FindAction("Game/Click");
    deltaAction = inputActions.FindAction("Game/Delta");
  }

  protected virtual void Update() {
    DoMovement();
    if (DO_ROTATION) DoRotation();
  }

  private void OnEnable() {
    Setup();
  }

  private void OnDisable() {
    Cleanup();
  }

  private void OnDestroy() {
    Cleanup();
  }

  protected virtual void Setup() {
    clickAction.started += OnDragStartCallback;
    clickAction.canceled += OnDragEndCallback;
  }

  protected virtual void Cleanup() {
    clickAction.started -= OnDragStartCallback;
    clickAction.canceled -= OnDragEndCallback;
  }

  protected virtual void OnDragStart() {
    isDragging = true;
    dragOffset = transform.position - GetPointerWorldPosition();
  }

  protected virtual void OnDragEnd() {
    isDragging = false;
  }

  private void OnDragStartCallback(InputAction.CallbackContext context) {
    if (!IsOver()) return;
    OnDragStart();
  }

  private void OnDragEndCallback(InputAction.CallbackContext context) {
    if (!isDragging) return;
    OnDragEnd();
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

  protected bool IsOver() {
    Collider2D hitCollider = Physics2D.OverlapPoint(GetPointerWorldPosition());
    return hitCollider != null && hitCollider.gameObject == gameObject;
  }

  protected Vector3 GetPointerWorldPosition() {
    Vector3 screenPosition = pointAction.ReadValue<Vector2>();
    Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 0));
    return (Vector2) worldPosition;
  }

  protected Vector2 GetPointerDelta() {
    return deltaAction.ReadValue<Vector2>();
  }
}
