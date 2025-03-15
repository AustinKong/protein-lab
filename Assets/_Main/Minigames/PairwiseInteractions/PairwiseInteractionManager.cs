using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PairwiseInteractionManager : MonoBehaviour
{
  public static PairwiseInteractionManager Instance;

  private void Awake() {
    Instance = this;
    mainCamera = Camera.main;
    inputActions = InputManager.Instance.inputActions;
    clickAction = inputActions.FindAction("Game/Click");
    pointAction = inputActions.FindAction("Game/Point");
  }

  [SerializeField] private Collider2D[] sourceProteins;
  [SerializeField] private Collider2D[] destinationProteins;
  [SerializeField] private LineRenderer linePrefab;

  private InputActionAsset inputActions;
  private InputAction clickAction, pointAction;
  private Camera mainCamera;

  private void OnEnable() {
    clickAction.started += OnDragStart;
    clickAction.canceled += OnDragEnd;
  }

  private void OnDisable() {
    clickAction.started -= OnDragStart;
    clickAction.canceled -= OnDragEnd;
  }

  private int selectedSource = -1;
  private LineRenderer selectedLineRenderer = null;

  private void OnDragStart(InputAction.CallbackContext context) {
    Collider2D hitCollider = Physics2D.OverlapPoint(GetPointerWorldPosition());
    if (hitCollider != null && sourceProteins.Contains(hitCollider)) {
      selectedSource = Array.IndexOf(sourceProteins, hitCollider);
      selectedLineRenderer = Instantiate(linePrefab.gameObject, hitCollider.transform.position, Quaternion.identity).GetComponent<LineRenderer>();
      selectedLineRenderer.SetPosition(0, hitCollider.transform.position);
    }
  }

  private void Update() {
    if (selectedLineRenderer != null) {
      selectedLineRenderer.SetPosition(1, GetPointerWorldPosition());
    }
  }

  private void OnDragEnd(InputAction.CallbackContext context) {
    Collider2D hitCollider = Physics2D.OverlapPoint(GetPointerWorldPosition());
    if (hitCollider != null && destinationProteins.Contains(hitCollider)) {
      int destination = Array.IndexOf(destinationProteins, hitCollider);
      selectedLineRenderer.SetPosition(1, hitCollider.transform.position);
    } else {
      Destroy(selectedLineRenderer.gameObject);
    }
    selectedSource = -1;
    selectedLineRenderer = null;
  }

  private Vector3 GetPointerWorldPosition() {
    Vector3 screenPosition = pointAction.ReadValue<Vector2>();
    Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 0));
    return (Vector2) worldPosition;
  }

}
