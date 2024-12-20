using System.Linq;
using TMPro;
using UnityEngine;

public class Minigame_CentrifugeTube : Minigame_Draggable
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

  protected override void OnDragEnd() {
    base.OnDragEnd();
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

  public void SetWeight(float weight) {
    this.weight = weight;
    weightText.text = weight.ToString();
  }
}