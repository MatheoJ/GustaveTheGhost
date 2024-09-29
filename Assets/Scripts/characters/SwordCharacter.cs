using System.Collections;
using UnityEngine;

public class SwordCharacter : AbstractCharacter
{
    private Sword swordScript;
    [SerializeField] private int SwordDamage = 10;

    public bool IsSwinging { get => swordScript.IsSwinging; }
    
    protected override void Awake()
    {
        base.Awake();

        swordScript = GetComponent<Sword>();
        swordScript.SwordDamage = SwordDamage;
    }

    protected override void Start() => base.Start();

    protected override void Update() => base.Update();


    public override void PlayDeathAnim()
    {
        _Anim.SetTrigger("SwordDeath");
    }

    public override void HandleHit(Vector3 direction, int damage, string weapon="gun")
    {
        base.HandleHit(direction, damage);
        TakeDamage(damage);
        AudioManager.Instance.PlaySound("scream", 0.07f);
        
    }

    public override void OnAttack(Vector3 direction)
    {
        if(swordScript != null)
        {
            StartCoroutine(swordScript.Swing());
        }
        _Anim.SetTrigger("Swing");
        AudioManager.Instance.PlaySound("swing", 0.1f);

    }
}