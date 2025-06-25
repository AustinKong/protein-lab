using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CentrifugeTubeInteraction : DraggableUI
{
    public GameObject levelEmpty;
    public GameObject levelFull;
    public GameObject pHProbe;
    public GameObject conductivityProbe;
    public bool isFull = false;
    public StepManager stepManager;
    protected override void Start()
    {
        base.Start();
        stepManager = FindObjectOfType<StepManager>();
    }

    public override void ExecuteCustomInteraction(DraggableUI other)
    {
        if (other.itemID == "Pipette")
        {
            PipetteInteraction pipette = other as PipetteInteraction;
            if (pipette.isFull)
            {
                levelEmpty.SetActive(false);
                levelFull.SetActive(true);
                isFull = true;
                if (stepManager.currentStepIndex == 3)
                {
                    FindObjectOfType<HintPageUI>().MarkStepComplete(3);
                }
                else if (stepManager.currentStepIndex == 5)
                {
                    FindObjectOfType<HintPageUI>().MarkStepComplete(0);
                }
            }
        }
        else if (other.itemID == "pH Probe")
        {
            if (isFull) 
            {   
                other.gameObject.SetActive(false);
                pHProbe.SetActive(true);
                FindObjectOfType<PHValueChange>().InitializePHValue();
                FindObjectOfType<HintPageUI>().MarkStepComplete(4);
            }
            else 
            {
            }
        }
        else if (other.itemID == "Conductivity Probe")
        {
            if (isFull)
            {
                other.gameObject.SetActive(false);
                conductivityProbe.SetActive(true);
                FindObjectOfType<HintPageUI>().MarkStepComplete(1);
            }
            else
            {
            }
        }
    }
}
