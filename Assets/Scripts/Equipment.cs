using UnityEngine.InputSystem;

public class Equipment : Combineable
{
  public string minigameSceneName;

  protected override void OnDestroy() {
    base.OnDestroy();
    SceneManager.Instance.LoadScene(minigameSceneName);
  }
}
