using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class TetrisBlock : MonoBehaviour
{
  private InputActionAsset inputActions;
  private InputAction clickAction, pointAction;
  private Camera mainCamera;

  private bool isDragging = false;
  private Vector2 floatDirection;
  private float rotation;
  private float floatSpeed;
  [SerializeField] private bool isStatic = false; // Serialized for Seeds

  private void Awake() {
    mainCamera = Camera.main;
    inputActions = InputManager.Instance.inputActions;
    clickAction = inputActions.FindAction("Game/Click");
    pointAction = inputActions.FindAction("Game/Point");
    floatDirection = UnityEngine.Random.insideUnitCircle.normalized;
    rotation = UnityEngine.Random.Range(-30f, 30f);
    floatSpeed = UnityEngine.Random.Range(0, 0.6f);
    if (!isStatic) GetComponent<SpriteRenderer>().color = Color.gray;
  }

  private void Update() {
    if (isDragging && !isStatic) {
      transform.position = Vector3.Lerp(transform.position, GetPointerWorldPosition(), Time.deltaTime * 2f);
      // OnDrag();
    } else if (!isDragging && !isStatic) {
      transform.position += floatSpeed * Time.deltaTime * (Vector3) floatDirection;
      transform.Rotate(Vector3.forward, rotation * Time.deltaTime);
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
    if (!IsOver() || isStatic) return;
    isDragging = true;
    GetComponent<SpriteRenderer>().color = Color.white;
  }

  /*
  private void OnDrag() {
    List<Collider2D> others = new List<Collider2D>();
    Physics2D.OverlapCollider(GetComponent<Collider2D>(), others);
    others = others.Where(o => o.GetComponent<TetrisBlock>() != null && o.GetComponent<TetrisBlock>().isStatic).ToList();

    if (others.Count > 0) {
      isStatic = true;
      TetrisManager.Instance.RegisterCombineOrDestroy();
    }
  }
  */

  private void OnDragEnd(InputAction.CallbackContext context) {
    if (!isDragging) return;

    List<Collider2D> others = new List<Collider2D>();
    Physics2D.OverlapCollider(GetComponent<Collider2D>(), others);
    others = others.Where(o => o.GetComponent<TetrisBlock>() != null && o.GetComponent<TetrisBlock>().isStatic).ToList();

    if (others.Count > 0) {
      isStatic = true;
      TetrisManager.Instance.RegisterCombine(gameObject);
    } else {
      GetComponent<SpriteRenderer>().color = Color.gray;
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

  private void OnBecameInvisible() {
    TetrisManager.Instance.RegisterOutOfBounds(gameObject);
  }
}
