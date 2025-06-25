using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public GameObject recipeSelectionPanel;

    public void OnStartExperimentClicked()
    {
        recipeSelectionPanel.SetActive(true);
    }

    public void OnExitClicked()
    {
        Application.Quit();
    }

    public void OnSelectRecipe(string recipeName)
    {
        PlayerPrefs.SetString("SelectedBuffer", recipeName);
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainExperiment");

    }
}
