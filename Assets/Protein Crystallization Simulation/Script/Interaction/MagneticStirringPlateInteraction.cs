using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagneticStirringPlateInteraction : DraggableUI
{
    public GameObject inputParameterUI;
    public GameObject wellPreparedBottle;
    public override void ExecuteCustomInteraction(DraggableUI other)
    {   
        if (other.itemID == "GlassBottle800ml" && other.GetComponent<GlassBottleInteraction>().magneticStirrer.activeSelf && other.GetComponent<GlassBottleInteraction>().bottleCap.activeSelf)
        {
            inputParameterUI.SetActive(true);
            other.gameObject.SetActive(false);
            wellPreparedBottle.SetActive(true);
        }
        else if (other.itemID == "Pipette")
        {
            PipetteInteraction pipette = other as PipetteInteraction;
            pipette.isFull = true;
        }
        else if (other.itemID == "Dropper SodiumHydroxide")
        {
            FindObjectOfType<PHValueChange>().IncreasePHValue();
        }
    }
}
