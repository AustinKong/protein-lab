using System.Collections;
using System.Collections.Generic;
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
  [SerializeField] private TMP_Text timerText;
  [SerializeField] private TMP_Text scoreText;

  private RectTransform scoreTextRect;

  private int score = 0;
  private float timer = 0f;
  private bool useTimer = false;
  private Sprite completionSprite;
  private string completionString;
  private float topScore;
  private bool gameStarted = false;

  private void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
    }
    else
    {
      Destroy(gameObject);
    }

    scoreTextRect = scoreText.GetComponent<RectTransform>();
  }

  public void Initialize(
      float startTime,
      bool useTimer = false,
      bool useScore = false,
      Sprite completionSprite = null,
      string completionText = null,
      float topScore = 0f
    )
  {
    timer = startTime;
    score = 0;
    this.useTimer = useTimer;
    this.completionSprite = completionSprite;
    this.completionString = completionText;
    this.topScore = topScore;
    this.scoreText.text = score.ToString();
    this.timerText.text = Mathf.Round(timer).ToString();

    if (!useTimer) timerText.transform.parent.gameObject.SetActive(false);
    else timerText.transform.parent.gameObject.SetActive(true);
    if (!useScore) scoreText.transform.parent.gameObject.SetActive(false);
    else scoreText.transform.parent.gameObject.SetActive(true);

  }

  public void StartGame()
  {
    if (gameStarted) return;
    gameStarted = true;
  }

  private void Update()
  {
    if (gameStarted && useTimer)
    {
      timer -= Time.deltaTime;
      timerText.text = Mathf.Round(timer).ToString();
      if (timer <= 0)
      {
        Completion();
      }
    }
  }

  public void Score(int points = 1)
  {
    score += points;
    scoreText.text = score.ToString();
    StartCoroutine(PopScoreText());
  }

private IEnumerator PopScoreText()
{
    Vector3 startScale = Vector3.one;
    Vector3 popScale = Vector3.one * 1.2f;
    float duration = 0.1f;

    // Scale up
    float t = 0f;
    while (t < duration)
    {
        t += Time.deltaTime;
        scoreTextRect.localScale = Vector3.Lerp(startScale, popScale, t / duration);
        yield return null;
    }

    // Scale down
    t = 0f;
    while (t < duration)
    {
        t += Time.deltaTime;
        scoreTextRect.localScale = Vector3.Lerp(popScale, startScale, t / duration);
        yield return null;
    }

    scoreTextRect.localScale = Vector3.one;
}


  public void Completion()
  {
    useTimer = false;
    if (score / topScore >= 0.33f) completionStars[0].color = Color.white;
    if (score / topScore >= 0.66f) completionStars[1].color = Color.white;
    if (score / topScore >= 0.99f) completionStars[2].color = Color.white;

    completionText.text = completionString;
    completionItem.sprite = completionSprite;

    // Basically match the sprite to its aspect ratio without exceeding the size 240x400
    float aspectRatio = completionSprite.bounds.size.x / completionSprite.bounds.size.y;

    float targetWidth = 240f;
    float targetHeightForWidth = targetWidth / aspectRatio; // Height when width is fixed at 240
    float targetHeight = 400f;
    float targetWidthForHeight = targetHeight * aspectRatio; // Width when height is fixed at 400

    float finalWidth, finalHeight;
    if (targetHeightForWidth <= targetHeight)
    {
      finalWidth = targetWidth;
      finalHeight = targetHeightForWidth;
    }
    else
    {
      finalWidth = targetWidthForHeight;
      finalHeight = targetHeight;
    }

    RectTransform rectTransform = completionItem.GetComponent<RectTransform>();
    rectTransform.sizeDelta = new Vector2(finalWidth, finalHeight);

    completionObject.SetActive(true);
  }

  private Dictionary<string, string> currentSceneToNextUnlock = new Dictionary<string, string>
  {
    { "Nucleation", "SequenceAlignment" },
    { "SequenceAlignment3", "Xray" },
  };

  public void CloseCompletion()
  {
    if (currentSceneToNextUnlock.ContainsKey(SceneManager.Instance.GetActiveScene()))
    {
      SceneManager.Instance.UnlockScene(currentSceneToNextUnlock[SceneManager.Instance.GetActiveScene()]);
    }
    SceneManager.Instance.LoadScene("LevelSelect");
  }
}
