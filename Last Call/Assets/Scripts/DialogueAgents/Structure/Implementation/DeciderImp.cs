using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class DeciderImp : Decider
{
    protected float thoughtSpeed = 30f;
    
    protected Thought currentThought = null;
    protected Thought lastThought = null;
    protected int lastStage = 0;
    protected Thought lastHeardThought = null;
    protected int lastHeardStage = 0;
    
    protected float currentThoughtProgress = 0f;
    protected float lastSpokeAt = 0f;
    protected float lastHeardSpokeAt = 0f;
    
    protected List<RankedThought> Thoughts { get; set;}
    protected Dictionary<Emotion.Type, float> Weights { get; set; }
    protected Dictionary<string, bool> ActorSpeaking { get; set; }

    public abstract void ImpReceive(Emotion emotion);
    public abstract void ImpReceive(SocialInput socialInput);
    public abstract void ImpReceive(ActingInput actingInput);
    public abstract void ImpReceive(List<RankedThought> thoughts);

    public override void Receive(Emotion emotion)
    {
        Weights[emotion.MyType] = emotion.Strength;
        RankThoughts();
        
        ImpReceive(emotion);
    }

    public override void Receive(SocialInput socialInput)
    {
        if (myOutput.IsMe(socialInput.Actor))
        {
            lastThought = socialInput.Thought;
            lastStage = socialInput.Stage;
        }
        else
        {
            lastHeardThought = socialInput.Thought;
            lastHeardStage = socialInput.Stage;
            lastHeardSpokeAt = Time.time;
        }

        ImpReceive(socialInput);
    }

    public override void Receive(ActingInput actingInput)
    {
        ActorSpeaking[actingInput.Actor] = actingInput.Acting;

        ImpReceive(actingInput);
    }

    public override void Receive(List<RankedThought> thoughts)
    {
        this.Thoughts = thoughts;
        RankThoughts();
        
        ImpReceive(thoughts);
    }

    private void Awake()
    {
        Thoughts = new List<RankedThought>();
        Weights = new Dictionary<Emotion.Type, float>();
        ActorSpeaking = new Dictionary<string, bool>();
    }

    protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.H)) DebugText();
    }

    public void DebugText()
    {
        string text = myOutput.myStack.Me + ":";
        foreach (RankedThought rankedThought in Thoughts)
        {
            text += "\n" + rankedThought.Rank + ": " + rankedThought.Text;
        }
        Debug.Log(text);
    }

    private void RankThoughts()
    {
        foreach (RankedThought thought in Thoughts)
        {
            float emotionalAffinity = thought.Emotions.Sum(emotion => emotion.Strength * (Weights[emotion.MyType] / 100f));
            
            thought.Rank = thought.OriginalRank * 2f - emotionalAffinity;
        }
        
        Thoughts.Sort();
    }

    protected bool SomeoneElseIsSpeaking()
    {
        return ActorSpeaking.Any(item => !myOutput.IsMe(item.Key) && item.Value);
    }

    public void SelectThought(Thought thought)
    {
        currentThought = thought;
        currentThoughtProgress = 0f;
    }
    
    protected abstract void FinishThought();
    
    protected void ReleaseLastThought()
    {
        lastThought = null;
        lastStage = 0;
    }

    protected void ReleaseLastHeardThought()
    {
        lastHeardThought = null;
        lastHeardStage = 0;
    }
    
    protected void ProgressThought()
    {
        if (currentThought == null) return;
        
        currentThoughtProgress += thoughtSpeed / currentThought.Text.Length * Time.fixedDeltaTime;
        Speech speech = new Speech(myOutput.myStack.Me, currentThoughtProgress, currentThought);
        
        Send(speech);
        lastSpokeAt = Time.time;

        if (currentThoughtProgress >= 1f) FinishThought();
    }

    protected float TimeSinceLastSpeech() => Time.time - lastSpokeAt;
    protected float TimeSinceLastHeardSpeech() => Time.time - lastHeardSpokeAt;

    protected void ExperienceRudeness(Thought thought, float strength)
    {
        myOutput.Route(new Argument(Argument.Type.Superiority, strength * Time.deltaTime));
    }
    
    protected Thought FindThoughtOfTopic(string topic) => Thoughts.FirstOrDefault(rankedThought => rankedThought.Topic.Equals(topic));
}