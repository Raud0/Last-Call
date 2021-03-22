using System.Collections.Generic;

public abstract class StateManagerImp : StateManager
{
    private Dictionary<Emotion.Type, float> state = new Dictionary<Emotion.Type, float>();

    public abstract void ImpReceive(Attack attack); 
    
    private void ChangeState(Emotion.Type type, float change)
    {
        state[type] += change;
        Emotion emotion = new Emotion(type, state[type]);
        
        Send(emotion);
    }
    
    public override void Receive(Attack attack)
    {
        ImpReceive(attack);
        switch (attack.MyType)
        {
            
        }
    }
}