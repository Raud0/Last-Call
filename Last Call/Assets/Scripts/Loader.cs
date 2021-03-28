using System;
using System.Collections.Generic;
using UnityEngine;

public static class Loader
{
    private static Dictionary<string, Topic.Stage> stageMap = new Dictionary<string, Topic.Stage>()
    {
        {"none", Topic.Stage.None},
        {"hidden", Topic.Stage.Hidden},
        {"orientation", Topic.Stage.Orientation},
        {"complication", Topic.Stage.Complication},
        {"climax", Topic.Stage.Climax},
        {"denouement", Topic.Stage.Denouement},
        {"coda", Topic.Stage.Coda},
        {"ended", Topic.Stage.Ended},
    };

    private static Dictionary<string, Thought.Interrupt> interruptMap = new Dictionary<string, Thought.Interrupt>()
    {
        {"want", Thought.Interrupt.Want},
        {"none", Thought.Interrupt.None},
        {"hate", Thought.Interrupt.Hate}
    };

    private static Dictionary<string, Thought.Turn> turnMap = new Dictionary<string, Thought.Turn>()
    {
        {"give", Thought.Turn.Give},
        {"none", Thought.Turn.None},
        {"keep", Thought.Turn.Keep}
    };
    
    private static Dictionary<string, Thought.Affinity> affinityMap = new Dictionary<string, Thought.Affinity>()
    {
        {"love", Thought.Affinity.Love},
        {"none", Thought.Affinity.None},
        {"hate", Thought.Affinity.Hate}
    };

private static Dictionary<int, Emotion.Type> emotionIndexMap = new Dictionary<int, Emotion.Type>()
    {
        {10, Emotion.Type.Social},
        {11, Emotion.Type.Will},
        {12, Emotion.Type.Love}
    };
    
    private static Dictionary<int, Attack.Type> attackIndexMap = new Dictionary<int, Attack.Type>()
    {
        {13, Attack.Type.Shame},
        {14, Attack.Type.Praise},
        {15, Attack.Type.Scare},
        {16, Attack.Type.Encourage},
        {17, Attack.Type.Mistrust},
        {18, Attack.Type.Trust},
        {19, Attack.Type.Silence}
    };

    public static Dictionary<string,HashSet<Topic>> LoadTopics(TextAsset textAsset, HashSet<ActorStack> actors)
    {
        Dictionary<string,HashSet<Topic>> actorTopics = new Dictionary<string, HashSet<Topic>>();

        foreach (ActorStack actorStack in actors)
        {
            char[] nextLineChars = {'\n'};
            char[] nextValueChars = {','};
        
            string text = textAsset.text;
            string[] lines = text.Split(nextLineChars);

            Dictionary<string, Topic> identities = new Dictionary<string, Topic>();
            Dictionary<string, HashSet<Topic>> branches = new Dictionary<string, HashSet<Topic>>();
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];
                string[] values = line.Split(nextValueChars, 2);
            
                try
                {
                    string topicName = values[0].Trim();
                    if (topicName.Equals("")) continue;

                    Topic topic = new Topic(topicName, null);
                    identities[topicName] = topic;
                
                    if (values.Length < 2) continue;
                
                    string parentName = values[1].Trim();
                    if (!branches.ContainsKey(parentName)) branches[parentName] = new HashSet<Topic>();
                    branches[parentName].Add(topic);
                }
                catch (Exception e)
                {
                    Debug.Log("Skipped reading: \"" + line + "\". (" + e.Message + ")");
                }
            
            }

            foreach (KeyValuePair<string,HashSet<Topic>> pair in branches)
            {
                string parentName = pair.Key;
                HashSet<Topic> children = pair.Value;

                Topic parent = identities[parentName];
                foreach (Topic child in children) child.Parent = parent;
            }

            HashSet<Topic> newTopics = new HashSet<Topic>();
            foreach (Topic topic in identities.Values) newTopics.Add(topic);

