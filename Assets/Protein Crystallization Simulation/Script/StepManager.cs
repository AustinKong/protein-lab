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
        // ȷ���������ظ�
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

        // ��������ϵͳ�� index
        itemLoader.currentSceneIndex = stepIndex;
        itemLoader.currentRecipe = recipe;
        itemLoader.LoadSceneContent(); // ����԰� Start() ����������������������

        hintUI.currentSceneIndex = stepIndex;
        hintUI.recipe = recipe;
        hintUI.RefreshSteps(); // ���� GenerateStepUI()
    }

    public void GoToSaltSolutionPreparation()
    {
        //-----------------OLD VERSION--------------------
        /*int next = currentStepIndex + 1;
        var exists = recipe.sceneItemRequirements.Exists(r => r.sceneIndex == next);
        if (exists)
            LoadStep(next);
        else
            Debug.Log("���в�������ɣ�");*/

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
        LoadStep(currentStepIndex); // ���¼��ص�ǰ����� UI ��ʵ����Ʒ
    }

}
