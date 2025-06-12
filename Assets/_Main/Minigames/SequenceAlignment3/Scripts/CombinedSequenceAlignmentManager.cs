using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Linq;
using System;

public enum ProteinColor
{
    Purple, Green, Orange, Blue
}

public class CombinedSequenceAlignmentManager : MonoBehaviour
{
    [Header("Animation Settings")]
    private const float FALL_DISTANCE = 6f;      // how far above the row to start / end
    private const float ANIM_DURATION = 0.2f;    // how long each block takes
    private const float BASE_STAGGER = 0.02f;   // per‐index delay
    private const float STAGGER_VARIANCE = 0.03f;   // ± randomness
    private const float SPACING = 1f;

    [Header("Level Settings")]
    [Tooltip("Target sequence length per level.")]
    [SerializeField] private int[] levelTargetLengths = { 5, 9, 13 };
    [SerializeField] private Sprite[] sirenSprites;
    [SerializeField] private Color[] sirenLightColors;

    private const int DATABASE_LENGTH = 20;

    [Header("Prefab & Spacing")]
    [SerializeField] private GameObject colorBlockPrefab;   // Must have a SpriteRenderer
    [SerializeField] private Sprite completionSprite;

    [Header("Parents / Transforms")]
    [SerializeField] private Transform targetParent;
    [SerializeField] private Transform databaseParent;

    [Header("UI References")]
    [SerializeField] private Slider alignmentSlider;        // 0→1 for left/right
    [SerializeField] private Button submitButton;           // “Submit” your guess
    [SerializeField] private TMP_Text similarityText;       // Shows “XX%”
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private Image siren;
    [SerializeField] private Image sirenLight;

    [SerializeField] private GameObject plusOneFx;
    [SerializeField] private GameObject perfectFx;
    [SerializeField] private GameObject amazingFx;
    [SerializeField] private GameObject mehFx;

    // Internal state
    private int targetLength;
    private int minOffset, maxOffset;
    private float xOffsetTarget = 0f;

    private List<ProteinColor> targetSequence;
    private List<ProteinColor> databaseSequence;

    // How strongly to tint toward gray for non-matches / non-overlap
    private const float tintAmount = 0.6f;
    private float timer = 0f;
    private bool tickDownTimer = false;

    private void Start()
    {
        MinigameManager.Instance.Initialize(90, true, true, completionSprite, "Great Job!", 600);
        // Hook events
        alignmentSlider.onValueChanged.AddListener(OnSliderValueChanged);
        submitButton.onClick.AddListener(OnLevelSubmit);

        InitializeLevel();
        MinigameManager.Instance.StartGame();
    }

    private void InitializeLevel()
    {
        StartCoroutine(LerpSliderBack());
        submitButton.interactable = false;
        alignmentSlider.interactable = false;
        tickDownTimer = false;
        // Set dynamic target length & offset bounds
        targetLength = levelTargetLengths[UnityEngine.Random.Range(0, levelTargetLengths.Length)];
        minOffset = -(DATABASE_LENGTH - 1);
        maxOffset = targetLength - 1;

        databaseParent.localPosition = new Vector3(0, databaseParent.localPosition.y, databaseParent.localPosition.z);
        xOffsetTarget = targetLength * SPACING / 2f;
        targetParent.localPosition = new Vector3(-xOffsetTarget, targetParent.localPosition.y, targetParent.localPosition.z);
        similarityText.text = "Wait!";

        GenerateRandomSequences();
        CreateTargetDisplay();
        CreateDatabaseDisplay();

        int offset = Mathf.RoundToInt(Mathf.Lerp(minOffset, maxOffset, 0.5f));
        databaseParent.localPosition = new Vector3(offset * SPACING - xOffsetTarget, databaseParent.localPosition.y, databaseParent.localPosition.z);
        ApplyGrayTintToBothSequences(offset);

        Invoke(nameof(SetTickDownTimerTrue), 1.5f);
    }

    private void SetTickDownTimerTrue()
    {
        timer = 15f;
        timerText.text = $"{Math.Ceiling(timer)}s";
        tickDownTimer = true;
        submitButton.interactable = true;
        alignmentSlider.interactable = true;
        alignmentSlider.value = 0.5f;
        OnSliderValueChanged(0.5f);
        SoundManager.Instance.PlaySFX("snare roll");
    }

