using UnityEngine;
using UnityEngine.UI;

public class UnlockAll : MonoBehaviour
{
    string[] scenesToUnlock = new string[]
    {
        "Act1", "Act2", "Act3", "Nucleation", "SequenceAlignment", "Xray", "BufferPreparation", "SaltSolutionPreparation",
        "ProteinSolutionPreparation", "ProteinCrystallization"
    };
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            foreach (string scene in scenesToUnlock)
            {
                SceneManager.Instance.UnlockScene(scene);
            }
            SoundManager.Instance.PlaySFX("SFX-impact-mechanical-01_wav");
        });
    }
}
