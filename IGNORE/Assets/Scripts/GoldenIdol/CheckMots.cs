using System.Collections.Generic;
using UnityEngine;

public class CheckMots : MonoBehaviour
{
    public List<ItemSlot> itemSlots; 
    private int nbError; 

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
            Debug.Log("PAS TOUS LES ELEMENTS");
        }
        else if (nbError > 0)
        {
            Debug.Log("NB error : " + nbError);
        }
        else
        {
            Debug.Log("VICTOIRE");
        }
    }
}
