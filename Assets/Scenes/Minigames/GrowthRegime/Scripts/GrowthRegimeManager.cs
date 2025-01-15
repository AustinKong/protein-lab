using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GrowthRegimeManager : MonoBehaviour
{
  public static GrowthRegimeManager Instance;

  [SerializeField] private Collider2D areaCollider;
  [SerializeField] private GameObject[] shapePrefabs;

  private List<Collider2D> shapeColliders = new List<Collider2D>();
  private int shapesPlaced = -1;

  private const int SAMPLE_RESOLUTION = 100;
  private const int SHAPE_COUNT = 15;

  private void Awake() {
    if (Instance == null) {
      Instance = this;
    } else {
      Destroy(gameObject);
    }
  }

  private void Start() {
    Instantiate(shapePrefabs[UnityEngine.Random.Range(0, shapePrefabs.Length)], new Vector3(-4, -4, 0), Quaternion.Euler(0, 0, UnityEngine.Random.Range(0, 360)));
  }

  private void Update() {
    if (Input.GetKeyDown(KeyCode.Space)) {
      Debug.Log(GetCoverage());
    }
  }

  public void NextShape(Collider2D shapeCollider) {
    shapesPlaced++;
    shapeColliders.Add(shapeCollider);
    if (shapesPlaced < SHAPE_COUNT) {
      Instantiate(shapePrefabs[UnityEngine.Random.Range(0, shapePrefabs.Length)], new Vector3(-4, -4, 0), Quaternion.Euler(0, 0, UnityEngine.Random.Range(0, 360)));
    } else {
      Debug.Log("Coverage percentage: " + GetCoverage() + "%");
    }
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

    return (float) coveredPoints / totalPoints;
  }
}
