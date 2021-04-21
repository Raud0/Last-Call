using UnityEngine;

public class Ticker : MonoBehaviour
{
    private double secondCounter;
    private static double secondUnit = 1.0;
    public AudioSource audioSource;
    public Manager manager;

    private int seconds;
    private int minutes;

    private int initialMinutes = 15;
    private int initialSeconds = 0;

    private int tick = 1;

    public int GetSeconds()
    {
        return initialSeconds + initialMinutes * 60 - (seconds + minutes * 60);
    }

    public int GetSecondsLeft()
    {
        return seconds + minutes * 60;
    }
    
    private void Start()
    {
        secondCounter = secondUnit;
        
        seconds = initialSeconds;
        minutes = initialMinutes;
    }

    private void Update()
    {
        secondCounter -= Time.deltaTime;

        if (secondCounter < secondUnit)
        {
            secondCounter += secondUnit;
            Tick();
        }

        if (Input.GetKey(KeyCode.G))
        {
            if (Input.GetKeyDown(KeyCode.A)) tick--;
            if (Input.GetKeyDown(KeyCode.S)) tick++;
        }
    }

    private void Tick()
    {
        if (seconds <= 0)
        {
            if (minutes <= 0)
            {
                if (manager != null) manager.CallEvent(8);
                audioSource.volume = 0f;
            }
            else
            {
                minutes--;
            }
            seconds = 59 + seconds;
        }
        else
        {
            seconds -= tick;
        }
        
        Events.UpdateTime(minutes,seconds);

        audioSource.Play();
    }
}
