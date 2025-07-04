using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PHValueChange : MonoBehaviour
{
    public TMP_Text pHValue;

    public void InitializePHValue()
    {
        pHValue.text = "4.5";
    }

    public void IncreasePHValue()
    {
        if (float.TryParse(pHValue.text, out float currentPH))
        {
            currentPH += 0.1f;
            pHValue.text = currentPH.ToString("0.0");

            if (currentPH == 4.5f)
            {
                FindObjectOfType<HintPageUI>().MarkStepComplete(5);
            }
        }
    }
}
