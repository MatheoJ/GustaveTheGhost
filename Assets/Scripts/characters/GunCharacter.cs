using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class GunCharacter : AbstractCharacter
{
    protected override void Awake()
    {
        base.Awake();
        _Anim.SetBool("IsGunCharacter", false);
    }

    protected override void Start() => base.Start();

    protected override void Update() => base.Update();

    public override void PlayDeathAnim()
    {
        print("Gun Death");
        _Anim.SetTrigger("GunDeath");
    }

    public override void OnAttack(Vector3 direction)
    {
        Vector3 position = EyePosition + 0.5f * ForwardDirection + 0.2f * Vector3.down;
        
        direction.x = 0;
        direction.z = (Flipped ? -1 : 1) * Mathf.Abs(direction.z);
        
        Bullet.Create(gameObject, position, direction.normalized);

        _Anim.SetTrigger("Shoot");
    }
}