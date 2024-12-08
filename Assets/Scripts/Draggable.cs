using UnityEngine;
using UnityEngine.InputSystem;

public class Draggable : MonoBehaviour
{
  [SerializeField] protected InputActionAsset inputActions;
  private Camera mainCamera;
  private InputAction clickAction;
  private InputAction pointAction;

  protected bool isDragging = false;
  protected Vector3 dragOffset;

  protected virtual void Awake() {
    pointAction = inputActions.FindAction("Game/Point");
    clickAction = inputActions.FindAction("Game/Click");
    mainCamera = Camera.main;
  }

  private void OnEnable() {
    clickAction.started += OnDragStart;
    clickAction.canceled += OnDragEnd;
  }

  private void OnDisable() {
    clickAction.started -= OnDragStart;
    clickAction.canceled -= OnDragEnd;
  }

  private void OnDestroy() {
    clickAction.started -= OnDragStart;
    clickAction.canceled -= OnDragEnd;
  }

  protected virtual void Update() {
    if (isDragging) {
      transform.position = GetPointerWorldPosition() + dragOffset;
    }
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

  protected Vector3 GetPointerWorldPosition() {
    Vector3 screenPosition = pointAction.ReadValue<Vector2>();
    Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 0));
    return worldPosition;
  }

  private bool IsClicked() {
    Collider2D hitCollider = Physics2D.OverlapPoint(GetPointerWorldPosition());
    return hitCollider != null && hitCollider.gameObject == gameObject;
  }
}
