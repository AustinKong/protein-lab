using UnityEngine;
using UnityEngine.InputSystem;

public class XrayShooter : MonoBehaviour
{
  [SerializeField] private GameObject crystal;

  private InputActionAsset inputActions;
  private InputAction clickAction, pointAction;
  private Camera mainCamera;

  private bool isDragging = false;

  private void Awake() {
    mainCamera = Camera.main;
    inputActions = InputManager.Instance.inputActions;
    clickAction = inputActions.FindAction("Game/Click");
    pointAction = inputActions.FindAction("Game/Point");
  }

  private void OnEnable() {
    clickAction.started += OnDragStart;
    clickAction.canceled += OnDragEnd;
  }

  private void OnDisable() {
    clickAction.started -= OnDragStart;
    clickAction.canceled -= OnDragEnd;
  }

  private void Update() {
    if (isDragging) {
      Vector3 direction = crystal.transform.position - transform.position;
      float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
      transform.rotation = Quaternion.Euler(0, 0, angle);
      transform.position = new Vector3(transform.position.x, GetPointerWorldPosition().y, transform.position.z);
    }
  }

  private void OnDragStart(InputAction.CallbackContext context) {
    if (!IsOver()) return;
    isDragging = true;
  }

  private void OnDragEnd(InputAction.CallbackContext context) {
    if (!isDragging) return;
    isDragging = false;
  }

  private Vector3 GetPointerWorldPosition() {
    Vector3 screenPosition = pointAction.ReadValue<Vector2>();
    Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 0));
    return (Vector2) worldPosition;
  }

  private bool IsOver() {
    Collider2D hitCollider = Physics2D.OverlapPoint(GetPointerWorldPosition());
    return hitCollider != null && hitCollider.gameObject == gameObject;
  }
}
