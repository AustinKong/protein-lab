using UnityEngine;
using UnityEngine.InputSystem;

public class Combineable : Draggable
{
  public string itemName;
  public string actionId;

  protected Combineable target;

  protected override void Update() {
    base.Update();
    if (target != null && target.target == this) {
      transform.localScale = Vector2.one * 1.1f;
    } else {
      transform.localScale = Vector2.one;
    }
  }

  protected override void OnDragEnd(InputAction.CallbackContext context) {
    if (isDragging && target != null) {
      string result = CombinationRules.GetCombinationResult(this.itemName, target.itemName);
      Vector3 newPosition = (target.transform.position + this.transform.position) / 2f;
      Instantiate(Resources.Load<GameObject>($"Items/{result}"), newPosition, Quaternion.identity);
      GameEventSystem.Instance.TriggerActionCompleted(Resources.Load<GameObject>($"Items/{result}").GetComponent<Combineable>().actionId);
      ParticlePoolManager.Instance.PlayParticle("Smoke", newPosition);

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
