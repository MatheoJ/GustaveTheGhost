using UnityEngine;

public class SwordEnemyAI : EnemyAI
{
    protected override void OnAlert()
    {
        base.OnAlert();
        if (Character == null || SeenPlayer == null)
        {
            return;
        }
        // Walk towards the player, updating direction X seconds
        float period = 0.5f;
        float pursueSpeed = 0.8f;

        float time = Time.time;
        if (time % period <= Time.deltaTime)
        {
            float deltaY = SeenPlayer.Position.y - Character.Position.y;
            float deltaZ = SeenPlayer.Position.z - Character.Position.z;
            float normalizedDeltaZ = pursueSpeed * deltaZ / Mathf.Abs(deltaZ);

            if (deltaY > 0.5f && Character.Grounded && deltaZ < 5f)
            {
                Character.Jump();
            }
            WalkingDirection = normalizedDeltaZ;
        }
        Vector3 direction = SeenPlayer.Position - Character.Position;
        if (direction.magnitude < 0.7f)
        {
            WalkingDirection = 0;
            Character.Attack(direction);
        }
        
    }
}