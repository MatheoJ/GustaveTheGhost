using UnityEngine;

public class Bullet : MonoBehaviour
{
    const string ASSET_PATH = "Prefabs/projectiles/Bullet";

    [SerializeField] private float BulletForce = 20f;
    [SerializeField] private float BulletHitDamage = 10f;

    private bool Collided { get; set; } = false;

    public GameObject Shooter { get; private set; }
    public Vector3 shootDirection { get; set; }
    public GameObject hitImpact;

    public static Bullet Create(GameObject shooter, Vector3 position, Vector3 direction)
    {
        // rotation is aligned with direction 
        Quaternion quaternion = Quaternion.LookRotation(direction);
        quaternion *= Quaternion.Euler(90, 0, 0);

        GameObject asset = Resources.Load<GameObject>(ASSET_PATH);
        GameObject o = Instantiate(asset, position, quaternion);
        Rigidbody rb = o.GetComponent<Rigidbody>();
        Bullet bullet = o.GetComponent<Bullet>();

        bullet.Shooter = shooter;        
        bullet.shootDirection = direction.normalized;

        rb.AddForce(direction.normalized * bullet.BulletForce, ForceMode.Impulse);

        return bullet;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (Collided) return;

        if(hitImpact != null)
        {
            GameObject impact = Instantiate(hitImpact, transform.position, Quaternion.identity);
        }

        GameObject target = collision.gameObject;
        if (target == Shooter) return;

        if (target.layer == LayerMask.NameToLayer("Floor") || target.layer == LayerMask.NameToLayer("Default"))
        {
            Destroy(gameObject);
            AudioManager.Instance.PlaySound("bulletImpact", 1.0f);
            Collided = true;
            return;
        }
        
        if(target.layer == LayerMask.NameToLayer("Character"))
        {

            int damage = Mathf.FloorToInt(BulletHitDamage);

            AbstractCharacter character = target.GetComponent<AbstractCharacter>();
            character.HandleHit(shootDirection, damage);

            Destroy(gameObject);

            Collided = true;
            return;
        }

        if (target.layer == LayerMask.NameToLayer("Chest"))
        {
            // target parent is BoxOfPandora
            target.transform.parent.GetComponent<BoxScript>().HandleCollision();

            Destroy(gameObject);
            Collided = true;
            return;
        }
    }
}
