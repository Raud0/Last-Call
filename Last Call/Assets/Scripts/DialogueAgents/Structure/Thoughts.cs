public abstract class Thoughts : InputStackModule
{
    public abstract void Receive(ThoughtRequest thoughtRequest);

    public void Send(Thought thought)
    { myInput.Route(thought); }
}