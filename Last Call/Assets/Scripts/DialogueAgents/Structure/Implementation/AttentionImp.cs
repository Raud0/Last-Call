using System;
using System.Collections.Generic;
using UnityEngine;

public class AttentionImp : Attention
{
    private Dictionary<string, Topic> topicsByName;
    private List<RankedThought> thoughts;

    private void Awake()
    {
        Events.OnUpdateTime += Decay;
    }

    private void OnDestroy()
    {
        Events.OnUpdateTime -= Decay;
    }

    private void FilterThoughts()
    {
        List<RankedThought> filteredRankedThoughts = new List<RankedThought>();

        foreach (RankedThought rankedThought in thoughts)
        {
            Appraise(rankedThought);
            Topic topic = topicsByName[rankedThought.topic];
            Topic.Stage stage = rankedThought.stage;

            bool add = false;
            
            // TODO: Check for climaxes and ignore other topics?
            
            switch (topic.stage)
            {
                case Topic.Stage.Orientation:
                    add = stage == Topic.Stage.Orientation ||
                          stage == Topic.Stage.Complication;
                    break;
                case Topic.Stage.Complication:
                    add = stage == Topic.Stage.Orientation ||
                          stage == Topic.Stage.Complication ||
                          stage == Topic.Stage.Climax ||
                          stage == Topic.Stage.Denouement;
                    break;
                case Topic.Stage.Climax:
                    add = stage == Topic.Stage.Complication ||
                          stage == Topic.Stage.Climax;
                    break;
                case Topic.Stage.Denouement:
                    add = stage == Topic.Stage.Complication ||
                          stage == Topic.Stage.Denouement ||
                          stage == Topic.Stage.Coda;
                    break;
                case Topic.Stage.Coda:
                    add = stage == Topic.Stage.Coda;
                    break;
            }
            
            if (add) filteredRankedThoughts.Add(rankedThought);
        }

        filteredRankedThoughts = filteredRankedThoughts.GetRange(0, 10);
        List<Thought> filteredThoughts = new List<Thought>(filteredRankedThoughts);
        Send(filteredThoughts);
    }

    public void Remove(Thought thought)
    {
        RankedThought toRemove = null;
        foreach (RankedThought rankedThought in thoughts)
        {
            if (((Thought) rankedThought).Equals(thought))
            {
                toRemove = rankedThought;
                break;
            }
        }

        if (toRemove != null) thoughts.Remove(toRemove);
    }
    
    public override void Receive(ThoughtFocus thoughtFocus)
    {
        Thought original = thoughtFocus.originalThought;
        if (original != null) { Remove(original); }
        
        Topic topic = topicsByName[thoughtFocus.topicName];
        
        foreach (String tangent in thoughtFocus.tangents)
        {
            Topic tangentialTopic = topicsByName[tangent];
            if (tangentialTopic.stage == Topic.Stage.Hidden) ExitTo(topic, Topic.Stage.Orientation);
        }
        
        TopicHeard(topic, thoughtFocus);
    }

    public override void Receive(Thought thought)
    {
        thoughts.Add(RankNewThought(thought));
    }

    private void Decay(int seconds, int minutes)
    {
        ShiftAllTopics(0f, -0.15f, 1f); // decays by 9 points per minute, intuition tells me to switch to percentage, since decay is usually exponential
        FilterThoughts();
    }
    
    private void TopicHeard(Topic topic, ThoughtFocus thoughtFocus)
    {
        switch (topic.stage)
        {
            case Topic.Stage.Orientation:
                Orientation(topic, thoughtFocus);
                break;
            case Topic.Stage.Complication:
                Complication(topic, thoughtFocus);
                break;
            case Topic.Stage.Climax:
                Climax(topic, thoughtFocus);
                break;
            case Topic.Stage.Denouement:
                Denouement(topic, thoughtFocus);
                break;
            case Topic.Stage.Coda:
                Coda(topic, thoughtFocus);
                break;
        }

        ShiftAllTopics(-0.02f, 0f, thoughtFocus.finalMultiplier);
        FilterThoughts();
    }

    private void ShiftAllTopics(float percentage, float flat, float finalMultiplier)
    {
        foreach (Topic topic in topicsByName.Values)
        {  ShiftFocusChange(topic, topic.focus * (1f + percentage) + flat, true, finalMultiplier); }
    }

    private void ShiftFocusChange(Topic topic, float desiredFocus, bool canIncrease, float finalMultiplier)
    {
        float currentFocus = topic.focus;
        float distance = desiredFocus - currentFocus;
        
        float low = float.NegativeInfinity;
        float high = float.PositiveInfinity;
        if (!canIncrease) high = currentFocus;

        float newFocus = Mathf.Clamp(currentFocus + distance, low, high);
        float actualChange = (newFocus - currentFocus) * finalMultiplier;
        
        topic.FocusToRoots(actualChange, 0.7f, 3);
    }

