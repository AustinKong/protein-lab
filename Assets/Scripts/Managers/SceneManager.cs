using System.Collections;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
  public static SceneManager Instance;

  [SerializeField] private Animator transitionAnimator;

  private const float TRANSITION_DURATION = 1f;

  private void Awake() {
    if (Instance == null) {
      Instance = this;
    } else {
      Destroy(gameObject);
    }
  }

  public void LoadScene(string sceneName) {
    StartCoroutine(LoadSceneWithTransition(sceneName));
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

  private IEnumerator LoadSceneWithTransition(string sceneName) {
    transitionAnimator.SetTrigger("Start");
    yield return new WaitForSeconds(TRANSITION_DURATION);
    UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
  }
}
