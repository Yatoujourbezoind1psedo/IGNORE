using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;

//https://www.youtube.com/watch?v=30FpzpFNY-E&t=1698s

//Gère le spawn des lettres mais est un gérant des lettres au final
public class LetterSpawn : MonoBehaviour
{
    [SerializeField] private GameObject letterPrefab; 

    [SerializeField] private Draw drawScript; 
    [SerializeField] private Transform lettersSpawn; //Endroit où les lettres seront rangées 

    [SerializeField] private TextAsset possibleWord; //mot à deviner
    [SerializeField] private GameObject wordContainer, letterContainer; //zone contenant chaque lettre du mot

    private int correctGuesses; 
    private string word; 

    private bool isGameFinished = false; 

    [SerializeField] private int nbRandomLetter = 10; 
    //Création d'une liste contenant toutes les lettres (en public car Draw.cs)
    public List<Letter> letters = new List<Letter>(); 

    void Start()
    {
        InitialiseGame(); 
    }

    /*TEST
    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            SpawnRandomLetter(); 
            
        }
    }*/

    private void InitialiseGame()
    {
        //reset data to original state
        correctGuesses = 0; 
        foreach(Transform child in wordContainer.GetComponentInChildren<Transform>())
        {
            Destroy(child.gameObject); 
        }

        //Generate new word 
        word = GenerateWord().ToUpper(); //POur avoir tout en MAJ
        foreach(char letter in word)
        {
            var temp = Instantiate(letterContainer, wordContainer.transform); 
            temp.GetComponentInChildren<TextMeshProUGUI>().text = letter.ToString().ToUpper();

            //Intialisation des lettres sur terrain qui sont dans mots
            SpawnLetter(letter.ToString().ToUpper()); 
        }

        for(int i = 0; i < nbRandomLetter; i++)//Pour faire apparaitre un nombre de lettre random
        {
            SpawnRandomLetter(); 
        }
    }

    private string GenerateWord()//Prend un mot aléatoire dans la liste qui lui est fourni 
    {
        string[] wordList = possibleWord.text.Split("\n"); 
        string line = wordList[Random.Range(0, wordList.Length - 1)]; //-1 pour enlever /\n 
        return line.Substring(0, line.Length - 1);
    }

    private void SpawnLetter(string letter)
    {
        GameObject clone = Instantiate(letterPrefab, lettersSpawn);

        //Definition de la position locale à donner
        clone.transform.localPosition = new Vector3(Random.Range(-0.45f, 0.45f), Random.Range(-0.45f, 0.45f), 0);  
        
        //Récupération du script 
        Letter letterScript = clone.GetComponent<Letter>();

        //Pour lui donner la lettre en para 
        letterScript.SetLetterValue(letter); 

        AddLetter(letterScript) ;
    }

    private void SpawnRandomLetter()
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
        AddLetter(letterScript) ; 
        //Debug.Log(letterScript.transform.localPosition); 
    }

    public void CheckLetter(string letter)//Cherche letter dans mot 
    {
        if (!isGameFinished)
        {
            for(int i = 0; i < word.Length; i++){ //Souci avec détection de la même lettre + avec lettres doubles, seul al première ezst affichée 
                if(letter == word[i].ToString() && wordContainer.GetComponentsInChildren<TextMeshProUGUI>()[i].color != Color.yellow){ //si la lettre est trouvée et qu'elle a pas déjà été rajoutée
                    Debug.Log(letter + " in " + word); 
                    correctGuesses ++; 

                    wordContainer.GetComponentsInChildren<TextMeshProUGUI>()[i].color = Color.yellow; //affiche la lettre 
                    CheckOutcome(); //Donc mot complètement fini
                    
                    return; //return empêche valdiation des lettres identiques  
                }
            }
        }

    }

    private void CheckOutcome()
    {
        if(correctGuesses == word.Length) //Si toute sles lettres sont trouvées 
        {
            isGameFinished = true; 
            for(int i = 0; i < word.Length; i++)
            {
                wordContainer.GetComponentsInChildren<TextMeshProUGUI>()[i].color = Color.red; //Met toutes les lettre en vert
            }
            
        }
    }

        public void AddLetter(Letter letter)
    {
        letters.Add(letter);
        
    }

    public void EraseLetter(Letter letter)
    {
        letters.Remove(letter); 
        Destroy(letter.gameObject, 0.2f); //Destruction avec un peu de délai 
    }
}
