using UnityEngine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public int currentSceneIndex;
    public BufferRecipe currentRecipe;

    [Header("UI Container for Prefabs")]
    public RectTransform itemParent; // 拖入 Canvas 下的 ItemContainer

    public void LoadSceneContent()
    {
        var sceneSet = currentRecipe.sceneItemRequirements.Find(s => s.sceneIndex == currentSceneIndex);
        if (sceneSet == null) return;

        foreach (Transform child in itemParent)
            Destroy(child.gameObject); // 清除上一步物品

        foreach (var item in sceneSet.items)
        {
            GameObject obj = Instantiate(item.prefab, itemParent);
            obj.name = item.instanceID;

            RectTransform rt = obj.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchoredPosition = item.uiPosition;
                rt.localRotation = Quaternion.identity;
            }
        }
    }

    public void LoadMainMenu()
    {
        DestroyAllManagers(); // 销毁静态单例或 DontDestroy 对象

        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu_Simulattion");
    }

    void DestroyAllManagers()
    {
        if (LabStateManager.Instance != null)
            Destroy(LabStateManager.Instance.gameObject);

        if (StepManager.Instance != null)
            Destroy(StepManager.Instance.gameObject);


        // 其他你用 DontDestroyOnLoad 保留的脚本也加上
    }
}
