using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StoreItem : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public bool IsSelected { get; private set; }

    StoreScreen storeScreen;
    Image image;
    float startLocalScale, hoverScaleMultiplier = 1.05f, selectScaleMultiplier = 1.1f;

    void Start()
    {
        storeScreen = GameObject.Find("Screen").GetComponent<StoreScreen>();
        image = GetComponent<Image>();
        startLocalScale = transform.localScale.x;
    }

    void Update()
    {

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!IsSelected)
            storeScreen.SelectItem(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsSelected)
        {
            float newScale = startLocalScale * hoverScaleMultiplier;
            transform.localScale = new Vector3(newScale, newScale, newScale);
        }

        MenuController.Instance.SetCursor(Cursors.Pointer);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!IsSelected)
        {
            transform.localScale = new Vector3(startLocalScale, startLocalScale, startLocalScale);
        }

        MenuController.Instance.SetCursor(Cursors.Arrow);
    }

    public void Select()
    {
        IsSelected = true;
        float newScale = startLocalScale * selectScaleMultiplier;
        transform.localScale = new Vector3(newScale, newScale, newScale);
        image.color = new Color32(100, 200, 255, 255);
    }

    public void Deselect()
    {
        IsSelected = false;
        transform.localScale = new Vector3(startLocalScale, startLocalScale, startLocalScale);
        image.color = new Color32(255, 255, 255, 255);
    }
}
