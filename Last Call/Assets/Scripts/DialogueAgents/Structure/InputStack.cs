using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputStack : MonoBehaviour
{
    public ActorStack myStack;
    
    public Attention attention;
    public Thoughts thoughts;
    public Observer observer;
    
    public void Initialize(ActorStack stack, HashSet<Topic> newTopics, HashSet<Thought> newThoughts)
    {
        myStack = stack;
        myStack.conversation.OnSpeech += Route;
        myStack.conversation.OnInteraction += Route;

        attention.Initialize(this);
        thoughts.Initialize(this);
        observer.Initialize(this);

        attention.Load(newTopics);
        thoughts.Load(newThoughts);
    }

    public void DeInitialize()
    {
        myStack.conversation.OnSpeech -= Route;
        myStack.conversation.OnInteraction -= Route;
    }

    public void Route(ThoughtFocus thoughtFocus)
    { attention.Receive(thoughtFocus); }
    public void Route(ContextInput contextInput)
    { myStack.outputStack.Route(contextInput);}
    public void Route(Attack attack)
    { myStack.outputStack.Route(attack); }
    public void Route(ThoughtRequest thoughtRequest)
    { thoughts.Receive(thoughtRequest); }
    public void Route(ThoughtResponse thoughtResponse)
    { attention.Receive(thoughtResponse); }
    public void Route(List<RankedThought> thoughts)
    { myStack.outputStack.Route(thoughts); }
    public void Route(Speech speech)
    { observer.Receive(speech); }
    public void Route(Interaction interaction)
    { observer.Receive(interaction); }
    
    public bool IsMe(string name)
    {
        return myStack.IsMe(name);
    }
    
    public HashSet<string> KnownActors()
    {
        return myStack.KnownActors;
    }
}
