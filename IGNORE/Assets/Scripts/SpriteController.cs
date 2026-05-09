using System.Collections; 
using UnityEngine;

public class SpriteController : MonoBehaviour
{
    private SpriteSwitcher switcher; 
    private Animator animator; 
    private RectTransform rect;

    private void Awake()
    {
        switcher = GetComponent<SpriteSwitcher>();
        animator = GetComponent<Animator>();
        rect = GetComponent<RectTransform>();
    }

    public void Setup(Sprite sprite)
    {
        switcher.SetImage(sprite); 
    }

    public void Show(Vector2 coords) //affiche perso aux coordonées avec fade 
    {
        animator.SetTrigger("Show"); 
        rect.localPosition = coords; 
    }

    public void Hide() //Disparition 
    {
        animator.SetTrigger("Hide"); 
    }

    public void Move(Vector2 coords, float speed)
    {
        StartCoroutine(MoveCoroutine(coords, speed));
    }

    private IEnumerator MoveCoroutine(Vector2 coords, float speed) //déplace progressivement 
    {
        while(Vector2.Distance(rect.localPosition, coords) > 0.1f) //Permet de mettre une petite marge d'erreur avec les floats
        {
            rect.localPosition = Vector2.MoveTowards(rect.localPosition, coords, Time.deltaTime * 1000f * speed); //movetowards déplace petit à petit
            yield return new WaitForSeconds(0.01f); 
        }
    }

    public void SwitchSprite(Sprite sprite)
    {
        if(switcher.GetImage() != sprite) //evite de changer sprite inutilement 
        {
            switcher.SwitchImage(sprite);  
        }
    }
}
