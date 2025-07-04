using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class InputChecker_ForPlate : MonoBehaviour
{
    public TMP_InputField inputField; // 使用 TMP_InputField 而不是 InputField
    public TMP_Text feedbackText;     // 使用 TMP_Text 而不是 Text
    public GameObject parameterInputUI;
    public int targetValue;
    public void CheckInput()
    {
        // 获取输入内容并尝试转为数字
        if (int.TryParse(inputField.text, out int value))
        {
            if (value == targetValue)
            {
            
                this.gameObject.SetActive(false);
                parameterInputUI.SetActive(true);
            }
            else
            {
                feedbackText.text = "Try again";
            }
        }
        else
        {
            feedbackText.text = "Please enter a valid number";
        }
    }

}
