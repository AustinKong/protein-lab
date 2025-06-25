using UnityEngine;
using UnityEngine.UI;

public class MeasuringCylinderInteraction : DraggableUI
{
    [Header("Water Levels")]
    public GameObject levelEmpty;     // 初始为空
    public GameObject levelHalf;      // 中等液位
    public GameObject levelFull;      // 满液位
    public AudioSource fillSound;

    [Header("Settings")]
    public int currentLevel = 0;      // 0 = 空，1 = 半，2 = 满
    public float[] levelVolumes = { 0f, 800f, 1000f }; // 对应体积（单位 ml）

    public override void ExecuteCustomInteraction(DraggableUI other)
    {
        // 1. 判断是去离子水被拖过来
        if (other.itemID == "De-ionised Water")
        {
            if (currentLevel < 2)
            {
                currentLevel += 1;

                if (fillSound != null)
                    fillSound.Play();
                    //FindObjectOfType<HintPageUI>().MarkStepComplete(0);
                UpdateWaterLevelVisual();
            }
            else
            {
                Debug.Log("已满，不能再加水");
            }
        }
        else if (other.itemID == "Trash_Bin")
        {
            // 倒掉所有液体
            currentLevel = 0;
            UpdateWaterLevelVisual();
        }
    }

    public void UpdateWaterLevelVisual()
    {
        levelEmpty.SetActive(currentLevel == 0);
        levelHalf.SetActive(currentLevel == 1);
        levelFull.SetActive(currentLevel == 2);
    }

    public float GetCurrentVolume()
    {
        return levelVolumes[currentLevel];
    }
}
