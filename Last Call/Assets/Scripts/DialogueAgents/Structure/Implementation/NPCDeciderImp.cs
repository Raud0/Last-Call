using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class NPCDeciderImp : DeciderImp
{
    public override void ImpReceive(Emotion emotion)
    {
        //TODO: Display emotions
        return;
    }

    public override void ImpReceive(ContextInput contextInput)
    {
        return;
    }

    public override void ImpReceive(List<RankedThought> thoughts)
    {
        return;
    }

    private void FixedUpdate()
    {
        DoSpeaking();
    }

    private bool ShouldSpeak() => currentThought != null || !SomeoneElseIsSpeaking();

    private void DoSpeaking()
    {
        if (!ShouldSpeak()) return;
        
        if (currentThought == null) PickThought();

        ProgressThought();
    }

    private void PickThought()
    {
        if (thoughts.Count == 0) return; 
        
        currentThought = thoughts[0];
        currentThoughtProgress = 0f;
    }

    private void ProgressThought()
    {
        if (currentThought == null) return;
        
        currentThoughtProgress += thoughtSpeed / currentThought.Text.Length * Time.fixedDeltaTime;
        Speech speech = new Speech(myOutput.myStack.Me, currentThoughtProgress, currentThought);
        
        Send(speech);

        if (currentThoughtProgress >= 1f)
        {
            currentThought = null;
            currentThoughtProgress = 0f;
        }
    }
}