using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputChecker : MonoBehaviour
{
    public TMP_InputField inputField;   // �����ֶ�
    public TMP_Text feedbackText;       // ��ʾ�ı�
    public int stepNumber;              // ������
    public float targetValue;           // Ŀ��ֵ����ΪС��
    public float tolerance = 0f;      // ������Χ������ ��0.1��
    public GameObject bottleWithWater;
    public GlassBottle100mlInteraction glassBottle100MlInteraction;
    public GlassBottle5mlInteraction glassBottle5MlInteraction;
    public void CheckInput()
    {
        // ��ȡ���벢����תΪ������
        if (float.TryParse(inputField.text, out float value))
        {
            // �ж��Ƿ���Ŀ��ֵ������Χ��
            if (Mathf.Abs(value - targetValue) <= tolerance)
            {
                if (bottleWithWater != null)
                {
                    bottleWithWater.SetActive(true);
                }
                if (glassBottle100MlInteraction != null)
                {
                    glassBottle100MlInteraction.isFulled = true;
                }
                if (glassBottle5MlInteraction != null)
                {
                    glassBottle5MlInteraction.isFulled = true;
                }
                FindObjectOfType<HintPageUI>().MarkStepComplete(stepNumber);
                this.gameObject.SetActive(false);
            }
            else
            {
                feedbackText.text = $"Try again";
            }
        }
        else
        {
            feedbackText.text = "Please enter a valid number";
        }
    }
}
