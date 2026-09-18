using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

//https://www.youtube.com/watch?v=BGr-7GZJNXg
public class DragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    [SerializeField] private Canvas canvas; 
    private RectTransform rectTransform; 
    private CanvasGroup canvasGroup; 

    [SerializeField] private float tempsFade; 

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        //Les rends transparents au début
        this.GetComponent<CanvasGroup>().alpha = 0f; 
        //Puis réaffiche
        StartCoroutine(FadeFromTransparent(tempsFade)); 
    }

    //évite que les mots ne tournent sur eux mêmes 
    void LateUpdate ()
    {
        transform.rotation = Quaternion.identity;   
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("On Begin Drag"); 
        canvasGroup.blocksRaycasts = false; //Comme ça le drop pourra s'activer sur l'item slot
        canvasGroup.alpha = .6f; 
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("On Drag"); 
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor; 
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("On End Drag"); 
        canvasGroup.blocksRaycasts = true; 
        canvasGroup.alpha = 1f; 
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("On Pointer Down"); 
    }

    public void OnDrop(PointerEventData eventData)
    {
        
    }

    public IEnumerator FadeFromTransparent(float duration)
    {

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            this.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(0f, 1f, t / duration);
            yield return null;
        }

    }
}
