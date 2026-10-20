using UnityEngine;

public class RotationCube : MonoBehaviour
{
    private float rotationSpeed = 100f;

    private static bool spin = false; 
    private Vector3 rotationAxis; 

    void Start()
    {
        rotationAxis = Random.onUnitSphere; //IMPORTANT : génère un vecteur 1 dans n'importe quelle direction 
        rotationSpeed = Random.Range(50, 666); 

    }

    
    void Update()
    {
        if(spin){
            transform.rotation = Quaternion.AngleAxis(rotationSpeed * Time.deltaTime, rotationAxis) * transform.rotation; //aciton de tourner
        };
        
    }


    public void Rotato()
    {
        spin = true; 
    }
}
