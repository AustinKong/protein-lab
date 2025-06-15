using System.Collections;
using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    [Header("Prefabs & Timing")]
    public GameObject ballPrefab;
    private const float ballSpeed      = 5f;
    private const float spawnInterval  = 0.5f;

    [Header("Bounds for Waypoints")]
    private const float targetMinX = -7f, targetMaxX = +8.5f;
    private const float sourceMinY = -3.5f, sourceMaxY = +4.5f;

    [Header("Static Offsets")]
    private const float verticalOffset = 1f;

    // initial “reset” positions at round start
    private readonly Vector3 initialSourcePosition = new Vector3(-10f, 0f, 0f);
    private readonly Vector3 initialTargetPosition = new Vector3(0f, -4.8f, 0f);

    // these drive the spawn logic & get lerped each round
    private Vector3 sourcePosition;
    private Vector3 targetPosition;

    void Start()
    {
        MinigameManager.Instance.Initialize(60f, true, true);
        MinigameManager.Instance.StartGame();
        StartCoroutine(RoundLoop());
    }

    private IEnumerator RoundLoop()
    {
        while (true)
        {
            // --- Round setup: reset and pick new destinations ---
            sourcePosition = new Vector3(
                initialSourcePosition.x,
                Random.Range(sourceMinY, sourceMaxY),
                0f
            );

            targetPosition = new Vector3(
                Random.Range(targetMinX, targetMaxX),
                initialTargetPosition.y,
                0f
            );

            Vector3 roundDestSource = new Vector3(
                initialSourcePosition.x,
                Random.Range(sourceMinY, sourceMaxY),
                0f
            );
            Vector3 roundDestTarget = new Vector3(
                Random.Range(targetMinX, targetMaxX),
                initialTargetPosition.y,
                0f
            );

            float elapsed    = 0f;
            float nextSpawn  = 0f;
            float roundDuration = Random.Range(1f, 2.5f);

            while (elapsed < roundDuration)
            {
                // lerp positions
                float t = elapsed / roundDuration;
                sourcePosition = Vector3.Lerp(initialSourcePosition, roundDestSource, t);
                targetPosition = Vector3.Lerp(initialTargetPosition, roundDestTarget, t);

                // spawn if it’s time
                if (elapsed >= nextSpawn)
                {
                    SpawnBallPair();
                    nextSpawn += spawnInterval;
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(3f);
        }
    }

    private void SpawnBallPair()
    {
        float halfOffset = verticalOffset * 0.5f;

        // top ball
        Vector3 topStart = sourcePosition + Vector3.up * halfOffset;
        Vector2 topDir   = (targetPosition + Vector3.up * halfOffset - topStart).normalized;
        var top = Instantiate(ballPrefab, topStart, Quaternion.identity);
        top.GetComponent<Ball>().Initialize(topDir, ballSpeed, true);

        // bottom ball
        Vector3 botStart = sourcePosition - Vector3.up * halfOffset;
        Vector2 botDir   = (targetPosition - Vector3.up * halfOffset - botStart).normalized;
        var bot = Instantiate(ballPrefab, botStart, Quaternion.identity);
        bot.GetComponent<Ball>().Initialize(botDir, ballSpeed, false);
    }
}
