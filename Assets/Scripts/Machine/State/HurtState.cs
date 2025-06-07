using System;

[Serializable]
public class HurtState: State
{
    public override void OnEnter(StateController sc)
    {
        base.OnEnter(sc);
        // Start animation
    }

    public override void OnUpdate()
    {
    }
}

