using System;

[Serializable]
public class AttackState: State
{
    public override void OnEnter(StateController sc)
    {
        base.OnEnter(sc);
        // Start animation
    }

    public override void OnUpdate()
    {
        // Patrol for player
    }
}
