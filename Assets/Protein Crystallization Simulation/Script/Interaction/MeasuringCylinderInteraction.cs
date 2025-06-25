using UnityEngine;
using UnityEngine.UI;

public class MeasuringCylinderInteraction : DraggableUI
{
    [Header("Water Levels")]
    public GameObject levelEmpty;     // ��ʼΪ��
    public GameObject levelHalf;      // �е�Һλ
    public GameObject levelFull;      // ��Һλ
    public AudioSource fillSound;

    [Header("Settings")]
    public int currentLevel = 0;      // 0 = �գ�1 = �룬2 = ��
    public float[] levelVolumes = { 0f, 800f, 1000f }; // ��Ӧ�������λ ml��

    public override void ExecuteCustomInteraction(DraggableUI other)
    {
        // 1. �ж���ȥ����ˮ���Ϲ���
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
                Debug.Log("�����������ټ�ˮ");
            }
        }
        else if (other.itemID == "Trash_Bin")
        {
            // ��������Һ��
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
