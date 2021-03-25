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
    
    private static Dictionary<int, Emotion.Type> emotionIndexMap = new Dictionary<int, Emotion.Type>()
    {
        {8, Emotion.Type.Social},
        {9, Emotion.Type.Will},
        {10, Emotion.Type.Love}
    };
    
    private static Dictionary<int, Attack.Type> attackIndexMap = new Dictionary<int, Attack.Type>()
    {
        {11, Attack.Type.Shame},
        {12, Attack.Type.Praise},
        {13, Attack.Type.Scare},
        {14, Attack.Type.Encourage},
        {15, Attack.Type.Mistrust},
        {16, Attack.Type.Trust},
        {17, Attack.Type.Silence}
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
                if (!values[1].Equals("")) stage = stageMap[values[1].Trim().ToLower()];
                if (!values[2].Equals(""))
                {
                    actor = values[2].Trim();
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
                if (values[4].Equals(""))
                {
                    continue;
                }

                // Read Tangental Topics
                string[] tangentStrings = values[5].Trim().Split(nextSubValueChars);
                HashSet<string> tangents = new HashSet<string>();
                foreach (string tangent in tangentStrings)
                {
                    if (!tangent.Equals("")) tangents.Add(tangent.Trim());
                }

                // Read Special Event Codes 
                int eventCode = -1;
                int.TryParse(values[7].Trim(), out eventCode);

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
                
                Thought thought = new Thought(topic, stage, complexity, thoughtText, tangents, eventCode, attacks, emotions);
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