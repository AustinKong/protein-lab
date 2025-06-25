using UnityEngine;
using TMPro;

public class DualInputChecker : MonoBehaviour
{
    public TMP_InputField inputField1; // 第一个输入框（例如盐质量）
    public TMP_InputField inputField2; // 第二个输入框（例如水体积）
    public int targetValue1;
    public int targetValue2;
    public TMP_Text feedbackText;     // 第一个提示文本
    public StepManager stepManager;

    void Start()
    {
        stepManager = FindObjectOfType<StepManager>();
    }

    public void CheckInputs()
    {
        bool input1Correct = false;
        bool input2Correct = false;

        // 检查第一个输入框
        if (int.TryParse(inputField1.text, out int value1))
        {
            if (value1 == targetValue1)
            {
                input1Correct = true;
                feedbackText.text = "";
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

        // 检查第二个输入框
        if (int.TryParse(inputField2.text, out int value2))
        {
            if (value2 == targetValue2)
            {
                input2Correct = true;
                feedbackText.text = "";
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

        // 如果两个都正确
        if (input1Correct && input2Correct)
        {   
            if(stepManager.currentStepIndex == 2)
            {
                FindObjectOfType<HintPageUI>().MarkStepComplete(3);
            }
            else if( stepManager.currentStepIndex == 3)
            {
                FindObjectOfType<HintPageUI>().MarkStepComplete(2);
            }

            this.gameObject.SetActive(false);
        }
    }
}
