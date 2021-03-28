using System.Collections.Generic;

public abstract class Decider : OutputStackModule
{
    public abstract void Receive(Emotion emotion);
    public abstract void Receive(SocialInput socialInput);
    public abstract void Receive(ActingInput actingInput);
    public abstract void Receive(List<RankedThought> thoughts);

    public void Send(Speech speech)
    { myOutput.Speak(speech); }
    public void Send(Interaction interaction)
    { myOutput.Interact(interaction);}
}