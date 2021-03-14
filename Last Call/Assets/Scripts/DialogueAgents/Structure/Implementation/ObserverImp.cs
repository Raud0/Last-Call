using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class ObserverImp : Observer
{
    private Queue<Action> newActions;
    private Dictionary<Thought, ObservationProgress> thoughts;
    private Queue<Thought> delete = new Queue<Thought>();
    
    private bool newSpeech;
    private bool newInteractions;
    private float lastCheck;
    private float timeSinceLastCheck;

    private void Awake()
    {
        ConversationMedium.OnSpeech += Receive;
        ConversationMedium.OnInteraction += Receive;
        
        thoughts = new Dictionary<Thought, ObservationProgress>();
        
        newSpeech = false;
        newInteractions = false;
    }

    private void OnDestroy()
    {
        ConversationMedium.OnSpeech -= Receive;
        ConversationMedium.OnInteraction -= Receive;
    }

    private void HandleNewActions()
    {
        newSpeech = false;
        newInteractions = false;
        while (newActions.Count > 0)
        {
            Action action = newActions.Dequeue();

            if (action.type == Action.Type.Speech)
            {
                Handle((Speech) action);
            } else if (action.type == Action.Type.Interaction)
            {
                Handle((Interaction) action);
            }
        }
    }

    private void Handle(Speech speech)
    {
        newSpeech = true;
        
        ContextInput contextInput = new ContextInput();
        contextInput.acting = true;
        contextInput.actor = speech.actor;
        Send(contextInput);
    }

    private void Handle(Interaction interaction)
    {
        newInteractions = true;
    }

    private void Update()
    {
        HandleNewActions();
        
        timeSinceLastCheck = Time.time - lastCheck;
        if (timeSinceLastCheck > 5f || newSpeech) { HandleSpeech(); }
    }

    public void HandleSpeech()
    {
        foreach (Thought thought in thoughts.Keys)
        {
            ObservationProgress observationProgress = thoughts[thought];
            observationProgress.time += timeSinceLastCheck;

            if (observationProgress.stage == 0 && observationProgress.progress >= 0.1f)
            {
                observationProgress.stage = 1;
                HandleStageOne(thought);
            }

            if (observationProgress.stage == 1 && observationProgress.progress >= 0.5f)
            {
                observationProgress.stage = 2;
                HandleStageTwo(thought);
            }

            if (observationProgress.stage == 2 && observationProgress.progress >= 1.0f)
            {
                observationProgress.stage = 3;
                HandleStageThree(thought);
            }

            if (observationProgress.stage == 3 || observationProgress.time >= 30.0f)
            {
                delete.Enqueue(thought);
            }
        }

        while (delete.Count > 0) { thoughts.Remove(delete.Dequeue()); }

        lastCheck = Time.time;
    }

    public void HandleStageOne(Thought thought)
    {
        ThoughtFocus thoughtFocus = new ThoughtFocus();
        thoughtFocus.originalThought = thought;
        thoughtFocus.topicName = thought.topic;
        thoughtFocus.stage = Topic.Stage.None;
        thoughtFocus.complexity = thought.complexity;
        thoughtFocus.finalMultiplier = 0.2f;
        thoughtFocus.tangents = new List<string>();
        Send(thoughtFocus);
    }

    public void HandleStageTwo(Thought thought)
    {
        ThoughtFocus thoughtFocus = new ThoughtFocus();
        thoughtFocus.originalThought = null;
        thoughtFocus.topicName = thought.topic;
        thoughtFocus.stage = thought.stage;
        thoughtFocus.complexity = thought.complexity;
        thoughtFocus.finalMultiplier = 0.3f;
        thoughtFocus.tangents = thought.tangents;
        Send(thoughtFocus);

        foreach (Affection affection in thought.affections)
        {
            Affection newAffection = new Affection();
            newAffection.strength *= 0.5f;
            Send(newAffection);
        }
    }

    public void HandleStageThree(Thought thought)
    {
        ThoughtFocus thoughtFocus = new ThoughtFocus();
        thoughtFocus.originalThought = null;
        thoughtFocus.topicName = thought.topic;
        thoughtFocus.stage = Topic.Stage.None;
        thoughtFocus.complexity = thought.complexity;
        thoughtFocus.finalMultiplier = 0.5f;
        thoughtFocus.tangents = new List<string>();
        Send(thoughtFocus);
        
        foreach (Affection affection in thought.affections)
        {
            Affection newAffection = new Affection();
            newAffection.strength *= 0.5f;
            Send(newAffection);
        }
    }

    public override void Receive(Speech speech)
    {
        newActions.Enqueue(speech);
    }

    public override void Receive(Interaction interaction)
    {
        newActions.Enqueue(interaction);
    }
}