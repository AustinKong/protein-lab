using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue {
  // Open to future extension
  public string character;
  public string text;
}

[System.Serializable]
public class DialogueScene {
  public string sceneName;
  public List<Dialogue> dialogues;
}

[System.Serializable]
public class DialogueData {
  public List<DialogueScene> scenes;
}

// Default dictionary is non-serializable
[System.Serializable]
public struct Character {
  public string name;
  public Sprite sprite;
}