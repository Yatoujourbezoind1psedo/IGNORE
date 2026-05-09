using System;
using System.Collections.Generic; //permet d'utiliser liste dynamiques List 
using UnityEngine;

[CreateAssetMenu(fileName = "NewStoryScene", menuName = "Data/New Story Scene")]
[System.Serializable]

//Page de visual novel, scène de dialogue ou morceau d'histoire 
public class StoryScene : ScriptableObject
{
    public List<Sentence> sentences; //List de phrases 
    public Sprite background; //Image de fond
    public StoryScene nextScene; //prochaine scène 

    [System.Serializable] //Nécessaire pour l'afficher dans l'inspecteur
    public struct Sentence //Donc une ligne de dialogue 
    {
        public string text; //Ce qui est dit 
        public Speaker speaker; //et qui le dit 

        public List<Action> actions;

        [System.Serializable]
        public struct Action //Les actions possibles pour un perso 
        {
            public Speaker speaker; 
            public int spriteIndex; //Permet de savoir quel sprite doit être affiché 
            public Type actionType; 
            public Vector2 coords; 
            public float moveSpeed; //utilisé que si le perso bouge 

            [System.Serializable]
            public enum Type
            {
                NONE, APPEAR, MOVE, DISAPPEAR
            }
        }
    }
}
