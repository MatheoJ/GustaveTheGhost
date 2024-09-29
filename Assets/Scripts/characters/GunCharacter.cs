using UnityEngine;

public class GunCharacter : AbstractCharacter
{
    public
    GameObject muzzleFlash;

    protected override void Awake()
    {
        base.Awake();
        //_Anim.SetBool("IsGunCharacter", false);
    }

    protected override void Start() => base.Start();

    protected override void Update() => base.Update();

    public override void HandleHit(Vector3 direction, int damage, string weapon="gun")
    {
        base.HandleHit(direction, damage);
        TakeDamage(damage);
        AudioManager.Instance.PlaySound("scream", 0.07f);
    }
    public override void PlayDeathAnim()
    {
        _Anim.SetTrigger("GunDeath");
    }

    public override void OnAttack(Vector3 direction)
    {
 
        direction.x = 0;

        if (Vector3.Dot(ForwardDirection, direction) < 0)
        {
            Flip(ForwardDirection.z > 0 ? -1 : 1);
        }

        Vector3 position = EyePosition + 0.5f * ForwardDirection + 0.2f * Vector3.down;

        Bullet.Create(gameObject, position, direction.normalized);

        if (muzzleFlash != null)
        {
            Instantiate(muzzleFlash, position + 0.1f * direction.normalized, Quaternion.LookRotation(direction.normalized));
        }
        _Anim.SetTrigger("Shoot");
        AudioManager.Instance.PlaySound("gunshot", 0.5f);
    }
}