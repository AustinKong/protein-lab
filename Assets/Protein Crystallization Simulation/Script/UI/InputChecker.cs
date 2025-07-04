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
    public float tolerance = 0.1f;      // ������Χ������ ��0.1��

    public void CheckInput()
    {
        // ��ȡ���벢����תΪ������
        if (float.TryParse(inputField.text, out float value))
        {
            // �ж��Ƿ���Ŀ��ֵ������Χ��
            if (Mathf.Abs(value - targetValue) <= tolerance)
            {
                FindObjectOfType<HintPageUI>().MarkStepComplete(stepNumber);
                this.gameObject.SetActive(false);
            }
            else
            {
                feedbackText.text = $"Try again (Hint: must be close to {targetValue})";
            }
        }
        else
        {
            feedbackText.text = "Please enter a valid number (e.g., 12.5)";
        }
    }
}
