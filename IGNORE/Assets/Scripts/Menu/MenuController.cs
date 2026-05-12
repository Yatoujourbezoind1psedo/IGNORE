using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement; 
using TMPro; 
using UnityEngine.Audio; 
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    private Animator animator; 
    [SerializeField] private string gameScene;

    //Textes
    public TextMeshProUGUI musicValue, soundsValue; 

    //Mixers
    public AudioMixer musicMixer, soundsMixer; 

    public Button loadButton; //POur le rendre actif s'il y a une sauvegarde 

    //variable global
    private int _window = 0; 
    private void Start()
    {
        animator = GetComponent<Animator>();
        loadButton.interactable = SaveManager.IsGameSaved(); 
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame && _window == 1) //window permet de pas afficher tout le temps 
        {
            HideOptions(); 
            _window = 0; 
        }
    }
    public void NewGame()
    {
        SaveManager.ClearSavedGame(); 
        Load(); 
    }

    public void Load()
    {
        SceneManager.LoadScene(gameScene, LoadSceneMode.Single); //La nouvelle scène va prendre la place de la précédente et pas de mémoire sup

    }

    public void Quit()
    {
        Application.Quit();
    }

    //POur les animations 
    public void ShowOptions()
    {
        animator.SetTrigger("ShowOptions"); 
        _window = 1; 
    }
    public void HideOptions()
    {
        animator.SetTrigger("HideOptions"); 
    }

    //Les textes
    public void OnMusicChanged(float value)
    {
        musicValue.SetText(value + "%"); 
        musicMixer.SetFloat("volume", -50 + value /2); 
    }

    public void OnSoundsChanged(float value)
    {
        soundsValue.SetText(value + "%"); 
        soundsMixer.SetFloat("volume", -50 + value /2); 
    }
}
