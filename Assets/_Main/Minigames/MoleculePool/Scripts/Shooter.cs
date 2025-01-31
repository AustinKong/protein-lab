using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shooter : MonoBehaviour
{
  [SerializeField] private Molecule[] moleculePrefabs;

  private Molecule nextMolecule;
  private InputActionAsset inputActions;
  private InputAction tapAction;

  private float angle = 0;
  private float lastShot = 0;

  private const float ROTATION_SPEED = 35f;
  private const float ROTATION_ARC = 160f;
  private const float SHOOTING_COOLDOWN = 0.8f;

  private void Awake() {
    nextMolecule = Instantiate(moleculePrefabs[Random.Range(0, moleculePrefabs.Length)], transform.position, Quaternion.identity).GetComponent<Molecule>();
    inputActions = InputManager.Instance.inputActions;
    tapAction = inputActions.FindAction("Game/Tap");
  }

  private void Start() {
    StartCoroutine(RotateRoutine());
    tapAction.performed += OnTapCallback;
  }

  private void Update() {
    if (Time.time - lastShot > SHOOTING_COOLDOWN && nextMolecule == null) {
      nextMolecule = Instantiate(moleculePrefabs[Random.Range(0, moleculePrefabs.Length)], transform.position, Quaternion.identity).GetComponent<Molecule>();
    }
  }

  private void OnTapCallback(InputAction.CallbackContext context) {
    if (Time.time - lastShot < SHOOTING_COOLDOWN) return;
    nextMolecule.Shoot();
    nextMolecule = null;
    lastShot = Time.time;
  }

  private IEnumerator RotateRoutine() {
    while (true) {
      while (angle < ROTATION_ARC / 2) {
        angle += ROTATION_SPEED * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        if (nextMolecule != null) nextMolecule.transform.rotation = transform.rotation;
        yield return null;
      }
      while (angle > ROTATION_ARC / -2) {
        angle -= ROTATION_SPEED * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        if (nextMolecule != null) nextMolecule.transform.rotation = transform.rotation;
        yield return null;
      }
    }
  }
}
