using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem; 
using UnityEngine.SceneManagement; 

public class GameManagerVisualNovel : MonoBehaviour
{
    public GameScene currentScene; 
    public BottomBarController bottomBarController; 
    public SpriteSwitcher backgroundController; //ICI 

    public AudioController audioController;

    //POur la save
    public DataHolder data; 
    private State state = State.IDLE; 

    private List<StoryScene> history = new List<StoryScene>(); //permet de stock les scènes déjà vues pour les rewind 
    public string menuScene; 

    private enum State
    {
        IDLE, ANIMATE
    }
    void Start()
    {
        if (SaveManager.IsGameSaved()) //Si le jeu est sauvegardé 
        {
            SaveData data = SaveManager.LoadGame(); //Récupère la data
            data.prevScenes.ForEach(scene =>
            {
                history.Add(this.data.scenes[scene] as StoryScene);  //Ajoute chaque scène déjà visitéedans l'histoire
            }); 
            currentScene = history[history.Count - 1]; //La scène actuelle -1 parce que liste commence à zéro donc récupère derniere scène
            history.RemoveAt(history.Count - 1); //et on enlève la dernière pour éviter de faire doublon avec le reste du start (et le playscene qui enclenche history.Add)
            bottomBarController.SetSentenceIndex(data.sentence -1); //Va réafficher dans la boîte de texte la dernière sentence  / -1 car bottombarController.PlayScene joue NextSEntence et incrémente avant
        }
        if(currentScene is StoryScene)
        {
            StoryScene storyScene = currentScene as StoryScene; 
            history.Add(storyScene);
            bottomBarController.PlayScene(storyScene, bottomBarController.GetSentenceIndex()); 
            backgroundController.SetImage(storyScene.background); 
            PlayAudio(storyScene.sentences[bottomBarController.GetSentenceIndex()]); //joue l'audio à partir de l'index de la sentence
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(state == State.IDLE)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (bottomBarController.IsCompleted())
                {
                    bottomBarController.StopTyping(); //Si le texte a fini de taper on stop la coroutine et permet de spam pour skip
                    if (bottomBarController.IsLastSentence())
                    {
                        PlayScene((currentScene as StoryScene).nextScene); 
                    }
                    else
                    {
                        bottomBarController.PlayNextSentence(); 
                        PlayAudio((currentScene as StoryScene).sentences[bottomBarController.GetSentenceIndex()]); 
                    }
                    
                }
                else
                {
                    bottomBarController.SpeedUp(); //Permet d'accéléréer si c'est pas fini
                }
            }
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                if (bottomBarController.IsFirstSentence())//Si c'est la première phrase de la scène
                {
                    if(history.Count > 1)// Et si on est pas à la première scène 
                    {
                        bottomBarController.StopTyping(); 
                        bottomBarController.HideSprites(); 
                        history.RemoveAt(history.Count - 1); //on enlève la dernière scène 
                        StoryScene scene = history[history.Count - 1]; //On va à la dernière scène de la liste 
                        history.RemoveAt(history.Count - 1); //On enlève la scène sur laquelle on est puisqu'on l'a pas finie et dans PlayScene -> SwitchScene > history.Add = pour éviter doublons
                        PlayScene(scene, scene.sentences.Count - 2, false);  //Avec -1 on joue l'actuel du coup -2 = 1 en arrière 
                    }
                }
                else
                {
                    bottomBarController.GoBack(); 
                }
            }

            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                //liste des scènes déjà visitées 
                List<int> historyIndices = new List<int>(); 
                history.ForEach(scene =>
                {
                    historyIndices.Add(this.data.scenes.IndexOf(scene));
                }); 

                //Création de la donnée de sauvegarde 
                SaveData data = new SaveData
                {
                    sentence = bottomBarController.GetSentenceIndex(), prevScenes = historyIndices
                };
                
                SaveManager.SaveGame(data); 
                SceneManager.LoadScene(menuScene); //retour au menu
            }
        }
        
    }

    private void PlayScene(GameScene scene, int sentenceIndex = -1, bool isAnimated = true)
    {
        StartCoroutine(SwitchScene(scene, sentenceIndex, isAnimated)); 
    }

    
    private IEnumerator SwitchScene(GameScene scene, int sentenceIndex, bool isAnimated = true)
    {
        state = State.ANIMATE; 
        currentScene = scene;

        if (isAnimated)//Donc grosso modo si on revient en arrière 
        {
            bottomBarController.Hide();
            yield return new WaitForSeconds(1f);
        }


        if(scene is StoryScene)
        {
            StoryScene storyScene = scene as StoryScene;
            history.Add(storyScene); //ajout pour le rewind
            PlayAudio(storyScene.sentences[sentenceIndex + 1]); //Va jouer juste l'audio présent sur la première sentence de la scène

            if (isAnimated)
            {
                backgroundController.SwitchImage(storyScene.background); 
                yield return new WaitForSeconds(1f); 
                bottomBarController.ClearText();
                bottomBarController.Show(); 
                yield return new WaitForSeconds(1f);
            }
            else
            {
                backgroundController.SetImage(storyScene.background);
                bottomBarController.ClearText(); 
            }
            
            bottomBarController.PlayScene(storyScene, sentenceIndex, isAnimated); 
            state = State.IDLE; 
        }
        /*
        else if (scene is ChooseScene)
        {
            Debug.Log("ChooseScene"); //pas implémenté, sert si on veut faire des choix 
            
            state = State.CHOOSE; 
            chooseController.SetupChoose(scene as ChooseScene); 
            
        }
        */ 


    }

    private void PlayAudio(StoryScene.Sentence sentence)
    {
        audioController.PlayAudio(sentence.music, sentence.sound); 
    }
}
