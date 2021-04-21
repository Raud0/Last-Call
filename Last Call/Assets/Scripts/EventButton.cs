using UnityEngine;

public class EventButton : MonoBehaviour
{
    public int eventNumber;
    public Manager manager;
    
    public void CallEvent()
    {
        manager.CallEvent(eventNumber);
    }
}
