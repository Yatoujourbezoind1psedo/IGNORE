using UnityEngine;
using UnityEngine.SceneManagement; 

//utilisé pour le loading screen
public class SceneSwitcher : MonoBehaviour
{
    [SerializeField] private string sceneToUnload; 
    [SerializeField] private string loaderScene; 
    [SerializeField] private string sceneToLoad; 

    void Start()
    {
        UnloadStartScene(); 
    }

    private void UnloadStartScene() //Décharge la scène de départ
    {
        SceneManager.sceneUnloaded += OnStartUnloaded; //abonnement à un événement = "Quand une scène sera déchargée appelle OnStartUnloaded" 
        SceneManager.UnloadSceneAsync(sceneToUnload); //Enlève la scène en arrière plan, sans freeze et jeu continue
    }

    private void OnStartUnloaded(Scene scene) //Déclenché automatiquement quand scène complètement supprimée 
    {
        SceneManager.sceneUnloaded -= OnStartUnloaded; //Désabonnement de la fonction pour éviter appel multiple 
        SceneManager.sceneLoaded += OnEndLoaded; //Abonnement à autre fonction quand scène sera chargée 
        SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive); //Chargement de la nouvelle scène en Additive parce que Loading screen actif et nouvelle scène se charge
    }

    private void OnEndLoaded(Scene scene, LoadSceneMode mode)//Se déclenche quand la nouvelle scène a fini de charger
    {
        SceneManager.sceneLoaded -= OnEndLoaded; //désabo
        UnloadLoader(); //Suppresion loading screen
    }

    private void UnloadLoader() //Supprimer la scène de Load
    {
        SceneManager.UnloadSceneAsync(loaderScene); 
    }

    /*
    Etat initial : Menu et LoadingScreen actifs 
    */
}
