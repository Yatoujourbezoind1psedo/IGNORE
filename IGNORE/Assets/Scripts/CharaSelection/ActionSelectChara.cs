using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

public class ActionSelectChara : MonoBehaviour
{
    [SerializeField] private GameObject fichesPerso, ficheDuPerso, erreur; 
    [SerializeField] private float delayRedirection = 3f;


    //Pour un perso séléctionné 
    public void SelectChara()
    {
        Debug.Log("Press : " + this.name); 
        fichesPerso.SetActive(true); 
        ficheDuPerso.SetActive(true); 
    }

    //Pour le perso spécial sélectionné 
    public void SelectCharaSpecial()
    {
        Debug.Log("ACTION SPECIALE"); 
        Application.Quit();
    }

    //action de fermeture de description perso
    public void Retour()
    {
        ficheDuPerso.SetActive(false); 
        fichesPerso.SetActive(false); 
    }

    public void ConfirmationPerso()
    {
        StartCoroutine(CoroutineConfirmationPerso()); 
    }

    private IEnumerator CoroutineConfirmationPerso()
    {
        Retour(); 
        erreur.SetActive(true); 
        yield return new WaitForSeconds(delayRedirection); 
        Redirection(); 
    }

    public void ConfirmationNourrice()
    {
        Retour(); 
        Redirection(); 
    }

    private void Redirection()
    {
        Debug.Log("PAF LA REDIRECTION"); 
    }
}
