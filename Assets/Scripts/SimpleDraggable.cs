using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleDraggable : MonoBehaviour
{
  [SerializeField] protected InputActionAsset inputActions;
  private Camera mainCamera;
  private InputAction clickAction;
  private InputAction pointAction;

  protected bool isDragging = false;
  private Vector3 dragOffset;

  protected virtual void Awake() {
    pointAction = inputActions.FindAction("Game/Point");
    clickAction = inputActions.FindAction("Game/Click");
    mainCamera = Camera.main;
  }

  protected virtual void Update() {
    DoMovement();
  }

  private void OnEnable() {
    clickAction.started += OnDragStart;
    clickAction.canceled += OnDragEnd;
  }

  private void OnDisable() {
    clickAction.started -= OnDragStart;
    clickAction.canceled -= OnDragEnd;
  }

  protected virtual void OnDestroy() {
    clickAction.started -= OnDragStart;
    clickAction.canceled -= OnDragEnd;
  }

  protected virtual void OnDragStart(InputAction.CallbackContext context) {
    if (IsClicked()) {
      isDragging = true;
      dragOffset = transform.position - GetPointerWorldPosition();
    }
  }

  protected virtual void OnDragEnd(InputAction.CallbackContext context) {
    if (isDragging) {
      isDragging = false;
    }
  }

  private void DoMovement() {
    if (isDragging) {
      transform.position = GetPointerWorldPosition() + transform.rotation * dragOffset;
    }
  }

  protected Vector3 GetPointerWorldPosition() {
    Vector3 screenPosition = pointAction.ReadValue<Vector2>();
    Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 0));
    return (Vector2) worldPosition;
  }

  private bool IsClicked() {
    Collider2D hitCollider = Physics2D.OverlapPoint(GetPointerWorldPosition());
    return hitCollider != null && hitCollider.gameObject == gameObject;
  }
}
