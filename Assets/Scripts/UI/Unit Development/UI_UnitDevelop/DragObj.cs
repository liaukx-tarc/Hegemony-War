using UnityEngine;
using UnityEngine.UI;

public class DragObj : MonoBehaviour
{
    public bool isDraging = false;
    public Image image;
    public RectTransform rectTransform;
    public AccessorySelection accessory;
  

    public void Start()
    {
        image= GetComponent<Image>();
        rectTransform= GetComponent<RectTransform>();
        gameObject.SetActive(false);
    }

    public void Update()
    {
        rectTransform.position = Input.mousePosition;

        if (Input.GetMouseButtonDown(1) && isDraging)
        {
            isDraging = false;
            gameObject.SetActive(false);
        }
    }
}
