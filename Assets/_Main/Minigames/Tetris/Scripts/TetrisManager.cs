using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class TetrisManager : MonoBehaviour {
  public static TetrisManager Instance;

  [SerializeField] private GameObject[] tetrisBlockPrefabs;
  [SerializeField] private GameObject[] scoringShadows;
  [SerializeField] private TMP_Text coverageText;
  private Collider2D scoringShadowCollider;
  private List<Collider2D> tetrisBlockColliders = new List<Collider2D>();

  private const int BLOCKS_ACTIVE = 3;
  private const int SAMPLE_RESOLUTION = 400;

  private void Awake() {
    if (Instance == null) {
      Instance = this;
    } else {
      Destroy(gameObject);
    }
  }

  private void Start() {
    GameObject s = scoringShadows[UnityEngine.Random.Range(0, scoringShadows.Length)];
    s.SetActive(true);
    scoringShadowCollider = s.GetComponent<Collider2D>();
    for (int i = 0; i < BLOCKS_ACTIVE; i++) {
      ReplenishBlock();
    }
  }

  private void ReplenishBlock() {
    Instantiate(tetrisBlockPrefabs[UnityEngine.Random.Range(0, tetrisBlockPrefabs.Length)],
      new Vector3(UnityEngine.Random.Range(-8f, 8f), UnityEngine.Random.Range(-3f, 4f), 0), 
      Quaternion.Euler(new Vector3(0 ,0, UnityEngine.Random.Range(0, 360f))));
  }

  public void RegisterOutOfBounds(GameObject o) {
    o.transform.SetPositionAndRotation(
      new Vector3(UnityEngine.Random.Range(-8f, 8f), UnityEngine.Random.Range(-3f, 4f), 0), 
      Quaternion.Euler(new Vector3(0 ,0, UnityEngine.Random.Range(0, 360f))));
  }

  public void RegisterCombine(GameObject o) {
    tetrisBlockColliders.Add(o.GetComponent<Collider2D>());
    ReplenishBlock();
    coverageText.text = Mathf.Round(GetCoverage() * 100).ToString() + "%";
  }

  // Returns [% of area covered, no. of points out of bounds]
  private float GetCoverage() {
    Bounds bounds = scoringShadowCollider.bounds;
    int coveredPoints = 0;
    int totalPoints = 0;

    for (int x = 0; x < SAMPLE_RESOLUTION; x++) {
      for (int y = 0; y < SAMPLE_RESOLUTION; y++) {
        Vector2 point = new Vector2(
          Mathf.Lerp(bounds.min.x, bounds.max.x, x / (float) (SAMPLE_RESOLUTION - 1)),
          Mathf.Lerp(bounds.min.y, bounds.max.y, y / (float) (SAMPLE_RESOLUTION - 1))
        );

        if (scoringShadowCollider.OverlapPoint(point)) {
          totalPoints++;
          if (tetrisBlockColliders.Any(collider => collider.OverlapPoint(point))) {
            coveredPoints++;
          }
        }
      }
    }

    return Mathf.Clamp(1.2f * coveredPoints / totalPoints, 0, 1);
  } 
}