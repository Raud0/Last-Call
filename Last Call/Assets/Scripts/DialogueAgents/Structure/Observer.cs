using System;

public abstract class Observer : InputStackModule
{
    public abstract void Receive(Speech speech);
    public abstract void Receive(Interaction interaction);
    
    public void Send(ThoughtFocus thoughtFocus)
    { myInput.Route(thoughtFocus); }
    public void Send(Affection affection)
    { myInput.Route(affection); }
    public void Send(ContextInput contextInput)
    { myInput.Route(contextInput); }
}