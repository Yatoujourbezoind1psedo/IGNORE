using UnityEngine;
using UnityEngine.UI; 

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
}
