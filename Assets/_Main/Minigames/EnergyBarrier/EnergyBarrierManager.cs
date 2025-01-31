using TMPro;
using UnityEngine;

public enum Supersaturation {
  Low, Medium, High
}

public class EnergyBarrierManager : MonoBehaviour
{
  [SerializeField] private TMP_Text[] moleculeEnergyUnitDisplays;
  [SerializeField] private TMP_Text energyBarrierDisplay;
  [SerializeField] private TMP_Text rewardDisplay;
  [SerializeField] private TMP_Text superSaturationDisplay;
  [SerializeField] private TMP_Text scoreDisplay;
  [SerializeField] private Sprite completionSprite;

  private int[] moleculeEnergyUnits = new int[3];
  private int energyBarrier;
  private int originalEnergyBarrier;
  private int reward = 5;
  private Supersaturation superSaturation = Supersaturation.Low;
  private int score = 0;

  private bool hasGottenLastMolecule = false;

  private void Start() {
    originalEnergyBarrier = Random.Range(8, 13);
    energyBarrier = originalEnergyBarrier + 2;
    energyBarrierDisplay.text = energyBarrier.ToString();

    moleculeEnergyUnits[0] = Random.Range(2, 6);
    moleculeEnergyUnits[1] = Random.Range(2, 6);
    moleculeEnergyUnitDisplays[0].text = moleculeEnergyUnits[0].ToString() + " Energy Units";
    moleculeEnergyUnitDisplays[1].text = moleculeEnergyUnits[1].ToString() + " Energy Units";
    moleculeEnergyUnitDisplays[2].text = "?";
  }

  public void GetLastMolecule() {
    if (hasGottenLastMolecule) return;

    hasGottenLastMolecule = true;
    moleculeEnergyUnits[2] = Random.Range(2, 6);
    moleculeEnergyUnitDisplays[2].text = moleculeEnergyUnits[2].ToString() + " Energy Units";

    score += (moleculeEnergyUnits[0] + moleculeEnergyUnits[1] + moleculeEnergyUnits[2]) >= energyBarrier ? reward : 0;
    scoreDisplay.text = score.ToString();
  }

  public void AttemptTSNM() {
    if ((moleculeEnergyUnits[0] + moleculeEnergyUnits[1] + moleculeEnergyUnits[2]) < 0.5f * energyBarrier) return;

    switch (superSaturation) {
      case Supersaturation.Low:
        if (Random.Range(1, 4) == 1) {
          if (score > 0) {
            score += 2;
          } else {
            score += reward / 2;
          }
        }
        break;
      case Supersaturation.Medium:
        if (Random.Range(1, 3) == 1) {
          if (score > 0) {
            score += 2;
          } else {
            score += reward / 2;
          }
        }
        break;
      case Supersaturation.High:
        if (Random.Range(1, 4) != 1) {
          if (score > 0) {
            score += 2;
          } else {
            score += reward / 2;
          }
        }
        break;
    }
    scoreDisplay.text = score.ToString();
    MinigameManager.Instance.Completion(completionSprite, "Great Job!", score / 5f);
  }

  public void TweakSupersaturation(float newValue) {
    if (newValue < 0.33f) superSaturation = Supersaturation.Low;
    else if (newValue < 0.66f) superSaturation = Supersaturation.Medium;
    else superSaturation = Supersaturation.High;

    switch (superSaturation) {
      case Supersaturation.Low:
        energyBarrier = originalEnergyBarrier + 2;
        reward = 5;
        break;
      case Supersaturation.Medium:
        energyBarrier = originalEnergyBarrier;
        reward = 3;
        break;
      case Supersaturation.High:
        energyBarrier = originalEnergyBarrier - 2;
        reward = 1;
        break;
    }
    rewardDisplay.text = reward.ToString();
    superSaturationDisplay.text = superSaturation.ToString();
    energyBarrierDisplay.text = energyBarrier.ToString();
  }
}
