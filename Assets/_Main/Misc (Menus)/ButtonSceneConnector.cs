using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSceneConnector : MonoBehaviour
{
  [SerializeField] private string sceneToLoad;

  public void Start()
  {
    GetComponent<Button>().onClick.AddListener(() =>
    {
      SceneManager.Instance.LoadScene(sceneToLoad);
      SoundManager.Instance.PlaySFX("SFX-impact-mechanical-01_wav");
    });
  }
}
