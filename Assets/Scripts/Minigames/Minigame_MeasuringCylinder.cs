using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Minigame_MeasuringCylinder : MonoBehaviour
{
  // Assume endless supply of original liquid
  [SerializeField] private RectTransform originalContainer;
  [SerializeField] private TMP_Text contentUnitsDisplay; // Temporary until i get the water level setup
  [SerializeField] private InputActionAsset inputActions;

  private float contentUnits = 0;
  private bool isPouring = false;
  private InputAction clickAction;

  private const float POUR_SPEED = 20f;
  private const int POUR_INCREMENTS = 5;

  private void Awake() {
    clickAction = inputActions.FindAction("Game/Click");
  }

  private void OnEnable() {
    clickAction.started += OnPourStart;
    clickAction.canceled += OnPourEnd;
  }

  private void OnDisable() {
    clickAction.started -= OnPourStart;
    clickAction.canceled -= OnPourEnd;
  }

  private void OnDestroy() {
    clickAction.started -= OnPourStart;
    clickAction.canceled -= OnPourEnd;
  }

  private void OnPourStart(InputAction.CallbackContext context) {
    isPouring = true;
  }

  private void OnPourEnd(InputAction.CallbackContext context) {
    isPouring = false;
    SceneManager.Instance.ReturnToOriginalScene(Mathf.RoundToInt(contentUnits / POUR_INCREMENTS) * POUR_INCREMENTS);
  }

  private void Update() {
    if (isPouring) Pour();
    else {
      originalContainer.rotation = Quaternion.identity;
    }
  }

  public void Pour() {
    originalContainer.rotation = Quaternion.Euler(0, 0, 130f);
    contentUnits += POUR_SPEED * Time.deltaTime;

    contentUnitsDisplay.text = "" + Mathf.RoundToInt(contentUnits / POUR_INCREMENTS) * POUR_INCREMENTS;
  }
}
