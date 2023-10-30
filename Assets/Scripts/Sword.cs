using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{

    public GameObject player;
    public int SwordDamage = 10;

    private bool isSwinging = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isSwinging) return;

        GameObject target = other.gameObject;
        if (target != null)
        {
            if (target == player) return;
            
            if(target.layer == LayerMask.NameToLayer("Character"))
            {
                AbstractCharacter character = target.GetComponent<AbstractCharacter>();
                character.TakeDamage(SwordDamage);
            }

            if(target.layer == LayerMask.NameToLayer("Chest"))
            {
                // target parent is BoxOfPandora
                target.transform.parent.GetComponent<BoxScript>().HandleCollision();
            }
        }
    }

    public void setIsSwinging(bool isSwinging)
    {
        this.isSwinging = isSwinging;
    }

    public bool getIsSwinging()
    {
        return isSwinging;
    }
}
