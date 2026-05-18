using Unity.VisualScripting;
using UnityEngine;
using TMPro; 

public class Letter : MonoBehaviour
{
    public string letterValue; 
    private TextMeshPro visuel; 

    private void Awake()
    {
        visuel = GetComponentInChildren<TextMeshPro>();

        SetLetterValue(letterValue); 
    }

    public void SetLetterValue(string letter)
    {
        if (letter == "")
        {
            Debug.Log("LETTRE VIDE"); 
            return; 
        }else if (letter.Length != 1)
        {
            Debug.Log("PAS UNE SEULE LETTRE"); 
            return; 
        }
        
        letterValue = letter; 
        visuel.SetText(letterValue); 
    }

    public Vector2 GetPositionInCanvas(Transform canvasTransform, Vector3 topLeftLocal, Vector3 bottomRightLocal, int totalXPixels, int totalYPixels)
    {

        //Souci avec drawTransform 
        float xMult = (float)totalXPixels / (bottomRightLocal.x - topLeftLocal.x);
        float yMult = (float)totalYPixels / (bottomRightLocal.y - topLeftLocal.y);

        

        Vector3 localPos = canvasTransform.InverseTransformPoint(transform.position); // Convertit une position du monde unity en position locale par rapport à un objet
        
        float x = (int)((localPos.x - topLeftLocal.x) * xMult);
        float y = (int)((localPos.y - topLeftLocal.y) * yMult);

        //Debug.Log("Letter value" + new Vector2(x, y));

        return new Vector2(x, y);
    }
    
}
