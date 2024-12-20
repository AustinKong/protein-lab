using UnityEngine;

public class Centrifuge : Interactable, IConsumer
{
  [SerializeField] private Draggable postMixPrefab;

  private float dragStartY;
  private const float RETRIEVE_DELTA_Y = 1f;
  private int contentCount = 0;
  private bool hasCentrifuged = false;

  public bool CanConsume(string otherItemName) {
    Debug.Log(otherItemName + " expected: Untreated protein solution");
 return otherItemName == "Untreated protein solution";
  }
  
  public string Consume(string otherItemName) {
    contentCount++;
    Debug.Log("im here");
    return "Container";
  }

  protected override void OnTap() {
    base.OnTap();
    if (contentCount >= 2) {
      SceneManager.Instance.LoadMinigameScene("Centrifuge");
      hasCentrifuged = true;
    }
  }

  protected override void OnDragStart() {
    base.OnDragStart();
    dragStartY = GetPointerWorldPosition().y;
  }

  protected override void OnDragEnd() {
    base.OnDragEnd();
    if (GetPointerWorldPosition().y - dragStartY > RETRIEVE_DELTA_Y && contentCount > 0 && hasCentrifuged) {
      Instantiate(postMixPrefab.gameObject, transform.position + Vector3.up * 2f, Quaternion.identity);
      contentCount--;
      if (contentCount == 0) hasCentrifuged = false;
    }
  }
}
