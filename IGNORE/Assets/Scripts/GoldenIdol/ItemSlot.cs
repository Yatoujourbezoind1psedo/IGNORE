using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private string nomAttendu;
    private bool motTrouve = false; 
    public void OnDrop(PointerEventData eventData)
    {
        //Debug.Log("OnDrop"); 
        //eventData.pointerDrag = objet qui est drop (objet qui reçoit OnDrag)

        
        if(eventData.pointerDrag != null)
        {
            //Pour le mettre au centre de l'item slot
            eventData.pointerDrag.GetComponent<RectTransform>().position = GetComponent<RectTransform>().position;

            Transform child = eventData.pointerDrag.transform.GetChild(0); //Récupère le premier enfant qui est le texte

            //Debug.Log(child.GetComponent<TextMeshProUGUI>().text);

            if(child.GetComponent<TextMeshProUGUI>().text.ToLower() == nomAttendu.ToLower()) //si le texte présent sur l'objet est le même que celui attendu
            {
                Debug.Log(child.GetComponent<TextMeshProUGUI>().text); 
                motTrouve = true;
            }
            else //DOIT RETOURNER FALSE SI RIEN N'EST DANS LE SLOT AUSSI 
            {
                motTrouve = false;
            }     
        
        }
    }   

    public bool MotTrouve()
    {
        return motTrouve;
    }
}
