﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class PCDeciderImp : DeciderImp
{
    public override void ImpReceive(Emotion emotion)
    {
        return;
    }

    public override void ImpReceive(ContextInput contextInput)
    {
        return;
    }

    public override void ImpReceive(List<RankedThought> thoughts)
    {
        OptionDisplayer.Instance.DeciderImp = this;
        OptionDisplayer.Instance.UpdateThoughts(new List<Thought>(thoughts));
    }
    
    private void FixedUpdate()
    {
        DoSpeaking();
    }

    private void DoSpeaking()
    {
        ProgressThought();
    }

    public void SelectThought(Thought thought)
    {
        currentThought = thought;
        currentThoughtProgress = 0f;
    }

    public void ReleaseThought(Thought thought)
    {
        if (currentThought != null && currentThought.Equals(thought)) currentThought = null;
        currentThoughtProgress = 0f;
    }

    public void FinishThought()
    {
        OptionDisplayer.Instance.FinishThought(currentThought);
        currentThought = null;
        currentThoughtProgress = 0f;
    }
    
    private void ProgressThought()
    {
        if (currentThought == null) return;
        
        currentThoughtProgress += thoughtSpeed / currentThought.Text.Length * Time.fixedDeltaTime;
        Speech speech = new Speech(myOutput.myStack.Me, currentThoughtProgress, currentThought);
        
        Send(speech);

        if (currentThoughtProgress >= 1f) FinishThought();
    }
}