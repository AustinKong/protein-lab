using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoryManager : MonoBehaviour
{
  [Header("References")]
  [SerializeField] private Image image;
  [SerializeField] private GameObject nextIcon;
  [SerializeField] private GameObject choicePanel;
  [SerializeField] private Button[] choiceButtons;

  [Header("Data")]
  [SerializeField] private StoryNode currentNode;
  [SerializeField] private string nextSceneName;

  private void Start() {
    DisplayNode(currentNode);
  }

  private void DisplayNode(StoryNode node) {
    if (node == null) {
      SceneManager.Instance.LoadScene(nextSceneName);
      return;
    }

    currentNode = node;
    image.sprite = currentNode.storyImage;

    if (currentNode.choices.Count > 0) {
      ShowChoices();
    } else {
      choicePanel.SetActive(false);
    }

    if (currentNode.nextPage != null) {
      nextIcon.SetActive(true);
    } else {
      nextIcon.SetActive(false);
    }
  }

  private void ShowChoices() {
    choicePanel.SetActive(true);
    for (int i = 0; i < choiceButtons.Length; i++) {
      if (i < currentNode.choices.Count) {
        choiceButtons[i].gameObject.SetActive(true);
        choiceButtons[i].GetComponentInChildren<TMP_Text>().text = currentNode.choices[i].choiceText;

        choiceButtons[i].onClick.RemoveAllListeners();
        // Need to capture the reference to nextNode, if not the button will only use i = 4
        StoryNode nextNode = currentNode.choices[i].nextNode;
        choiceButtons[i].onClick.AddListener(() => {
          DisplayNode(nextNode);
        });
      } else {
        choiceButtons[i].gameObject.SetActive(false);
      }
    }
  }

  public void NextPage() {
    DisplayNode(currentNode.nextPage);
  }
}
