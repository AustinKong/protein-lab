using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[Serializable]
public class TutorialStep
{
    public string text;
    public Sprite image;
}

public class TutorialManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button nextButton;
    [SerializeField] private TMP_Text nextButtonText;
    [SerializeField] private TMP_Text instructionText;
    [SerializeField] private Image instructionImage;

    [Header("Tutorial Steps")]
    [SerializeField] private string nextScene;
    [SerializeField] private List<TutorialStep> tutorialSteps;

    private int currentStep = 0;

    private void Start()
    {
        if (tutorialSteps == null || tutorialSteps.Count == 0)
        {
            Debug.LogWarning("Tutorial steps not set.");
            return;
        }

        nextButton.onClick.AddListener(AdvanceTutorial);
        ShowStep(currentStep);
    }

    private void ShowStep(int index)
    {
        if (index >= tutorialSteps.Count) return;

        instructionText.text = tutorialSteps[index].text;
        instructionImage.sprite = tutorialSteps[index].image;

        if (index == tutorialSteps.Count - 1) nextButtonText.text = "Start";
        else nextButtonText.text = "Next";
    }

    private void AdvanceTutorial()
    {
        currentStep++;

        if (currentStep >= tutorialSteps.Count) SceneManager.Instance.LoadScene(nextScene);
        else ShowStep(currentStep);
    }
}
