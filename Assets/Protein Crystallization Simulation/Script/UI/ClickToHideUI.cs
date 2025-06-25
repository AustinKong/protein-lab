using UnityEngine;

public class ClickToHideUI : MonoBehaviour
{
    // 是否已经被点击隐藏
    private bool isHidden = false;

    void Start()
    {
        // 一秒后自动隐藏
        Invoke(nameof(AutoHide), 1f);
    }

    void Update()
    {
        // 检测鼠标点击
        if (!isHidden && Input.GetMouseButtonDown(0))
        {
            HideUI();
        }
    }

    private void AutoHide()
    {
        if (!isHidden)
        {
            HideUI();
        }
    }

    private void HideUI()
    {
        gameObject.SetActive(false);
        isHidden = true;
    }
}
