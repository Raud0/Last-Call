﻿using System.Collections.Generic;

public class ThoughtFocus
{
    public Thought MainThought { get; set; }
    public string Topic { get; set; }
    public Topic.Stage MyStage { get; set; }
    public float Complexity { get; set; }
    public HashSet<string> Tangents { get; set; }
    public float FinalMultiplier { get; set; }
    public bool RemoveThought { get; set; }

    public ThoughtFocus(Thought mainThought, string topic, Topic.Stage myStage, float complexity, HashSet<string> tangents, float finalMultiplier, bool removeThought)
    {
        MainThought = mainThought;
        Topic = topic;
        MyStage = myStage;
        Complexity = complexity;
        Tangents = tangents;
        FinalMultiplier = finalMultiplier;
        RemoveThought = removeThought;
    }

    public ThoughtFocus(Thought mainThought, float finalMultiplier, bool removeThought) :
        this(mainThought, mainThought.Topic, mainThought.Stage, mainThought.Complexity, mainThought.Tangents, finalMultiplier, removeThought) { }
}