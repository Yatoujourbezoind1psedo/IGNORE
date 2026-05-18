using UnityEngine;
using UnityEngine.InputSystem;

public class LetterSpawn : MonoBehaviour
{
    [SerializeField] private GameObject letterPrefab; 

    [SerializeField] private Draw drawScript; 
    [SerializeField] private Transform lettersSpawn; //Endroit où les lettres seront rangées 

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {

            GameObject clone = Instantiate(letterPrefab, lettersSpawn); //Instanciation du prefab dans lettersSpawn

            //Definition de la position locale à donner
            clone.transform.localPosition = new Vector3(Random.Range(-0.45f, 0.45f), Random.Range(-0.45f, 0.45f), 0);  //0.45 parce que la taille du cube = 1 divisé par 2 (et 0.05 de marge)
            
            //Récupération du script 
            Letter letterScript = clone.GetComponent<Letter>();

            //Pour lui donner une lettre quelconque 
            string randomLetter = ((char)Random.Range(65, 91)).ToString();//65 = A et Z = 90 mais exclusion borne majorante (A CHANGER PROBABLEMENT POUR AVOIR LETTRES DE MOTS)
            letterScript.SetLetterValue(randomLetter); 

            //Spawn lettre en 0.45 et - 0.45 pour x et y 
            //Ajout du script letter dans draw 
            drawScript.AddLetter(letterScript) ; 
            //Debug.Log(letterScript.transform.localPosition); 
        }
    }
}
