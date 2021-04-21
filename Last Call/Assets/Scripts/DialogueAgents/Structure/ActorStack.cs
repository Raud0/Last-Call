using System.Collections.Generic;
using UnityEngine;

public class ActorStack : MonoBehaviour
{
    public ConversationMedium conversation;

    public string Me;
    public Color32 fg;
    public Color32 bg;
    
    public HashSet<string> KnownActors = new HashSet<string>();

    public GameObject inputStackObject;
    public GameObject outputStackObject;
    [HideInInspector]
    public InputStack inputStack;
    [HideInInspector]
    public OutputStack outputStack;
    
    public void Initialize(ConversationMedium conversationMedium)
    {
        conversation = conversationMedium;
        KnownActors = conversation.GetActorNames();
        
        KnownActors.Add(Me);
        inputStack = Instantiate(inputStackObject, transform).GetComponent<InputStack>();
        inputStack.Initialize(this, conversationMedium.GetTopics(Me), conversationMedium.GetThoughts(Me));
        outputStack = Instantiate(outputStackObject, transform).GetComponent<OutputStack>();
        outputStack.Initialize(this);
    }

    public void DeInitialize()
    {
        KnownActors.Clear();
        
        inputStack.DeInitialize();
        outputStack.DeInitialize();
    }

    public bool IsMe(string name)
    {
        return Me.Equals(name);
    }
}
