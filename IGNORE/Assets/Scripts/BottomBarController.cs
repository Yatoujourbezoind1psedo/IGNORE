using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic; //pour IEnumerator 

public class BottomBarController : MonoBehaviour
{
    public TextMeshProUGUI barText, personNameText;
    
    private int sentenceIndex = -1;
    public StoryScene currentScene; 
    private State state = State.COMPLETED; 
    private Animator animator; //Va permettre de faire disparaitre en fade la boite pendant transition scène
    private bool isHidden = false; //Pour savoir si on cache ou pas le bottom

    public Dictionary<Speaker, SpriteController> sprites; //pour éviter de créer constamment de nouveaux objets
    public GameObject spritesPrefab; 
    
    private enum State //machine à état 
    {
        COMPLETED, PLAYING 
    }

    private void Start()
    {
        sprites = new Dictionary<Speaker, SpriteController>();
        animator = GetComponent<Animator>();
    }

    public int GetSentenceIndex()
    {
        return sentenceIndex; 
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


    public void PlayScene(StoryScene scene) //Se joue quand la scène est finie et passe à la prochaine
    {
        currentScene = scene; 
        sentenceIndex = -1; 
        PlayNextSentence(); 
    }
    
    public void PlayNextSentence() //Dans une scene joue la sentence puis fait leurs actions
    {
        StartCoroutine(TypeText(currentScene.sentences[++sentenceIndex].text)); //récupère le texte de la première phrase de la scène
        personNameText.text = currentScene.sentences[sentenceIndex].speaker.SpeakerName; 
        personNameText.color = currentScene.sentences[sentenceIndex].speaker.TextColor; 
        ActSpeakers(); 

    }

    public bool IsCompleted() //Pour savoir si le typewriting est fini
    {
        return state == State.COMPLETED; 
    }

    public bool IsLastSentence() //Permet de changer si la dernière sentence de la scène
    {
        return sentenceIndex + 1 == currentScene.sentences.Count;
    }

    private IEnumerator TypeText(string text) //Tape le texte en effet machien à écrire
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

    private void ActSpeakers() //Joue toutes les actions d'une sentence 
    {
        List<StoryScene.Sentence.Action> actions = currentScene.sentences[sentenceIndex].actions; 
        for(int i = 0; i < actions.Count; i++)
        {
            ActSpeaker(actions[i]); 
        }
    }

    private void ActSpeaker(StoryScene.Sentence.Action action)
    {
        SpriteController controller = null;
        switch (action.actionType)
        {
            case StoryScene.Sentence.Action.Type.APPEAR: //doit forcément appear en début de scène 
                if (!sprites.ContainsKey(action.speaker)) //si le perso existe pas encore créé sprite + ajoute au dictionnaire 
                {
                    controller = Instantiate(action.speaker.prefab.gameObject, spritesPrefab.transform).GetComponent<SpriteController>();
                    sprites.Add(action.speaker, controller); 
                }
                else
                {
                    controller = sprites[action.speaker]; 
                }
                controller.Setup(action.speaker.sprites[action.spriteIndex]); //configure l'image en la faisant apparaitre brusquement
                controller.Show(action.coords); //Affiche à une position 
                return; 

            case StoryScene.Sentence.Action.Type.MOVE:
                if (sprites.ContainsKey(action.speaker))
                {
                    controller = sprites[action.speaker]; 
                    controller.Move(action.coords, action.moveSpeed); 
                }
                break;

            case StoryScene.Sentence.Action.Type.DISAPPEAR:
                if (sprites.ContainsKey(action.speaker))
                {
                    controller = sprites[action.speaker]; 
                    controller.Hide(); 
                }
                break;

            case StoryScene.Sentence.Action.Type.NONE: //jsute pour changer le sprite 
                if (sprites.ContainsKey(action.speaker))
                {
                    controller = sprites[action.speaker]; 
                }
                break;
        }

        if(controller != null)
        {
            controller.SwitchSprite(action.speaker.sprites[action.spriteIndex]); 
        }
    }
}
