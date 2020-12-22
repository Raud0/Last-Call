using UnityEngine;

public class Ticker : MonoBehaviour
{
    private double secondCounter;
    private static double secondUnit = 1.0;
    private AudioSource audioSource;

    private int seconds;
    private int minutes;
    
    public AudioClip ClockSound;

    private void Start()
    {
        secondCounter = 0.0;
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = ClockSound;
        audioSource.volume = 0.5f;
        
        seconds = 0;
        minutes = 30;
    }

    private void Update()
    {
        secondCounter -= Time.deltaTime;

        if (secondCounter < secondUnit)
        {
            secondCounter += secondUnit;
            Tick();
        }
    }

    private void Tick()
    {
        if (seconds == 0)
        {
            minutes--;
            seconds = 59;
        }
        else
        {
            seconds--;
        }
        
        Events.UpdateTime(minutes,seconds);

        audioSource.Play();
    }
}
