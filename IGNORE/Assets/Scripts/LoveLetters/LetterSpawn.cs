using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;
using System.Collections;

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

    private bool isGameFinished = false; //Pour la partie (A voir si ça rentre pas en conflit avec reset de nouvelles partie une fois mot trouvé)

    [SerializeField] private int nbRandomLetter = 10; 
    //Création d'une liste contenant toutes les lettres (en public car Draw.cs)
    public List<Letter> letters = new List<Letter>(); 

    //Gestion du spawn pour éviter que les lettres se superposent 
    [SerializeField] private float minDistanceBetweenLetters = 0.1f; 
    [SerializeField] private int maxSpawnAttempts = 100; //Nb d'essai pour trouver une place à la lettre, permet d'éviter que le jeu crash s'il trouve jamais d'emplacement

    [SerializeField] private float maxTempsDelayRespawn, delayPostAcceleration, delayRefreshNormal; //les maxTempsDelayRespawn et delayRefreshNormal doivent être égaux pour qu'on voie pas l'accélération
    private bool blocEndGame = false; //permet d'éviter que la coroutine end game s'active plusieurs fois 
    [SerializeField] private int nbAvantAcceleration; //nb de fois où le joueur peut jouer chill avant que le jeu parte en COUILLES 


    void Start()
    {
        InitialiseGame(); 
    }

    ///*TEST
    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            InitialiseGame(); 
            
        }
    }

    private IEnumerator EndGame()
    {
        yield return new WaitForSeconds(delayPostAcceleration);
        Debug.Log("FIN"); 

        //Arret du pinceau

        //Arret du spawn
        StopAllCoroutines(); //Ne fonctionne pas en ciblant juste acceleration pt parce que délai trop court 
    }

    private IEnumerator CoroutineRefreshMot()
    {
        yield return new WaitForSeconds(delayRefreshNormal); 
        InitialiseGame(); 
    }

    //S'appelle elle même pour accélérer vitesse petit à petit 
    private IEnumerator CoroutineAcceleration()
    {
        yield return new WaitForSeconds(maxTempsDelayRespawn);
        if (maxTempsDelayRespawn >= 1) //Possible de rajouter courbe d'accélération en mettant différents stades
        {
            maxTempsDelayRespawn -= 1; 
        }

        if (maxTempsDelayRespawn <= 0 && !blocEndGame)
        {
            blocEndGame = true; 
            maxTempsDelayRespawn = 0f; //A changer au besoin d'accélération plus rapide ou plus crescendo
            StartCoroutine(EndGame()); 
        }
        InitialiseGame(); 
        StartCoroutine(CoroutineAcceleration()); 
    }

    private void InitialiseGame()
    {
        //Le jeu peut reprendre 
        isGameFinished = false;  
        //Reset des lettres pour création d'un nouveau game
        letters.Clear(); 
        foreach(Transform letter in lettersSpawn.GetComponentInChildren<Transform>())
        {
            Destroy(letter.gameObject); 
        }

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

    private void SpawnLetter(string letter) //Fait apparaitre une lettre sur le board
    {
        Vector3 randomPos = Vector3.zero; 
        bool validPosition = false; 
        
        //Esssaye de trouver un endroit éloigné des autres lettres 
        for(int attempt = 0; attempt < maxSpawnAttempts; attempt++)
        {
            randomPos = new Vector3(Random.Range(-0.45f, 0.45f), Random.Range(-0.45f, 0.45f), 0f); 

            validPosition = true; 

            foreach(Letter otherLetter in letters)
            {
                float dist = Vector3.Distance(randomPos, otherLetter.transform.localPosition);

                if(dist < minDistanceBetweenLetters)
                {
                    validPosition = false; 
                    break; 
                }
            }

            if (validPosition)
            {
                break; 
            }
        }

        if (!validPosition)
        {
            Debug.Log("Pas de place pour spawn lettre"); 
            return; 
        }

        GameObject clone = Instantiate(letterPrefab, lettersSpawn);

        //Definition de la position locale à donner
        clone.transform.localPosition = randomPos;   
        
        //Récupération du script 
        Letter letterScript = clone.GetComponent<Letter>();

        //Pour lui donner la lettre en para 
        letterScript.SetLetterValue(letter); 

        AddLetter(letterScript) ;
    }

    private void SpawnRandomLetter() //Fait apparaitre une lettre random sur le board
    {
        Vector3 randomPos = Vector3.zero; 
        bool validPosition = false; 
        
        for(int attempt = 0; attempt < maxSpawnAttempts; attempt++)
        {
            randomPos = new Vector3(Random.Range(-0.45f, 0.45f), Random.Range(-0.45f, 0.45f), 0f); 

            validPosition = true; 

            foreach(Letter otherLetter in letters)
            {
                float dist = Vector3.Distance(randomPos, otherLetter.transform.localPosition);

                if(dist < minDistanceBetweenLetters)
                {
                    validPosition = false; 
                    break; 
                }
            }

            if (validPosition)
            {
                break; 
            }
        }

        if (!validPosition)
        {
            Debug.Log("Pas de place pour spawn lettre"); 
            return; 
        }

        GameObject clone = Instantiate(letterPrefab, lettersSpawn); //Instanciation du prefab dans lettersSpawn

        //Definition de la position locale à donner
        clone.transform.localPosition = randomPos;  //0.45 parce que la taille du cube = 1 divisé par 2 (et 0.05 de marge)
        
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

            
            if(nbAvantAcceleration > 0) //Dans ce cas on continue normal
            {
                nbAvantAcceleration --; 
                StartCoroutine(CoroutineRefreshMot());
            }
            
            else //Autrement lancement corout récursive accélérant
            {
                StartCoroutine(CoroutineAcceleration()); 
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
