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
    public HashSet<ActorStack> Actors { get; set; }
    public HashSet<Topic> Topics { get; set; }
    public Dictionary<string,HashSet<Thought>> Thoughts { get; set; }
    
    public HashSet<string> GetActorNames()
    {
        HashSet<string> names = new HashSet<string>();
        foreach (ActorStack actor in Actors) { names.Add(actor.Me); }
        return names;
    }

    private void LoadData(TextAsset topicTextAsset, TextAsset thoughtTextAsset)
    {
        Topics = Loader.LoadTopics(topicTextAsset);
        Thoughts = Loader.LoadThoughts(thoughtTextAsset);
    }

    private void Initialize()
    {
        LoadData(topicTextAsset, thoughtTextAsset);
        
        foreach (ActorStack actor in Actors) actor.Initialize(this);
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