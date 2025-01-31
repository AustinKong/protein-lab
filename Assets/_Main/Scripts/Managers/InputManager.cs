using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
  public static InputManager Instance;

  private void Awake() {
    if (Instance == null) {
      Instance = this;
      if (transform.parent != null) transform.SetParent(null);
      DontDestroyOnLoad(gameObject);
    } else {
      if (!Instance.isActiveAndEnabled) Instance.gameObject.SetActive(true);
      Destroy(gameObject);
    }
  }

  public InputActionAsset inputActions;
}
