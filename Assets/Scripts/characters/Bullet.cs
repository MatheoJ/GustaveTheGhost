using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float BulletForce = 20f;
    [SerializeField] private float BulletHitDamage = 10f;

    public GameObject Shooter { get; private set; }

    public static Bullet Create(GameObject shooter, Vector3 position, Vector3 direction)
    {
        // rotation is aligned with direction 
        Quaternion quaternion = Quaternion.LookRotation(direction);
        quaternion *= Quaternion.Euler(90, 0, 0);

        GameObject asset = Resources.Load<GameObject>("Prefabs/Bullet");
        GameObject o = Instantiate(asset, position, quaternion);
        Rigidbody rb = o.GetComponent<Rigidbody>();
        Bullet bullet = o.GetComponent<Bullet>();

        bullet.Shooter = shooter;        

        rb.AddForce(direction.normalized * bullet.BulletForce, ForceMode.Impulse);

        return bullet;
    }

    private void OnTriggerEnter(Collider collision)
    {
        GameObject target = collision.gameObject;
        if (target == Shooter) return;

        if (target.layer == LayerMask.NameToLayer("Floor"))
        {
            Destroy(gameObject);
            return;
        }
        
        if(target.layer == LayerMask.NameToLayer("Character"))
        {

            int damage = Mathf.FloorToInt(BulletHitDamage);

            AbstractCharacter character = target.GetComponent<AbstractCharacter>();
            character.TakeDamage(damage);

            /*print(character.name + " took " + BulletHitDamage + " damage");
            print(character.name + " has " + character.CurrentHP + " hp left");*/

            Destroy(gameObject);
            return;
        }

        if (target.layer == LayerMask.NameToLayer("Chest"))
        {
            // target parent is BoxOfPandora
            target.transform.parent.GetComponent<BoxScript>().HandleCollision();
            Destroy(gameObject);
        }
    }
}
