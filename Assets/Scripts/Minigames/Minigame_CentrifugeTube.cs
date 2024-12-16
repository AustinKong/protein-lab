using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Minigame_CentrifugeTube : SimpleDraggable 
{
  [HideInInspector] public float weight { get; private set; }
  [SerializeField] private TMP_Text weightText;

  private Transform originalParent;
  private Vector3 originalPosition;

  protected override void Awake() {
    base.Awake();
    originalParent = transform.parent;
    originalPosition = transform.position;
  }

  protected override void OnDragEnd(InputAction.CallbackContext context) {
    if (isDragging) {
      Collider2D[] hitColliders =  Physics2D.OverlapPointAll(GetPointerWorldPosition()).Where(c => c.CompareTag("CentrifugeSlot")).ToArray();

      if (hitColliders.Length > 0) {
        Transform slotTransform = hitColliders[0].transform;
        transform.position = slotTransform.position;
        transform.SetParent(slotTransform);
      } else {
        transform.position = originalPosition;
        transform.SetParent(originalParent);
      }
    }
    base.OnDragEnd(context);
  }

  public void SetWeight(float weight) {
    this.weight = weight;
    weightText.text = weight.ToString();
  }
}