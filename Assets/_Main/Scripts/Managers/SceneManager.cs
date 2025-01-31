using System;
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
  private Action<int> preAdditiveSceneCallback;

  private const float TRANSITION_DURATION = 1f;

  private void Awake() {
    if (Instance == null) {
      Instance = this; 
      if (transform.parent != null) transform.SetParent(null);
      // This line is for development, such that we can open any scene to test
      currentBaseScene = GetActiveScene();
      DontDestroyOnLoad(gameObject);
    } else {
      if (!Instance.isActiveAndEnabled) Instance.gameObject.SetActive(true);
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
    currentBaseScene = sceneName;
    UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    transitionAnimator.SetTrigger("Transition");
  }

  // Callback is when the result of a minigame will affect some state in the main game
  // An example of this is filling a measuring cylinder with water, the callback should
  // set the value of contentUnits in measuring cylinder based on the result provided by minigame
  // FIXME: This is currently unused, can be removed possibly
  public void LoadMinigameScene(string sceneName, Action<int> callback = null) {
    if (!string.IsNullOrEmpty(currentAdditiveScene)) {
      Debug.LogWarning("An additive scene is already loaded. Unload it first.");
      return;
    }

    preAdditiveSceneCallback = callback;
    StartCoroutine(LoadSceneAdditivelyRoutine(sceneName));
  }

  private IEnumerator LoadSceneAdditivelyRoutine(string sceneName) {
    transitionAnimator.SetTrigger("Transition");
    yield return new WaitForSeconds(TRANSITION_DURATION);
    currentAdditiveScene = sceneName;
    yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive);
    if (!string.IsNullOrEmpty(currentBaseScene)) {
      HideBaseScene();
    }
    transitionAnimator.SetTrigger("Transition");
  }

  public void ReturnToOriginalScene(int result = -1) {
    if (string.IsNullOrEmpty(currentBaseScene)) {
      Debug.LogError("No original scene found.");
      return;
    }

    StartCoroutine(ReturnToOriginalSceneRoutine(result));
  }

  private IEnumerator ReturnToOriginalSceneRoutine(int result = -1) {
    transitionAnimator.SetTrigger("Transition");
    yield return new WaitForSeconds(TRANSITION_DURATION);
    ShowBaseScene();
    yield return UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(currentAdditiveScene);  
    currentAdditiveScene = null;
    transitionAnimator.SetTrigger("Transition");

    if (result >= 0 && preAdditiveSceneCallback != null) {
      preAdditiveSceneCallback(result);
      preAdditiveSceneCallback = null;
    }
  }

  public string GetActiveScene() {
    if (!string.IsNullOrEmpty(currentAdditiveScene)) {
      return currentAdditiveScene;
    } else if (!string.IsNullOrEmpty(currentBaseScene)) {
      return currentBaseScene;
    } else {
      // Fallback, it should never reach this unless I fuked up
      return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
    }
  }

  private void HideBaseScene() {
    UnityEngine.SceneManagement.Scene baseScene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(currentBaseScene);
    foreach (GameObject obj in baseScene.GetRootGameObjects()) {
      if (!obj.CompareTag("Manager")) {
        obj.SetActive(false);
      }
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
