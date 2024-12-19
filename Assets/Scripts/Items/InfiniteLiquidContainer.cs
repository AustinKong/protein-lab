using UnityEngine;
using System.Linq;

public class InfiniteLiquidContainer : Draggable
{
  [Header("Infinite Liquid Container Properties")]
  [SerializeField] private string contents;

  private Animator animator;

  protected override void Awake() {
    base.Awake();
    animator = GetComponentInChildren<Animator>(); 
  }

  protected override void OnDragEnd() {
    base.OnDragEnd();

    IConsumer[] consumers = GetNearbyInteractables().Where(i => i is IConsumer consumer && consumer.CanConsume(contents)).Select(i => i as IConsumer).ToArray();
    if (consumers.Length > 0) {
      consumers[0].Consume(contents);
      animator.SetTrigger("Pour");
    }
  }
}