using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Draggable : MonoBehaviour
{
  [SerializeField] private InputActionAsset inputActions;
  private Camera mainCamera;
  private InputAction clickAction;
  private InputAction pointAction;
  protected InputAction deltaAction;

  protected bool isDragging = false;
  private Vector3 dragOffset;

  private const float ROTATION_THRESHOLD = 5f; // Threshold in pixels travelled by pointer in 1 frame to trigger rotation
  private const float ROTATION_SPEED = 5f;

  protected virtual void Awake() {
    pointAction = inputActions.FindAction("Game/Point");
    clickAction = inputActions.FindAction("Game/Click");
    deltaAction = inputActions.FindAction("Game/Delta");
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

  protected virtual void OnDestroy() {
    clickAction.started -= OnDragStart;
    clickAction.canceled -= OnDragEnd;
  }

  protected virtual void Update() {
    DoMovement();
    DoRotation();
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
  
  private void DoRotation() {
    Vector2 delta = deltaAction.ReadValue<Vector2>();
    float deltaSign = Mathf.Sign(delta.x);

    if (isDragging && Mathf.Abs(delta.x) > ROTATION_THRESHOLD) {
      transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, -45f * deltaSign), Time.deltaTime * ROTATION_SPEED);
    } else {
      transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, Time.deltaTime * ROTATION_SPEED * 1.5f);
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
