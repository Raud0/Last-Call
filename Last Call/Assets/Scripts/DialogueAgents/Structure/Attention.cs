using System.Collections.Generic;
using UnityEngine;

public abstract class Attention : InputStackModule
{
    public abstract void Receive(ThoughtFocus thoughtFocus);
    public abstract void Receive(Thought thought);
    
    public void Send(ThoughtRequest thoughtRequest)
    { myInput.Route(thoughtRequest); }
    public void Send(List<Thought> thoughts)
    { myInput.Route(thoughts); }
}