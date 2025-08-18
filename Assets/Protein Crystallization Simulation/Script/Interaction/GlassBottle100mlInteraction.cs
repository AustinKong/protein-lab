using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassBottle100mlInteraction : DraggableUI
{
    public bool isFulled = false;
    public GameObject emptyGlassBottle100ml;
    public GameObject fullGlassBottle100ml;
    public GameObject magneticStirrer;
    public GameObject inputQuantityUI_1;
    public GameObject inputQuantityUI_2;
    public GameObject inputQuantityUI_3;

    public override void ExecuteCustomInteraction(DraggableUI other)
    {
        if (other.itemID == "GlassBottle800ml")
        {
            /*emptyGlassBottle100ml.SetActive(false);
            fullGlassBottle100ml.SetActive(true);
            isFulled = true;
            FindObjectOfType<HintPageUI>().MarkStepComplete(0);*/
            inputQuantityUI_3.SetActive(true);
        }
        else if (other.itemID == "Magnetic Stirrer")
        {
            if (isFulled)
            {
                magneticStirrer.SetActive(true);
                FindObjectOfType<HintPageUI>().MarkStepComplete(1);
                other.gameObject.SetActive(false);
            }
            else
            {
                FindObjectOfType<HintPageUI>().ShowMindOrderPopup();
            }
        }
        else if (other.itemID == "Sodium Chloride")
        {
            if (FindObjectOfType<HintPageUI>().stepCompletedRuntime[1])
            {
                inputQuantityUI_1.SetActive(true);
            }
            else
            {
                FindObjectOfType<HintPageUI>().ShowMindOrderPopup();
            }
        }
        else if (other.itemID == "Polyethylene Glycol")
        {
            if (FindObjectOfType<HintPageUI>().stepCompletedRuntime[2])
            {
                inputQuantityUI_2.SetActive(true);
            }
            else
            {
                FindObjectOfType<HintPageUI>().ShowMindOrderPopup();
            }
        }
        else if (other.itemID == "Pipette")
        {
            PipetteInteraction pipette = other as PipetteInteraction;
            pipette.withSalt = true;
            pipette.withProtein = false;
            pipette.UpdateSolutionUI();
        }
    }
}
