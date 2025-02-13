using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SequenceAlignmentManager : MonoBehaviour
{
  public static SequenceAlignmentManager Instance;

  [SerializeField] private GameObject beadPrefab;
  [SerializeField] private RectTransform[] sequencePanels;
  [SerializeField] private RectTransform targetSequencePanel;
  private string[] databaseSequences;

  private readonly string[] SUBSEQUENCES = { "MKTLL", "GLYAS", "PRTYS", "FVLIM", "HSDEN" };
  private readonly Dictionary<char, Color> PROTEIN_LETTERS = new Dictionary<char, Color> {
    { 'A', new Color(0.8f, 0.8f, 0.8f) },  // Alanine - Light Gray
    { 'R', new Color(0.0f, 0.0f, 1.0f) },  // Arginine - Blue
    { 'N', new Color(0.5f, 0.2f, 0.8f) },  // Asparagine - Purple
    { 'D', new Color(1.0f, 0.0f, 0.0f) },  // Aspartic Acid - Red
    { 'C', new Color(1.0f, 1.0f, 0.0f) },  // Cysteine - Yellow
    { 'E', new Color(0.8f, 0.0f, 0.0f) },  // Glutamic Acid - Dark Red
    { 'Q', new Color(0.6f, 0.4f, 0.8f) },  // Glutamine - Light Purple
    { 'G', new Color(0.5f, 0.5f, 0.5f) },  // Glycine - Gray
    { 'H', new Color(0.3f, 0.3f, 0.7f) },  // Histidine - Medium Blue
    { 'I', new Color(1.0f, 0.5f, 0.0f) },  // Isoleucine - Orange
    { 'L', new Color(1.0f, 0.6f, 0.0f) },  // Leucine - Orange-Yellow
    { 'K', new Color(0.0f, 0.0f, 0.8f) },  // Lysine - Dark Blue
    { 'M', new Color(1.0f, 0.8f, 0.0f) },  // Methionine - Gold
    { 'F', new Color(0.5f, 0.5f, 0.0f) },  // Phenylalanine - Olive
    { 'P', new Color(0.6f, 0.3f, 0.3f) },  // Proline - Brown
    { 'S', new Color(0.0f, 1.0f, 0.0f) },  // Serine - Green
    { 'T', new Color(0.0f, 0.8f, 0.0f) },  // Threonine - Dark Green
    { 'W', new Color(0.2f, 0.2f, 0.6f) },  // Tryptophan - Deep Blue
    { 'Y', new Color(0.8f, 0.8f, 0.0f) },  // Tyrosine - Yellow-Green
    { 'V', new Color(0.8f, 0.4f, 0.0f) }   // Valine - Orange-Brown
  };
  private const string TARGET_SEQUENCE = "FVLIMGLYASMKTLLPRTYSHSDEN";
  private const int DATABASE_SEQUENCE_LENGTH = 69; // 20 (buffer) & 29 (usable) & 20 (buffer)

  private int selectedDatabaseSequence = 0;

  private void Start() {
    databaseSequences = new string[sequencePanels.Length];
    for (int i = 0; i < databaseSequences.Length; i++) {
      databaseSequences[i] = Enumerable.Range(0, DATABASE_SEQUENCE_LENGTH)
        .Select(_ => PROTEIN_LETTERS.Keys.ElementAt(UnityEngine.Random.Range(0, PROTEIN_LETTERS.Keys.Count)))
        .Select(x => x.ToString())
        .Aggregate((acc, x) => acc + x);

      int insertionPoint = UnityEngine.Random.Range(20, DATABASE_SEQUENCE_LENGTH - 25);
      databaseSequences[i] = databaseSequences[i].Remove(insertionPoint, 5).Insert(insertionPoint, SUBSEQUENCES[i]);
    }

    for (int i = 0; i < sequencePanels.Length; i++) {
      for (int j = 0; j < databaseSequences[i].Length; j++) {
        GameObject bead = Instantiate(beadPrefab, sequencePanels[i]);
        bead.GetComponent<Image>().color = PROTEIN_LETTERS[databaseSequences[i][j]];
        bead.GetComponentInChildren<TMP_Text>().text = databaseSequences[i][j].ToString();
      }
    }

    for (int j = 0; j < TARGET_SEQUENCE.Length; j++) {
      GameObject bead = Instantiate(beadPrefab, targetSequencePanel);
        bead.GetComponent<Image>().color = PROTEIN_LETTERS[TARGET_SEQUENCE[j]];
      bead.GetComponentInChildren<TMP_Text>().text = TARGET_SEQUENCE[j].ToString();
    }
  }

  private int currentOffset = 0;

  public void OnSliderChange(float value) {
    currentOffset = Mathf.RoundToInt(Mathf.Lerp(0, 28, value));
    float width = sequencePanels[selectedDatabaseSequence].sizeDelta.x;
    sequencePanels[selectedDatabaseSequence].anchoredPosition = new Vector3(Mathf.Round(Mathf.Lerp(-1247, 1247, value) / (width / DATABASE_SEQUENCE_LENGTH)) * width / DATABASE_SEQUENCE_LENGTH, sequencePanels[selectedDatabaseSequence].anchoredPosition.y, 0);
    bool[] result = Match(TARGET_SEQUENCE, databaseSequences[selectedDatabaseSequence].Substring(20 + currentOffset, TARGET_SEQUENCE.Length));
    Debug.Log(result.Select(x => x ? "🟩" : "🟥").ToCommaSeparatedString());
  }

  private bool[] Match(string a, string b) {
    bool[] result = new bool[a.Length];
    for (int i = 0; i < a.Length; i++) {
      result[i] = a[i] == b[i];
    }
    return result;
  }

  public void OnSubmit() {
    if (selectedDatabaseSequence + 1 < databaseSequences.Length) {
      selectedDatabaseSequence++;
    } else {
      // see corrctneess
    }
  }
}
