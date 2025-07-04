using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class InputChecker_ForPlate : MonoBehaviour
{
    public TMP_InputField inputField; // ʹ�� TMP_InputField ������ InputField
    public TMP_Text feedbackText;     // ʹ�� TMP_Text ������ Text
    public GameObject parameterInputUI;
    public int targetValue;
    public void CheckInput()
    {
        // ��ȡ�������ݲ�����תΪ����
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
