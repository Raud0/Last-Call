public abstract class StateManager : OutputStackModule
{
    public abstract void Receive(Affection affection);

    public void Send(Emotion emotion)
    { myOutput.Route(emotion); }
}