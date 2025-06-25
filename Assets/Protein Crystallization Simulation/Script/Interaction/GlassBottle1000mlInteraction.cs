using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassBottle1000mlInteraction : DraggableUI
{   
    public bool isFulled = false;
    public int expectedLevel = 2; // 2代表1000ml
    public GameObject waterVisual;
    public GameObject markVisual;

    public override void ExecuteCustomInteraction(DraggableUI other)
    {
        if (other.itemID == "Measuring Cylinder")
        {
            MeasuringCylinderInteraction cylinder = other.GetComponent<MeasuringCylinderInteraction>();
            if (cylinder == null)
            {
                Debug.Log("交互对象不是量筒");
                return;
            }

            if (cylinder.currentLevel == expectedLevel)
            {
                Debug.Log("[1000ml瓶] 成功接收液体");
                isFulled = true;
                if (waterVisual != null)
                    waterVisual.SetActive(true);
                FindObjectOfType<HintPageUI>().MarkStepComplete(1);
                cylinder.currentLevel = 0;
                cylinder.UpdateWaterLevelVisual();
            }
            else
            {
                Debug.Log("[1000ml瓶] 液体等级不匹配，拒绝接收");
                //cylinder.ShowInvalidInteraction();
            }
        }
        else if(other.itemID == "Marker")
        {
            if (isFulled)
            {
                Debug.Log("标记1000ml玻璃瓶");
                waterVisual.SetActive(false);
                markVisual.SetActive(true);
                FindObjectOfType<HintPageUI>().MarkStepComplete(2);


            }
        }
    }
}
