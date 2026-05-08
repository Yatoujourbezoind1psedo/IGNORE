using UnityEngine;
using TMPro;
using System.Collections; //pour IEnumerator 

public class BottomBarController : MonoBehaviour
{
    public TextMeshProUGUI barText, personNameText;
    
    private int sentenceIndex = -1;
    public StoryScene currentScene; 
    private State state = State.COMPLETED; 
    private Animator animator; //Va permettre de faire disparaitre en fade la boite pendant transition scène
    private bool isHidden = false; //Pour savoir si on cache ou pas le bottom
    
    private enum State //machine à état 
    {
        COMPLETED, PLAYING 
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Hide() //Cache la barre avec tout le texte
    {
        if (!isHidden)
        {
            animator.SetTrigger("Hide"); 
            isHidden = true; 
        }
   
    }

    public void Show() 
    {
        if (isHidden)
        {
            animator.SetTrigger("Show"); 
            isHidden = false; 
        }

    }

    public void ClearText()
    {
        barText.text = "";
        personNameText.text = ""; 
    }


    public void PlayScene(StoryScene scene)
    {
        currentScene = scene; 
        sentenceIndex = -1; 
        PlayNextSentence(); 
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void PlayNextSentence()
    {
        StartCoroutine(TypeText(currentScene.sentences[++sentenceIndex].text)); //récupère le texte de la première phrase de la scène
        personNameText.text = currentScene.sentences[sentenceIndex].speaker.SpeakerName; 
        personNameText.color = currentScene.sentences[sentenceIndex].speaker.TextColor; 

    }

    public bool IsCompleted()
    {
        return state == State.COMPLETED; 
    }

    public bool IsLastSentence()
    {
        return sentenceIndex + 1 == currentScene.sentences.Count;
    }

    private IEnumerator TypeText(string text)
    {
        barText.text = ""; //vide la barre
        state = State.PLAYING; //le texte est en train de s'afficher
        int caraIndex = 0; //En commençant du caractère 0 

        while (state != State.COMPLETED)
        {
            barText.text = text.Substring(0,caraIndex + 1); //affiche caractère par caractère (passe par substring pour que ce soit moins couteux)
            yield return new WaitForSeconds(0.05f);

            if (++caraIndex == text.Length) //Incrémente puis vérifie 
            {
                state = State.COMPLETED; 
                break; 
                
            }
        }
    }
}
