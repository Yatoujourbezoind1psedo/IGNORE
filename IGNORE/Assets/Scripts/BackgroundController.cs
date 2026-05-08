using UnityEngine;
using UnityEngine.UI; 

public class BackgroundController : MonoBehaviour
{
    public bool isSwitched = false; 
    public Image background1, background2; 
    public Animator animator; 

    public void SwitchImage(Sprite sprite) //pour avoir une transition douce 
    {
        if (isSwitched)
        {
            background2.sprite = sprite; 
            animator.SetTrigger("SwitchFirst");
        }
        else
        {
            background1.sprite = sprite; 
            animator.SetTrigger("SwitchSecond");
        }
        isSwitched = !isSwitched;
    }
    
    public void SetImage(Sprite sprite) //pour changer abruptement au début 
    {
        if (isSwitched)
        {
            background2.sprite = sprite; 
        }
        else
        {
            background1.sprite = sprite; 
        }
    }
}
