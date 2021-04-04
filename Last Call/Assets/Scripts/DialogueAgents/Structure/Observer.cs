using System;

public abstract class Observer : InputStackModule
{
    public abstract void Receive(Speech speech);
    public abstract void Receive(Interaction interaction);
    
    public void Send(ThoughtFocus thoughtFocus)
    { myInput.Route(thoughtFocus); }
    public void Send(SocialInput socialInput)
    { myInput.Route(socialInput); }
    public void Send(Argument argument)
    { myInput.Route(argument); }
    public void Send(ActingInput actingInput)
    { myInput.Route(actingInput); }
}