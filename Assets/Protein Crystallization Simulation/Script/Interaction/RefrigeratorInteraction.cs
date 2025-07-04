using UnityEngine;

public class RefrigeratorInteraction : DraggableUI
{
    public GameObject inputParameterUI;
    public override void ExecuteCustomInteraction(DraggableUI other)
    {
        if (other.itemID == "24WellPlate")
        {
            other.gameObject.SetActive(false);
            inputParameterUI.SetActive(true);
        }
    }
}
