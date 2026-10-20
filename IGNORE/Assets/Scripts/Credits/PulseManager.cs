using System.Collections;
using System; 
using UnityEngine;

public class PulseManager : MonoBehaviour
{
    public float bpm; 
    [SerializeField] private PulseObject[] pulseObjects; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(BebeFautQueCaBounce()); 
    }
    
    IEnumerator BebeFautQueCaBounce()
    {
        foreach(PulseObject pulse in pulseObjects)
        {
            pulse.Pulse(); 
        }

        yield return new WaitForSeconds(bpm); 
        StartCoroutine(BebeFautQueCaBounce()); 
    }
}
