using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/*
  To the person who would be maintaining this. This script uses a lot of lazy shortcuts to save development time. Mainly:
  1. Finding all references to spriteRenderer, label, labelText relative to this using GetComponentInChildren
  2. Finding reference to mainCamera and inputActions using dependency injection
  3. Dynamically changing the label text and size on Start
  4. Dynamically changing BoxCollider2D bounds based on SpriteRenderer bounds

  If ever there are performance problems, this script could be optimized a lot by changing all these runtime references to
  compile time references via Unity Editor directly

  To correctly initialize the Interactable object, follow this hierarchy:

  Item (Box Collider, Rigidbody 2D, Interactable script)
  ├─ Label (UI Canvas, disabled on start)
  │  ├─ Panel (With horizontal layout group)
  │  │  ├─ Spacer (Whitespace)
  │  │  ├─ Label Text
  │  │  ├─ Spacer
  ├─ Sprite (SpriteRenderer, Animator)

  Good thing about this is that we do not need to tweak the size of the box collider nor the position of the label.
*/

/// <summary>
/// Simple base class for all game entities which can be clicked, and has a label.
/// IMPORTANT: Remember to override Setup and Cleanup to add new cursor events
/// Generally, all the non-virtual methods should not be overriden.
/// </summary>
public class Interactable : MonoBehaviour
{
  [Header("Interactable Properties")]
  [SerializeField] protected string itemName;

  protected TMP_Text labelText;
  protected SpriteRenderer spriteRenderer;
  private RectTransform label;
  private InputActionAsset inputActions;
  private Camera mainCamera;
  protected InputAction clickAction, pointAction, deltaAction, tapAction;

  protected bool isDragging = false;

  protected virtual void Setup() {
    tapAction.performed += OnTapCallback;
    clickAction.started += OnDragStartCallback;
    clickAction.canceled += OnDragEndCallback;
  }

  protected virtual void Cleanup() {
    tapAction.performed -= OnTapCallback;
    clickAction.started -= OnDragStartCallback;
    clickAction.canceled -= OnDragEndCallback;
  }

  protected virtual void OnTap() {}

  protected virtual void OnDragStart() {
    isDragging = true;
    spriteRenderer.sortingOrder = 100;
  }

  protected virtual void OnDragEnd() {
    isDragging = false;
    spriteRenderer.sortingOrder = 0;
  }

  #region Callback abstractions
  private void OnTapCallback(InputAction.CallbackContext context) {
    if (!IsOver()) return;
    OnTap();
  }

  private void OnDragStartCallback(InputAction.CallbackContext context) {
    if (!IsOver()) return;
    OnDragStart();
  }

  private void OnDragEndCallback(InputAction.CallbackContext context) {
    if (!isDragging) return;
    OnDragEnd();
  }
  #endregion Callback abstractions

  #region Unity lifecycle methods
  protected virtual void Awake() {
    mainCamera = Camera.main;
    labelText = transform.GetComponentsInChildren<TMP_Text>(true)[0];
    spriteRenderer = transform.GetComponentsInChildren<SpriteRenderer>()[0];
    label = transform.GetComponentInChildren<RectTransform>(true);
    inputActions = InputManager.Instance.inputActions;
    
    pointAction = inputActions.FindAction("Game/Point");
    clickAction = inputActions.FindAction("Game/Click");
    deltaAction = inputActions.FindAction("Game/Delta");
    tapAction = inputActions.FindAction("Game/Tap");
  }

  private void Start() {
    label.gameObject.SetActive(true);
    SetLabel(itemName);
    transform.GetComponent<BoxCollider2D>().size = spriteRenderer.sprite.bounds.size;
    transform.GetComponent<BoxCollider2D>().offset = Vector2.zero;
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
  #endregion Unity lifecycle methods

  #region Helper functions
  // Returns true if the pointer is over this gameobject's collider
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

  protected Interactable[] GetNearbyInteractables() {
    Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 1f);
    return hitColliders.Where(c => c.CompareTag("Interactable") && c.gameObject != gameObject).Select(c => c.GetComponent<Interactable>()).ToArray();
  }

  protected void SetLabel(string text, bool force = false) {
    if (labelText.text == text && !force) return;
    labelText.text = text;
    LayoutRebuilder.ForceRebuildLayoutImmediate(labelText.rectTransform);
    label.position = new Vector3(transform.position.x, 0.1f + spriteRenderer.bounds.max.y, 0);
  }
  #endregion Helper functions
}
