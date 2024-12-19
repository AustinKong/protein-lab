using UnityEngine;

/// <summary>
/// Generic class for items that hold an infinite amount of other items.x
/// To create an instance of one of its contents, simply click and drag upwards.
/// </summary>
public class InfiniteSolidsContainer : Interactable
{
  [Header("Infinite Container Properties")]
  [SerializeField] private bool hasLid = false;
  [SerializeField] private Draggable contentsPrefab;

  private bool isOpen = false;
  private float dragStartY;

  // Distance in world units which the user has to drag upwards to take an item from this container
  private const float RETRIEVE_DELTA_Y = 1f;

  protected override void OnTap() {
    base.OnTap();
    if (hasLid) isOpen = !isOpen;
  }

  protected override void OnDragStart() {
    base.OnDragStart();
    dragStartY = GetPointerWorldPosition().y;
  }

  protected override void OnDragEnd() {
    base.OnDragEnd();
    if ((!hasLid || isOpen) && GetPointerWorldPosition().y - dragStartY > RETRIEVE_DELTA_Y) {
      Instantiate(contentsPrefab.gameObject, transform.position + Vector3.up * 2f, Quaternion.identity);
    }
  }
}
