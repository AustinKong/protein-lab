using UnityEngine;
using System.Collections.Generic;

public class XrayDiffractionManager : MonoBehaviour
{
  [SerializeField] private GameObject targetPrefab;

  [SerializeField] private LineRenderer lineRenderer;
  [SerializeField] private Transform shooter;
  [SerializeField] private Transform crystal;

  private const int NUMBER_OF_TARGETS = 3;
  private const float MIN_Y = -2f;
  private const float MAX_Y = 3f;
  private const float MIN_SPEED = 1f;
  private const float MAX_SPEED = 3f;
  private float[] X_POSITIONS = { 7.3f, 7.8f, 8.3f };

  private List<GameObject> targets = new List<GameObject>();

  // Manages the targets
  private void Start()
  {
    for (int i = 0; i < NUMBER_OF_TARGETS; i++)
    {
      GameObject target = Instantiate(targetPrefab, new Vector3(X_POSITIONS[Random.Range(0, X_POSITIONS.Length)], Random.Range(MIN_Y, MAX_Y), 0), Quaternion.identity);
      targets.Add(target);
      float speed = Random.Range(MIN_SPEED, MAX_SPEED);
      target.GetComponent<Target>().Initialize(MIN_Y, MAX_Y, speed);
    }
    wavePoints = new Vector3[resolution];
    lineRenderer.positionCount = resolution;
  }

  private void Update()
  {
    Vector3 endPos = CalculateEndPosition();
    GenerateSineWave(shooter.position, crystal.position, endPos);

    RaycastHit2D hit = Physics2D.Linecast(crystal.position, endPos);
    if (hit.collider != null && hit.collider.GetComponent<Target>() != null) {
      hit.collider.GetComponent<Target>().Hit();
    }
  }

  public float wavelength = 0.2f;
  public float amplitude = 0.5f;
  public float speed = 10f;
  public int resolution = 100;
  private Vector3[] wavePoints;

  void GenerateSineWave(Vector3 startPos, Vector3 midPos, Vector3 endPos)
  {
    float time = Time.time * speed;
    int pointsBetweenStartAndMid = Mathf.CeilToInt(Vector3.Distance(startPos, midPos) / (Vector3.Distance(startPos, midPos) + Vector3.Distance(midPos, endPos)) * resolution);
    int pointsBetweenMidAndEnd = resolution - pointsBetweenStartAndMid;

    for (int i = 0; i < pointsBetweenStartAndMid; i++) {
      float t = (float)i / (pointsBetweenStartAndMid - 1);
      Vector3 interpolated = Vector3.Lerp(startPos, midPos, t);
      Vector3 direction = (midPos - startPos).normalized;
      Vector3 perpendicular = new Vector3(-direction.y, direction.x, 0);
      interpolated += perpendicular * Mathf.Sin((t * Mathf.PI * 2 / wavelength) + time) * amplitude;
      wavePoints[i] = interpolated;
    }

    for (int i = 0; i < pointsBetweenMidAndEnd; i++) {
      float t = (float)i / (pointsBetweenMidAndEnd - 1);
      Vector3 interpolated = Vector3.Lerp(midPos, endPos, t);
      Vector3 direction = (endPos - midPos).normalized;
      Vector3 perpendicular = new Vector3(-direction.y, direction.x, 0);
      interpolated += perpendicular * Mathf.Sin((t * Mathf.PI * 2 / wavelength) + time) * amplitude;
      wavePoints[i + pointsBetweenStartAndMid] = interpolated;
    }

    lineRenderer.SetPositions(wavePoints);
  }

  private const float REFRACTION_Y_MULTIPLE = 2f;
  private const float X_RAY_END_X = 9f;

  private Vector3 CalculateEndPosition()
  {
    return new Vector3(X_RAY_END_X, -1 * shooter.position.y * REFRACTION_Y_MULTIPLE, 0);
  }
}
