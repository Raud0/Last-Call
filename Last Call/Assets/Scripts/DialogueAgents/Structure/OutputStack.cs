using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutputStack : MonoBehaviour
{
    public AgentSystem myStack;
    
    public StateManager stateManager;
    public Decider decider;

    public void Initialize(AgentSystem stack)
    {
        this.myStack = stack;

        stateManager.Initialize(this);
        decider.Initialize(this);
    }
    
    public void Route(Affection affection)
    { stateManager.Receive(affection); }
    
    public void Route(Emotion emotion)
    { decider.Receive(emotion); }

    public void Route(ContextInput contextInput)
    { decider.Receive(contextInput); }

    public void Route(List<Thought> thoughts)
    { decider.Receive(thoughts); }
}
