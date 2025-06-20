using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LockableButtonSceneConnector : MonoBehaviour
{
  [SerializeField] private GameObject lockOverlay;
  [SerializeField] private string sceneToLoad;
  [SerializeField] private string unlockName;

  public void Start()
  {
    if (SceneManager.Instance.IsSceneUnlocked(unlockName))
    {
      lockOverlay.SetActive(false);
      GetComponent<Button>().onClick.AddListener(() =>
      {
        SceneManager.Instance.LoadScene(sceneToLoad);
        SoundManager.Instance.PlaySFX("SFX-impact-mechanical-01_wav");
      });
    }
    else
    {
      lockOverlay.SetActive(true);
      GetComponent<Button>().interactable = false;
    }
  }
}
