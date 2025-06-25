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
        if (StepManager.Instance == null) return;

        int currentScene = StepManager.Instance.CurrentStepIndex;

        switch (currentScene)
        {
            case 1:
                CheckStepGroup1(); break;
            case 2:
                CheckStepGroup2(); break;
            case 3:
                CheckStepGroup3(); break;
            case 4:
                CheckStepGroup4(); break;
            case 5:
                CheckStepGroup5(); break;
        }
    }

    void CheckStepGroup1()
    {
        var glassBottle = GameObject.Find("GlassBottle800ml").GetComponent<GlassBottleInteraction>();
        bool step1 = glassBottle.bottle_1000ml.activeSelf || glassBottle.bottle_1000ml_Marked.activeSelf || glassBottle.bottle_Empty_Marked.activeSelf;
        HintPageUI.Instance.SetStepState(0, step1);


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