    private void Update()
    {
        if (!tickDownTimer) return;
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            timer = 0f;
            timerText.text = "0s";
            OnLevelSubmit();  // Auto-submit when time runs out
        }
        else
        {
            timerText.text = $"{Math.Ceiling(timer)}s";
            siren.sprite = sirenSprites[timer > 8f ? 0 : timer > 4f ? 1 : 2];
            sirenLight.color = sirenLightColors[timer > 8f ? 0 : timer > 4f ? 1 : 2];
        }
    }

    #region ─── Sequence Generation & Display ───

    private void GenerateRandomSequences()
    {
        var rng = new System.Random();
        targetSequence = new List<ProteinColor>(targetLength);
        databaseSequence = new List<ProteinColor>(DATABASE_LENGTH);

        for (int i = 0; i < targetLength; i++)
            targetSequence.Add((ProteinColor)rng.Next(0, 4));
        for (int i = 0; i < DATABASE_LENGTH; i++)
            databaseSequence.Add((ProteinColor)rng.Next(0, 4));
    }

    private void CreateTargetDisplay()
    {
        // Only reliable way to destory children in untiy, since destorying actually sets a flag "tobedestroyed" and does not remove the object immediately
        while (targetParent.childCount > 0)
        {
            var child = targetParent.GetChild(0);
            child.parent = null;
            Destroy(child.gameObject);
        }

        for (int i = 0; i < targetLength; i++)
        {
            var go = Instantiate(colorBlockPrefab, targetParent);
            Vector3 orig = new Vector3(i * SPACING, 0, 0);
            var sr = go.GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = GetColor(targetSequence[i]);

            Vector3 start = orig + Vector3.up * FALL_DISTANCE;
            go.transform.localPosition = start;

            float delay = i * BASE_STAGGER + UnityEngine.Random.Range(-STAGGER_VARIANCE, STAGGER_VARIANCE);
            StartCoroutine(AnimatePosition(go.transform, start, orig, ANIM_DURATION, delay, true));
        }
    }

    private void CreateDatabaseDisplay()
    {
        // Only reliable way to destory children in untiy, since destorying actually sets a flag "tobedestroyed" and does not remove the object immediately
        while (databaseParent.childCount > 0)
        {
            var child = databaseParent.GetChild(0);
            child.parent = null;
            Destroy(child.gameObject);
        }

        for (int i = DATABASE_LENGTH - 1; i >= 0; i--)
        {
            var go = Instantiate(colorBlockPrefab, databaseParent);
            // 1) compute final spot
            Vector3 orig = new Vector3((DATABASE_LENGTH - i - 1) * SPACING, 0, 0);
            // 2) color
            var sr = go.GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = GetColor(databaseSequence[i]);

            // 3) start up offscreen
            Vector3 start = orig + Vector3.up * FALL_DISTANCE;
            go.transform.localPosition = start;

            // 4) schedule the drop
            float delay = i * BASE_STAGGER + UnityEngine.Random.Range(-STAGGER_VARIANCE, STAGGER_VARIANCE);
            StartCoroutine(AnimatePosition(go.transform, start, orig, ANIM_DURATION, delay, true));
        }

        // for (int i = 0; i < databaseLength; i++)
        // {
        //     var go = Instantiate(colorBlockPrefab, databaseParent);
        //     go.transform.localPosition = new Vector3(i * spacing, 0, 0);
        //     var sr = go.GetComponent<SpriteRenderer>();
        //     if (sr != null) sr.color = GetColor(databaseSequence[i]);
        // }
        // drop the whole row down
        // databaseParent.localPosition = new Vector3(0, yOffsetDatabase, 0);
    }

    #endregion

    #region ─── Slider & Tinting & Similarity ───
    int lastOffset = 0;

    private void OnSliderValueChanged(float val)
    {
        if (!tickDownTimer) return;

        // Map slider → integer offset
        int offset = Mathf.RoundToInt(Mathf.Lerp(minOffset, maxOffset, val));

        if (lastOffset != offset) SoundManager.Instance.PlaySFX("SFX-impact-mechanical-01_wav");

        lastOffset = offset;

        // Shift DB row
        var bp = databaseParent.localPosition;
        databaseParent.localPosition = new Vector3(offset * SPACING - xOffsetTarget, bp.y, bp.z);

        // Tint non-overlap or mismatches in BOTH rows
        ApplyGrayTintToBothSequences(offset);

        // Recompute & display similarity (matches ÷ targetLength)
        int sim = ComputeOverlapSimilarity(offset);
        similarityText.text = $"{sim}%";
    }

    private void ApplyGrayTintToBothSequences(int offset)
    {
        // Target row
        for (int i = 0; i < targetLength; i++)
        {
            var child = targetParent.GetChild(i);
            var sr = child.GetComponent<SpriteRenderer>();
            if (sr == null) continue;

            int dbIdx = i - offset;
            bool isMatch = dbIdx >= 0 && dbIdx < DATABASE_LENGTH
                           && targetSequence[i] == databaseSequence[dbIdx];

            var orig = GetColor(targetSequence[i]);
            sr.color = isMatch
                       ? orig
                       : Color.Lerp(orig, Color.black, tintAmount);

            child.transform.localPosition = isMatch
                ? new Vector3(child.transform.localPosition.x, 0.1f, child.transform.localPosition.z)
                : new Vector3(child.transform.localPosition.x, 0, child.transform.localPosition.z);
        }

        // Database row
        for (int d = 0; d < DATABASE_LENGTH; d++)
        {
            var child = databaseParent.GetChild(d);
            var sr = child.GetComponent<SpriteRenderer>();
            if (sr == null) continue;

            int tIdx = offset + d;
            bool isMatch = tIdx >= 0 && tIdx < targetLength
                           && databaseSequence[d] == targetSequence[tIdx];

            var orig = GetColor(databaseSequence[d]);
            sr.color = isMatch
                       ? orig
                       : Color.Lerp(orig, Color.black, tintAmount);
        }
    }

    private int ComputeOverlapSimilarity(int offset)
    {
        int matchCount = 0;
        for (int i = 0; i < targetLength; i++)
        {
            int dbIdx = i - offset;
            if (dbIdx >= 0 && dbIdx < DATABASE_LENGTH &&
                targetSequence[i] == databaseSequence[dbIdx])
            {
                matchCount++;
            }
        }
        float ratio = (float)matchCount / targetLength;
        return Mathf.RoundToInt(ratio * 100f);
    }

    private int ComputeMaxPossibleSimilarity()
    {
        int best = 0;
        for (int off = minOffset; off <= maxOffset; off++)
            best = Mathf.Max(best, ComputeOverlapSimilarity(off));
        return best;
    }

    #endregion

    #region ─── Level Submit & Progression ───

    private void OnLevelSubmit()
    {
        // int curOffset = Mathf.RoundToInt(Mathf.Lerp(minOffset, maxOffset, alignmentSlider.value));
        // int currentScore = ComputeOverlapSimilarity(curOffset);
        // int bestScore = ComputeMaxPossibleSimilarity();

        // // Maybe play an animation for optimal
        // MinigameManager.Instance.Score(Mathf.RoundToInt(100f * currentScore / bestScore));
        // InitializeLevel();
        SoundManager.Instance.PlaySFX("DM-CGS-31");
        StartCoroutine(SubmitAnimationRoutine());
    }

    private void OnFinalLevelPassed()
    {
        Debug.Log("🎉 You’ve aced level 15! (Final‐level stub called.)");
        // TODO: implement end‐of‐game behavior here
    }

    #endregion

    #region ─── Utility: Color Mapping ───

    private Color GetColor(ProteinColor c)
    {
        return c switch
        {
            ProteinColor.Purple => new Color(168f / 255f, 36f / 255f, 217f / 255f),
            ProteinColor.Green => new Color(7f / 255f, 167f / 255f, 58f / 255f),
            ProteinColor.Blue => new Color(117f / 255f, 195f / 255f, 253f / 255f),
            ProteinColor.Orange => new Color(245f / 255f, 142f / 255f, 0f / 255f),
            _ => Color.white,
        };
    }

    #endregion

    // Robert Penner “back” easing
    private float EaseOutBack(float t)
    {
        float s = 1.70158f;
        t = t - 1f;
        return (t * t * ((s + 1f) * t + s) + 1f);
    }

    private float EaseInBack(float t)
    {
        float s = 1.70158f;
        return t * t * ((s + 1f) * t - s);
    }

    private IEnumerator AnimatePosition(
        Transform tr,
        Vector3 from,
        Vector3 to,
        float duration,
        float delay,
        bool isDrop
    )
    {
        if (delay > 0) yield return new WaitForSeconds(delay);
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float eased = isDrop ? EaseOutBack(t) : EaseInBack(t);
            tr.localPosition = Vector3.LerpUnclamped(from, to, eased);
            yield return null;
        }
        tr.localPosition = to;
    }

    private IEnumerator SubmitAnimationRoutine()
    {
        similarityText.text = "Wait!";
        submitButton.interactable = false;
        alignmentSlider.interactable = false;
        tickDownTimer = false; // stop the timer
        // compute offset & prepare list of non-matched blocks
        int curOffset = Mathf.RoundToInt(
            Mathf.Lerp(minOffset, maxOffset, alignmentSlider.value)
        );

        var animList = new List<Transform>();
        var successfulList = new List<Transform>();

        // target row
        for (int i = 0; i < targetParent.childCount; i++)
        {
            var child = targetParent.GetChild(i);
            int dbIdx = i - curOffset;
            bool isMatch = dbIdx >= 0 && dbIdx < DATABASE_LENGTH
                           && targetSequence[i] == databaseSequence[dbIdx];
            if (!isMatch) animList.Add(child);
        }

        // database row
        for (int d = 0; d < databaseParent.childCount; d++)
        {
            var child = databaseParent.GetChild(d);
            int tIdx = curOffset + d;
            bool isMatch = tIdx >= 0 && tIdx < targetLength
                           && databaseSequence[d] == targetSequence[tIdx];
            if (!isMatch) animList.Add(child);
            else successfulList.Add(child);
        }

        // if nothing to animate, just score+reset immediately
        if (animList.Count > 0 && successfulList.Count > 0)
        {
            foreach (var tr in animList)
            {
                Vector3 from = tr.localPosition;
                Vector3 to = from + Vector3.up * FALL_DISTANCE * 1.2f; // dk why its not going fully offscreen but this will do
                float delay = BASE_STAGGER + UnityEngine.Random.Range(-STAGGER_VARIANCE, STAGGER_VARIANCE);
                // increment `done` only when each completes
                StartCoroutine(AnimatePosition(
                    tr, from, to, ANIM_DURATION, delay, false
                ));
            }

            yield return new WaitForSeconds(1f);
            Transform last = successfulList.Last();
            successfulList.Remove(last); // remove last, since we will animate it separately

            foreach (var tr in successfulList)
            {
                Instantiate(plusOneFx, tr.position, Quaternion.identity);
                tr.gameObject.SetActive(false); // hide successful blocks
                SoundManager.Instance.PlaySFX("DM-CGS-32");
                yield return new WaitForSeconds(0.24f);
            }

            int currentScore = ComputeOverlapSimilarity(curOffset);
            int bestScore = ComputeMaxPossibleSimilarity();
            float scoreRatio = (float)currentScore / bestScore;

            if (scoreRatio == 1f) Instantiate(perfectFx, last.position, Quaternion.identity);
            else if (scoreRatio >= 0.5f) Instantiate(amazingFx, last.position, Quaternion.identity);
            else Instantiate(mehFx, last.position, Quaternion.identity);
            SoundManager.Instance.PlaySFX("DM-CGS-28");
        }
        else
        {
            Instantiate(mehFx, Vector3.zero, Quaternion.identity);
        }
        ScoreAndReset(curOffset);
    }


    private void ScoreAndReset(int curOffset)
    {
        int currentScore = ComputeOverlapSimilarity(curOffset);
        int bestScore = ComputeMaxPossibleSimilarity();
        MinigameManager.Instance.Score(
            Mathf.RoundToInt(100f * currentScore / bestScore)
        );
        InitializeLevel();
    }

    private IEnumerator LerpSliderBack()
    {
        float startValue = alignmentSlider.value;
        float targetValue = 0.5f; // center position
        float elapsed = 0f;
        while (elapsed < 1.2f)
        {
            elapsed += Time.deltaTime;
            alignmentSlider.value = Mathf.Lerp(startValue, targetValue, elapsed / 0.5f);
            yield return null;
        }
        alignmentSlider.value = targetValue; // ensure it ends exactly at center
    }
}
