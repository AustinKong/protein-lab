using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System;

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
            else 
            {
                ShowMindOrderPopup();
            }

        }

        CheckIfAllStepsDone();
    }


    void CheckIfAllStepsDone()
    {
        bool allDone = stepCompletedRuntime.TrueForAll(done => done);

        if (allDone && nextSceneUI != null)
        {
            if (currentSceneIndex != 5)
            {
                nextSceneUI.SetActive(true);
            }
            else
            {
                backToMainMenuUI.SetActive(true);
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

}

