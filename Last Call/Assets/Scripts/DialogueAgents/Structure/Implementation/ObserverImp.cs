using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class ObserverImp : Observer
{
    private Queue<Action> newActions = new Queue<Action>();
    private HashSet<string> newSpeechActors = new HashSet<string>();
    private Dictionary<Thought, ObservationProgress> thoughts = new Dictionary<Thought, ObservationProgress>();
    private Queue<Thought> delete = new Queue<Thought>();
    
    private bool newSpeech = false;
    private bool newInteractions = false;
    private float lastCheck;
    private float timeSinceLastCheck;
    
    private void HandleNewActions()
    {
        newSpeech = false;
        newInteractions = false;
        while (newActions.Count > 0)
        {
            Action action = newActions.Dequeue();

            if (action.MyType == Action.Type.Speech)
            {
                Handle((Speech) action);
            } else if (action.MyType == Action.Type.Interaction)
            {
                Handle((Interaction) action);
            }
        }
    }

    private void Handle(Speech speech)
    {
        if (!thoughts.ContainsKey(speech.Thought)) thoughts[speech.Thought] = new ObservationProgress();
        
        thoughts[speech.Thought].Progress = speech.Progress;
        
        newSpeech = true;
        newSpeechActors.Add(speech.Actor);
    }

    private void Handle(Interaction interaction)
    {
        newInteractions = true;
    }

    private void Update()
    {
        HandleNewActions();
        
        timeSinceLastCheck = Time.time - lastCheck;
        if (timeSinceLastCheck > 1f || newSpeech) { HandleSpeech(); }
    }

    private void HandleSpeech()
    {
        foreach (Thought thought in thoughts.Keys)
        {
            ObservationProgress observationProgress = thoughts[thought];
            observationProgress.Time += timeSinceLastCheck;

            if (observationProgress.Stage == 0 && observationProgress.Progress >= 0.1f)
            {
                observationProgress.Stage = 1;
                HandleStageOne(thought);
            }

            if (observationProgress.Stage == 1 && observationProgress.Progress >= 0.5f)
            {
                observationProgress.Stage = 2;
                HandleStageTwo(thought);
            }

            if (observationProgress.Stage == 2 && observationProgress.Progress >= 1.0f)
            {
                observationProgress.Stage = 3;
                HandleStageThree(thought);
            }

            if (observationProgress.Stage == 3 || observationProgress.Time >= 30.0f)
            {
                delete.Enqueue(thought);
            }
        }

        while (delete.Count > 0) { thoughts.Remove(delete.Dequeue()); }

        HandleContext();

        lastCheck = Time.time;
    }

    private void HandleContext()
    {
        HashSet<string> actors = myInput.KnownActors();
        
        foreach (string actor in actors)
        {
            bool acting = newSpeechActors.Contains(actor);
            Send(new ActingInput(actor, acting));
        }
        
        newSpeechActors.Clear();
    }

    private void HandleStageOne(Thought thought)
    {
        ThoughtFocus thoughtFocus = new ThoughtFocus(thought, 0.2f, false, false);
        Send(thoughtFocus);
        
        SocialInput socialInput = new SocialInput(thought.Actor, thought, 1);
        Send(socialInput);
    }

    private void HandleStageTwo(Thought thought)
    {
        ThoughtFocus thoughtFocus = new ThoughtFocus(thought, 0.3f, true, false);
        Send(thoughtFocus);

        SocialInput socialInput = new SocialInput(thought.Actor, thought, 2);
        Send(socialInput);

        if (!myInput.IsMe(thought.Actor))
        {
            foreach (Argument argument in thought.Arguments)
            {
                Argument newArgument = new Argument(argument.MyType, argument.Strength * 0.5f);
                Send(newArgument);
            }
        }
    }

    private void HandleStageThree(Thought thought)
    {
        ThoughtFocus thoughtFocus = new ThoughtFocus(thought, 0.5f, true, true);
        Send(thoughtFocus);
        
        SocialInput socialInput = new SocialInput(thought.Actor, thought, 3);
        Send(socialInput);
        
        if (!myInput.IsMe(thought.Actor))
        {
            foreach (Argument argument in thought.Arguments)
            {
                Argument newArgument = new Argument(argument.MyType, argument.Strength * 0.5f);
                Send(newArgument);
            }
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