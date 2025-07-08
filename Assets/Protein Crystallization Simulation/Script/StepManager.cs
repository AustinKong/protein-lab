using UnityEngine;
using UnityEngine.SceneManagement;
public class StepManager : MonoBehaviour
{
    //public static StepManager Instance;

    public BufferRecipe recipe;
    public SceneLoader itemLoader;
    public HintPageUI hintUI;

    public int currentStepIndex = 1;
    public int CurrentStepIndex => currentStepIndex;


    /*void Awake()
    {
        // 确保单例不重复
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }*/

    void Start()
    {
        LoadStep(currentStepIndex);
    }

    public void LoadStep(int stepIndex)
    {
        currentStepIndex = stepIndex;

        // 更新两个系统的 index
        itemLoader.currentSceneIndex = stepIndex;
        itemLoader.currentRecipe = recipe;
        itemLoader.LoadSceneContent(); // 你可以把 Start() 的内容提出来做成这个函数

        hintUI.currentSceneIndex = stepIndex;
        hintUI.recipe = recipe;
        hintUI.RefreshSteps(); // 类似 GenerateStepUI()
    }

    public void GoToSaltSolutionPreparation()
    {
        //-----------------OLD VERSION--------------------
        /*int next = currentStepIndex + 1;
        var exists = recipe.sceneItemRequirements.Exists(r => r.sceneIndex == next);
        if (exists)
            LoadStep(next);
        else
            Debug.Log("所有步骤已完成！");*/

        //----------------NEWVERSION-----------------------
        SceneManager.Instance.UnlockScene("SaltSolutionPreparation");
        SceneManager.Instance.LoadScene("ExperimentSelect");
    }

    public void GoToProteinSolutionPreparation()
    {
        SceneManager.Instance.UnlockScene("ProteinSolutionPreparation");
        SceneManager.Instance.LoadScene("ExperimentSelect");
    }

    public void GoToProteinCrystalisation()
    {
        SceneManager.Instance.UnlockScene("ProteinCrystalisation");
        SceneManager.Instance.LoadScene("ExperimentSelect");
    }
    public void RestartCurrentStep()
    {
        LoadStep(currentStepIndex); // 重新加载当前步骤的 UI 和实验物品
    }

}
