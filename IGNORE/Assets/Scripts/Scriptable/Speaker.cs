using UnityEngine;

//Créer dans Unity Assets > Create > Data > New Speaker pour nouveau speaker avec par défaut nom = NewSpeakeret chemin affiché dans menu Create = "Data/New Speaker" 
[CreateAssetMenu(fileName = "NewSpeaker", menuName = "Data/New Speaker")]
//Dit à unity que ça peut être sérializé (ici pas obligatoire parce qeu déjà ScriptableObject )
[System.Serializable]

//Stock infos sur personen qui parle 
/*
ScriptableObject = quoi ?

C’est un type spécial Unity qui sert à stocker des données dans des assets.

Contrairement à un MonoBehaviour :

il n’est pas attaché à un GameObject ;
il existe comme fichier .asset dans le projet.

Très utile pour :

dialogues,
stats de personnages,
objets,
configurations,
données réutilisables.

Avantages :

centralise les données ;
évite les duplications ;
modifiable sans toucher au code ;
léger en mémoire ;
réutilisable partout.
*/
public class Speaker : ScriptableObject
{
    [SerializeField] private string speakerName; 
    [SerializeField] private Color textColor; 

    public string SpeakerName => speakerName; // Les getters récupèrent les valeurs données pour pouvoir les afficher autre part
    public Color TextColor => textColor;
}
