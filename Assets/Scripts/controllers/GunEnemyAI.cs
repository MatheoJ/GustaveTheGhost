using UnityEngine;

public class GunEnemyAI : EnemyAI
{
    protected override void OnAlert()
    {
        base.OnAlert();
        if(SeenPlayer == null) {
            return;
        }
        if (WalkingDirection != 0) WalkingDirection = 0;

        // Turn towards the player, updating direction X second
        float period = 0.5f;
        float pursueSpeed = 0.2f;

        float time = Time.time;
        if (time % period <= Time.deltaTime)
        {
            float deltaZ = SeenPlayer.Position.z - Character.Position.z;
            float normalizedDeltaZ = pursueSpeed * deltaZ / Mathf.Abs(deltaZ);

            WalkingDirection = normalizedDeltaZ;
        }
        Vector3 direction = SeenPlayer.Position - Character.Position;
        if (checkPlayerLos())
        {
            Character.Attack(direction);
        }
    }

    private bool checkPlayerLos()
    {
        Vector3 direction = SeenPlayer.Position - Character.Position;
        float distance = direction.magnitude;
        direction.Normalize();

        RaycastHit hit;
        if (Physics.Raycast(Character.EyePosition, direction, out hit, distance))
        {
            if (hit.collider.gameObject == SeenPlayer.gameObject)
            {
                return true;
            }
        }
        return false;
    }
}