    private void ShiftFocusChangeDiminishing(Topic topic, float desiredFocus, bool canIncrease, float a, float finalMultiplier)
    {
        float currentFocus = topic.focus;
        float distance = desiredFocus - currentFocus;
        
        float multiplier = 1f;

        if (distance < -a || distance > a)
        {
            multiplier = Math.Abs(a / distance);
        }

        float low = float.NegativeInfinity;
        float high = float.PositiveInfinity;
        if (!canIncrease) high = currentFocus;

        float newFocus = Mathf.Clamp(currentFocus + distance * multiplier, low, high);
        float actualChange = (newFocus - currentFocus) * finalMultiplier;
        
        topic.FocusToRoots(actualChange, 0.7f, 3);
    }
    
    private void Orientation(Topic topic, ThoughtFocus thoughtFocus)
    {
        if (thoughtFocus.stage == Topic.Stage.Complication) ExitTo(topic, Topic.Stage.Complication);
        ShiftFocusChangeDiminishing(topic, thoughtFocus.complexity, true, 5f, thoughtFocus.finalMultiplier);
    }

    private void Complication(Topic topic, ThoughtFocus thoughtFocus)
    {
        if (thoughtFocus.stage == Topic.Stage.Climax) ExitTo(topic, Topic.Stage.Climax);
        if (thoughtFocus.stage == Topic.Stage.Denouement) ExitTo(topic, Topic.Stage.Denouement);
        ShiftFocusChangeDiminishing(topic, thoughtFocus.complexity, true, 3f, thoughtFocus.finalMultiplier);
    }

    private void Climax(Topic topic, ThoughtFocus thoughtFocus)
    {
        float focusBefore = topic.focus;
        ShiftFocusChangeDiminishing(topic, thoughtFocus.complexity, true, 1f, thoughtFocus.finalMultiplier);
        float focusAfter = topic.focus;
        
        if (focusAfter < focusBefore) ExitTo(topic, Topic.Stage.Denouement);
    }

    private void Denouement(Topic topic, ThoughtFocus thoughtFocus)
    {
        if (thoughtFocus.stage == Topic.Stage.Coda) ExitTo(topic, Topic.Stage.Coda);
        ShiftFocusChangeDiminishing(topic, thoughtFocus.complexity, false, 3f, thoughtFocus.finalMultiplier);
    }

    private void Coda(Topic topic, ThoughtFocus thoughtFocus)
    {
        if (thoughtFocus.stage == Topic.Stage.Coda) ExitTo(topic, Topic.Stage.Depleted);
        ShiftFocusChangeDiminishing(topic, thoughtFocus.complexity, true, 5f, thoughtFocus.finalMultiplier);
    }

    private void ExitTo(Topic topic, Topic.Stage stage)
    {
        topic.stage = stage;

        switch (stage)
        {
            case Topic.Stage.Climax:
                RemoveThoughts(topic, Topic.Stage.Orientation);
                break;
            case Topic.Stage.Denouement:
                RemoveThoughts(topic, Topic.Stage.Orientation);
                RemoveThoughts(topic, Topic.Stage.Climax);
                break;
            case Topic.Stage.Coda:
                RemoveThoughts(topic, Topic.Stage.Complication);
                RemoveThoughts(topic, Topic.Stage.Denouement);
                break;
            case Topic.Stage.Depleted:
                RemoveThoughts(topic, Topic.Stage.Coda);
                break;
        }
        
        ThoughtRequest thoughtRequest = new ThoughtRequest();
        thoughtRequest.topic = topic.topicName;
        thoughtRequest.stage = topic.stage;
        Send(thoughtRequest);
    }

    private void RemoveThoughts(Topic topic, Topic.Stage stage)
    {
        Queue<RankedThought> delete = new Queue<RankedThought>();
        foreach (RankedThought thought in thoughts)
        { if (topicsByName[thought.topic].Equals(topic) && thought.stage == stage) delete.Enqueue(thought); }

        while (delete.Count > 0) { thoughts.Remove(delete.Dequeue()); }
    }

    private RankedThought RankNewThought(Thought thought)
    {
        RankedThought rankedThought = (RankedThought) thought;
        Appraise(rankedThought);
        return rankedThought;
    }

    private void Appraise(RankedThought rankedThought)
    {
        rankedThought.rank = Math.Abs(rankedThought.complexity - topicsByName[rankedThought.topic].focus);
        rankedThought.rank *= AttentionShare(topicsByName[rankedThought.topic]);
    }

    private float AttentionShare(Topic soughtTopic)
    {
        List<Topic> allTopics = new List<Topic>(topicsByName.Values);

        float total = 0f;
        float sought = 0f;

        foreach (Topic topic in allTopics)
        {
            float topicFocus = topic.RootFocus(0.7f, 3);
            total += topicFocus;
            if (topic.Equals(soughtTopic)) { sought = topicFocus; }
        }

        return sought / total;
    }
}