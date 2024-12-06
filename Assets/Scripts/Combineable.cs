using UnityEngine;
using UnityEngine.InputSystem;

public class Combineable : Draggable
{
  public string itemName;

  private Combineable target;

  protected override void Update() {
    base.Update();
    if (isDragging && target != null) {
      // Do highlighting + size increase
      // TODO: Update this
      this.GetComponent<SpriteRenderer>().size = new Vector2(1.2f, 1.2f);
    } else {
      this.GetComponent<SpriteRenderer>().size = Vector2.one;
    }
  }

  protected override void OnDragEnd(InputAction.CallbackContext context) {
    if (isDragging && target != null) {
      Debug.Log(this.itemName + " + " + target.itemName);
      string result = CombinationRules.GetCombinationResult(this.itemName, target.itemName);
      GameObject newItem = Instantiate(Resources.Load<GameObject>($"Items/{result}"), target.transform.position, Quaternion.identity);
      ParticlePoolManager.Instance.PlayParticle("Star", transform.position);

      Destroy(target.gameObject);
      Destroy(gameObject);
    }
    base.OnDragEnd(context);
  }

  private void OnTriggerEnter2D(Collider2D other) {
    if (other.TryGetComponent<Combineable>(out Combineable otherCombineable)) {
      string result = CombinationRules.GetCombinationResult(itemName, otherCombineable.itemName);

      if (!string.IsNullOrEmpty(result)) {
        target = otherCombineable;
      }
    }
  }

  private void OnTriggerExit2D(Collider2D other) {
    if (other.TryGetComponent<Combineable>(out Combineable otherCombineable)) {
      if (otherCombineable == target) {
        target = null;
      }
    }
  }
}
