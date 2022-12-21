using UnityEngine;

public class MainMenuSound : MonoBehaviour
{
    //Sound
    public AudioSource menuAudioSource;
    public AudioClip buttonClickSound;

    public void ClickSound()
    {
        menuAudioSource.Stop();
        menuAudioSource.clip = buttonClickSound;
        menuAudioSource.Play();
    }
}
