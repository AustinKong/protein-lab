using UnityEngine;

public class NoNeedInputChecker : MonoBehaviour
{
    public void CheckInputs()
    {       
        FindObjectOfType<HintPageUI>().MarkStepComplete(4);
    }
}
