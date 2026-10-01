using System.Collections;
using UnityEngine;

public class RotatoFasterBanana : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 100f;
    [SerializeField] float tempsRotation = 3f; 
    float currentAngle = 0f;

    private bool tempsExpire = false; //Pour éviter qu'il calcule trop souvent timeSinceLevelLoad, c'est une bonne idée je pense 

    private bool mouvementFini = false; 

    //tentative avec que animations
    [SerializeField] Animator animatorCadreSlots, animatorCadreMots; 
    [SerializeField] private float tempsAnimationRotation = 3f; //DOIT Correspondre au temps de l'animation de l'apparition des mots

    void Start()
    {
        animatorCadreMots.SetBool("Aplanissement", true); 
        StartCoroutine(MouvementFini()); 
    }

    void Update()
    {
        
        /*
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
            animatorCadreSlots.SetBool("Apparition", true); 
            animatorCadreMots.SetBool("Aplanissement", true); 
            

            //Si le mouvement est pas considéré comme fini et si les deux animations sont terminés alors mouvement fini = true 
            if(mouvementFini == false && !(animatorCadreMots.GetCurrentAnimatorStateInfo(0).length > animatorCadreMots.GetCurrentAnimatorStateInfo(0).normalizedTime) && !(animatorCadreSlots.GetCurrentAnimatorStateInfo(0).length > animatorCadreSlots.GetCurrentAnimatorStateInfo(0).normalizedTime))
            {//animatorCadreMots.GetCurrentAnimatorStateInfo(0).length > animatorCadreMots.GetCurrentAnimatorStateInfo(0).normalizedTime = n'importe quel animation est en train d'être joué avec l'animator 
                Debug.Log("AAYYYAYE"); 
                mouvementFini = true; 
            }
        }


        */
    }

    private IEnumerator MouvementFini()
    {
        yield return new WaitForSeconds(tempsAnimationRotation);
        animatorCadreMots.enabled = false; //Pour que le joueur puisse bouger librement les mots 
        animatorCadreSlots.SetBool("Apparition", true); 
        mouvementFini = true; 
    }

    public bool GetMouvementFini()
    {
        return mouvementFini;
    }
}
