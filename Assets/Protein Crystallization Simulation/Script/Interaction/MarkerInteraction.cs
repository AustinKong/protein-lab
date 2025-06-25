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
            Debug.Log("交互对象不是1000ml玻璃瓶");
            return;
        }

        if (bottle.isFulled)
        {
            Debug.Log("标记1000ml玻璃瓶");
            if (bottle.markVisual != null)
                bottle.waterVisual.SetActive(false);
                bottle.markVisual.SetActive(true);
                FindObjectOfType<HintPageUI>().MarkStepComplete(2);


        }
    }
}
