using UnityEngine;

public class StepConditionManager : MonoBehaviour
{
    public static StepConditionManager Instance;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Update()
    {
        CheckCurrentStepConditions();
    }

    void CheckCurrentStepConditions()
    {

    }

    void CheckStepGroup1()
    {
        /*var glassBottle = GameObject.Find("GlassBottle800ml").GetComponent<GlassBottleInteraction>();
        bool step1 = glassBottle.bottle_1000ml.activeSelf || glassBottle.bottle_1000ml_Marked.activeSelf || glassBottle.bottle_Empty_Marked.activeSelf;
        HintPageUI.Instance.SetStepState(0, step1);*/


    }

    void CheckStepGroup2()
    {

    }

    void CheckStepGroup3()
    {

    }

    void CheckStepGroup4()
    {

    }

    void CheckStepGroup5()
    {

    }
}
