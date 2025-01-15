using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
  [Header("References")]
  [SerializeField] private TMP_Text characterNameText;
  [SerializeField] private TMP_Text dialogueText;
  [SerializeField] private Image characterImage;

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
    TextAsset jsonFile = Resources.Load<TextAsset>("dialogueData");
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
    if (currentScene.Count == 0 || currentScene.Peek().triggerEventType != TriggerEventType.Action || currentScene.Peek().triggerEvent != actionId) {
      return;
    }
    DisplayDialogue();
  }

  // Internal helper function to abstract common functionality
  private void DisplayDialogue() {
    Dialogue dialogue = currentScene.Dequeue();
    characterNameText.text = dialogue.character;
    dialogueText.text = dialogue.text;
    characterImage.sprite = characters.Where((c) => c.name.ToLower() == dialogue.character.ToLower()).ToList().ElementAt(0).sprite;
  }
}
