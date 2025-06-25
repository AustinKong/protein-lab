using UnityEngine;
using TMPro;

public class DualInputChecker : MonoBehaviour
{
    public TMP_InputField inputField1; // ��һ�������������������
    public TMP_InputField inputField2; // �ڶ������������ˮ�����
    public int targetValue1;
    public int targetValue2;
    public TMP_Text feedbackText;     // ��һ����ʾ�ı�
    public StepManager stepManager;

    void Start()
    {
        stepManager = FindObjectOfType<StepManager>();
    }

    public void CheckInputs()
    {
        bool input1Correct = false;
        bool input2Correct = false;

        // ����һ�������
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

        // ���ڶ��������
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

        // �����������ȷ
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
