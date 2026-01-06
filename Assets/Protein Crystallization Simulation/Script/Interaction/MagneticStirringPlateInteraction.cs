using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MagneticStirringPlateInteraction : DraggableUI
{
    public GameObject inputParameterUI;
    public GameObject inputParameterUI_New;
    public GameObject wellPreparedBottle;
    public GameObject wellPreparedBottle100ml;
    public GameObject wellPreparedBottle5ml;
    public GameObject inputQuantityUI_AceticAcid;
    public override void OnBeginDrag(PointerEventData eventData) { }  // ½ûÖ¹ÍÏ¶¯
    public override void OnDrag(PointerEventData eventData) { }
    public override void OnEndDrag(PointerEventData eventData) { }
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
        else if (other.itemID == "Centrifuge Tube")
        {
            if (FindObjectOfType<HintPageUI>().stepCompletedRuntime[4])
            {
                CentrifugeTubeInteraction centrifugeTube = other as CentrifugeTubeInteraction;
                centrifugeTube.levelEmpty.SetActive(false);
                centrifugeTube.levelFull.SetActive(true);
                centrifugeTube.isFull = true;
                FindObjectOfType<HintPageUI>().MarkStepComplete(5);
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
                inputParameterUI_New.SetActive(true);
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
