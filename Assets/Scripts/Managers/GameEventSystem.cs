using System;
using UnityEngine;

public class GameEventSystem : MonoBehaviour
{
  public static GameEventSystem Instance;

  private void Awake() {
    if (Instance == null) {
      Instance = this;
    } else {
      Destroy(gameObject);
    }
  }

  public event Action<string> OnActionCompleted;

  public void TriggerActionCompleted(string actionId) {
    OnActionCompleted?.Invoke(actionId);
  }
}
