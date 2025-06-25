using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassBottle1000mlInteraction : DraggableUI
{   
    public bool isFulled = false;
    public int expectedLevel = 2; // 2����1000ml
    public GameObject waterVisual;
    public GameObject markVisual;

    public override void ExecuteCustomInteraction(DraggableUI other)
    {
        if (other.itemID == "Measuring Cylinder")
        {
            MeasuringCylinderInteraction cylinder = other.GetComponent<MeasuringCylinderInteraction>();
            if (cylinder == null)
            {
                Debug.Log("������������Ͳ");
                return;
            }

            if (cylinder.currentLevel == expectedLevel)
            {
                Debug.Log("[1000mlƿ] �ɹ�����Һ��");
                isFulled = true;
                if (waterVisual != null)
                    waterVisual.SetActive(true);
                FindObjectOfType<HintPageUI>().MarkStepComplete(1);
                cylinder.currentLevel = 0;
                cylinder.UpdateWaterLevelVisual();
            }
            else
            {
                Debug.Log("[1000mlƿ] Һ��ȼ���ƥ�䣬�ܾ�����");
                //cylinder.ShowInvalidInteraction();
            }
        }
        else if(other.itemID == "Marker")
        {
            if (isFulled)
            {
                Debug.Log("���1000ml����ƿ");
                waterVisual.SetActive(false);
                markVisual.SetActive(true);
                FindObjectOfType<HintPageUI>().MarkStepComplete(2);


            }
        }
    }
}
