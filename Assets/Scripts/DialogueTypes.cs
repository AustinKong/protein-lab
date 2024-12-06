using System.Collections.Generic;

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