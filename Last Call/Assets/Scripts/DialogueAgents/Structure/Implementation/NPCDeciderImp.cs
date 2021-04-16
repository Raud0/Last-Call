using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class NPCDeciderImp : DeciderImp
{
    public override void ImpReceive(Emotion emotion)
    {
        //TODO: Display emotions
        return;
    }

    public override void ImpReceive(SocialInput socialInput)
    {
        return;
    }

    public override void ImpReceive(ActingInput actingInput)
    {
        return;
    }

    public override void ImpReceive(List<RankedThought> thoughts)
    {
        return;
    }

    private void FixedUpdate()
    {
        DoSpeaking();
    }

    private void DoSpeaking()
    {
        if (currentThought == null) SpeechStrategy();
        else if (SomeoneElseIsSpeaking())
        {
            ExperienceRudeness(currentThought, 2f);
            if (CareAboutCivility()) InterruptSelf();
        }
        
        ProgressThought();
    }

    private void InterruptSelf()
    {
        ReleaseThought();
    }

    private void SpeechStrategy()
    {
        if (Thoughts.Count == 0) return;

        float waitThreshold = 0.5f;
        bool waitingForAnswer = WaitingForAnswer();
        bool otherWaitingForAnswer = OtherWaitinForAnswer();
        bool continuingThought = ContinuingTurn();
        bool otherContinuingThought = OtherContinuingTurn();

        if (SomeoneElseIsSpeaking() || TimeSinceLastHeardSpeech() < 0.5f)
        {
            waitThreshold += 4f * (2f - Weights[Emotion.Type.Anger] / 100f - Weights[Emotion.Type.Ego] / 100f + Weights[Emotion.Type.Respect] / 100f);
        }
        
        // Do I respect the other people enough to care about being civil?
        if (!CareAboutCivility())
        {
            otherWaitingForAnswer = false;
            otherContinuingThought = false;
        }
        else
        {
            waitThreshold += 1f + Weights[Emotion.Type.Respect] / 100f;
        }
        
        if (Thoughts.Count > 0) waitThreshold *= Thoughts[0].Rank / 10f;

        if (waitingForAnswer) waitThreshold *= (1f + Weights[Emotion.Type.Respect] / 100f - Weights[Emotion.Type.Anger] / 100f) * 2f;
        if (otherWaitingForAnswer) waitThreshold *= (1f - Weights[Emotion.Type.Respect] / 100f) * 2f;
        if (continuingThought) waitThreshold *= (1f + Weights[Emotion.Type.Respect] / 100f) * 0.2f;
        if (otherContinuingThought) waitThreshold *= (1f - Weights[Emotion.Type.Respect] / 100f) * 0.2f;
        
        if (TimeSinceLastSpeech() < waitThreshold)
        {
            ReleaseThought();
            return;
        }

        if (waitingForAnswer)
        {
            ExperienceRudeness(lastThought, 5f);
            ReleaseLastThought();
        }
        
        Thought thought = null;
        if (thought == null && lastHeardThought != null && otherWaitingForAnswer)
        {
            thought = FindThoughtOfTopic(lastHeardThought.Topic);
            ReleaseLastHeardThought();
        }

        if (thought == null && lastThought != null && continuingThought)
        {
            thought = FindThoughtOfTopic(lastThought.Topic);
            ReleaseLastThought();
        }
        if (thought == null && Thoughts.Count > 0) thought = Thoughts[0];
        if (thought == null) return;
        SelectThought(thought);
    }

    private bool WaitingForAnswer()
    {
        if (lastThought == null) return false;
        if (lastStage < 3) return false;
        
        return lastThought.TurnStrategy.Equals(Thought.Turn.Give);
    }

    private bool OtherWaitinForAnswer()
    {
        if (lastHeardThought == null) return false;
        if (lastHeardStage < 3) return false;
        
        return lastHeardThought.TurnStrategy.Equals(Thought.Turn.Give);
    }

    private bool ContinuingTurn()
    {
        if (lastThought == null) return false;
        if (lastStage < 3) return false;
        
        return lastThought.TurnStrategy.Equals(Thought.Turn.Keep);
    }

    private bool OtherContinuingTurn()
    {
        if (lastHeardThought == null) return false;
        if (lastHeardStage < 3) return false;
        
        return lastHeardThought.TurnStrategy.Equals(Thought.Turn.Keep);
    }

    private void ReleaseThought()
    {
        currentThought = null;
        currentThoughtProgress = 0f;
    }

    private bool CareAboutCivility() => Weights[Emotion.Type.Respect] - Weights[Emotion.Type.Anger] > -10f;

    protected override void FinishThought()
    {
        ReleaseThought();
    }
}