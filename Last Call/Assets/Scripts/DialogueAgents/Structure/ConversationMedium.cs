using System;
using System.Collections.Generic;
using UnityEngine;

public class ConversationMedium : MonoBehaviour
{
    public event Action<Speech> OnSpeech;
    public void Speak(Speech speech) => OnSpeech?.Invoke(speech);

    public event Action<Interaction> OnInteraction;
    public void Interact(Interaction interaction) => OnInteraction?.Invoke(interaction);

    public TextAsset topicTextAsset;
    public TextAsset thoughtTextAsset;
    public List<ActorStack> ActorStacks;
    public HashSet<ActorStack> Actors { get; set; }
    public string lead;
    public Dictionary<string,HashSet<Topic>> Topics { get; set; }
    public Dictionary<string,HashSet<Thought>> Thoughts { get; set; }

    public HashSet<Topic> GetTopics(string actor)
    {
        if (Topics.ContainsKey(actor)) return Topics[actor];
        return new HashSet<Topic>();
    }

    public HashSet<Thought> GetThoughts(string actor)
    {
        if (Thoughts.ContainsKey(actor)) return Thoughts[actor];
        return new HashSet<Thought>();
    }

    public ActorStack GetActorByName(string actor)
    {
        foreach (ActorStack actorStack in Actors)
        {
            if (actorStack.Me.Equals(actor))
            {
                return actorStack;
            }
        }

        return null;
    }

    public HashSet<string> GetActorNames()
    {
        HashSet<string> names = new HashSet<string>();
        foreach (ActorStack actor in Actors) { names.Add(actor.Me); }
        return names;
    }

    private void LoadData(TextAsset topicTextAsset, TextAsset thoughtTextAsset)
    {
        Topics = Loader.LoadTopics(topicTextAsset, Actors);
        Thoughts = Loader.LoadThoughts(thoughtTextAsset);
    }

    private void Initialize()
    {
        Actors = new HashSet<ActorStack>(ActorStacks);
        
        LoadData(topicTextAsset, thoughtTextAsset);

        foreach (ActorStack actor in Actors) actor.Initialize(this);

        foreach (ActorStack actor in Actors)
        {
            if (!actor.Me.Equals("Joe")) continue;
            
            actor.inputStack.Route(new ThoughtFocus(
                    null, 
                    null,
                    Topic.Stage.None,
                    0f,
                    new HashSet<string>() {"Call"},
                    1.0f,
                    false,
                    true
                    ));
    
        }
    }

    private void DeInitialize()
    {
        foreach (ActorStack actor in Actors) actor.DeInitialize();
    }

    private void Awake()
    {
        Initialize();
    }

    private void OnDestroy()
    {
        DeInitialize();
    }
}