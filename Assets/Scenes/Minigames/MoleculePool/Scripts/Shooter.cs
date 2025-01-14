using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shooter : MonoBehaviour
{
  [SerializeField] private GameObject moleculePrefab;
  [SerializeField] private Transform crosshairTransform;

  private InputActionAsset inputActions;
  private Camera mainCamera;
  private InputAction tapAction;

  private float angle = 0;

  private const float ROTATION_SPEED = 35f;
  private const float ROTATION_ARC = 180f;

  private void Awake() {
    mainCamera = Camera.main;
    inputActions = InputManager.Instance.inputActions;
    tapAction = inputActions.FindAction("Game/Tap");
  }

  private void Start() {
    StartCoroutine(RotateRoutine());
    tapAction.performed += OnTapCallback;
  }

  private void OnTapCallback(InputAction.CallbackContext context) {
    Molecule molecule = Instantiate(moleculePrefab, transform.position, Quaternion.Euler(0, 0, angle)).GetComponent<Molecule>();
    molecule.Shoot();
  }

  private IEnumerator RotateRoutine() {
    while (true) {
      while (angle < ROTATION_ARC / 2) {
        angle += ROTATION_SPEED * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        PositionCrosshair();
        yield return null;
      }
      while (angle > ROTATION_ARC / -2) {
        angle -= ROTATION_SPEED * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        PositionCrosshair();
        yield return null;
      }
    }
  }

  private void PositionCrosshair() {
    RaycastHit2D hit = Physics2D.Raycast(transform.position, Quaternion.Euler(0, 0, angle) * Vector3.up, 30f);

    if (hit.collider != null && hit.collider.GetComponent<Molecule>()) {
      crosshairTransform.position = hit.point;
    } else {
      crosshairTransform.position = new Vector3(999, 999, 0);
    }
  }
}
