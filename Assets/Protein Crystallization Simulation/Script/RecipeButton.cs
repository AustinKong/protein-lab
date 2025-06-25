using UnityEngine;
using UnityEngine.SceneManagement;

public class RecipeButton : MonoBehaviour
{
    public BufferRecipe recipeToLoad;

    public void OnClick()
    {
        BufferRecipeManager.currentRecipe = recipeToLoad;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainExperiment");
    }
}
