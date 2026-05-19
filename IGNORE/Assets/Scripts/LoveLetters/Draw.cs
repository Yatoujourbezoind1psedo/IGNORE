using System.Security.Cryptography;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

//https://pastebin.com/1X0wgrqw
//https://www.youtube.com/watch?v=qOP83fot3c0

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
    [SerializeField] private float brushCostPerPixel = 0.2f;
    private float valPinceau; 

    //Tracé du pinceau action 
    List<Vector2> drawnPoints = new List<Vector2>(); //Va permettre de conserver une liste des points dessinés 
    [SerializeField] private float minPointDistance = 5f; //Pour éviter que les points se rajoutent sans cesse dans drawnPoints 

    //lettres 
    [SerializeField] private Transform canvasTransform; // nécessaire pour la fonction pour savoir où se trouve les lettres dans le canvas dans letters 
    private Vector3 topLeftLocal, bottomRightLocal; //Sert de conversion pour garder valeur dans le monde

    //Variable pour le spawn de lettre
    [SerializeField] private LetterSpawn letterSpawn; 

    void Start()
    {
        colorMap = new Color[totalXPixels * totalYPixels]; //Initialisation color map
        generatedTexture = new Texture2D(totalYPixels, totalXPixels,TextureFormat.RGBA32, false);  //génère nouvelle texture avec hauteur et largeur
        generatedTexture.filterMode = FilterMode.Point;
        material.SetTexture("_BaseMap", generatedTexture); //Donne à matérial la nouvelle texture

        ResetColor(); //Reset la couleur du canvas à blanc

        topLeftLocal = canvasTransform.InverseTransformPoint(topLeftCorner.position); 
        bottomRightLocal = canvasTransform.InverseTransformPoint(bottomRightCorner.position); 

        xMult = (float)totalXPixels / (bottomRightLocal.x - topLeftLocal.x); //Precalculating constants 
        yMult = (float)totalYPixels / (bottomRightLocal.y - topLeftLocal.y);

        //Pinceau valeur
        valPinceau = maxPinceau; 
    }

    private void Update()
    {
        if (Mouse.current.leftButton.isPressed)
        {
            CalculatePixel();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            pressedLastFrame = false; //Pas d'interpolation à la frame si pas pressé
            ResetPinceau(); 
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
            float distFloat = Vector2.Distance(new Vector2(lastX, lastY), new Vector2(xPixel, yPixel)); //Calcul de la distance (plus précis en float)
            DecreasePinceau(distFloat); 
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

    private void DecreasePinceau(float dist)
    {
        if(valPinceau > 0)
        {
            valPinceau -= dist * brushCostPerPixel; //Réduit par la distance * cout en pixel (permet que si joueur dessine lentement alors même taille que dessiné rapidement)

        }
        else //si le pinceau arrive à zéro ou en dessous alors on efface tout
        {
            ResetPinceau(); 
        }
    }

    private void CalculatePixel()
    {
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue()); 
        //RaycastHit hit; 
        if(Physics.Raycast(ray, out RaycastHit hit, 10f)) //(Physics.Raycast(ray, out hit, 10f))
        {/*
            point.position = hit.point; 
            xPixel = (int)((point.localPosition.x - topLeftCorner.localPosition.x) * xMult); 
            yPixel = (int)((point.localPosition.y - topLeftCorner.localPosition.y) * yMult); */
            
            Vector3 localHit = canvasTransform.InverseTransformPoint(hit.point);
            
            xPixel = (int)((localHit.x - topLeftLocal.x) * xMult); 
            yPixel = (int)((localHit.y - topLeftLocal.y) * yMult); 

            //Debug.Log(new Vector2(xPixel, yPixel)); 
            /*
            Vector2 uv = hit.textureCoord; 
            xPixel= (int)(uv.x * totalXPixels); 
            yPixel= (int)(uv.y * totalYPixels);*/

/*
            Vector3 hitLocal = canvasTransform.InverseTransformPoint(hit.point);

float u = (hitLocal.x - topLeftLocal.x) / (bottomRightLocal.x - topLeftLocal.x);
float v = (hitLocal.y - bottomRightLocal.y) / (topLeftLocal.y - bottomRightLocal.y);


            xPixel = (int)(u * totalXPixels);
yPixel = (int)(v * totalYPixels);*/
            
            //Ajout du point dans la liste qui conserve tracé
            Vector2 newPoint = new Vector2(xPixel, yPixel); 
            

            if(drawnPoints.Count == 0 || Vector2.Distance(drawnPoints[drawnPoints.Count - 1], newPoint) > minPointDistance) //Permet de mettre une distance minimum
            {
                drawnPoints.Add(new Vector2(xPixel, yPixel)); 
                //Debug.Log(drawnPoints[drawnPoints.Count-1]); 

                CheckSelfIntersection(); //on vérifie s'il y a une boucle
            }

            //Afficher les pixels 
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

    private void ResetPinceau()
    {
        valPinceau = maxPinceau;
        drawnPoints.Clear(); //Permet de dire que les points sont à refaire 
        ResetColor(); 
    }

    private bool Intersects(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2) //Segment A coupe segment B ? 
    {
        float d = (a2.x - a1.x) * (b2.y - b1.y) - (a2.y - a1.y) * (b2.x - b1.x); //Segment parallèle ? : oui d = 0 

        if(Mathf.Abs(d) < 0.0001f)
        {
            return false; 
        }

        float u = ((b1.x - a1.x) * (b2.y - b1.y) - (b1.y - a1.y) * (b2.x - b1.x)) / d; //  = 0 : début / = 1 : fin du segment 

        float v = ((b1.x - a1.x) * (a2.y - a1.y) - (b1.y - a1.y) * (a2.x - a1.x)) / d; 

        return (u >= 0 && u <= 1 && v >= 0 && v <= 1); //Vérifie qu'intersection bien sur segment
    }


    private void CheckSelfIntersection() //Vérifie si nouveau segment coupe un ancien segment 
    {
        if(drawnPoints.Count < 4) // Au moins deux segments anciens et 1 nouveau segment nécessaire donc A - B - C - D = 4 points
        {
            return; 
        }

        Vector2 newA = drawnPoints[drawnPoints.Count - 2]; // Récupère avant dernier point : newA -> newB
        Vector2 newB = drawnPoints[drawnPoints.Count - 1]; //récupère dernier point

        for (int i = 0; i < drawnPoints.Count - 10; i++) //Parcours les anciens segments / - 3 parce qu'enlève les deux derniers points qui sont les nouveaux à tester (si on teste P4 -> P5, alors forcément P3 -> P4 touche) / - 10 pour éviter que si on revient en arrière ce soit trop la merde
        {
            Vector2 oldA = drawnPoints[i]; 
            Vector2 oldB = drawnPoints[i + 1]; 

            if(Intersects(newA, newB, oldA, oldB)){//Si une intersection est trouvée 
                List<Vector2> loopPoints = new List<Vector2>(); 

                for (int j = i; j < drawnPoints.Count; j++)//Je rajoute tous les points à partir de l'intersection 
                {
                    loopPoints.Add(drawnPoints[j]); 
                }

                InsideTheCircle(loopPoints); 
            }
        }
    }

    bool IsPointInside(Vector2 p, List<Vector2> poly) // permet de savoir si un point est dans un polygone
    {
        bool inside = false;

        for (int i = 0, j = poly.Count - 1; i < poly.Count; j = i++) //Algo de PNPOLY de W.Randolph Franklin 
        {
            if (((poly[i].y > p.y) != (poly[j].y > p.y)) && 
            (p.x < (poly[j].x - poly[i].x) * (p.y - poly[i].y) / (poly[j].y - poly[i].y) + poly[i].x))
            {
                inside = !inside;
            }
        }

        return inside;
    }

    private void InsideTheCircle(List<Vector2> polygon)
    {
        List<Letter> lettersToRemove = new List<Letter>(); 
        
        foreach(Letter letter in letterSpawn.letters)
        {
            Vector2 screenPos = letter.GetPositionInCanvas(canvasTransform, topLeftLocal, bottomRightLocal, totalXPixels, totalYPixels); 
            //Debug.Log(screenPos); 

            if(IsPointInside(screenPos, polygon)) //Souci ici avec xPixel et yPixel qui est pas sur la même valeur et localPosition 
            // xPixel = (int)((point.localPosition.x - topLeftCorner.localPosition.x) * xMult); 
            {
                //Debug.Log("Lettre entourée : " + letter.letterValue);


                //ICI AJOUT DE L'ACTION QUAND UNE LETTRE EST AJOUTEE
                letterSpawn.CheckLetter(letter.letterValue); 
                
                lettersToRemove.Add(letter); //Travail terminé pour la lettre 

                
            }
        }

        //Détruit les lettres entourées 
        foreach(Letter letter in lettersToRemove)
        {
            letterSpawn.EraseLetter(letter); 
        }

        if(lettersToRemove.Count > 0) //Se déclenche qui  si au moins une lettre (à voir)
        {
            //On annule le pinceau pour éviter des problèmes 
            ResetPinceau(); 
        }
        
    }
}
