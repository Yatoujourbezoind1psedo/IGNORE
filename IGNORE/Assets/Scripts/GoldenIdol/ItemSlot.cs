using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private string nomAttendu;
    private bool? motTrouve = null; 

    public GameObject currentItem; //Va permettre de savoir quel objet est le current

    private CheckMots scriptCheckMots; 

    private void Start()
    {
        //Recup du script qui va s'activer à chauqe fois qu'un objet est mis dans un slot
        scriptCheckMots = Object.FindFirstObjectByType<CheckMots>();
    } 

    public void OnDrop(PointerEventData eventData)
    {
        //Debug.Log("OnDrop"); 
        //eventData.pointerDrag = objet qui est drop (objet qui reçoit OnDrag)

        GameObject item = eventData.pointerDrag; 

        
        if(item != null)
        {

            if(item.GetComponent<DragDrop>() != null && item.GetComponent<DragDrop>().originalSlot != null)
            {
                item.GetComponent<DragDrop>().originalSlot.currentItem = null; 
            }

            //Pour le mettre au centre de l'item slot
            item.GetComponent<RectTransform>().position = GetComponent<RectTransform>().position;

            Transform child = item.transform.GetChild(0); //Récupère le premier enfant qui est le texte

            //Debug.Log(child.GetComponent<TextMeshProUGUI>().text);

            if(child.GetComponent<TextMeshProUGUI>().text.ToLower() == nomAttendu.ToLower()) //si le texte présent sur l'objet est le même que celui attendu
            {
                //Debug.Log(child.GetComponent<TextMeshProUGUI>().text);
                motTrouve = true;
            }
            else if(child.GetComponent<TextMeshProUGUI>().text.ToLower() != nomAttendu.ToLower())
            {
                motTrouve = false; 
            }

            currentItem = item;
            item.transform.SetParent(transform); //Permet de dire à l'objet que son parent est le slot 
             
            //Check de tous les slots au dépot d'un item
            scriptCheckMots.CheckSlots(); 
        }  
    }   

    //Permet d'enlever l'item du slot et que current item soit au courant 
    public void RemoveItem (GameObject item)
    {
        if(currentItem == item)
        {
            currentItem = null; 
            motTrouve = null; //Si l'item est retiré le mottrouvé est null 
            //Debug.Log("Objet retiré du slot"); 

            //Reaffichage des réponses pour éviter confusion
            scriptCheckMots.CheckSlots(); 
        }
    }

    public bool? MotTrouve()
    {
        return motTrouve;
    }
}
