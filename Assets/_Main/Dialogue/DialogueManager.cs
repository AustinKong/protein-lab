using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
  [Header("References")]
  [SerializeField] private TMP_Text characterNameText;
  [SerializeField] private TMP_Text dialogueText;
  [SerializeField] private Image characterImage;
  [SerializeField] private Image nextIcon;
  [SerializeField] private TextAsset jsonFile;

  [Header("Characters")]
  [SerializeField] private Character[] characters;

  private DialogueData dialogueData;
  private Queue<Dialogue> currentScene;

  private void Start() {
    LoadDialogueData();
    StartDialogue(SceneManager.Instance.GetActiveScene());
    GameEventSystem.Instance.OnActionCompleted += TryDisplayDialogue;
  }

  private void LoadDialogueData() {
    if (jsonFile == null) Debug.LogError("Dialogue file not found");
    dialogueData = JsonUtility.FromJson<DialogueData>(jsonFile.text);
  }

  void StartDialogue(string sceneName) {
    currentScene = new Queue<Dialogue>(dialogueData.scenes.Find(scene => scene.sceneName == sceneName).dialogues);
    if (currentScene == null || currentScene.Count == 0) {
      Debug.LogError($"Dialogue scene with name {sceneName} not found");
      return;
    }
    TryDisplayDialogue();
  }

  public void NextDialogue() {
    TryDisplayDialogue();
  }

  // Try to display for user clicked dialogue
  private void TryDisplayDialogue() {
    if (currentScene.Count == 0 || currentScene.Peek().triggerEventType != TriggerEventType.None) {
      return;
    }
    DisplayDialogue();
  }

  // Try to display for action triggered dialogue
  private void TryDisplayDialogue(string actionId) {
    // Allows for skipping forwards if the user has not manually done so
    bool hasDialogue = false;
    foreach (Dialogue dialogue in currentScene) {
      if (dialogue.triggerEventType == TriggerEventType.Action && dialogue.triggerEvent == actionId) {
        hasDialogue = true;
      }
    }

    if (hasDialogue) {
      Dialogue dialogue = currentScene.Peek();
      while (!(dialogue.triggerEventType == TriggerEventType.Action && dialogue.triggerEvent == actionId)) {
        DisplayDialogue();
        dialogue = currentScene.Peek();
      }
    }
  }

  // Internal helper function to abstract common functionality
  private void DisplayDialogue() {
    Dialogue dialogue = currentScene.Dequeue();
    characterNameText.text = dialogue.character;
    dialogueText.text = dialogue.text;

    // Display sprite with correct aspect ratio
    Sprite sprite = characters.Where((c) => c.name.ToLower() == dialogue.character.ToLower()).ToList().ElementAt(0).sprite;
    characterImage.sprite = sprite;
    characterImage.rectTransform.sizeDelta = new Vector2(300, sprite.bounds.size.y / sprite.bounds.size.x * 300);

    // Show next icon if player needs to click to display next dialogue
    if (currentScene.Count > 0 && currentScene.Peek().triggerEventType == TriggerEventType.None) {
      nextIcon.gameObject.SetActive(true);
    } else {
      nextIcon.gameObject.SetActive(false);
    }
  }
}
