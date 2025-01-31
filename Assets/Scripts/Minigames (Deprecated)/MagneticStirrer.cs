using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MagneticStirrer : MonoBehaviour
{
  [SerializeField] private Slider speedSlider;
  [SerializeField] private SpriteRenderer solution;
  [SerializeField] private TMP_Text progressText;
  [SerializeField] private RectTransform validArea;

  private float currentSpeed;
  private float currentOptimalSpeed;
  private float progress = 0;
  private float nextFluctuationTime = 0;

  private const float tolerance = 30f;
  private const float fluctuationInterval = 3f;
  // Minimum and maximum speed for fluctuations
  private const float minSpeed = tolerance;
  private const float maxSpeed = 100f - tolerance;

  private void Update() {
    currentSpeed = speedSlider.value * 100f;

    float difference = Mathf.Abs(currentSpeed - currentOptimalSpeed);
    if (difference < tolerance) {
      progress += Time.deltaTime;
    } else {
      progress -= Time.deltaTime * 2f;
      progress = Mathf.Max(progress, 0);
    }

    progressText.text = $"{Mathf.RoundToInt(progress * 10f)}%";

    if (progress >= 10f) {
      OnMinigameComplete();
    }

    Fluctuation();
  }

  // Periodically changes the optimal range
  private void Fluctuation() {
    if (Time.time > nextFluctuationTime) {
      currentOptimalSpeed = Random.Range(minSpeed, maxSpeed);
      nextFluctuationTime += fluctuationInterval;
      validArea.anchorMin = new Vector2(currentOptimalSpeed / 100f, 0);
      validArea.anchorMax = new Vector2(currentOptimalSpeed / 100f, 1);
    }
  }

  private void UpdateSolutionColor() {
    // TODO : Update
    solution.color = Color.red;
  }

  private void OnMinigameComplete() {
    Debug.Log("Completed stirrer minigame");
  }
}
