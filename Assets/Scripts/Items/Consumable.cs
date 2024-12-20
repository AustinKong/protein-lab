using System.Collections.Concurrent;
using System.Linq;

/// <summary>
/// A generic class for Interactables that are to be consumed as a whole form
/// </summary>
public class Consumable : Draggable
{
  protected override void OnDragEnd() {
    base.OnDragEnd();

    IConsumer[] consumers = GetNearbyInteractables()
      .Where(i => i is IConsumer c && c.CanConsume(GetItemName()))
      .Select(i => i as IConsumer).ToArray();

    if (consumers.Length > 0) {
      consumers[0].Consume(GetItemName());
      Destroy(gameObject);
    }
  }
}
