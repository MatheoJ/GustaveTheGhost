using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SwordEnemyAI : EnemyAI
{
    protected override void OnAlert()
    {
        if(Character == null || SeenPlayer == null)
        {
            return;
        }
        // Walk towards the player, updating direction X seconds
        float period = 0.5f;
        float pursueSpeed = 0.8f;

        float time = Time.time;
        if (time % period <= Time.deltaTime)
        {
            float deltaZ = SeenPlayer.Position.z - Character.Position.z;
            float normalizedDeltaZ = pursueSpeed * deltaZ / Mathf.Abs(deltaZ);

            WalkingDirection = normalizedDeltaZ;
        }
        Vector3 direction = SeenPlayer.Position - Character.Position;
        if (direction.magnitude < 1.0f)
        {
            Character.Attack(direction);
        }
        
    }
}