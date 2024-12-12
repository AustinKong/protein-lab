using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class CentrifugeTube : Draggable 
{
  /* [HideInInspector] */ public float weight;

  private Transform originalParent;
  private Vector3 originalPosition;

  private void Start() {
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
}