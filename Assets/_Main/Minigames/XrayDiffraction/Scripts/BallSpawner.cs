using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultySettings
{
    public float roundGap;
    public float ballSpeed;
    public float paddleWidth;
    public float roundDuration;
    public int bombSpawnCount; 
}

public class BallSpawner : MonoBehaviour
{
    public int difficulty = 3; // 1 (easiest) to 5 (hardest)

    private readonly Dictionary<int, DifficultySettings> difficultyMap = new Dictionary<int, DifficultySettings>
    {
        { 1, new DifficultySettings { roundGap = 2.0f, ballSpeed = 4.5f, paddleWidth = 1.2f, roundDuration = 2.0f, bombSpawnCount = 1 } },
        { 2, new DifficultySettings { roundGap = 1.85f, ballSpeed = 5.0f, paddleWidth = 1.075f, roundDuration = 1.75f, bombSpawnCount = 1 } },
        { 3, new DifficultySettings { roundGap = 1.7f, ballSpeed = 5.5f, paddleWidth = 0.95f, roundDuration = 1.5f, bombSpawnCount = 2 } },
        { 4, new DifficultySettings { roundGap = 1.55f, ballSpeed = 6.0f, paddleWidth = 0.825f, roundDuration = 1.25f, bombSpawnCount = 2 } },
        { 5, new DifficultySettings { roundGap = 1.4f, ballSpeed = 6.5f, paddleWidth = 0.7f, roundDuration = 1.0f, bombSpawnCount = 3 } },
    };

    public static BallSpawner Instance; // Add this line at the top

    [Header("Speed Lines")]
    [SerializeField] private GameObject[] speedLines;

    [Header("Bomb Settings")]
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private float bombIntervalMin = 3f;
    [SerializeField] private float bombIntervalMax = 6f;

    [Header("Prefabs & Timing")]
    public GameObject ballPrefab;
    public GameObject evilPhotonPrefab;

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

    public Sprite completionSprite;

    private DifficultySettings currentSettings;

    void Awake()
    {
        Instance = this;
    }

    public void SetDifficulty(int newDifficulty)
    {
        difficulty = Mathf.Clamp(newDifficulty, 1, 5);
        foreach (var line in speedLines)
        {
            line.SetActive(false);
        }

        if (difficulty > 1) speedLines[difficulty - 2].SetActive(true);

        DifficultySettings settings = difficultyMap[difficulty];
        // Apply paddle width
        PaddleController.Instance.SetPaddleWidth(settings.paddleWidth);
        currentSettings = settings;
    }

    void Start()
    {
        SetDifficulty(1);
        MinigameManager.Instance.Initialize(60f, true, true, completionSprite, "Good job", 500);
        MinigameManager.Instance.StartGame();
        StartCoroutine(RoundLoop());
        StartCoroutine(BombSpawnerLoop());
        StartCoroutine(LevelUpRoutine());


    }

    private IEnumerator LevelUpRoutine()
    {
        while (difficulty < 5)
        {
            yield return new WaitForSeconds(12f);

            if (difficulty < 5)
            {
                SetDifficulty(difficulty + 1);
            }
        }
    }

    private IEnumerator BombSpawnerLoop()
    {
        while (true)
        {
            float wait = Random.Range(bombIntervalMin, bombIntervalMax);
            yield return new WaitForSeconds(wait);

            for (int i = 0; i < currentSettings.bombSpawnCount; i++)
            {
                SpawnBomb();
            }
            yield return new WaitForSeconds(0.5f);
            SoundManager.Instance.PlaySFX("DM-CGS-06");
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
            // Set this wave's start/end positions
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

            // Choose number of ball pairs this wave
            int numBallPairs = Random.Range(3, 6); // 3 to 5 inclusive

            // Time between each ball pair
            float spawnInterval = currentSettings.roundDuration / numBallPairs;

            // Decide if this wave is evil
            bool thisWaveIsEvil = Random.value < 0.3f;

            for (int i = 0; i < numBallPairs; i++)
            {
                // Lerp spawn and target points for this ball
                float t = i / (float)(numBallPairs - 1);
                sourcePosition = Vector3.Lerp(initialSourcePosition, roundDestSource, t);
                targetPosition = Vector3.Lerp(initialTargetPosition, roundDestTarget, t);

                SpawnBallPair(thisWaveIsEvil);
                yield return new WaitForSeconds(spawnInterval);
            }

            // Wait before next wave
            yield return new WaitForSeconds(currentSettings.roundGap);
        }
    }


    private void SpawnBallPair(bool isEvilWave)
    {
        float halfOffset = verticalOffset * 0.5f;
        GameObject toSpawn = isEvilWave ? evilPhotonPrefab : ballPrefab;

        // top ball
        Vector3 topStart = sourcePosition + Vector3.up * halfOffset;
        Vector2 topDir = (targetPosition + Vector3.up * halfOffset - topStart).normalized;
        var top = Instantiate(toSpawn, topStart, Quaternion.identity);
        top.GetComponent<Ball>().Initialize(topDir, currentSettings.ballSpeed, true);

        // bottom ball
        Vector3 botStart = sourcePosition - Vector3.up * halfOffset;
        Vector2 botDir = (targetPosition - Vector3.up * halfOffset - botStart).normalized;
        var bot = Instantiate(toSpawn, botStart, Quaternion.identity);
        bot.GetComponent<Ball>().Initialize(botDir, currentSettings.ballSpeed, false);
    }
}
