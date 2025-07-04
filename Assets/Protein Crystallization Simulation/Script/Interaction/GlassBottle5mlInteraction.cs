using UnityEngine;

public class GlassBottle5mlInteraction : DraggableUI
{
    public bool isFulled = false;
    public GameObject emptyGlassBottle5ml;
    public GameObject fullGlassBottle5ml;
    public GameObject magneticStirrer;
    public GameObject inputQuantityUI_1;

    public override void ExecuteCustomInteraction(DraggableUI other)
    {
        if (other.itemID == "GlassBottle800ml")
        {
            emptyGlassBottle5ml.SetActive(false);
            fullGlassBottle5ml.SetActive(true);
            isFulled = true;
            FindObjectOfType<HintPageUI>().MarkStepComplete(0);
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
        else if (other.itemID == "Lysozyme")
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
        else if (other.itemID == "Pipette")
        {
            if (FindObjectOfType<HintPageUI>().stepCompletedRuntime[0])
            {
                PipetteInteraction pipette = other as PipetteInteraction;
                pipette.withProtein = true;
                pipette.withSalt = false;
            }
            else
            {
                FindObjectOfType<HintPageUI>().ShowMindOrderPopup();
            }
        }
    }
}
