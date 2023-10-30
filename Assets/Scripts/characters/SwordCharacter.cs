using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class SwordCharacter : AbstractCharacter
{
    [SerializeField]
    private GameObject Sword;
    
    private Sword swordScript;
    protected override void Awake()
    {
        base.Awake();
        swordScript = Sword.GetComponent<Sword>();
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
        if(Sword != null)
        {
            StartCoroutine(SwingSword());
        }
        _Anim.SetTrigger("Swing");
    }

    IEnumerator SwingSword()
    {
        swordScript.setIsSwinging(true);
        yield return new WaitForSeconds(0.5f);
        swordScript.setIsSwinging(false);
    }
}