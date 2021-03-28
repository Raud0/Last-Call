using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputStack : MonoBehaviour
{
    public ActorStack myStack;

    public GameObject attentionObject;
    public GameObject thoughtsObject;
    public GameObject observerObject;
    [HideInInspector]
    public Attention attention;
    [HideInInspector]
    public Thoughts thoughts;
    [HideInInspector]
    public Observer observer;
    
    public void Initialize(ActorStack stack, HashSet<Topic> newTopics, HashSet<Thought> newThoughts)
    {
        myStack = stack;
        myStack.conversation.OnSpeech += Route;
        myStack.conversation.OnInteraction += Route;
        
        attention = Instantiate(attentionObject, transform).GetComponent<Attention>();
        attention.Initialize(this);
        thoughts = Instantiate(thoughtsObject, transform).GetComponent<Thoughts>();
        thoughts.Initialize(this);
        observer = Instantiate(observerObject, transform).GetComponent<Observer>();
        observer.Initialize(this);

        attention.Load(newTopics);
        thoughts.Load(newThoughts);
    }
    
    public void DeInitialize()
    {
        myStack.conversation.OnSpeech -= Route;
        myStack.conversation.OnInteraction -= Route;
    }
    
    public void Route(ThoughtFocus thoughtFocus) => attention.Receive(thoughtFocus);
    public void Route(SocialInput socialInput) => myStack.outputStack.Route(socialInput);
    public void Route(ActingInput actingInput) => myStack.outputStack.Route(actingInput);
    public void Route(Attack attack) => myStack.outputStack.Route(attack);
    public void Route(ThoughtRequest thoughtRequest) => thoughts.Receive(thoughtRequest);
    public void Route(ThoughtResponse thoughtResponse) => attention.Receive(thoughtResponse);
    public void Route(List<RankedThought> thoughts) => myStack.outputStack.Route(thoughts);
    public void Route(Speech speech) => observer.Receive(speech);
    public void Route(Interaction interaction) => observer.Receive(interaction);

    public bool IsMe(string name)
    {
        return myStack.IsMe(name);
    }
    
    public HashSet<string> KnownActors()
    {
        return myStack.KnownActors;
    }
}
