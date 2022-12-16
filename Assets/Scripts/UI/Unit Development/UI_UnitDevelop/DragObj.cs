using UnityEngine;
using UnityEngine.UI;

public class DragObj : MonoBehaviour
{
    const string Cancel = "Cancel";

    public static bool isDraging = false;
    public static Image image;
    public static RectTransform rectTransform;
    public static AccessorySelection accessory;
  

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
