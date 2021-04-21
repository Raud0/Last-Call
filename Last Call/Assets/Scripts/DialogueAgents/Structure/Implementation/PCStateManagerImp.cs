using System.Collections.Generic;

public class PCStateManagerImp : StateManagerImp
{
    public override void Awake()
    {
        Beliefs = new Dictionary<Argument.Type, float>()
        {
            {Argument.Type.Humanism, 0.0f},
            {Argument.Type.Idealism, 0.0f},
            {Argument.Type.Pacifism, 0.0f},
            {Argument.Type.Altruism, 0.0f},
            {Argument.Type.Fatalism, 0.0f},
            {Argument.Type.Futurism, 0.0f},
            {Argument.Type.Tribalism, 0.0f},
            {Argument.Type.Authoritarianism, 0.0f},
            {Argument.Type.Militarism, 0.0f},
            {Argument.Type.Utilitarianism, 0.0f},
            {Argument.Type.Superiority, 0.0f}
        };
        States = new Dictionary<Emotion.Type, float>()
        {
            {Emotion.Type.Anger, 0f},
            {Emotion.Type.Fear, 0f},
            {Emotion.Type.Ego, 0f},
            {Emotion.Type.Respect, 0f}
        };
        InitialStates = new Dictionary<Emotion.Type, float>()
        {
            {Emotion.Type.Anger, 50f},
            {Emotion.Type.Fear, 50f},
            {Emotion.Type.Ego, 50f},
            {Emotion.Type.Respect, 50f}
        };
        base.Awake();
    }

    public override void ImpReceive(Argument argument)
    {
        return;
    }
}