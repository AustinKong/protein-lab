using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class DraggableShape : MonoBehaviour
{
  private InputActionAsset inputActions;
  private InputAction clickAction, pointAction;
  private Camera mainCamera;

  private Vector3 originalPosition;
  private bool isDragging = false;
  [SerializeField] private bool isStatic = false; // Serialized for Seeds

  private void Awake() {
    mainCamera = Camera.main;
    inputActions = InputManager.Instance.inputActions;
    clickAction = inputActions.FindAction("Game/Click");
    pointAction = inputActions.FindAction("Game/Point");
  }

  private void Update() {
    if (isDragging && !isStatic) {
      transform.position = GetPointerWorldPosition();
    }
  }

  private void OnEnable() {
    clickAction.started += OnDragStart;
    clickAction.canceled += OnDragEnd;
  }

  private void OnDisable() {
    clickAction.started -= OnDragStart;
    clickAction.canceled -= OnDragEnd;
  }

  private void OnDragStart(InputAction.CallbackContext context) {
    if (!IsOver()) return;
    isDragging = true;
    originalPosition = transform.position;
  }

  private void OnDragEnd(InputAction.CallbackContext context) {
    if (!isDragging) return;
    List<Collider2D> others = new List<Collider2D>();
    Physics2D.OverlapCollider(GetComponent<Collider2D>(), others);
    others = others.Where(o => o.GetComponent<DraggableShape>() != null && o.GetComponent<DraggableShape>().isStatic).ToList();

    if (others.Count > 0) {
      isStatic = true;
      GrowthRegimeManager.Instance.NextShape(GetComponent<Collider2D>());
    } else {
      transform.position = originalPosition;
    }
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