            actorTopics[actorStack.Me] = newTopics;
        }
        
        return actorTopics;
    }

    public static Dictionary<string,HashSet<Thought>> LoadThoughts(TextAsset textAsset)
    {
        char[] nextLineChars = {'\n'};
        char[] nextValueChars = {'\t'};
        char[] nextSubValueChars = {';'};
        
        string text = textAsset.text;
        string[] lines = text.Split(nextLineChars);
        
        Dictionary<string, HashSet<Thought>> newThoughts = new Dictionary<string, HashSet<Thought>>();
        string topic = null;
        Topic.Stage stage = Topic.Stage.None;
        string actor = null;

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            string[] values = line.Split(nextValueChars, 18);
            
            try
            {
                // Update running values: Topic, Stage, Actor
                if (!values[0].Equals("")) topic = values[0].Trim();
                string stageText = values[1].Trim().ToLower();
                if (stageMap.ContainsKey(stageText)) stage = stageMap[stageText];
                string actorText = values[2].Trim();
                if (!actorText.Equals(""))
                {
                    actor = actorText;
                    if (!newThoughts.ContainsKey(actor)) newThoughts[actor] = new HashSet<Thought>();
                }

                // Read Complexity
                float complexity = 0f;
                if (!float.TryParse(values[3].Trim(), out complexity))
                {
                    Debug.Log("Failed to read: \"" + line + "\".");
                    continue;
                }

                // Read Text
                string thoughtText = values[4].Trim();
                if (values[4].Equals("")) continue;
                
                // Read Social Expectations
                Thought.Interrupt interruptStrategy = Thought.Interrupt.None;
                string interruptText = values[5].Trim().ToLower();
                if (interruptMap.ContainsKey(interruptText)) interruptStrategy = interruptMap[interruptText];

                Thought.Turn turnStrategy = Thought.Turn.None;
                string turnText = values[6].Trim().ToLower();
                if (turnMap.ContainsKey(turnText)) turnStrategy = turnMap[turnText];

                Thought.Affinity affinity = Thought.Affinity.None;
                string affinityText = values[7].Trim().ToLower();
                if (affinityMap.ContainsKey(affinityText)) affinity = affinityMap[affinityText];

                // Read Tangental Topics
                string[] tangentStrings = values[8].Trim().Split(nextSubValueChars);
                HashSet<string> tangents = new HashSet<string>();
                foreach (var tangent in tangentStrings)
                {
                    if (!tangent.Equals("")) tangents.Add(tangent.Trim());
                }

                // Read Special Event Codes 
                int eventCode = -1;
                int.TryParse(values[9].Trim(), out eventCode);

                // Read Emotional Weight Values
                HashSet<Emotion> emotions = new HashSet<Emotion>();
                foreach (KeyValuePair<int, Emotion.Type> pair in emotionIndexMap)
                {
                    int index = pair.Key;
                    if (index > values.Length - 1) continue;

                    Emotion.Type type = pair.Value;

                    float strength = 0f;
                    if (float.TryParse(values[index].Trim(), out strength))
                    {
                        Emotion emotion = new Emotion(type, strength);
                        emotions.Add(emotion);
                    }
                }

                // Read Emotional Attack Values
                HashSet<Attack> attacks = new HashSet<Attack>();
                foreach (KeyValuePair<int, Attack.Type> pair in attackIndexMap)
                {
                    int index = pair.Key;
                    if (index > values.Length - 1) continue;
                    
                    Attack.Type type = pair.Value;

                    float strength = 0f;
                    if (float.TryParse(values[index].Trim(), out strength))
                    {
                        Attack attack = new Attack(type, strength);
                        attacks.Add(attack);
                    }
                }
                
                Thought thought = new Thought(topic, stage, actor, complexity, thoughtText, interruptStrategy, turnStrategy, affinity, tangents, eventCode, attacks, emotions);
                newThoughts[actor].Add(thought);
                
            } catch (Exception e)
            {
                Debug.Log("Skipped reading: \"" + line + "\" with value count " + values.Length + ". (" + e.Message + ")");
            }
        }

        foreach (KeyValuePair<string, HashSet<Thought>> pair in newThoughts)
        {
            string actorName = pair.Key;
            int length = pair.Value.Count;

            Debug.Log("Read in " + length + " thoughts for " + actorName + ".");
        }

        return newThoughts;
    }
}