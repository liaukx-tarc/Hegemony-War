using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tab_Button : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public TabGroup tabGroup;
    public GameObject ui;
    public Image background;

    public UnityEvent action;

    private void Start()
    {
        background = GetComponent<Image>();
        tabGroup.Subscribe(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        WorldController.instance.uiController.ClickSound();
        tabGroup.OnTabButtonSelected(this);
        action.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tabGroup.OnTabButtonEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tabGroup.OnTabButtonExit(this);
    }
}
