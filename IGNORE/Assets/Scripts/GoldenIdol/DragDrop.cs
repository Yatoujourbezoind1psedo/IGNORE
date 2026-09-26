using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

//https://www.youtube.com/watch?v=BGr-7GZJNXg
public class DragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    [SerializeField] private Canvas canvas; 
    private RectTransform rectTransform; 
    private CanvasGroup canvasGroup; 

    [SerializeField] private float tempsFade, delayFade; 

    private RotatoFasterBanana rotatoFasterBanana; 

    public ItemSlot originalSlot; 

    [SerializeField] private Transform motsLayer;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        //Recup du script de rotation pour avoir la durée
        rotatoFasterBanana = Object.FindFirstObjectByType<RotatoFasterBanana>();

        //Les rends transparents au début
        this.GetComponent<CanvasGroup>().alpha = 0f; 
        //Puis réaffiche
        StartCoroutine(FadeFromTransparent(tempsFade, delayFade)); 

        //REcup du motsLayer pour mettre mots dans le vide
        motsLayer = GameObject.Find("Mots").transform; 
    }

    //évite que les mots ne tournent sur eux mêmes 
    void LateUpdate ()
    {
        transform.rotation = Quaternion.identity;   
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (rotatoFasterBanana.GetMouvementFini())
        {
            //Debug.Log("On Begin Drag"); 
            canvasGroup.blocksRaycasts = false; //Comme ça le drop pourra s'activer sur l'item slot
            canvasGroup.alpha = .6f; 

            originalSlot = GetComponentInParent<ItemSlot>();
            if(originalSlot != null)
            {
                originalSlot.RemoveItem(gameObject); 
            }

            //Met en enfant du layer de mots, son parent de base
            transform.SetParent(motsLayer, true); 
        }
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (rotatoFasterBanana.GetMouvementFini())
        {
            //Debug.Log("On Drag"); 
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor; 
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (rotatoFasterBanana.GetMouvementFini())
        {
            //Debug.Log("On End Drag"); 
            canvasGroup.blocksRaycasts = true; 
            canvasGroup.alpha = 1f; 

            //Suppresion de l'item si l'objet est mis dans un autre
            ItemSlot newSlot = GetComponentInParent<ItemSlot>();

            //Debug.Log(newSlot.name); 
            /*
            if(originalSlot != null && originalSlot != newSlot) //PLUS NECESSAIRE : removeitem au begin drag 
            {
                //Debug.Log("Objet retiré de slot"); 
                //originalSlot.RemoveItem(gameObject); 
            }
            */
            if (newSlot == null)
            {
                Debug.Log("OBJET LACEH VIDE"); 
            }
            originalSlot = null; 
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("On Pointer Down"); 
    }

    public void OnDrop(PointerEventData eventData)
    {
        
    }

    public IEnumerator FadeFromTransparent(float duration, float delay)
    {
        yield return new WaitForSeconds(delay);
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            this.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(0f, 1f, t / duration);
            yield return null;
        }

    }
}
