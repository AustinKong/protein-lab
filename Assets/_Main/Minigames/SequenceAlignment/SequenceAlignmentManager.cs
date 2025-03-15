using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SequenceAlignmentManager : MonoBehaviour
{
  public static SequenceAlignmentManager Instance;

  [SerializeField] private RectTransform[] sequencePanels;
  [SerializeField] private RectTransform targetSequencePanel;

  [SerializeField] private GameObject[] magnifyingGlasses;

  [SerializeField] private Sprite completionSprite;

  // ARNDC
  [SerializeField] private char[] proteinLetters;
  [SerializeField] private GameObject[] proteinLettersPrefabs;
  private readonly string[] DATABASE_SEQUENCES = {
    "NADRDNDDAARNDCDNAAAACRCRRRACCCDCRAANCARCA",
    "DCRCCADRDCDDRCARNDRCRNCDRCCDACDRNARDARADA",
    "RACDAARDDRRADDCACRNCDDACRADDDAANRDCDCANCA",
    "ANRNRNDCANRRNDCRADDCNRNCNDRACCCDADANNADNA",
    "ADRACNDDCCARDDRCNDADDCRCCRCNNCCRRRRDRCAAA"
  };
  private readonly int[] matches = { 34, 20, 44, 9, 28 };
  private const string TARGET_SEQUENCE = "ARNDCCCARDAANRDDDRCARNDCA";

  private int selectedDatabaseSequence = 0;

  private void Start() {
    // Create database sequence prefabs
    for (int i = 0; i < sequencePanels.Length; i++) {
      for (int j = 0; j < DATABASE_SEQUENCES[i].Length; j++) {
        Instantiate(proteinLettersPrefabs[Array.IndexOf(proteinLetters, DATABASE_SEQUENCES[i][j])], sequencePanels[i]);
      }
    }

    // Generate target sequence prefabs
    for (int j = 0; j < TARGET_SEQUENCE.Length; j++) {
      Instantiate(proteinLettersPrefabs[Array.IndexOf(proteinLetters, TARGET_SEQUENCE[j])], targetSequencePanel);
    }

    LayoutRebuilder.ForceRebuildLayoutImmediate(sequencePanels[0].transform.parent.GetComponent<RectTransform>());

    magnifyingGlasses[0].SetActive(true);
  }

  private int currentOffset = 0;

  public void OnSliderChange(float value) {
    // 0 is completely out of frame to the left, full is completely out of frame to the right
    // -5 to give some leeway, make it possible to score even if it is not perfectly alignd
    currentOffset = Mathf.RoundToInt(Mathf.Lerp(DATABASE_SEQUENCES[0].Length + TARGET_SEQUENCE.Length, 0, value));
    float width = sequencePanels[selectedDatabaseSequence].sizeDelta.x;
    sequencePanels[selectedDatabaseSequence].anchoredPosition = new Vector3(Mathf.Round(Mathf.Lerp(-2115, 2115, value) / (width / DATABASE_SEQUENCES[0].Length)) * width / DATABASE_SEQUENCES[0].Length, sequencePanels[selectedDatabaseSequence].anchoredPosition.y, 0);
    Debug.Log(currentOffset + ", " + (matches[selectedDatabaseSequence] == currentOffset));
  }

  public void OnSubmit() {
    if (selectedDatabaseSequence + 1 < DATABASE_SEQUENCES.Length) {
      if (matches[selectedDatabaseSequence] == currentOffset) {
        magnifyingGlasses[selectedDatabaseSequence].SetActive(false);
        selectedDatabaseSequence++;
        magnifyingGlasses[selectedDatabaseSequence].SetActive(true);
      }
    } else {
      MinigameManager.Instance.Completion(completionSprite, "Well done");
    }
  }
}
