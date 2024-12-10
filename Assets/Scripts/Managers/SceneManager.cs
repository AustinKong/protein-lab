using System.Collections;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
  public static SceneManager Instance;

  [SerializeField] private Animator transitionAnimator;

  // NOTE: Current system only supports two scenes at one time. If more scenes are required,
  // A stack of strings can be used. With the topmost scene in the stack always active. everything else
  // Should be hidden. If a scene is loaded additvely, push it onto the stack and hide others
  // If a scene is loaded normally, empty the stack, unload all previous scenes, and load the new one
  private string currentBaseScene;
  private string currentAdditiveScene;

  private const float TRANSITION_DURATION = 1f;

  private void Awake() {
    if (Instance == null) {
      Instance = this;
      DontDestroyOnLoad(gameObject);
    } else {
      Destroy(gameObject);
    }
  }

  public void LoadScene(string sceneName) {
    if (!string.IsNullOrEmpty(currentAdditiveScene)) {
      UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(currentAdditiveScene);
      currentAdditiveScene = null;
    }

    StartCoroutine(LoadSceneRoutine(sceneName));
  }

  private IEnumerator LoadSceneRoutine(string sceneName) {
    transitionAnimator.SetTrigger("Transition");
    yield return new WaitForSeconds(TRANSITION_DURATION);
    UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    currentBaseScene = sceneName;
    transitionAnimator.SetTrigger("Transition");
  }

  public void LoadSceneAdditively(string sceneName) {
    if (!string.IsNullOrEmpty(currentAdditiveScene)) {
      Debug.LogWarning("An additive scene is already loaded. Unload it first.");
      return;
    }

    StartCoroutine(LoadSceneAdditivelyRoutine(sceneName));
  }

  private IEnumerator LoadSceneAdditivelyRoutine(string sceneName) {
    transitionAnimator.SetTrigger("Transition");
    yield return new WaitForSeconds(TRANSITION_DURATION);
    UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive);
    currentAdditiveScene = sceneName;
    if (!string.IsNullOrEmpty(currentBaseScene)) {
      HideBaseScene();
    }
    transitionAnimator.SetTrigger("Transition");
  }

  public void ReturnToOriginalScene() {
    if (string.IsNullOrEmpty(currentBaseScene)) {
      Debug.LogError("No original scene found.");
      return;
    }

    StartCoroutine(ReturnToOriginalSceneRoutine());
  }

  private IEnumerator ReturnToOriginalSceneRoutine() {
    transitionAnimator.SetTrigger("Transition");
    yield return new WaitForSeconds(TRANSITION_DURATION);
    ShowBaseScene();
    yield return UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(currentAdditiveScene);  
    currentAdditiveScene = null;
    transitionAnimator.SetTrigger("Transition");
  }

  public string GetActiveScene() {
    return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
  }

  private void HideBaseScene() {
    UnityEngine.SceneManagement.Scene baseScene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(currentBaseScene);
    foreach (GameObject obj in baseScene.GetRootGameObjects()) {
      obj.SetActive(false);
    }
  }

  private void ShowBaseScene() {
    UnityEngine.SceneManagement.Scene baseScene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(currentBaseScene);
    Debug.Log(baseScene);
    foreach (GameObject obj in baseScene.GetRootGameObjects()) {
      obj.SetActive(true);
    }
  }
}
