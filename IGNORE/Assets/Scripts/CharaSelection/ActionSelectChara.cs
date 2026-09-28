using UnityEngine;
using UnityEngine.UI;

public class ActionSelectChara : MonoBehaviour
{
    [SerializeField] private GameObject fichesPerso, ficheDuPerso; 

    public void ActionButton()
    {
        Debug.Log("Press : " + this.name); 
        fichesPerso.SetActive(true); 
        ficheDuPerso.SetActive(true); 
    }

    public void ActionSpecial()
    {
        Debug.Log("ACTION SPECIALE"); 
    }
}
