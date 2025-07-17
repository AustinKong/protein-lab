using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipetteInteraction : DraggableUI
{   
    public bool withSalt = false;
    public bool withProtein = false;
    public bool isFull = false;
    public GameObject getProteinSolution;
    public GameObject getSaltSolution;

    public void UpdateSolutionUI()
    {
        if (getSaltSolution != null)
        {
            getSaltSolution.SetActive(withSalt);
        }

        if (getProteinSolution != null)
        {
            getProteinSolution.SetActive(withProtein);
        }
    }
}
