using System;
using System.Collections.Generic;
using UnityEngine;

public class Thought
{
    public enum Interrupt
    {
        Want,
        None,
        Hate
    }
    public enum Turn
    {
        Give,
        None,
        Keep
    }
    public enum Affinity
    {
        Love,
        None,
        Hate
    }
    
    public string Topic { get; set; }
    public Topic.Stage Stage { get; set; }
    public string Actor { get; set; }
    public float Complexity { get; set; }
    public string Text { get; set; }
    public Interrupt InterruptStrategy { get; set; }
    public Turn TurnStrategy { get; set; }
    public Affinity MyAffinity { get; set; }
    public HashSet<string> Tangents { get; set; }
    public int EventCode { get; set; }
    public HashSet<Attack> Affections { get; set; }
    public HashSet<Emotion> Emotions { get; set; }

    public Thought(string topic, Topic.Stage stage, string actor, float complexity, string text,
        Interrupt interruptStrategy, Turn turnStrategy, Affinity myAffinity, HashSet<string> tangents,
        int eventCode, HashSet<Attack> affections, HashSet<Emotion> emotions)
    {
        Topic = topic;
        Stage = stage;
        Actor = actor;
        Complexity = complexity;
        Text = text;
        InterruptStrategy = interruptStrategy;
        TurnStrategy = turnStrategy;
        MyAffinity = myAffinity;
        Tangents = tangents;
        EventCode = eventCode;
        Affections = affections;
        Emotions = emotions;
    }

    public Thought(Thought thought)
    {
        Topic = thought.Topic;
        Stage = thought.Stage;
        Actor = thought.Actor;
        Complexity = thought.Complexity;
        Text = thought.Text;
        InterruptStrategy = thought.InterruptStrategy;
        TurnStrategy = thought.TurnStrategy;
        MyAffinity = thought.MyAffinity;
        Tangents = thought.Tangents;
        EventCode = thought.EventCode;
        Affections = thought.Affections;
        Emotions = thought.Emotions;
    }
}

public class FocusedThought : Thought,  IComparable<FocusedThought>
{
    public float Distance { get; set; }
    public float Share { get; set; }

    public float Focus()
    {
        return Distance * (1.5f - Share);
    }
    
    public int CompareTo(FocusedThought other)
    {
        float dif = Focus() - other.Focus();

        if (dif < -float.Epsilon) return -1;
        if (dif > float.Epsilon) return 1;
        return 0;
    }

    public FocusedThought(Thought thought, float distance, float share) : base(thought)
    {
        Distance = distance;
        Share = share;
    }
}

public class RankedThought : Thought,  IComparable<RankedThought>
{
    public float OriginalRank { get; set; }
    public float Rank { get; set; }
    
    public int CompareTo(RankedThought other)
    {
        float dif = Rank - other.Rank;

        if (dif < -float.Epsilon) return -1;
        if (dif > float.Epsilon) return 1;
        return 0;
    }

    public RankedThought(Thought thought, float rank) : base(thought)
    {
        OriginalRank = rank;
        Rank = rank;
    }

    public RankedThought(FocusedThought focusedThought) : this(focusedThought, focusedThought.Focus())
    {} 
}