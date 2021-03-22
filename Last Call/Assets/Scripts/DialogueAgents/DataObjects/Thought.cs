using System;
using System.Collections.Generic;

public class Thought
{
    public string Topic { get; set; }
    public Topic.Stage Stage { get; set; }
    public float Complexity { get; set; }
    public string Text { get; set; }
    public HashSet<string> Tangents { get; set; }
    public int EventCode { get; set; }
    public HashSet<Attack> Affections { get; set; }
    public HashSet<Emotion> Emotions { get; set; }

    public Thought(string topic, Topic.Stage stage, float complexity, string text, HashSet<string> tangents, int eventCode, HashSet<Attack> affections, HashSet<Emotion> emotions)
    {
        Topic = topic;
        Stage = stage;
        Complexity = complexity;
        Text = text;
        Tangents = tangents;
        EventCode = eventCode;
        Affections = affections;
        Emotions = emotions;
    }

    public Thought(Thought thought)
    {
        Topic = thought.Topic;
        Stage = thought.Stage;
        Complexity = thought.Complexity;
        Text = thought.Text;
        Tangents = thought.Tangents;
        EventCode = thought.EventCode;
        Affections = thought.Affections;
        Emotions = thought.Emotions;
    }
}

public class FilteredThought : Thought,  IComparable<FilteredThought>
{
    public float Filter { get; set; }
    
    public int CompareTo(FilteredThought other)
    {
        float dif = Filter - other.Filter;

        if (dif < -float.Epsilon) return -1;
        if (dif > float.Epsilon) return 1;
        return 0;
    }

    public FilteredThought(Thought thought, float filter) : base(thought)
    {
        Filter = filter;
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

    public RankedThought(FilteredThought filteredThought) : base(filteredThought)
    {
        Rank = filteredThought.Filter;
    } 
}