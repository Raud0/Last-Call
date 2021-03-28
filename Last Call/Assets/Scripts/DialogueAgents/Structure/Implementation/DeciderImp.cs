using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class DeciderImp : Decider
{
    protected Thought currentThought = null;
    protected float thoughtSpeed = 30f;
    protected float currentThoughtProgress = 0f;
    protected float lastSpokeAt = 0f;
    protected List<RankedThought> thoughts = new List<RankedThought>();
    protected Dictionary<Emotion.Type, float> weights = new Dictionary<Emotion.Type, float>();
    protected Dictionary<string, bool> actorSpeaking = new Dictionary<string, bool>();

    public abstract void ImpReceive(Emotion emotion);
    public abstract void ImpReceive(SocialInput socialInput);
    public abstract void ImpReceive(ActingInput actingInput);
    public abstract void ImpReceive(List<RankedThought> thoughts);

    public override void Receive(Emotion emotion)
    {
        weights[emotion.MyType] = emotion.Strength;
        RankThoughts();
        
        ImpReceive(emotion);
    }

    public override void Receive(SocialInput socialInput)
    {
        
        
        ImpReceive(socialInput);
    }

    public override void Receive(ActingInput actingInput)
    {
        actorSpeaking[actingInput.Actor] = actingInput.Acting;

        ImpReceive(actingInput);
    }

    public override void Receive(List<RankedThought> thoughts)
    {
        this.thoughts = thoughts;
        RankThoughts();
        
        ImpReceive(thoughts);
    }

    protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.H)) DebugText();
    }

    public void DebugText()
    {
        string text = myOutput.myStack.Me + ":";
        foreach (RankedThought rankedThought in thoughts)
        {
            text += "\n" + rankedThought.Rank + ": " + rankedThought.Text;
        }
        Debug.Log(text);
    }

    private void RankThoughts()
    {
        foreach (RankedThought thought in thoughts)
        {
            float emotionalAffinity = thought.Emotions.Sum(emotion => emotion.Strength * weights[emotion.MyType]);
            
            thought.Rank = thought.OriginalRank + emotionalAffinity;
        }
        
        thoughts.Sort();
    }

    protected bool SomeoneElseIsSpeaking()
    {
        return actorSpeaking.Any(item => !myOutput.IsMe(item.Key) && item.Value);
    }

    public abstract void FinishThought();
    
    public void ProgressThought()
    {
        if (currentThought == null) return;
        
        currentThoughtProgress += thoughtSpeed / currentThought.Text.Length * Time.fixedDeltaTime;
        Speech speech = new Speech(myOutput.myStack.Me, currentThoughtProgress, currentThought);
        
        Send(speech);
        lastSpokeAt = Time.time;

        if (currentThoughtProgress >= 1f) FinishThought();
    }
}