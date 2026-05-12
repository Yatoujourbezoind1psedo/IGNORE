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

    //pour accélérer le texte 
    public Coroutine typingCoroutine; 
    private float speedFactor = 1f; 
    
    private enum State //machine à état 
    {
        COMPLETED, SPEEDED_UP, PLAYING 
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

    public void SetSentenceIndex(int sentenceIndex) //utilisé quand load du jeu
    {
        this.sentenceIndex = sentenceIndex;
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
        if (isHidden) //pt à retirer 
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


    public void PlayScene(StoryScene scene, int sentenceIndex = -1, bool isAnimated = true) //Se joue quand la scène est finie et passe à la prochaine
    {
        currentScene = scene; 
        this.sentenceIndex =sentenceIndex; 
        PlayNextSentence(isAnimated); 
    }
    
    public void PlayNextSentence(bool isAnimated = true) //Dans une scene joue la sentence puis fait leurs actions
    {
        sentenceIndex ++; 
        PlaySentence(isAnimated); 

    }

    public void GoBack()
    {
        sentenceIndex--; 
        StopTyping(); 
        HideSprites(); 
        PlaySentence(false); 
    }

    private void PlaySentence(bool isAnimated = true)
    {
        speedFactor = 1f;
        typingCoroutine = StartCoroutine(TypeText(currentScene.sentences[sentenceIndex].text)); //récupère le texte de la première phrase de la scène
        personNameText.text = currentScene.sentences[sentenceIndex].speaker.SpeakerName; 
        personNameText.color = currentScene.sentences[sentenceIndex].speaker.TextColor; 
        ActSpeakers(isAnimated); 
    }

    public bool IsCompleted() //Pour savoir si le typewriting est fini
    {
        return state == State.COMPLETED || state == State.SPEEDED_UP; 
    }

    public bool IsLastSentence() //Permet de changer si la dernière sentence de la scène
    {
        return sentenceIndex + 1 == currentScene.sentences.Count;
    }

    public bool IsFirstSentence()
    {
        return sentenceIndex == 0; 
    }

    public void SpeedUp()
    {
        state = State.SPEEDED_UP;
        speedFactor = 0.25f; //Donc quatre fois plus rapide 
    }

    public void StopTyping()
    {
        state = State.COMPLETED; 
        StopCoroutine(typingCoroutine); 
    }

    public void HideSprites()
    {
        while(spritesPrefab.transform.childCount > 0)
        {
            DestroyImmediate(spritesPrefab.transform.GetChild(0).gameObject); 
        }
        sprites.Clear(); 
    }

    private IEnumerator TypeText(string text) //Tape le texte en effet machien à écrire
    {
        barText.text = ""; //vide la barre
        state = State.PLAYING; //le texte est en train de s'afficher
        int caraIndex = 0; //En commençant du caractère 0 

        while (state != State.COMPLETED)
        {
            barText.text = text.Substring(0,caraIndex + 1); //affiche caractère par caractère (passe par substring pour que ce soit moins couteux)
            yield return new WaitForSeconds(speedFactor * 0.05f);

            if (++caraIndex == text.Length) //Incrémente puis vérifie 
            {
                state = State.COMPLETED; 
                break; 
                
            }
        }
    }

    private void ActSpeakers(bool isAnimated = true) //Joue toutes les actions d'une sentence 
    {
        List<StoryScene.Sentence.Action> actions = currentScene.sentences[sentenceIndex].actions; 
        for(int i = 0; i < actions.Count; i++)
        {
            ActSpeaker(actions[i], isAnimated); 
        }
    }

    private void ActSpeaker(StoryScene.Sentence.Action action, bool isAnimated = true)
    {
        SpriteController controller;
        if (!sprites.ContainsKey(action.speaker)) //si le perso existe pas encore créé sprite + ajoute au dictionnaire 
        {
            controller = Instantiate(action.speaker.prefab.gameObject, spritesPrefab.transform).GetComponent<SpriteController>();
            sprites.Add(action.speaker, controller); 
        }
        else
        {
            controller = sprites[action.speaker]; 
        }

        switch (action.actionType)
        {
            case StoryScene.Sentence.Action.Type.APPEAR: //doit forcément appear en début de scène 
                controller.Setup(action.speaker.sprites[action.spriteIndex]); //configure l'image en la faisant apparaitre brusquement
                controller.Show(action.coords, isAnimated); //Affiche à une position 
                return; 

            case StoryScene.Sentence.Action.Type.MOVE:
                controller.Move(action.coords, action.moveSpeed, isAnimated);
                break;

            case StoryScene.Sentence.Action.Type.DISAPPEAR:
                controller.Hide(isAnimated); 
                break;
        }

        controller.SwitchSprite(action.speaker.sprites[action.spriteIndex], isAnimated);
    }
}
