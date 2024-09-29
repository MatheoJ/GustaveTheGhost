using System.Collections;
using UnityEngine;

public class ShieldCharacter : AbstractCharacter
{
    //[SerializeField]
    //private float recoilAmount = 0.5f;
    private Sword swordScript;
    [SerializeField] private int SwordDamage = 10;

    public bool IsSwinging { get => swordScript.IsSwinging; }
    protected override void Awake()
    {
        base.Awake();
        swordScript = GetComponent<Sword>();
        swordScript.SwordDamage = SwordDamage;
        //_Anim.SetBool("isSwordCharacter", true);
    }

    protected override void Start() => base.Start();

    protected override void Update() => base.Update();


    public override void PlayDeathAnim()
    {
        _Anim.SetTrigger("SwordDeath");
    }

    public override void OnAttack(Vector3 direction)
    {
        if(swordScript != null)
        {
            Debug.Log("Swing");
            StartCoroutine(swordScript.Swing());
        }
        _Anim.SetTrigger("Swing");
        AudioManager.Instance.PlaySound("swing", 0.1f);

    }

    public override void HandleHit(Vector3 direction, int damage, string weapon="gun")
    {
        base.HandleHit(direction, damage);
        if (weapon == "gun")
        {
            Vector3 shieldDirection = ForwardDirection;
            // dot product of shield direction and hit direction
            float dot = Vector3.Dot(shieldDirection, direction);

            // if dot is positive, hit is in front of shield
            if (dot > 0)
            {
                TakeDamage(damage);
                AudioManager.Instance.PlaySound("scream", 0.07f);

            }
            else
            {
                PlayShieldAnim();
                StartCoroutine(MoveBackward(direction.z / 2));
                AudioManager.Instance.PlaySound("shield", 0.5f);
            }
        }
        else
        {
            TakeDamage(damage);
            AudioManager.Instance.PlaySound("scream", 0.07f);
        }
    }

    private IEnumerator MoveBackward(float z_displacement)
    {
        // Désactive les contrôles
        IsControlDisabled = true;

        float duration = 0.5f;
        Vector3 startPosition = transform.position;
        float startTime = Time.time;

        // Check for collision
        BoxCollider box = GetComponent<BoxCollider>();
        Vector3 halfExtents = box.bounds.extents;
        Vector3 direction = new Vector3(0, 0, z_displacement > 0 ? 1 : -1);

        RaycastHit hit;
        bool hasHitSomething = Physics.BoxCast(box.bounds.center, halfExtents, direction, out hit, transform.rotation, Mathf.Abs(z_displacement));
        if (hasHitSomething)
        {
            float adjustedZ = hit.distance - halfExtents.z;
            adjustedZ *= Mathf.Sign(z_displacement); 
            z_displacement = adjustedZ;
        }

        Vector3 endPosition = startPosition + new Vector3(0, 0, z_displacement);

        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            transform.position = Vector3.Lerp(startPosition, endPosition, t);
            yield return null;
        }

        transform.position = endPosition;
        IsControlDisabled = false;
    }

}