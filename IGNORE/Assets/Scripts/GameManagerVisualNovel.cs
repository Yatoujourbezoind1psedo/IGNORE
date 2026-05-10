using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; 

public class GameManagerVisualNovel : MonoBehaviour
{
    public GameScene currentScene; 
    public BottomBarController bottomBarController; 
    public SpriteSwitcher backgroundController; //ICI 

    public AudioController audioController;

    private State state = State.IDLE; 

    private List<StoryScene> history = new List<StoryScene>(); //permet de stock les scènes déjà vues pour les rewind 

    private enum State
    {
        IDLE, ANIMATE
    }
    void Start()
    {
        if(currentScene is StoryScene)
        {
            StoryScene storyScene = currentScene as StoryScene; 
            history.Add(storyScene);
            bottomBarController.PlayScene(storyScene); 
            backgroundController.SetImage(storyScene.background); 
            PlayAudio(storyScene.sentences[0]); 
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
                        history.RemoveAt(history.Count - 1); //on enlève la scène sur laquelle on est
                        StoryScene scene = history[history.Count - 1]; //On revien à la scène précédente
                        history.RemoveAt(history.Count - 1); 
                        PlayScene(scene, scene.sentences.Count - 2, false); 
                    }
                }
                else
                {
                    bottomBarController.GoBack(); 
                }
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
