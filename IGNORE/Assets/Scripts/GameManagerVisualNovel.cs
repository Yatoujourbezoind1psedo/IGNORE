using UnityEngine;
using UnityEngine.InputSystem; 

public class GameManagerVisualNovel : MonoBehaviour
{
    public StoryScene currentScene; 
    public BottomBarController bottomBarController; 
    public BackgroundController backgroundController;
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
            if (bottomBarController.IsCompleted())
            {
                if (bottomBarController.IsLastSentence())
                {
                    currentScene = currentScene.nextScene; 
                    bottomBarController.PlayScene(currentScene);
                    backgroundController.SwitchImage(currentScene.background); 
                }
                else
                {
                    bottomBarController.PlayNextSentence(); 
                }
                
            }
        }
    }
}
