using UnityEngine;

public class SceneManager : MonoBehaviour
{
  public static SceneManager Instance;

  private void Awake() {
    if (Instance == null) {
      Instance = this;
      DontDestroyOnLoad(gameObject);
    } else {
      Destroy(gameObject);
    }
  }

  public void QuitGame() {
    Application.Quit();
  }
}
