public abstract class StateManager : OutputStackModule
{
    public abstract void Receive(Attack attack);

    public void Send(Emotion emotion)
    { myOutput.Route(emotion); }
}