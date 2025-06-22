using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultySettings
{
    public float spawnInterval;
    public float ballSpeed;
    public float paddleWidth;
    public float roundDuration;
}

public class BallSpawner : MonoBehaviour
{
    public int difficulty = 3; // 1 (easiest) to 5 (hardest)

    private readonly Dictionary<int, DifficultySettings> difficultyMap = new Dictionary<int, DifficultySettings>
{
    { 1, new DifficultySettings { spawnInterval = 0.8f, ballSpeed = 3f, paddleWidth = 1.8f, roundDuration = 3.5f } },
    { 2, new DifficultySettings { spawnInterval = 0.7f, ballSpeed = 3.5f, paddleWidth = 1.5f, roundDuration = 3f } },
    { 3, new DifficultySettings { spawnInterval = 0.6f, ballSpeed = 4.5f, paddleWidth = 1.2f, roundDuration = 2.5f } },
    { 4, new DifficultySettings { spawnInterval = 0.5f, ballSpeed = 5.5f, paddleWidth = 1, roundDuration = 2f } },
    { 5, new DifficultySettings { spawnInterval = 0.4f, ballSpeed = 6.5f, paddleWidth = 0.7f, roundDuration = 1.6f } },
};

    public static BallSpawner Instance; // Add this line at the top

    [Header("Bomb Settings")]
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private float bombIntervalMin = 3f;
    [SerializeField] private float bombIntervalMax = 6f;

    [Header("Prefabs & Timing")]
    public GameObject ballPrefab;
    private const float ballSpeed = 5f;
    private const float spawnInterval = 0.5f;

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

    private DifficultySettings currentSettings;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        DifficultySettings settings = difficultyMap[Mathf.Clamp(difficulty, 1, 5)];

        // Apply paddle width
        PaddleController.Instance.SetPaddleWidth(settings.paddleWidth);

        // Store for use in spawn loop
        currentSettings = settings;

        MinigameManager.Instance.Initialize(60f, true, true);
        MinigameManager.Instance.StartGame();
        StartCoroutine(RoundLoop());
        StartCoroutine(BombSpawnerLoop());
    }

    public void IncreaseDifficulty()
    {
        difficulty = Mathf.Clamp(difficulty + 1, 1, 5);
        Debug.Log($"Difficulty increased to {difficulty}");
    }

    private IEnumerator BombSpawnerLoop()
    {
        while (true)
        {
            float wait = Random.Range(bombIntervalMin, bombIntervalMax);
            yield return new WaitForSeconds(wait);

            SpawnBomb();
        }
    }

    private void SpawnBomb()
    {
        Vector3 spawnPos = new Vector3(-10f, Random.Range(-1f, 3f), 0f);
        GameObject bombObj = Instantiate(bombPrefab, spawnPos, Quaternion.identity);

        Vector2 velocity = new Vector2(Random.Range(5f, 8f), Random.Range(5f, 10f)); // tweak to taste
        bombObj.GetComponent<Bomb>().Initialize(velocity);
    }

    private IEnumerator RoundLoop()
    {
        while (true)
        {
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

            float elapsed = 0f;
            float nextSpawn = 0f;
            float roundDuration = currentSettings.roundDuration;

            while (elapsed < roundDuration)
            {
                float t = elapsed / roundDuration;
                sourcePosition = Vector3.Lerp(initialSourcePosition, roundDestSource, t);
                targetPosition = Vector3.Lerp(initialTargetPosition, roundDestTarget, t);

                if (elapsed >= nextSpawn)
                {
                    SpawnBallPair();
                    nextSpawn += currentSettings.spawnInterval;
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
        Vector2 topDir = (targetPosition + Vector3.up * halfOffset - topStart).normalized;
        var top = Instantiate(ballPrefab, topStart, Quaternion.identity);
        top.GetComponent<Ball>().Initialize(topDir, currentSettings.ballSpeed, true);

        // bottom ball
        Vector3 botStart = sourcePosition - Vector3.up * halfOffset;
        Vector2 botDir = (targetPosition - Vector3.up * halfOffset - botStart).normalized;
        var bot = Instantiate(ballPrefab, botStart, Quaternion.identity);
        bot.GetComponent<Ball>().Initialize(botDir, currentSettings.ballSpeed, false);
    }
}
