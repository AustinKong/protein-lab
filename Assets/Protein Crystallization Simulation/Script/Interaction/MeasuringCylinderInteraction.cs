using UnityEngine;
using UnityEngine.UI;

public class MeasuringCylinderInteraction : DraggableUI
{
    [Header("Water Levels")]
    public GameObject levelEmpty;     
    //public GameObject levelHalf;      
    public GameObject levelFull;      
    public AudioSource fillSound;

    [Header("Settings")]
    public int currentLevel = 0;     
    public float[] levelVolumes = { 0f, 800f, 1000f }; 

    public override void ExecuteCustomInteraction(DraggableUI other)
    {
        // 1. 判断是去离子水被拖过来
        if (other.itemID == "De-ionised Water")
        {
            //Old Version
            /*if (currentLevel < 2)
            {
                currentLevel += 1;

                if (fillSound != null)
                    fillSound.Play();
                    //FindObjectOfType<HintPageUI>().MarkStepComplete(0);
                UpdateWaterLevelVisual();
            }*/


            //Simplified Version
            levelEmpty.SetActive(false);
            levelFull.SetActive(true);
        }
        else if (other.itemID == "Trash_Bin")
        {
            //Old Version
            /*currentLevel = 0;
            UpdateWaterLevelVisual();*/

            //Simplified Version
            levelEmpty.SetActive(true);
        }
    }

    public void UpdateWaterLevelVisual()
    {
        levelEmpty.SetActive(currentLevel == 0);
        //levelHalf.SetActive(currentLevel == 1);
        levelFull.SetActive(currentLevel == 2);
    }

    public float GetCurrentVolume()
    {
        return levelVolumes[currentLevel];
    }
}
