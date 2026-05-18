using UnityEngine;
using UnityEngine.InputSystem;

public class LetterSpawn : MonoBehaviour
{
    [SerializeField] private GameObject letterPrefab; 

    [SerializeField] private Draw drawScript; 
    

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {

            GameObject clone = Instantiate(letterPrefab, letterPrefab.transform.position, letterPrefab.transform.rotation); 
            Letter letterScript = clone.GetComponent<Letter>();
            letterScript.SetLetterValue("P"); 


            //Ajout du script letter dans draw 
            drawScript.AddLetter(letterScript) ; 
        }
    }
}
