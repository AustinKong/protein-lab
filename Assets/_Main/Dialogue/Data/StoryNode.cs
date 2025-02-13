using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class StoryNode : ScriptableObject
{
  [Tooltip("The sprite representing this story page.")]
  public Sprite storyImage;

  [Tooltip("List of choices available on this page.")]
  public List<StoryChoice> choices;

  [Tooltip("The next page if there are no choices (linear progression).")]
  public StoryNode nextPage;
} 

[System.Serializable]
public class StoryChoice
{
  [Tooltip("The text displayed for the choice.")]
  public string choiceText;

  [Tooltip("The next story node this choice leads to.")]
  public StoryNode nextNode;
}

// To use
[System.Serializable]
public enum StoryTransition {
  Fade, 
}