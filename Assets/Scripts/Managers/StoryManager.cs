using UnityEngine;
using UnityEngine.UI;

public class StoryManager : MonoBehaviour
{
  [SerializeField] private Image image;
  [SerializeField] private string nextSceneName;

  private bool isEnabled = true;

  private int currentPage = 0;

  private void Start() {
    NextPage();
  }

  public void NextPage() {
    if (!isEnabled) return;

    currentPage++;
    Sprite sprite = Resources.Load<Sprite>($"Story/{SceneManager.Instance.GetActiveScene()}/{string.Format("{0:00}", currentPage)}");
    if (sprite != null) {
      image.sprite = sprite;
    } else {
      TransitionToGame();
    }
  }

  public void PreviousPage() {
    if (!isEnabled) return;

    currentPage = Mathf.Max(currentPage - 1, 1);
    Sprite sprite = Resources.Load<Sprite>($"Story/{SceneManager.Instance.GetActiveScene()}/{string.Format("{0:00}", currentPage)}");
    if (sprite != null) {
      image.sprite = sprite;
    }
  }

  private void TransitionToGame() {
    isEnabled = false;
    SceneManager.Instance.LoadScene(nextSceneName);
  }
}
