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
      SetLabel(itemName);
    } else {
      SetLabel(contents);
    }
  }

  protected override void OnDragEnd() {
    base.OnDragEnd();

    IConsumer[] consumers = GetNearbyInteractables().Where(i => i is IConsumer consumer && consumer.CanConsume(contents)).Select(i => i as IConsumer).ToArray();
    if (consumers.Length > 0) {
      consumers[0].Consume(contents);
      contents = null;
    }
  }

  public bool CanConsume(string otherItemName) => string.IsNullOrEmpty(contents) || CombinationRules.GetCombinationResult(contents, otherItemName) != null;

  public void Consume(string otherItemName) {
    if (!CanConsume(otherItemName)) return;

    if (string.IsNullOrEmpty(contents)) {
      contents = otherItemName;
    } else {
      contents = CombinationRules.GetCombinationResult(contents, otherItemName);
    }
  }
}
