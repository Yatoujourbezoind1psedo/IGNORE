using System.Collections; 
using UnityEngine;

public class SpriteController : MonoBehaviour
{
    private SpriteSwitcher switcher; 
    private Animator animator; 
    private RectTransform rect;
    private CanvasGroup canvasGroup; // quand animation skip change opacité du canvas group 

    private void Awake()
    {
        switcher = GetComponent<SpriteSwitcher>();
        animator = GetComponent<Animator>();
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Setup(Sprite sprite)
    {
        switcher.SetImage(sprite); 
    }

    public void Show(Vector2 coords, bool isAnimated = true) //affiche perso aux coordonées avec fade 
    {
        if (isAnimated)
        {
            animator.enabled = true; 
            animator.SetTrigger("Show");
        }
        else
        {
            animator.enabled = false; //Doit être passé à faux pour change le canvas group
            canvasGroup.alpha = 1f; 
        }
        
        rect.localPosition = coords; 
    }

    public void Hide(bool isAnimated = true) //Disparition 
    {
        if (isAnimated)
        {
            animator.enabled = true; 
            switcher.SyncImages(); 
            animator.SetTrigger("Hide");
        }
        else
        {
            animator.enabled = false;
            canvasGroup.alpha = 0; 
        }
    }

    public void Move(Vector2 coords, float speed, bool isAnimated = true)
    {
        if (isAnimated)
        {
            StartCoroutine(MoveCoroutine(coords, speed));
        }
        else
        {
            rect.localPosition = coords;
        }
        
    }

    private IEnumerator MoveCoroutine(Vector2 coords, float speed) //déplace progressivement 
    {
        while(Vector2.Distance(rect.localPosition, coords) > 0.1f) //Permet de mettre une petite marge d'erreur avec les floats
        {
            rect.localPosition = Vector2.MoveTowards(rect.localPosition, coords, Time.deltaTime * 1000f * speed); //movetowards déplace petit à petit
            yield return new WaitForSeconds(0.01f); 
        }
    }

    public void SwitchSprite(Sprite sprite, bool isAnimated = true)
    {
        if(switcher.GetImage() != sprite) //evite de changer sprite inutilement 
        {
            if (isAnimated)
            {
                switcher.SwitchImage(sprite); 
            }
            else
            {
                switcher.SetImage(sprite);
            }
             
        }
    }
}
