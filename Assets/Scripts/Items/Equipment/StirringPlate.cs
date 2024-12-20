using UnityEngine;

// Possibly change to have a collider on where the player should place the item
// Then only detect if an item is there when this is clicked
public class StirringPlate : Interactable, IConsumer
{
  [SerializeField] private Sprite preConsumeSprite;
  [SerializeField] private Sprite postConsumeSprite;
  [SerializeField] private Draggable postMixPrefab;

  private float dragStartY;
  private const float RETRIEVE_DELTA_Y = 1f;
  private bool hasContents = false;

  public bool CanConsume(string otherItemName) => otherItemName == "Unmixed crude buffer solution";
  
  public string Consume(string otherItemName) {
    spriteRenderer.sprite = postConsumeSprite;
    hasContents = true;
    SetLabel(itemName);
    return "Container";
  }

  protected override void OnTap() {
    base.OnTap();
    if (hasContents) SceneManager.Instance.LoadMinigameScene("Mixing");
  }

  protected override void OnDragStart() {
    base.OnDragStart();
    dragStartY = GetPointerWorldPosition().y;
  }

  protected override void OnDragEnd() {
    base.OnDragEnd();
    if (GetPointerWorldPosition().y - dragStartY > RETRIEVE_DELTA_Y && hasContents) {
      Instantiate(postMixPrefab.gameObject, transform.position + Vector3.up * 2f, Quaternion.identity);
      spriteRenderer.sprite = preConsumeSprite;
      hasContents = false;
    }
  }
}
