using UnityEngine;
using UnityEngine.EventSystems;

public class TrashBinInteraction : DraggableUI
{
    public override void OnBeginDrag(PointerEventData eventData) { }  // ��ֹ�϶�
    public override void OnDrag(PointerEventData eventData) { }
    public override void OnEndDrag(PointerEventData eventData) { }

    public override void ExecuteCustomInteraction(DraggableUI other)
    {   
        if(other.itemID == "Measuring Cylinder")
        {
            // ����Ƿ�����Ͳ
            var cylinder = other.GetComponent<MeasuringCylinderInteraction>();
            if (cylinder != null)
            {
                cylinder.currentLevel = 0;
                cylinder.UpdateWaterLevelVisual();

                Debug.Log("����Ͱ���������Ͳ����");
            }
        }
        else if(other.itemID == "GlassBottle800ml")
        {
            var bottle = other.GetComponent<GlassBottleInteraction>();
            if (bottle.isMarked) 
            {
                bottle.bottle_Empty.SetActive(false);
                bottle.bottle_1000ml.SetActive(false);
                bottle.bottle_1000ml_Marked.SetActive(false);
                bottle.bottle_Empty_Marked.SetActive(true);
                bottle.bottle_800ml_Marked.SetActive(false);
                bottle.bottle_800ml.SetActive(false);
            }
            else
            {
                bottle.bottle_Empty.SetActive(true);
                bottle.bottle_1000ml.SetActive(false);
                bottle.bottle_1000ml_Marked.SetActive(false);
                bottle.bottle_Empty_Marked.SetActive(false);
                bottle.bottle_800ml_Marked.SetActive(false);
                bottle.bottle_800ml.SetActive(false);
            }

        }
    }
}
