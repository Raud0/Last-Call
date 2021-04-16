using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateManagerImp : StateManager
{
    protected Dictionary<Emotion.Type, float> States { get; set; }
    protected Dictionary<Argument.Type, float> Beliefs { get; set; }

    public abstract void ImpReceive(Argument argument);

    public virtual void Awake()
    {
        
    }

    private void Start()
    {
        foreach (KeyValuePair<Emotion.Type, float> statePair in States)
        {
            Emotion emotion = new Emotion(statePair.Key, statePair.Value);
            Send(emotion);
        }
    }

    protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) DebugText();
    }

    public void DebugText()
    {
        string text = myOutput.myStack.Me + ":";
        text += "\n" + "Anger" + ": " + States[Emotion.Type.Anger];
        text += "\n" + "Fear" + ": " + States[Emotion.Type.Fear];
        text += "\n" + "Ego" + ": " + States[Emotion.Type.Ego];
        text += "\n" + "Respect" + ": " + States[Emotion.Type.Respect];
        Debug.Log(text);
    }
    
    private void ChangeState(Emotion.Type type, float change)
    {
        States[type] += change;
        Emotion emotion = new Emotion(type, States[type]);
        Send(emotion);
    }
    
    public override void Receive(Argument argument)
    {
        ImpReceive(argument);
        float agreement = AppraiseAgreement(argument);
        switch (argument.MyType)
        {
            case Argument.Type.Humanism: HandleHumanism(argument, agreement); break;
            case Argument.Type.Idealism: HandleIdealism(argument, agreement); break;
            case Argument.Type.Pacifism: HandlePacifism(argument, agreement); break;
            case Argument.Type.Altruism: HandleAltruism(argument, agreement); break;
            case Argument.Type.Fatalism: HandleFatalism(argument, agreement); break;
            case Argument.Type.Futurism: HandleFuturism(argument, agreement); break;
            case Argument.Type.Tribalism: HandleTribalism(argument, agreement); break;
            case Argument.Type.Authoritarianism: HandleAuthoritarianism(argument, agreement); break;
            case Argument.Type.Militarism: HandleMilitarism(argument, agreement); break;
            case Argument.Type.Utilitarianism: HandleUtilitarianism(argument, agreement); break;
            case Argument.Type.Superiority: HandleSuperiority(argument, agreement); break;
        }
    }

    private float AppraiseAgreement(Argument argument)
    {
        return Beliefs[argument.MyType];
    }
    
    private void HandleHumanism(Argument argument, float agreement)
    {
        HandleAnything(0.25f, 0.3f, -0.25f, agreement, argument.Strength);
    }
    
    private void HandleIdealism(Argument argument, float agreement)
    {
        HandleAnything(-0.25f * agreement, 0.25f * agreement, agreement, agreement, argument.Strength);
    }

    private void HandlePacifism(Argument argument, float agreement)
    {
        HandleAnything(0.5f * agreement, 0.1f, 0.5f * agreement, agreement, argument.Strength);
    }

    private void HandleAltruism(Argument argument, float agreement)
    {
        HandleAnything(0f, 0.2f, 0.5f * agreement, agreement, argument.Strength);
    }

    private void HandleFatalism(Argument argument, float agreement)
    {
        HandleAnything(0f, -1f, 0f, agreement, argument.Strength);
    }

    private void HandleFuturism(Argument argument, float agreement)
    {
        HandleAnything(0.1f, 0f, 0.5f * agreement, agreement, argument.Strength);
    }

    private void HandleTribalism(Argument argument, float agreement)
    {
        HandleAnything(0.5f * agreement, 0.5f * agreement, 0f, agreement, argument.Strength);
    }

    private void HandleAuthoritarianism(Argument argument, float agreement)
    {
        HandleAnything(0.5f * agreement, 1f * agreement, 0f, agreement, argument.Strength);
    }

    private void HandleMilitarism(Argument argument, float agreement)
    {
        HandleAnything(0.5f, 0.1f * agreement, 0.5f * agreement, agreement, argument.Strength);
    }

    private void HandleUtilitarianism(Argument argument, float agreement)
    {
        HandleAnything(0f, 0.5f * agreement, 0f, agreement, argument.Strength);
    }
    
    private void HandleSuperiority(Argument argument, float agreement)
    {
        HandleAnything(0.2f, 0.1f * agreement, -0.2f, agreement, argument.Strength);
    }

    private void HandleAnything(float angerMod, float fearMod, float egoMod, float respectMod, float argumentStrength)
    {
        float angerDelta = 0f;
        float fearDelta = 0f;
        float egoDelta = 0f;
        float respectDelta = 0f;

        angerDelta += argumentStrength * angerMod;
        fearDelta += argumentStrength * fearMod;
        egoDelta += argumentStrength * egoMod;
        respectDelta += argumentStrength * respectMod;
        
        angerDelta = ModifyAnger(angerDelta);
        fearDelta = ModifyFear(fearDelta);
        egoDelta = ModifyEgo(egoDelta);
        respectDelta = ModifyRespect(respectDelta);
        
        ChangeState(Emotion.Type.Anger, angerDelta);
        ChangeState(Emotion.Type.Fear, fearDelta);
        ChangeState(Emotion.Type.Ego, egoDelta);
        ChangeState(Emotion.Type.Respect, respectDelta);
    }

    private float ModifyAnger(float angerDelta)
    {
        float A = 0.25f * States[Emotion.Type.Anger] / 100f;
        float F = 1.0f * States[Emotion.Type.Fear] / 100f;
        float E = 0.75f * States[Emotion.Type.Ego] / 100f;
        float R = 0.5f * -States[Emotion.Type.Respect] / 100f;
        float delta = angerDelta * (1f + A + F + E + R);
        delta = Mathf.Clamp(States[Emotion.Type.Anger] + delta, 0f, 100f) - States[Emotion.Type.Anger];
        return delta;
    }

    private float ModifyFear(float fearDelta)
    {
        float A = 0.3f * States[Emotion.Type.Anger] / 100f;
        float F = 0.8f * States[Emotion.Type.Fear] / 100f;
        float E = 0.1f * -States[Emotion.Type.Ego] / 100f;
        float R = 0.1f * States[Emotion.Type.Respect] / 100f;
        float delta = fearDelta * (1f + A + F + E + R);
        delta = Mathf.Clamp(States[Emotion.Type.Fear] + delta, 0f, 100f) - States[Emotion.Type.Fear];
        return delta;
    }

    private float ModifyEgo(float egoDelta)
    {
        float A = 0f;
        float F = 0.1f * -States[Emotion.Type.Fear] / 100f;
        float E = 0f;
        float R = 0.5f * States[Emotion.Type.Respect] / 100f;
        float delta = egoDelta * (1f + A + F + E + R);
        delta = Mathf.Clamp(States[Emotion.Type.Ego] + delta, 0f, 100f) - States[Emotion.Type.Ego];
        return delta;
    }

    private float ModifyRespect(float respectDelta)
    {
        float A = 0.5f * -States[Emotion.Type.Anger] / 100f;
        float F = 0.25f * States[Emotion.Type.Fear] / 100f;
        float E = 0.5f * States[Emotion.Type.Ego] / 100f;
        float R = 0.25f * -States[Emotion.Type.Respect] / 100f;
        float delta = respectDelta * (1f + A + F + E + R);
        delta = Mathf.Clamp(States[Emotion.Type.Respect] + delta, 0f, 100f) - States[Emotion.Type.Respect];
        return delta;
    }

    private void IncreaseBelief(Argument.Type argument, float mod) => ModifyBelief(argument, mod);
    private void DecreaseBelief(Argument.Type argument, float mod) => ModifyBelief(argument, -mod);
    private void ModifyBelief(Argument.Type argument, float mod)
    {
        Beliefs[argument] = Mathf.Clamp(Beliefs[argument] + 0.05f * mod, -1.5f, 1.5f);
    }
}