using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]

public class DialogBar : MonoBehaviour
{
    private Image barImage;

    private RectTransform rectTransform;

    private Vector2 hiddenPosition = new Vector2(0, -100);

    private Vector2 visiblePosition = Vector2.zero;

    private float animationSpeed = 10;
    // Start is called before the first frame update
    private void Awake()
    {
        barImage = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
    }
    void Start()
    {
        rectTransform.anchoredPosition = hiddenPosition;
    }
    public IEnumerator ShowBar()
    {
        while (rectTransform.anchoredPosition.y > visiblePosition.y)
        {
            rectTransform.anchoredPosition += Vector2.up * animationSpeed * Time.deltaTime;
            Debug.Log("oiii");
            yield return null;

        }
        rectTransform.anchoredPosition = visiblePosition;
    }
    public IEnumerator HideBar()
    {
        while (rectTransform.anchoredPosition.y < hiddenPosition.y)
        {
            rectTransform.anchoredPosition -= Vector2.up * animationSpeed * Time.deltaTime;
            yield return null;
        }
        rectTransform.anchoredPosition = hiddenPosition;
    }
    
    // Update is called once per frame

}
