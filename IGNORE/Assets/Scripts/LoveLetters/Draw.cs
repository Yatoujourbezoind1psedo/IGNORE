using System;
using UnityEngine;
using UnityEngine.InputSystem;

//https://pastebin.com/1X0wgrqw

public class Draw : MonoBehaviour
{
    public Camera cam; 

    //Canvas dimensions
    public int totalXPixels = 1024; 
    public int totalYPixels = 512;

    //brush properties
    public int brushSize = 4; 
    public Color brushColor; 

    //Wether the drawing system will use interpolation to make smoother lines (will affect the performance)
    public bool useInterpolation = true; 

    //references to the points on our drawable faces
    public Transform topLeftCorner, bottomRightCorner, point;

    //refernce to the material which will use this texture
    public Material material; 

    //la texture générée
    public Texture2D generatedTexture; 

    //L'array qui contient la couleur des pixels 
    Color[] colorMap; 

    //Coordonnées actuelles du curseur dans cette frame
    private int xPixel, yPixel = 0; 

    //Variables hold constants which are precalculated in order to save performance 
    private float xMult, yMult; 

    //variables pour interpolation
    private bool pressedLastFrame = false; //This bool remembers wether we click over the drawable area in the last frame
    private int lastX, lastY = 0; 

    //Ajout d'un maximum pour le pinceau
    [SerializeField] private int maxPinceau = 100; 
    private int valPinceau; 
    
    void Start()
    {
        colorMap = new Color[totalXPixels * totalYPixels]; //Initialisation color map
        generatedTexture = new Texture2D(totalYPixels, totalXPixels,TextureFormat.RGBA32, false);  //génère nouvelle texture avec hauteur et largeur
        generatedTexture.filterMode = FilterMode.Point;
        material.SetTexture("_BaseMap", generatedTexture); //Donne à matérial la nouvelle texture

        ResetColor(); //Reset la couleur du canvas à blanc

        xMult = totalXPixels / (bottomRightCorner.localPosition.x - topLeftCorner.localPosition.x); //Precalculating constants 
        yMult = totalYPixels / (bottomRightCorner.localPosition.y - topLeftCorner.localPosition.y);

        //Pinceau valeur
        valPinceau = maxPinceau; 
    }

    private void Update()
    {
        if (Mouse.current.leftButton.isPressed)
        {
            //DecreasePinceau(); 
            CalculatePixel();
        }
        else
        {
            pressedLastFrame = false; //Pas d'interpolation à la frame si pas pressé
            ReleaseButton(); 
        }
    }

    private void ChangePixelsAroundPoint()
    {
        if(useInterpolation && pressedLastFrame && (lastX != xPixel || lastY != yPixel))
        {
            int dist = (int)Mathf.Sqrt((xPixel - lastX) * (xPixel - lastX) + (yPixel - lastY) * (yPixel - lastY)); //Calcule la distance entre le pixel actuel et le pixel de denière frame
            for(int i = 1; i <= dist; i++) //loop through the points on the determined line
            {
                DrawBrush((i * xPixel + (dist - i) * lastX) / dist, (i * yPixel + (dist - i) * lastY) / dist); //Appelle fonction sur point déterminé
            }

            //Gestion du pinceau
            DecreasePinceau(); 
        }
        else //pas interpolation
        {
            DrawBrush(xPixel, yPixel); //utilise fonction sur dernier point
        }
        
        pressedLastFrame = true; 
        lastX = xPixel; 
        lastY = yPixel;
        SetTexture(); 
    }

    private void DecreasePinceau()
    {
        if(valPinceau > 0)
        {
            valPinceau -= 1;

        }
        else
        {
            ReleaseButton(); 
        }
        Debug.Log(valPinceau);
    }

    private void CalculatePixel()
    {
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue()); 
        RaycastHit hit; 
        if(Physics.Raycast(ray, out hit, 10f))
        {
            point.position = hit.point; 
            xPixel = (int)((point.localPosition.x - topLeftCorner.localPosition.x) * xMult); 
            yPixel = (int)((point.localPosition.y - topLeftCorner.localPosition.y) * yMult); 

            ChangePixelsAroundPoint();
        }
        else
        {
            pressedLastFrame = false; 
        }
    }

    private void DrawBrush(int xPix, int yPix)//prend un point sur le canva en paramètre et dessine un cercle avec un radius brushSize autour
    {
        int i = xPix - brushSize + 1, j = yPix - brushSize + 1, maxi = xPix + brushSize - 1, maxj = yPix + brushSize - 1; //Déclare limite des variables
        if(i < 0)
        {
            i = 0; 
        }
        if(j < 0)
        {
            j = 0; 
        }

        if(maxi >= totalXPixels) //if either upper boundary is more than the maximum amout of pixels, set it to be under
        {
            maxi = totalXPixels - 1; 
        }
        if (maxj >= totalYPixels)
        {
            maxj = totalYPixels - 1; 
        }

        for(int x=i; x <= maxi; x++) // Loop through all of the points on the square that frames the circle of radius brushSize
        {
            for(int y=j; y<=maxj; y++)
            {
                if((x - xPix) * (x - xPix) + (y - yPix) * (y-yPix) <= brushSize * brushSize) //using the circle's formula(x²+y² <= r²) we check if the current point is inside the circle
                {
                    colorMap[x * totalYPixels + y] = brushColor; 
                }
            }
        }
    }

    private void SetTexture() //apply texture
    {
        generatedTexture.SetPixels(colorMap); 
        generatedTexture.Apply(); 
    }

    private void ResetColor()
    {
        for(int i = 0; i < colorMap.Length; i++)
        {
            colorMap[i] = Color.white; 
        }
        SetTexture(); 
    }

    private void ReleaseButton()
    {
        valPinceau = maxPinceau;
        ResetColor(); 
    }
}
