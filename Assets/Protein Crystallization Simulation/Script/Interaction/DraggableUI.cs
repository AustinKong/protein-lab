using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class DraggableUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    private Vector2 startAnchoredPosition;
    public float returnDuration = 0.3f; 

    [Header("Interaction Settings")]
    public string itemID;
    public List<string> interactableIDs;

    public GameObject invalidInteractionUIPopupPrefab; 
    public Transform feedbackContainer;                

    private GameObject currentPopupInstance;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    protected virtual void Start()
    {
        feedbackContainer = GameObject.Find("Canvas/FeedBack")?.transform;
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        startAnchoredPosition = rectTransform.anchoredPosition; // 记录起始位置
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (canvas != null)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
    }

    public void OnDrop(PointerEventData eventData)
    {
        DraggableUI droppedItem = eventData.pointerDrag?.GetComponent<DraggableUI>();
        if (droppedItem == null) return;

        if (interactableIDs.Contains(droppedItem.itemID))
        {
            Debug.Log($"[✔] {droppedItem.itemID} 与 {itemID} 成功交互！");
            ExecuteCustomInteraction(droppedItem);
            droppedItem.ReturnToStartPositionAnimated();
        }
        else
        {
            ShowInvalidInteractionPopup();
            droppedItem.ReturnToStartPositionAnimated();
        }
    }

    public void ReturnToStartPositionAnimated()
    {
        StopAllCoroutines();
        if (gameObject.activeSelf)
        {
            StartCoroutine(AnimateReturn());
        }
    }

    private IEnumerator AnimateReturn()
    {
        Vector2 currentPos = rectTransform.anchoredPosition;
        float time = 0f;
        while (time < returnDuration)
        {
            time += Time.deltaTime;
            float t = time / returnDuration;
            rectTransform.anchoredPosition = Vector2.Lerp(currentPos, startAnchoredPosition, t);
            yield return null;
        }
        rectTransform.anchoredPosition = startAnchoredPosition;
    }

    private void ShowInvalidInteractionPopup()
    {
        if (invalidInteractionUIPopupPrefab != null && feedbackContainer != null)
        {
            // 如果之前已经生成过一个提示框，先销毁它（可选）
            if (currentPopupInstance != null)
            {
                Destroy(currentPopupInstance);
            }

            // 实例化一个新的提示框，作为 Feedback 的子对象
            currentPopupInstance = Instantiate(invalidInteractionUIPopupPrefab, feedbackContainer);
            currentPopupInstance.transform.localPosition = Vector3.zero; // 可根据需要调整位置

            // 自动隐藏或销毁
            Invoke(nameof(HidePopup), 1f);
        }
    }

    private void HidePopup()
    {
        if (currentPopupInstance != null)
        {
            Destroy(currentPopupInstance); // 或者使用 SetActive(false) 如果你不想销毁
        }
    }

    public virtual void ExecuteCustomInteraction(DraggableUI other)
    {
        Debug.Log($"Default interaction handler between {other.itemID} and {itemID}. Override this method in derived class.");
    }
}
