using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorStack : MonoBehaviour
{
    public ConversationMedium conversation;
    
    public string Me { get; set; }
    
    public HashSet<string> KnownActors = new HashSet<string>();
    
    public InputStack inputStack;
    public OutputStack outputStack;
    
    public void Initialize(ConversationMedium conversationMedium)
    {
        conversation = conversationMedium;
        KnownActors = conversation.GetActorNames();
        
        KnownActors.Add(Me);
        inputStack.Initialize(this, conversationMedium.Topics, conversationMedium.Thoughts[Me]);
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
