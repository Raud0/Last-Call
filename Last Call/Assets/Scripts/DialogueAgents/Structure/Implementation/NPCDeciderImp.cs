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

    public override void ImpReceive(SocialInput socialInput)
    {
        return;
    }

    public override void ImpReceive(ActingInput actingInput)
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

    private void DoSpeaking()
    {
        if (currentThought == null) SpeechStrategy();

        ProgressThought();
    }

    private void SpeechStrategy()
    {
        if (thoughts.Count == 0) return;
        
        float waitThreshold = 0f;
        
        if (SomeoneElseIsSpeaking())
        {
            //TODO: Read emotional state
            waitThreshold += 5f;
        }

        waitThreshold += thoughts[0].Rank;

        float timeSinceLastSpeech = Time.time - lastSpokeAt;
        if (timeSinceLastSpeech < waitThreshold) return;

        currentThought = thoughts[0];
        currentThoughtProgress = 0f;
    }
    
    public override void FinishThought()
    {
        currentThought = null;
        currentThoughtProgress = 0f;
    }
}