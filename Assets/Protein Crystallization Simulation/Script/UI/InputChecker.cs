using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputChecker : MonoBehaviour
{
    public TMP_InputField inputField;   // 输入字段
    public TMP_Text feedbackText;       // 提示文本
    public int stepNumber;              // 步骤编号
    public float targetValue;           // 目标值，可为小数
    public float tolerance = 0f;      // 允许误差范围（例如 ±0.1）
    public GameObject bottleWithWater;
    public GlassBottle100mlInteraction glassBottle100MlInteraction;
    public GlassBottle5mlInteraction glassBottle5MlInteraction;
    public void CheckInput()
    {
        // 获取输入并尝试转为浮点数
        if (float.TryParse(inputField.text, out float value))
        {
            // 判断是否在目标值的容许范围内
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
