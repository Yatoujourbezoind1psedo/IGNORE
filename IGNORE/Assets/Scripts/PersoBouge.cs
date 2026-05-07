using UnityEngine;
using UnityEngine.InputSystem; 

public class PersoBouge : MonoBehaviour
{
    public float amplitude = 0.1f;   // hauteur du mouvement
    public float speed = 20f;      // vitesse

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (Keyboard.current.spaceKey.isPressed)
        {
            float y = Mathf.Sin(Time.time * speed) * amplitude;
            transform.position = startPos + new Vector3(0, y, 0);
        }

    }
}
