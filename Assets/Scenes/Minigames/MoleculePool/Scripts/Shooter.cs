using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shooter : MonoBehaviour
{
  [SerializeField] private GameObject moleculePrefab;
  [SerializeField] private GameObject dotPrefab;

  private InputActionAsset inputActions;
  private Camera mainCamera;
  private InputAction tapAction;

  private float angle = 0;

  private const float ROTATION_SPEED = 25f;
  private const float ROTATION_ARC = 160f;

  private void Awake() {
    mainCamera = Camera.main;
    inputActions = InputManager.Instance.inputActions;
    tapAction = inputActions.FindAction("Game/Tap");
  }

  private void Start() {
    for (int i = 0; i < DOT_POOL_SIZE; i++) {
      dotPool[i] = Instantiate(dotPrefab);
    }
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
        DrawTrajectory();
        yield return null;
      }
      while (angle > ROTATION_ARC / -2) {
        angle -= ROTATION_SPEED * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        DrawTrajectory();
        yield return null;
      }
    }
  }

  private const int DOT_POOL_SIZE = 25;
  private const float MAX_DISTANCE = 20f;
  private GameObject[] dotPool = new GameObject[DOT_POOL_SIZE];

  private void DrawTrajectory() {
    Vector3 direction = Quaternion.Euler(0, 0, angle) * Vector3.up;
    Vector3 currentPosition = transform.position;
    bool hasBounced = false;
    int poolIndex = 0;

    while (poolIndex < DOT_POOL_SIZE) {
      Vector3 nextPosition = currentPosition + direction * MAX_DISTANCE / DOT_POOL_SIZE;

      RaycastHit2D hit = Physics2D.Linecast(currentPosition, nextPosition);
      if (hit.collider != null) {
        if (hit.collider.GetComponent<Molecule>() != null) {
          for (int i = poolIndex; i < DOT_POOL_SIZE; i++) {
            dotPool[i].transform.position = hit.point;
          }
          break;
        }

        if (!hasBounced) {
          hasBounced = true;
          direction = Vector3.Reflect(direction, hit.normal);
          nextPosition = hit.point;
          dotPool[poolIndex].transform.position = nextPosition;
          poolIndex++;
          nextPosition = nextPosition + direction * MAX_DISTANCE / DOT_POOL_SIZE;
        } else {
          dotPool[poolIndex].transform.position = hit.point;
          break;
        }
      }

      dotPool[poolIndex].transform.position = nextPosition;
      poolIndex++;

      currentPosition = nextPosition;
    }
  }
}
