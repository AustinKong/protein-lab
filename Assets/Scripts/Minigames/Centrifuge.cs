using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Centrifuge : MonoBehaviour
{
  [SerializeField] private Transform centrifuge;
  [SerializeField] private Transform[] centrifugeSlots;
  [SerializeField] private CentrifugeTube[] centrifugeTubes;

  private const float BALANCE_TOLERANCE = 0.1f; // Allowable weight difference
  private float[,] POSSIBLE_CONFIGURATIONS = new float[2, 8]{
    { 1, 2, 3, 4, 5, 6, 7, 8 },
    { 2, 4, 6, 8, 8, 6, 4, 2 },
  };

  private void Start() {
    InitializeTubeWeights();
  }

  public void StartCentrifuge() {
    if (IsBalanced()) {
      Debug.Log("This centrifuge is balanced");
      StartCoroutine(SuccessRoutine());
    } else {
      Debug.Log("This centrifuge is not balanced");
      StartCoroutine(FailureRoutine());
    }
  }

  private const float MAX_SPIN_SPEED = 600f;
  private const float TIME_SPINNING = 5f; // Time spent in max spinning speed
  private const float WIND_UP_BUFFER = 2f;
  private const float WIND_DOWN_BUFFER = 2f;

  private IEnumerator SuccessRoutine() {
    float currentSpinSpeed = 0;

    float windUpEnd = Time.time + WIND_UP_BUFFER;
    while (Time.time < windUpEnd) {
      currentSpinSpeed += MAX_SPIN_SPEED / WIND_UP_BUFFER * Time.deltaTime;
      centrifuge.Rotate(0, 0, currentSpinSpeed * Time.deltaTime);
      yield return null;
    }

    float maxSpinEnd = Time.time + TIME_SPINNING;
    while (Time.time < maxSpinEnd) {
      centrifuge.Rotate(0, 0, currentSpinSpeed * Time.deltaTime);
      yield return null;
    }

    float windDownEnd = Time.time + WIND_DOWN_BUFFER;
    while (Time.time < windDownEnd) {
      currentSpinSpeed -= MAX_SPIN_SPEED / WIND_DOWN_BUFFER * Time.deltaTime;
      centrifuge.Rotate(0, 0, currentSpinSpeed * Time.deltaTime);
      yield return null;
    }
  }
  
  private IEnumerator FailureRoutine() {
    float currentSpinSpeed = 0;

    float windUpEnd = Time.time + WIND_UP_BUFFER;
    while (Time.time < windUpEnd) {
      currentSpinSpeed += MAX_SPIN_SPEED / WIND_UP_BUFFER * Time.deltaTime;
      centrifuge.Rotate(0, 0, currentSpinSpeed * Time.deltaTime);
      yield return null;
    }

    float timeTillNextParticle = Time.time;
    float maxSpinEnd = Time.time + TIME_SPINNING;
    while (Time.time < maxSpinEnd) {
      if (Time.time > timeTillNextParticle) {
        timeTillNextParticle = Time.time + Random.Range(0, 1f);
        ParticlePoolManager.Instance.PlayParticle("Smoke", transform.position);
      }
      centrifuge.Rotate(0, 0, currentSpinSpeed * Time.deltaTime);
      yield return null;
    }

    float windDownEnd = Time.time + WIND_DOWN_BUFFER;
    while (Time.time < windDownEnd) {
      currentSpinSpeed -= MAX_SPIN_SPEED / WIND_DOWN_BUFFER * Time.deltaTime;
      centrifuge.Rotate(0, 0, currentSpinSpeed * Time.deltaTime);
      yield return null;
    }
  }

  // Function to initialize centrifuge tube weights with one of the possible configurations
  // While randomizing the order of weights
  private void InitializeTubeWeights() {
    int conf = Random.Range(0, POSSIBLE_CONFIGURATIONS.GetLength(0) - 1);
    List<float> weights = new List<float>();
    for (int i = 0; i < POSSIBLE_CONFIGURATIONS.GetLength(1); i++) {
      weights.Add(POSSIBLE_CONFIGURATIONS[conf, i]);
    }

    for (int i = 0; i < POSSIBLE_CONFIGURATIONS.GetLength(1); i++) {
      int weightIndex = Random.Range(0, weights.Count - 1);
      centrifugeTubes[i].weight = weights[weightIndex];
      weights.RemoveAt(weightIndex);
    }
  }

  // Loops thru all possible ways of splitting centrifuge in half, making sure weights is balanced <= BALANCE_TOLERANCE
  // At every possible split of the centrifuge
  private bool IsBalanced() {
    for (int i = 0; i < centrifugeSlots.Length; i++) {
      float leftWeight = 0, rightWeight = 0;
      for (int j = 0; j < centrifugeSlots.Length; j++) {
        CentrifugeTube tube = centrifugeSlots[(i + j) % centrifugeSlots.Length].GetComponentInChildren<CentrifugeTube>();
        if (tube != null) {
          float tubeWeight = tube.weight;
          if (j < centrifugeSlots.Length / 2) {
            leftWeight += tubeWeight;
          } else {
            rightWeight += tubeWeight;
          }
        }
      }
      if (Mathf.Abs(leftWeight - rightWeight) > BALANCE_TOLERANCE) {
        return false;
      }
    }
    return true;
  }
}
