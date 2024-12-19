using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MinigameManager : MonoBehaviour
{
  public static MinigameManager Instance;

  [Header("UI Elements")]
  [SerializeField] private GameObject completionObject;
  [SerializeField] private Image completionItem;
  [SerializeField] private TMP_Text completionText;
  [SerializeField] private Image[] completionStars;

  private void Awake() {
    if (Instance == null) {
      Instance = this;
    } else {
      Destroy(gameObject);
    }
  }

  // Score added for future extensibility [0, 1]
  public void Completion(Sprite sprite, string text, float score = 1f) {
    if (score >= 0.33f) completionStars[0].color = Color.white;
    if (score >= 0.66f) completionStars[1].color = Color.white;
    if (score >= 0.99f) completionStars[2].color = Color.white;

    completionText.text = text;
    completionItem.sprite = sprite;

    // Basically match the sprite to its aspect ratio without exceeding the size 240x400
    float aspectRatio = sprite.bounds.size.x / sprite.bounds.size.y;

    float targetWidth = 240f;
    float targetHeightForWidth = targetWidth / aspectRatio; // Height when width is fixed at 240
    float targetHeight = 400f;
    float targetWidthForHeight = targetHeight * aspectRatio; // Width when height is fixed at 400

    float finalWidth, finalHeight;
    if (targetHeightForWidth <= targetHeight) {
      finalWidth = targetWidth;
      finalHeight = targetHeightForWidth;
    } else {
      finalWidth = targetWidthForHeight;
      finalHeight = targetHeight;
    }

    RectTransform rectTransform = completionItem.GetComponent<RectTransform>();
    rectTransform.sizeDelta = new Vector2(finalWidth, finalHeight);

    completionObject.SetActive(true);
  }

  public void CloseCompletion() {
    SceneManager.Instance.ReturnToOriginalScene();
  }
}
