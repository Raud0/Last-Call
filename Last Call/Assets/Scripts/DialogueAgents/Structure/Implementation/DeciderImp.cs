using System.Collections.Generic;
using System.Linq;

public abstract class DeciderImp : Decider
{
    protected Thought currentThought = null;
    protected float thoughtSpeed = 1f;
    protected float currentThoughtProgress = 0f;
    protected List<RankedThought> thoughts = new List<RankedThought>();
    protected Dictionary<Emotion.Type, float> weights = new Dictionary<Emotion.Type, float>();
    protected Dictionary<string, bool> actorSpeaking = new Dictionary<string, bool>();

    public abstract void ImpReceive(Emotion emotion);
    public abstract void ImpReceive(ContextInput contextInput);
    public abstract void ImpReceive(List<RankedThought> thoughts);
    
    public override void Receive(Emotion emotion)
    {
        weights[emotion.MyType] = emotion.Strength;
        RankThoughts();
        
        ImpReceive(emotion);
    }
    
    public override void Receive(ContextInput contextInput)
    {
        actorSpeaking[contextInput.Actor] = contextInput.Acting;

        ImpReceive(contextInput);
    }

    public override void Receive(List<RankedThought> thoughts)
    {
        this.thoughts = thoughts;
        RankThoughts();
        
        ImpReceive(thoughts);
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
        bool speaking = false;

        foreach (KeyValuePair<string,bool> item in actorSpeaking)
        {
            speaking = speaking || (!myOutput.IsMe(item.Key) && item.Value);
        }

        return speaking;
    }
}