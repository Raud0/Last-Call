public abstract class StateManager : OutputStackModule
{
    public abstract void Receive(Argument argument);

    public void Send(Emotion emotion)
    { myOutput.Route(emotion); }
}