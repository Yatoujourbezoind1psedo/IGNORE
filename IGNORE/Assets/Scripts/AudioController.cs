using System.Collections;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public AudioSource musicSource; 
    public AudioSource soundSource; 

    public void PlayAudio(AudioClip music, AudioClip sound)
    {
        if (sound != null)
        {
            soundSource.clip = sound;
            soundSource.Play(); 
        }

        if(music != null && musicSource.clip != music)
        {
            StartCoroutine(SwitchMusic(music)); 
        }
    }

    private IEnumerator SwitchMusic(AudioClip music)
    {
        if(musicSource.clip != null) //Si déjà  musique
        {
            while(musicSource.volume > 0) //tant que le son est pas coupé on le diminue petit à petit
            {
                musicSource.volume -= 0.05f; 
                yield return new WaitForSeconds(0.05f);
            }
        }
        else 
        {
            musicSource.volume = 0;
        }
        musicSource.clip = music;
        musicSource.Play(); 

        while(musicSource.volume < 0.5) //puis on le remet jusqu'à la moitié du son
        {
            musicSource.volume += 0.05f; 
            yield return new WaitForSeconds(0.05f);
        }
    }
}
