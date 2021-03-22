using System;
using System.Collections.Generic;
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

    private void Update()
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
        currentThought = thoughts[0];
        currentThoughtProgress = 0f;
    }

    private void ProgressThought()
    {
        float progression = currentThoughtProgress + thoughtSpeed / currentThought.Text.Length;
        Speech speech = new Speech(myOutput.myStack.Me, progression, currentThought);
        
        Send(speech);

        if (progression >= 1f)
        {
            currentThought = null;
            currentThoughtProgress = 0f;
        }
    }
}