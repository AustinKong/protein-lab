using UnityEngine;

public class ClickToHideUI : MonoBehaviour
{
    // �Ƿ��Ѿ����������
    private bool isHidden = false;

    void Start()
    {
        // һ����Զ�����
        Invoke(nameof(AutoHide), 1f);
    }

    void Update()
    {
        // ��������
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
