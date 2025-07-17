using UnityEngine;

public class WellPlateInteraction : DraggableUI
{
    public GameObject liquid;
    public GameObject glassSlide;
    public override void ExecuteCustomInteraction(DraggableUI other)
    {
        if (other.itemID == "Pipette")
        {
            PipetteInteraction pipette = other as PipetteInteraction;
            if (pipette.withSalt)
            {
                pipette.withSalt = false;
                pipette.UpdateSolutionUI();
                liquid.SetActive(true);
                FindObjectOfType<HintPageUI>().MarkStepComplete(0);
            }
            else if(pipette.withProtein)
            {
                if (FindObjectOfType<HintPageUI>().stepCompletedRuntime[0])
                {
                    pipette.withProtein = false;
                    pipette.UpdateSolutionUI();
                    FindObjectOfType<HintPageUI>().MarkStepComplete(1);
                }
                else 
                {
                    FindObjectOfType<HintPageUI>().ShowMindOrderPopup();
                }

            }
        }
        else if(other.itemID == "GlassSlide")
        {
            if (FindObjectOfType<HintPageUI>().stepCompletedRuntime[1])
            {
                glassSlide.SetActive(true);
                FindObjectOfType<HintPageUI>().MarkStepComplete(2);
                other.gameObject.SetActive(false);
            }
            else
            {
                FindObjectOfType<HintPageUI>().ShowMindOrderPopup();
            }
        }
    }
}
