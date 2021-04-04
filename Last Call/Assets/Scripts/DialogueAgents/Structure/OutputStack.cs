using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutputStack : MonoBehaviour
{
    public ActorStack myStack;

    public GameObject stateManagerObject;
    public GameObject deciderObject;
    [HideInInspector]
    public StateManager stateManager;
    [HideInInspector]
    public Decider decider;

    public void Initialize(ActorStack stack)
    {
        this.myStack = stack;

        stateManager = Instantiate(stateManagerObject,transform).GetComponent<StateManager>();
        stateManager.Initialize(this);
        decider = Instantiate(deciderObject,transform).GetComponent<Decider>();
        decider.Initialize(this);
    }
    
    public void DeInitialize()
    {

    }

    public void Route(SocialInput socialInput) => decider.Receive(socialInput);
    public void Route(Argument argument) => stateManager.Receive(argument);
    public void Route(Emotion emotion) => decider.Receive(emotion);
    public void Route(ActingInput actingInput) => decider.Receive(actingInput);
    public void Route(List<RankedThought> thoughts) => decider.Receive(thoughts);
    public void Speak(Speech speech) => myStack.conversation.Speak(speech);
    public void Interact(Interaction interaction) => myStack.conversation.Interact(interaction);
    
    public bool IsMe(string name)
    {
        return myStack.IsMe(name);
    }

    public HashSet<string> KnownActors()
    {
        return myStack.KnownActors;
    }
}
