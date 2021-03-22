using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutputStack : MonoBehaviour
{
    public ActorStack myStack;
    
    public StateManager stateManager;
    public Decider decider;

    public void Initialize(ActorStack stack)
    {
        this.myStack = stack;

        stateManager.Initialize(this);
        decider.Initialize(this);
    }
    
    public void DeInitialize()
    {

    }
    
    public void Route(Attack attack)
    { stateManager.Receive(attack); }
    
    public void Route(Emotion emotion)
    { decider.Receive(emotion); }

    public void Route(ContextInput contextInput)
    { decider.Receive(contextInput); }

    public void Route(List<RankedThought> thoughts)
    { decider.Receive(thoughts); }

    public void Speak(Speech speech)
    { myStack.conversation.Speak(speech); }

    public void Interact(Interaction interaction)
    { myStack.conversation.Interact(interaction); }
    
    public bool IsMe(string name)
    {
        return myStack.IsMe(name);
    }

    public HashSet<string> KnownActors()
    {
        return myStack.KnownActors;
    }
}
