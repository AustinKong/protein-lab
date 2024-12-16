using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Generic dialogue manager
/// </summary>
public class DialogueManager : MonoBehaviour
{
  [Header("References")]
  [SerializeField] private TMP_Text characterNameText;
  [SerializeField] private TMP_Text dialogueText;
  [SerializeField] private Image characterImage;

  [Header("Characters")]
  [SerializeField] private Character[] characters;

  private DialogueData dialogueData;
  private DialogueScene currentScene;
  private int currentDialogueIndex;

  private void Start() {
    LoadDialogueData();
    StartDialogue(SceneManager.Instance.GetActiveScene());
  }

  private void LoadDialogueData() {
    TextAsset jsonFile = Resources.Load<TextAsset>("dialogueData");
    if (jsonFile == null) Debug.LogError("Dialogue file not found");
    dialogueData = JsonUtility.FromJson<DialogueData>(jsonFile.text);
  }

  public void StartDialogue(string sceneName) {
    currentScene = dialogueData.scenes.Find(scene => scene.sceneName == sceneName);
    if (currentScene == null) {
      Debug.LogError($"Dialogue scene with name {sceneName} not found");
      return;
    }
    currentDialogueIndex = 0;
    DisplayDialogue();
  }

  public void NextDialogue() {
    currentDialogueIndex = Math.Min(currentDialogueIndex + 1, currentScene.dialogues.Count - 1);
    DisplayDialogue();
  }

  public void PreviousDialogue() {
    currentDialogueIndex = Math.Max(currentDialogueIndex - 1, 0);
    DisplayDialogue();
  }

  private void DisplayDialogue() {
    Dialogue dialogue = currentScene.dialogues[currentDialogueIndex];
    characterNameText.text = dialogue.character;
    dialogueText.text = dialogue.text;
    characterImage.sprite = characters.Where((c) => c.name.ToLower() == dialogue.character.ToLower()).ToList().ElementAt(0).sprite;
  }
}
