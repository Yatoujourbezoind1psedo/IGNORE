using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem; 

public class GameManagerVisualNovel : MonoBehaviour
{
    public StoryScene currentScene; 
    public BottomBarController bottomBarController; 
    public BackgroundController backgroundController;
    private State state = State.IDLE; 

    private enum State
    {
        IDLE, ANIMATE
    }
    void Start()
    {
        bottomBarController.PlayScene(currentScene); 
        backgroundController.SetImage(currentScene.background); 
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
                    PlayScene(currentScene.nextScene); 
                }
                else
                {
                    bottomBarController.PlayNextSentence(); 
                }
                
            }
        }
    }

    private void PlayScene(StoryScene scene)
    {
        StartCoroutine(SwitchScene(scene)); 
    }

    private IEnumerator SwitchScene(StoryScene scene)
    {
        state = State.ANIMATE; 
        currentScene = scene; 
        bottomBarController.Hide();
        yield return new WaitForSeconds(1f);
        backgroundController.SwitchImage(scene.background); 
        yield return new WaitForSeconds(1f); 
        bottomBarController.ClearText();
        bottomBarController.Show(); 
        yield return new WaitForSeconds(1f);  
        bottomBarController.PlayScene(scene); 
        state = State.IDLE; 
    }
}
