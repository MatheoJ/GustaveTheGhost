using System.Collections;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public int SwordDamage { get; set;}
    public GameObject hitImpact;

    private AbstractCharacter Owner;

    public bool IsSwinging { get; set; } = false;

    private void Awake()
    {
        Owner = GetComponent<AbstractCharacter>();
    }
    private void Update()
    {
        if (IsSwinging)
        {
            SphereCast();
        }
    }
    public IEnumerator Swing()
    {
        IsSwinging = true;
        yield return new WaitForSeconds(2f);
        IsSwinging = false;
    }
    private void SphereCast()
    {
        //do a spherecast
        RaycastHit[] hits = Physics.SphereCastAll(transform.position+transform.forward*0.12f, 0.35f, transform.forward, 0f);
        // plot the spherecast
        Debug.DrawRay(transform.position, transform.forward * 1f, Color.red, 1f);
        Debug.Log("Number of hits: " + hits.Length);
        for (int i = 0; i < hits.Length; i++)
        {
            Debug.Log(hits[i].collider.gameObject.name);
            Collider hit = hits[i].collider;
            
            handleCollision(hit);
        }
    }
    private void handleCollision(Collider other)
    {
        if (!IsSwinging) return;
        
        GameObject target = other.gameObject;
        Debug.Log("1"+target.name);
        if (target == null) return;
        Debug.Log("2"+target.name);
        if (target == Owner) return;
        Debug.Log("3"+target.name);

        if (target.layer == LayerMask.NameToLayer("Character"))
        {
            Debug.Log("hit character");
            AbstractCharacter character = target.GetComponent<AbstractCharacter>();
            if (character == null) return;
            if (character.IsAlly(Owner)) return;

            if (hitImpact != null)
            {
                _ = Instantiate(hitImpact, transform.position, Quaternion.identity);
            }
            character.HandleHit(Owner.ForwardDirection, SwordDamage, "sword");
            IsSwinging = false;
        }

        if (target.layer == LayerMask.NameToLayer("Chest"))
        {
            if (hitImpact != null)
            {
                _ = Instantiate(hitImpact, transform.position, Quaternion.identity);
            }
            // target parent is BoxOfPandora
            BoxScript box = target.GetComponentInParent<BoxScript>();
            if (box == null) return;
            box.HandleCollision();
            IsSwinging = false;
        }
        
    }

}
