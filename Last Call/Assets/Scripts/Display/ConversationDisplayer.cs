using System;
using System.Collections.Generic;
using UnityEngine;

public class ConversationDisplayer : Singleton<ConversationDisplayer>
{
    public GameObject speechDisplayPrefab;
    private Dictionary<Thought, SpeechDisplay> speeches = new Dictionary<Thought, SpeechDisplay>();
    private HashSet<Thought> completed = new HashSet<Thought>();

    public ConversationMedium conversationMedium;
    private void Awake()
    {
        conversationMedium.OnSpeech += ReceiveSpeech;
    }

    private void OnDestroy()
    {
        conversationMedium.OnSpeech -= ReceiveSpeech;
    }

    private void ReceiveSpeech(Speech speech)
    {
        if (completed.Contains(speech.Thought)) return;
        
        ActorStack actor = GetActor(speech.Actor);

        SpeechDisplay speechDisplay = GetSpeechDisplay(speech);

        if (speechDisplay.UpdateProgress(speech.Progress))
        {
            speeches.Remove(speech.Thought);
            completed.Add(speech.Thought);
        }
    }

    private SpeechDisplay GetSpeechDisplay(Speech speech)
    {
        Thought thought = speech.Thought;
        if (!speeches.ContainsKey(thought)) CreateSpeechDisplay(speech);
        return speeches[thought];
    }

    private void CreateSpeechDisplay(Speech speech)
    {
        SpeechDisplay speechDisplay = Instantiate(speechDisplayPrefab, transform).GetComponent<SpeechDisplay>();
        speechDisplay.SetText(speech.Thought.Text);
        speechDisplay.SetColor(GetActor(speech.Actor).fg);
        speechDisplay.SetAlignment(ActorIsLead(speech.Actor));

        Thought thought = speech.Thought;
        speeches[thought] = speechDisplay;
    }

    private ActorStack GetActor(string actor)
    {
        return conversationMedium.GetActorByName(actor);
    }

    private bool ActorIsLead(string actor)
    {
        return conversationMedium.lead.Equals(actor);
    }
}