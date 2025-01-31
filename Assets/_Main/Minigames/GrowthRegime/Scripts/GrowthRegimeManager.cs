using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GrowthRegimeManager : MonoBehaviour
{
  public static GrowthRegimeManager Instance;

  private Collider2D areaCollider;
  [SerializeField] private GameObject[] shapePrefabs;
  [SerializeField] private GameObject[] shapesShadows;
  [SerializeField] private GameObject metalTray;
  [SerializeField] private Sprite completionSprite;
  [SerializeField] private TMP_Text coverageText;

  private List<Collider2D> shapeColliders = new List<Collider2D>();
  private int shapesRemaining;

  private const int SAMPLE_RESOLUTION = 200;

  private void Awake() {
    if (Instance == null) {
      Instance = this;
    } else {
      Destroy(gameObject);
    }
  }

  private void Start() {
    GameObject ss = shapesShadows[UnityEngine.Random.Range(0, shapesShadows.Length)];
    ss.SetActive(true);
    areaCollider = ss.GetComponent<Collider2D>();
    PopulateCrystals();
  }

  private readonly Vector2 BOUNDS_TOP_LEFT = new Vector2(-1.5f, 1.9f);
  private readonly Vector2 BOUNDS_BOTTOM_RIGHT = new Vector2(1.25f, -1.9f); 
  private const int CRYSTAL_COUNT = 15;

  private void PopulateCrystals() {
    for (int i = 0; i < CRYSTAL_COUNT; i++) {
      GameObject shape = Instantiate(shapePrefabs[UnityEngine.Random.Range(0, shapePrefabs.Length)]);
      shape.transform.parent = metalTray.transform;
      shape.transform.localPosition = new Vector3(UnityEngine.Random.Range(BOUNDS_TOP_LEFT.x, BOUNDS_BOTTOM_RIGHT.x), UnityEngine.Random.Range(BOUNDS_TOP_LEFT.y, BOUNDS_BOTTOM_RIGHT.y) , 0);
      shape.transform.localScale += Vector3.one * UnityEngine.Random.Range(-0.1f, 0.1f);
      shape.transform.rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(0, 360));
    }
    shapesRemaining += CRYSTAL_COUNT;
  }


  public void NextShape(Collider2D shapeCollider) {
    shapeColliders.Add(shapeCollider);
    shapesRemaining--;
    coverageText.text = Math.Round(GetCoverage() * 100).ToString() + "%";

    if (GetCoverage() >= 1f) {
      MinigameManager.Instance.Completion(completionSprite, "Good Job", GetCoverage());
    }

    if (shapesRemaining == 0) {
      PopulateCrystals();
    }
  }

  public void OnCompletion() {
  }

  private float GetCoverage() {
    Bounds bounds = areaCollider.bounds;
    int coveredPoints = 0;
    int totalPoints = 0;

    for (int x = 0; x < SAMPLE_RESOLUTION; x++) {
      for (int y = 0; y < SAMPLE_RESOLUTION; y++) {
        Vector2 point = new Vector2(
          Mathf.Lerp(bounds.min.x, bounds.max.x, x / (float) (SAMPLE_RESOLUTION - 1)),
          Mathf.Lerp(bounds.min.y, bounds.max.y, y / (float) (SAMPLE_RESOLUTION - 1))
        );

        if (areaCollider.OverlapPoint(point)) {
          totalPoints++;
          if (shapeColliders.Any(collider => collider.OverlapPoint(point))) {
            coveredPoints++;
          }
        }
      }
    }

    // Multiply up a little cuz it seems to report low
    return Math.Clamp(1.2f * coveredPoints / totalPoints, 0, 1);
  }
}
