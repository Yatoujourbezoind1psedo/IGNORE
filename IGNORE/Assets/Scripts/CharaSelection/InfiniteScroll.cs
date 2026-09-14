using UnityEngine;
using UnityEngine.UI; 
using System.Collections;

//https://youtu.be/DCndoQFN344?si=aM7H5GgXvFcpYiPm
public class InfiniteScroll : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect; 
    [SerializeField] private RectTransform viewPortTransform; 
    [SerializeField] private RectTransform contentPanelTransform; 
    [SerializeField] private HorizontalLayoutGroup HLG; 

    [SerializeField] private RectTransform[] ItemList; 
    
    private Vector2 OldVelocity; 
    private bool isUpdated; 

    //Secondes avant que perso n'apparaisse
    [SerializeField] private float tempsAttente = 5; 

    void Start()
    {
        isUpdated = false;
        OldVelocity = Vector2.zero;

        int ItemsToAdd = Mathf.CeilToInt(viewPortTransform.rect.width / (ItemList[0].rect.width + HLG.spacing));

        for(int i = 0; i < ItemsToAdd; i ++)
        {
            RectTransform RT = Instantiate(ItemList[i% ItemList.Length], contentPanelTransform);
            RT.SetAsLastSibling(); 
        }

        for(int i = 0; i < ItemsToAdd; i ++)
        {
            int num = ItemList.Length - i - 1; 
            while(num < 0)
            {
                num += ItemList.Length; 
            }
            RectTransform RT = Instantiate(ItemList[num], contentPanelTransform);
            RT.SetAsFirstSibling(); 

        }

        contentPanelTransform.localPosition  = new Vector3((0 - (ItemList[0].rect.width + HLG.spacing) * ItemsToAdd), contentPanelTransform.localPosition.y, contentPanelTransform.localPosition.z); 

        StartCoroutine(ApparitionIntruCoroutine()); 
    }

    // Update is called once per frame
    void Update()
    {
        if (isUpdated)
        {
            isUpdated = false; 
            scrollRect.velocity = OldVelocity; 
        }

        if(contentPanelTransform.localPosition.x > 0)
        {
            Canvas.ForceUpdateCanvases(); 
            OldVelocity = scrollRect.velocity; 
            contentPanelTransform.localPosition -= new Vector3(ItemList.Length * (ItemList[0].rect.width + HLG.spacing), 0, 0);
            isUpdated = true; 
        }

        if(contentPanelTransform.localPosition.x < 0 - (ItemList.Length * (ItemList[0].rect.width + HLG.spacing)))
        {
            Canvas.ForceUpdateCanvases();
            OldVelocity = scrollRect.velocity; 
            contentPanelTransform.localPosition += new Vector3(ItemList.Length * (ItemList[0].rect.width + HLG.spacing), 0, 0);
            isUpdated = true; 
        }

    }

    private IEnumerator ApparitionIntruCoroutine()
    {
        yield return new WaitForSeconds(tempsAttente);
        Debug.Log("Apparition perso"); 

        //On parcout la liste de tous les éléments pour les activer s'il ne le sont pas
        foreach(Transform child in contentPanelTransform.transform)
        {
            if(child.gameObject.activeSelf == false)
            {
                child.gameObject.SetActive(true); 
            }
        }


    }
}
