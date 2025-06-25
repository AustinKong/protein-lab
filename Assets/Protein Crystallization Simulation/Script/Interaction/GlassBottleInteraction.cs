using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassBottleInteraction : DraggableUI
{
    public bool isMarked = false;
    public GameObject bottle_Empty;
    public GameObject bottle_1000ml;
    public GameObject bottle_1000ml_Marked;
    public GameObject bottle_Empty_Marked;
    public GameObject bottle_800ml;
    public GameObject bottle_800ml_Marked;
    public GameObject bottleCap;
    public GameObject magneticStirrer;
    public GameObject inputQuantityUI_1;
    public GameObject inputQuantityUI_2;

    public StepManager stepManager;

    protected override void Start()
    {
        base.Start();
        stepManager = FindObjectOfType<StepManager>();
    }

    public override void ExecuteCustomInteraction(DraggableUI other)
    {
        if (other.itemID == "Measuring Cylinder")
        {
            MeasuringCylinderInteraction cylinder = other.GetComponent<MeasuringCylinderInteraction>();
            if (cylinder.currentLevel == 1f)
            {   
                if (bottle_Empty_Marked.activeSelf) 
                {
                    bottle_Empty.SetActive(false);
                    bottle_800ml_Marked.SetActive(true);
                    cylinder.currentLevel = 0;
                    cylinder.UpdateWaterLevelVisual();
                    FindObjectOfType<HintPageUI>().MarkStepComplete(2);
                }
                if(bottle_Empty.activeSelf)
                {
                    FindObjectOfType<HintPageUI>().MarkStepComplete(2);
                }
            }
            if (cylinder.currentLevel == 2)
            {
                if (isMarked)
                {
                    bottle_Empty.SetActive(false);
                    bottle_1000ml_Marked.SetActive(true);
                }
                else
                {
                    bottle_Empty.SetActive(false);
                    bottle_1000ml.SetActive(true);
                    FindObjectOfType<HintPageUI>().MarkStepComplete(0);
                }
                cylinder.currentLevel = 0;
                cylinder.UpdateWaterLevelVisual();
            }
            else
            {
                Debug.Log("[800ml瓶] 液体等级不匹配，拒绝接收");
                //cylinder.ShowInvalidInteraction();
            }
        }
        else if (other.itemID == "Magnetic Stirrer")
        {
            magneticStirrer.SetActive(true);
            other.gameObject.SetActive(false);
            FindObjectOfType<HintPageUI>().MarkStepComplete(0);
        }
        else if (other.itemID == "Sodium Acetate")
        {
            if (inputQuantityUI_1 != null && FindObjectOfType<HintPageUI>().stepCompletedRuntime[0])
            {
                inputQuantityUI_1.SetActive(true);
            }
            else
            {
                FindObjectOfType<HintPageUI>().mindOrderUI.SetActive(true);
            }
}
        else if (other.itemID == "Bottle Cap")
        {   
            if (stepManager.currentStepIndex == 2)
            {
                if (HintPageUI.Instance.stepCompletedRuntime[1])
                {
                    other.gameObject.SetActive(false);
                    bottleCap.SetActive(true);
                }
                FindObjectOfType<HintPageUI>().MarkStepComplete(2);
            }
            else if(stepManager.currentStepIndex == 3)
            {
                if (HintPageUI.Instance.stepCompletedRuntime[0])
                {
                    other.gameObject.SetActive(false);
                    bottleCap.SetActive(true);
                }
                FindObjectOfType<HintPageUI>().MarkStepComplete(1);
            }
            //bottleCap.SetActive(true);
        }
        else if (other.itemID == "Acetic Acid")
        {
            if (inputQuantityUI_2 != null)
            {
                inputQuantityUI_2.SetActive(true);
            }
        }
        else if (other.itemID == "Marker" && bottle_1000ml.activeSelf)
        {   
            isMarked = true;
            bottle_1000ml_Marked.SetActive(true);
            FindObjectOfType<HintPageUI>().MarkStepComplete(1);
        }
        else if (other.itemID == "Magnetic Rod")
        {
            magneticStirrer.SetActive(false);
            MagneticRodInteraction magneticRod = other.GetComponent<MagneticRodInteraction>();
            magneticRod.magnericStirrer.SetActive(true) ;
            FindObjectOfType<HintPageUI>().MarkStepComplete(0);
        }
        else if (other.itemID == "De-ionised Water")
        {
            if (FindObjectOfType<HintPageUI>().stepCompletedRuntime[0])
            {
                bottle_1000ml_Marked.SetActive(true);
                bottle_800ml_Marked.SetActive(false);
            }
            FindObjectOfType<HintPageUI>().MarkStepComplete(1);
        }
        else if (other.itemID == "Pipette")
        {
            PipetteInteraction pipette = other as PipetteInteraction;
            pipette.isFull = true;
        }
    }
}
