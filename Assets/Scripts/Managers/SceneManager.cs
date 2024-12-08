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

  public void LoadScene(string sceneName) {
    UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
  }

  public void ReloadCurrentScene() {
    UnityEngine.SceneManagement.SceneManager.LoadScene(GetCurrentScene());
  }

  public string GetCurrentScene() {
    return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
  }

  public void QuitGame() {
    Application.Quit();
  }
}
