using System.Collections.Generic;

public abstract class Attention : InputStackModule
{
    public abstract void Receive(ThoughtFocus thoughtFocus);
    public abstract void Receive(ThoughtResponse thought);
    public abstract void Load(HashSet<Topic> topics);

    public void Send(ThoughtRequest thoughtRequest)
    { myInput.Route(thoughtRequest); }
    public void Send(List<RankedThought> thoughts)
    { myInput.Route(thoughts); }
}