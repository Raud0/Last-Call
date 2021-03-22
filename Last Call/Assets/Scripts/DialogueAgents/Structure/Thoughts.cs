using System.Collections.Generic;

public abstract class Thoughts : InputStackModule
{
    public abstract void Receive(ThoughtRequest thoughtRequest);
    public abstract void Load(HashSet<Thought> thoughts);

    public void Send(ThoughtResponse thoughtResponse)
    { myInput.Route(thoughtResponse); }
}