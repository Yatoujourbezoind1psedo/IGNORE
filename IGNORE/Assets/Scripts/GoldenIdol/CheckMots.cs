using System.Collections.Generic;
using UnityEngine;

public class CheckMots : MonoBehaviour
{
    public List<ItemSlot> itemSlots; 

    //Fonction qui check si tous les emplacements retournent vrai
    public void CheckSlots()
    {
        bool tousSlotsCheck = true; 

        foreach (ItemSlot itemSlot in itemSlots)
        {
            tousSlotsCheck = tousSlotsCheck && itemSlot.MotTrouve(); //NE FONCTIONNE QU'UNE FOIS A CORRIGER
        }

        Debug.Log(tousSlotsCheck); 
    }
}
