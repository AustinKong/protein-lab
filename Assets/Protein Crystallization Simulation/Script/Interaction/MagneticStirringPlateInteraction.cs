using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagneticStirringPlateInteraction : DraggableUI
{
    public GameObject inputParameterUI;
    public GameObject wellPreparedBottle;
    public GameObject wellPreparedBottle100ml;
    public GameObject wellPreparedBottle5ml;
    public GameObject inputQuantityUI_AceticAcid;
    public override void ExecuteCustomInteraction(DraggableUI other)
    {   
        if (other.itemID == "GlassBottle800ml")
        {
            if (FindObjectOfType<HintPageUI>().stepCompletedRuntime[2])
            {
                inputParameterUI.SetActive(true);
                other.gameObject.SetActive(false);
                wellPreparedBottle.SetActive(true);
            }
            else
            {
                FindObjectOfType<HintPageUI>().ShowMindOrderPopup();
            }
        }
        else if (other.itemID == "Pipette")
        {
            if (FindObjectOfType<HintPageUI>().stepCompletedRuntime[4])
            {
                PipetteInteraction pipette = other as PipetteInteraction;
                pipette.isFull = true;
            }
            else
            {
                FindObjectOfType<HintPageUI>().ShowMindOrderPopup();
            }
        }
        else if (other.itemID == "Acetic Acid")
        {
            if (FindObjectOfType<HintPageUI>().stepCompletedRuntime[3])
            {
                inputQuantityUI_AceticAcid.SetActive(true);
            }
            else
            {
                FindObjectOfType<HintPageUI>().ShowMindOrderPopup();
            }

        }
        else if (other.itemID == "Dropper SodiumHydroxide")
        {
            FindObjectOfType<PHValueChange>().IncreasePHValue();
        }
        else if (other.itemID == "GlassBottle100ml")
        {   
            if (FindObjectOfType<HintPageUI>().stepCompletedRuntime[3])
            {
                inputParameterUI.SetActive(true);
                other.gameObject.SetActive(false);
                wellPreparedBottle100ml.SetActive(true);
            }
            else
            {
                FindObjectOfType<HintPageUI>().ShowMindOrderPopup();
            }
        }
        else if (other.itemID == "GlassBottle5ml")
        {   
            if (FindObjectOfType<HintPageUI>().stepCompletedRuntime[2])
            {
                inputParameterUI.SetActive(true);
                other.gameObject.SetActive(false);
                wellPreparedBottle5ml.SetActive(true);
            }
            else
            {
                FindObjectOfType<HintPageUI>().ShowMindOrderPopup();
            }
        }
    }
}
