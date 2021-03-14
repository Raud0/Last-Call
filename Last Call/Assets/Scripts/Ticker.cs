using System.Timers;
using UnityEngine;

public class Ticker : MonoBehaviour
{
    private double secondCounter;
    private static double secondUnit = 1.0;
    public AudioSource audioSource;

    private int seconds;
    private int minutes;

    private void Start()
    {
        secondCounter = secondUnit;
        
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
