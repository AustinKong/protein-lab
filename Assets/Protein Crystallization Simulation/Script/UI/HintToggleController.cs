using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HintToggleController : MonoBehaviour
{
    public GameObject stepContainer;          
    public Sprite iconShow;                   
    public Sprite iconHide;                   
    public Image iconImage;                   

    private bool isVisible = true;

    public void ToggleHintVisibility()
    {
        isVisible = !isVisible;

        if (iconImage != null)
        {
            iconImage.sprite = isVisible ? iconHide : iconShow;
        }

        foreach (Transform child in stepContainer.transform)
        {
            SetAlphaRecursively(child, isVisible ? 1f : 0f);
        }
    }

    private void SetAlphaRecursively(Transform target, float alpha)
    {
        TextMeshProUGUI tmp = target.GetComponent<TextMeshProUGUI>();
        if (tmp != null)
        {
            Color c = tmp.color;
            c.a = alpha;
            tmp.color = c;
        }

        Image img = target.GetComponent<Image>();
        if (img != null)
        {
            Color c = img.color;
            c.a = alpha;
            img.color = c;
        }

        foreach (Transform child in target)
        {
            SetAlphaRecursively(child, alpha);
        }
    }
}
