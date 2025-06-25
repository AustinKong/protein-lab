using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene1InteractionLogic : MonoBehaviour
{
    public GameObject bottle;
    public GameObject marker;

    private bool waterTransferred = false;
    private bool marked = false;

    public void TransferWater()
    {
        waterTransferred = true;
        Debug.Log("Water poured into bottle.");
    }

    public void MarkBottle()
    {
        marked = true;
        Debug.Log("Bottle marked.");
    }

    public void OnSceneComplete()
    {
        var bottleInstance = new LabItemInstance
        {
            instanceID = "bottle1",
            volumeML = waterTransferred ? 800f : 0f,
            content = waterTransferred ? "deionised water" : "",
            isMarked = marked
        };

        LabStateManager.Instance.AddOrUpdateItem(bottleInstance);

        UnityEngine.SceneManagement.SceneManager.LoadScene("Scene2");
    }
}
