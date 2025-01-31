using UnityEngine;

public class OrientationCheck : MonoBehaviour
{
  [SerializeField] private GameObject panel;
  private Camera mainCamera;

  private void Start() {
    mainCamera = Camera.main;
  }

  private void Update() {
    if (IsLandscape()) {
      panel.SetActive(false);
    } else {
      panel.SetActive(true);
    }
  }

  private bool IsLandscape() {
    return mainCamera.aspect > 1;
  }
}
