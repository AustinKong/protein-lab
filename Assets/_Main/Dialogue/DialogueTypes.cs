using System.Collections.Generic;
using UnityEngine;

public enum TriggerEventType {
  None, // Player has to manually click next
  Action // Auto transitions to this event when action is met
}

[System.Serializable]
public class Dialogue {
  public string character;
  public string text;
  public TriggerEventType triggerEventType;
  public string triggerEvent;
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