using System.Linq;
using UnityEngine;

/// <summary>
/// Generic class for containers that contain a liquid or solution.
/// Supports mixing with another valid Interactable defined in CombinationRules
/// </summary>
public class LiquidContainer : Draggable, IConsumer
{
  [Header("Liquid Container Properties")]
  [SerializeField] private string contents;

  private Animator animator;

  protected override void Awake() {
    base.Awake();
    animator = GetComponentInChildren<Animator>();
  }

  protected override void Update() {
    base.Update();
    if (string.IsNullOrEmpty(contents)) {
      SetLabel(GetItemName());
    } else {
      SetLabel(contents);
    }
  }

  protected override void OnDragEnd() {
    base.OnDragEnd();

    IConsumer[] consumers = GetNearbyInteractables()
      .Where(i => i is IConsumer c && c.CanConsume(contents))
      .Select(i => i as IConsumer).ToArray();

    if (consumers.Length > 0) {
      string method = consumers[0].Consume(contents);
      if (method == "Contents") {
        contents = null;
        animator.SetTrigger("Pour");
      } else {
        Destroy(gameObject);
      }
    }
  }

  public bool CanConsume(string otherItemName) => string.IsNullOrEmpty(contents) || CombinationRules.GetCombinationResult(contents, otherItemName) != null;

  public string Consume(string otherItemName) {
    if (string.IsNullOrEmpty(contents)) {
      contents = otherItemName;
      GameEventSystem.Instance.TriggerActionCompleted("onItemInput");
    } else {
      contents = CombinationRules.GetCombinationResult(contents, otherItemName);
      GameEventSystem.Instance.TriggerActionCompleted("onSolutionMix");
    }
    return "Contents";
  }
}
