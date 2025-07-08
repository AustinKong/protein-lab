using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System;
using System.Collections;

public class HintPageUI : MonoBehaviour
{
    public BufferRecipe recipe;
    public int currentSceneIndex;

    public GameObject stepItemPrefab;          // 每一条提示的 UI prefab
    public Transform stepContainer;            // 显示区域容器

    public List<GameObject> stepUIInstances = new List<GameObject>();
    public List<GameObject> stepStatusImages = new List<GameObject>();
    public List<bool> stepCompletedRuntime;
    
    public Transform uiParentOfMindOrderUI;
    public GameObject nextSceneUI;
    public GameObject mindOrderUI;
    public GameObject backToMainMenuUI;
    private GameObject currentMindOrderUI;
    public TextMeshProUGUI titleText; // 拖入 UI 中标题文本组件

    public static HintPageUI Instance;

    public GameObject blackScreenWithText; // 黑幕+“一天后”的图片
    public GameObject finalResultUI;       // 最终结果UI
    private bool allowClickToProceed = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Start()
    {
        RefreshSteps();
        var stepCount = recipe.sceneItemRequirements[currentSceneIndex - 1].stepDescriptions.Count;
        stepCompletedRuntime = new List<bool>(new bool[stepCount]);

        if (nextSceneUI != null)
            nextSceneUI.SetActive(false);
    }

    void Update()
    {
        if (allowClickToProceed && Input.GetKeyDown(KeyCode.Space))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("ExperimentSelect");
        }
    }


    void GenerateStepUI()
    {
        var scene = recipe.sceneItemRequirements.Find(s => s.sceneIndex == currentSceneIndex);
        if (scene == null) return;


        if (titleText != null)
            titleText.text = scene.titleName;

        foreach (Transform child in stepContainer)
        {
            Destroy(child.gameObject);
        }

        stepUIInstances.Clear();
        stepStatusImages.Clear();

        for (int i = 0; i < scene.stepDescriptions.Count; i++)
        {
            GameObject item = Instantiate(stepItemPrefab, stepContainer);
            item.GetComponentInChildren<TextMeshProUGUI>().text = $"{i + 1}. {scene.stepDescriptions[i]}";

            var tick = item.transform.Find("Tick").gameObject;
            tick.SetActive(false);
            stepStatusImages.Add(tick);  // 添加进 tick 列表

            stepUIInstances.Add(item);
        }

    }

    public void MarkStepComplete(int index)
    {
        if (index < 0 || index >= stepCompletedRuntime.Count)
            return;

        if (index >= stepUIInstances.Count || stepUIInstances[index] == null)
            return;

        if (index == 0)
        {
            stepCompletedRuntime[index] = true;

            var tick = stepUIInstances[index].transform.Find("Tick");
            if (tick != null)
                tick.gameObject.SetActive(true);

            var text = stepUIInstances[index].GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (text != null)
                text.color = Color.green;
        }
        else
        {

            if (stepCompletedRuntime[index - 1])
            {
                stepCompletedRuntime[index] = true;

                var tick = stepUIInstances[index].transform.Find("Tick");
                if (tick != null)
                    tick.gameObject.SetActive(true);

                var text = stepUIInstances[index].GetComponentInChildren<TMPro.TextMeshProUGUI>();
                if (text != null)
                text.color = Color.green;
            }
        }

        CheckIfAllStepsDone();
    }


    void CheckIfAllStepsDone()
    {
        bool allDone = stepCompletedRuntime.TrueForAll(done => done);

        if (allDone && nextSceneUI != null)
        {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "ProteinCrystalisation")
            {
                StartCoroutine(ShowBlackScreenSequence());
            }
            else
            {
                nextSceneUI.SetActive(true);
            }
        }
    }

    public void RefreshSteps()
    {
        GenerateStepUI();

        var stepCount = recipe.sceneItemRequirements[currentSceneIndex - 1].stepDescriptions.Count;
        stepCompletedRuntime = new List<bool>(new bool[stepCount]);

        if (nextSceneUI != null)
            nextSceneUI.SetActive(false);
    }

    public void SetStepState(int index, bool completed)
{
    if (index < 0 || index >= stepCompletedRuntime.Count) return;

    stepCompletedRuntime[index] = completed;

    if (stepStatusImages != null && index < stepStatusImages.Count)
    {
        stepStatusImages[index].gameObject.SetActive(completed);
    }
}

    public void ShowMindOrderPopup()
    {
        currentMindOrderUI = Instantiate(mindOrderUI, uiParentOfMindOrderUI);
        RectTransform rt = currentMindOrderUI.GetComponent<RectTransform>();
        rt.anchoredPosition = Vector2.zero;
        Destroy(currentMindOrderUI, 1f);
    }

    IEnumerator ShowBlackScreenSequence()
    {
        blackScreenWithText.SetActive(true);

        CanvasGroup group = blackScreenWithText.GetComponent<CanvasGroup>();
        if (group == null) group = blackScreenWithText.AddComponent<CanvasGroup>();

        group.alpha = 0f;
        float fadeDuration = 1f;

        // 渐显
        while (group.alpha < 1f)
        {
            group.alpha += Time.deltaTime / fadeDuration;
            yield return null;
        }

        yield return new WaitForSeconds(2f); // 停留时间
        finalResultUI.SetActive(true);
        // 渐隐
        while (group.alpha > 0f)
        {
            group.alpha -= Time.deltaTime / fadeDuration;
            yield return null;
        }
        blackScreenWithText.SetActive(false);
        allowClickToProceed = true;
    }

}

