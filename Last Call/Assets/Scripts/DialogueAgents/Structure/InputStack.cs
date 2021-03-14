using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputStack : MonoBehaviour
{
    public AgentSystem myStack;
    
    public Attention attention;
    public Thoughts thoughts;
    public Observer observer;

    public void Initialize(AgentSystem stack)
    {
        this.myStack = myStack;

        attention.Initialize(this);
        thoughts.Initialize(this);
        observer.Initialize(this);
    }

    public void Route(ThoughtFocus thoughtFocus)
    { attention.Receive(thoughtFocus); }

    public void Route(ContextInput contextInput)
    { myStack.outputStack.Route(contextInput);}
    public void Route(Affection affection)
    { myStack.outputStack.Route(affection); }
    public void Route(ThoughtRequest thoughtRequest)
    { thoughts.Receive(thoughtRequest); }
    public void Route(Thought thought)
    { attention.Receive(thought); }
    public void Route(List<Thought> thoughts)
    { myStack.outputStack.Route(thoughts); }
}
