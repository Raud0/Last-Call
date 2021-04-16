using System;
using System.Collections.Generic;

public class NPCStateManagerImp : StateManagerImp
{
    public override void Awake()
    {
        Beliefs = new Dictionary<Argument.Type, float>()
        {
            {Argument.Type.Humanism, 1.0f},
            {Argument.Type.Idealism, 0.9f},
            {Argument.Type.Pacifism, 0.9f},
            {Argument.Type.Altruism, 0.7f},
            {Argument.Type.Fatalism, 0.5f},
            {Argument.Type.Futurism, -0.5f},
            {Argument.Type.Tribalism, -0.6f},
            {Argument.Type.Authoritarianism, -0.8f},
            {Argument.Type.Militarism, -0.9f},
            {Argument.Type.Utilitarianism, -1.0f},
            {Argument.Type.Superiority, -0.7f}
        };
        States = new Dictionary<Emotion.Type, float>()
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