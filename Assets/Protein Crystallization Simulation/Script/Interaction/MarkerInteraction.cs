using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerInteraction : DraggableUI
{

    public override void ExecuteCustomInteraction(DraggableUI other)
    {
        GlassBottle1000mlInteraction bottle = other.GetComponent<GlassBottle1000mlInteraction>();
        if (bottle == null)
        {
            Debug.Log("����������1000ml����ƿ");
            return;
        }

        if (bottle.isFulled)
        {
            Debug.Log("���1000ml����ƿ");
            if (bottle.markVisual != null)
                bottle.waterVisual.SetActive(false);
                bottle.markVisual.SetActive(true);
                FindObjectOfType<HintPageUI>().MarkStepComplete(2);


        }
    }
}
