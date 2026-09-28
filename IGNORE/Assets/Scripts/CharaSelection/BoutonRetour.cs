using UnityEngine;

public class BoutonRetour : MonoBehaviour
{
    [SerializeField] private GameObject fichesPerso, ficheDuPerso; 
    

    void Start()
    {
        fichesPerso = transform.parent.parent.gameObject; 
        ficheDuPerso = transform.parent.gameObject; 
    }

    public void Retour()
    {
        ficheDuPerso.SetActive(false); 
        fichesPerso.SetActive(false); 
    }
}
