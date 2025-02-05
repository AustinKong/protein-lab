using UnityEngine;
using UnityEngine.UI;

public class StoryManager : MonoBehaviour
{
  [SerializeField] private Image image;
  [SerializeField] private string nextSceneName;
  [SerializeField] private GameObject nextIcon;

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
      Transition();
    }

    bool hasNext = Resources.Load<Sprite>($"Story/{SceneManager.Instance.GetActiveScene()}/{string.Format("{0:00}", currentPage + 1)}");
    if (hasNext) {
      nextIcon.SetActive(true);
    } else {
      nextIcon.SetActive(false);
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

  private void Transition() {
    isEnabled = false;
    SceneManager.Instance.LoadScene(nextSceneName);
  }
}
