using System.Collections.Generic;
using UnityEngine;
using TMPro; 


public class CheckMots : MonoBehaviour
{
    public List<ItemSlot> itemSlots; 
    private int nbError; 

    [SerializeField] private TextMeshProUGUI textAffichage;

    private void Start()
    {
        CheckSlots(); //Ne sert qu'à remplir avec texte disant que pas tous les éléments 
    }

    //Fonction qui check si tous les emplacements retournent vrai
    public void CheckSlots()
    {
        nbError = 0; 
        bool slotsContientsNull = false; 

        foreach (ItemSlot itemSlot in itemSlots)
        {
            if (itemSlot.MotTrouve() == false){
                nbError++; 
            }
            if (itemSlot.MotTrouve() == null){
                slotsContientsNull = true;
                break; 
            }
        }
        

        if(slotsContientsNull)
        {
            //Debug.Log("PAS TOUS LES ELEMENTS");
            textAffichage.text = "PAS TOUS LES ELEMENTS"; 
        }
        else if (nbError > 0)
        {
            //Debug.Log("NB error : " + nbError);
            textAffichage.text = "NB error : " + nbError; 
        }
        else
        {
            //Debug.Log("VICTOIRE");
            textAffichage.text = "VICTOIRE"; 
        }
    }
}
