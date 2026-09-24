using UnityEngine;

public class RotatoFasterBanana : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 100f;
    [SerializeField] float tempsRotation = 3f; 
    float currentAngle = 0f;

    private bool tempsExpire = false; //Pour éviter qu'il calcule trop souvent timeSinceLevelLoad, c'est une bonne idée je pense 

    private bool mouvementFini = false; 
    void Update()
    {
        if(!tempsExpire && Time.timeSinceLevelLoad > tempsRotation)
        {
            tempsExpire = true; 
        }
        if (!tempsExpire) //Pour le faire tourner juqu'à ce que le temps expire
        {
            currentAngle += rotationSpeed * Time.deltaTime;
        }
        else
        {
            //Ramène progressivement vers 0°
            currentAngle = Mathf.MoveTowardsAngle(currentAngle %360f, 0f, rotationSpeed * Time.deltaTime);

        }
        
        transform.rotation = Quaternion.Euler(0f, 0f, currentAngle);

        //Si le temps est écoulé et qu'on est revenu à 0°
        if(tempsExpire && Mathf.Approximately(Mathf.DeltaAngle(currentAngle, 0f), 0f)) //DeltaAngle donne plus petite différence angulaire entre angle actuel et 0°
        {
            //Calque rotation sur 0° pour éviter qu'il soit légèrement tourné
            currentAngle = 0f; 
            transform.rotation = Quaternion.identity; 
            mouvementFini = true; 
        }
        
    }

    public bool GetMouvementFini()
    {
        return mouvementFini;
    }
}
