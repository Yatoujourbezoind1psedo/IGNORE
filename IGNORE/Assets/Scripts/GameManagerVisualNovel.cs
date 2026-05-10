using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem; 

public class GameManagerVisualNovel : MonoBehaviour
{
    public GameScene currentScene; 
    public BottomBarController bottomBarController; 
    public BackgroundController backgroundController;

    public AudioController audioController;

    private State state = State.IDLE; 

    private enum State
    {
        IDLE, ANIMATE
    }
    void Start()
    {
        if(currentScene is StoryScene)
        {
            StoryScene storyScene = currentScene as StoryScene; 
            bottomBarController.PlayScene(storyScene); 
            backgroundController.SetImage(storyScene.background); 
            PlayAudio(storyScene.sentences[0]); 
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (state == State.IDLE && bottomBarController.IsCompleted())
            {
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
        }
    }

    private void PlayScene(GameScene scene)
    {
        StartCoroutine(SwitchScene(scene)); 
    }

    //SOUCI ICI AVEC GAMESCENE avec CHOOSESCENE Script 
    private IEnumerator SwitchScene(GameScene scene)
    {
        state = State.ANIMATE; 
        currentScene = scene; 
        bottomBarController.Hide();
        yield return new WaitForSeconds(1f);

        if(scene is StoryScene)
        {
            StoryScene storyScene = scene as StoryScene;
            backgroundController.SwitchImage(storyScene.background); 

            PlayAudio(storyScene.sentences[0]); //Va jouer juste l'audio présent sur la première sentence de la scène
    
            yield return new WaitForSeconds(1f); 
            bottomBarController.ClearText();
            bottomBarController.Show(); 
            yield return new WaitForSeconds(1f);  
            bottomBarController.PlayScene(storyScene); 
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
