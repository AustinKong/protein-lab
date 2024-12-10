using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
  public static DialogueManager Instance;

  [SerializeField] TMP_Text characterNameText;
  [SerializeField] TMP_Text dialogueText;
  [SerializeField] Button nextButton;

  private DialogueData dialogueData;
  private DialogueScene currentScene;
  private int currentDialogueIndex;

  private void Awake() {
    if (Instance == null) {
      Instance = this;
      DontDestroyOnLoad(gameObject);
    } else {
      Destroy(gameObject);
    }
    LoadDialogueData();
  }

  public void Start() {
    nextButton.onClick.AddListener(NextDialogue);
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

  private void NextDialogue() {
    currentDialogueIndex = Math.Min(currentDialogueIndex + 1, currentScene.dialogues.Count - 1);
    DisplayDialogue();
  }

  /*
  private void PreviousDialogue() {
    currentDialogueIndex = Math.Max(currentDialogueIndex - 1, 0);
    DisplayDialogue();
  }
  */

  private void DisplayDialogue() {
    Dialogue dialogue = currentScene.dialogues[currentDialogueIndex];
    characterNameText.text = dialogue.character;
    dialogueText.text = dialogue.text;
  }
}
