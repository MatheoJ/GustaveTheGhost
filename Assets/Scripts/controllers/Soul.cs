using UnityEngine;

public class Soul : Entity
{
    [SerializeField]
    private float minMoveSpeedSoul = 8f;

    [SerializeField]
    private float maxMoveSpeedSoul = 20f;

    [SerializeField]
    private float chargingtime = 3.0f;

    private float timeCharged = 0f;

    [SerializeField]
    private float gravityScale = 0.03f;

    private static float globalGravity = -9.81f;

    [SerializeField]
    private int maxNumberOfBounce = 10;
    private int numberOfBounce = 0;

    private bool hit;

    public PlayerController Owner { get; set; }

    public GameObject hitImpact;
    public GameObject deathImpact;

    protected override void Awake()
    {
        base.Awake();
        if (Skills.HasOneBounce) maxNumberOfBounce += 1;
        if (Skills.HasTwoBounce) maxNumberOfBounce += 2;

    }
    protected override void Start()
    {
        base.Start();

        FindObjectOfType<Game>().BounceCount = maxNumberOfBounce;
    }

    public GameObject characterLaunchFrom;

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (hit) return;

        Vector3 gravity = globalGravity * gravityScale * Vector3.up;
        Rb.AddForce(gravity, ForceMode.Acceleration);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Insta death
        if (other.gameObject.layer == LayerMask.NameToLayer("Death"))
        {
            Debug.Log("soul Out of bounds");
            Owner.Kill();
            return;
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("SoulCollision");

        string tagOfParent = collision.gameObject.transform.parent.tag;

        if (hitImpact != null) {
            GameObject impact = Instantiate(hitImpact, collision.GetContact(0).point, Quaternion.identity);
            impact.transform.localScale = new Vector3(2.0f, 2.0f, 2.0f);
        }

        if (collision.gameObject.tag == "SoulBouncer" || 
            (collision.gameObject.GetComponentInParent<EnemyAI>() != null 
                && collision.gameObject.GetComponentInParent<EnemyAI>().IsOnAlert))
        {
            numberOfBounce++;
            FindObjectOfType<Game>().BounceCount = maxNumberOfBounce - numberOfBounce;

            if (numberOfBounce > maxNumberOfBounce)
            {
                Debug.Log("soul max bounce");
                Owner.Kill();

                if (deathImpact != null)
                {

                    GameObject impact = Instantiate(deathImpact, collision.GetContact(0).point, Quaternion.identity);
                    impact.transform.Rotate(0, 90, 0);
                    impact.transform.localScale = new Vector3(4.0f, 4.0f, 4.0f);
                    AudioManager.Instance.PlaySound("soulExplosion", 0.5f);
                }

                Destroy(gameObject);

                FindObjectOfType<Game>().BounceCount = null;
            }
            AudioManager.Instance.PlaySound("bounce", 0.5f);

        }
        else if (tagOfParent == "Enemy" && (collision.gameObject != characterLaunchFrom || numberOfBounce != 0))
        {
            
            hit = true;
            GameObject characterEnemy = collision.gameObject;
            gameObject.transform.SetParent(null);
            SwapPlayerAndEnemy(characterEnemy);

            AudioManager.Instance.PlaySound("swap", 0.5f);
            Destroy(gameObject);
            FindObjectOfType<Game>().BounceCount = null;
        }
    }

    private void SwapPlayerAndEnemy( GameObject characterEnemy)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject enemy = characterEnemy.transform.parent.gameObject;

        characterEnemy.transform.SetParent(player.transform, true);

        player.GetComponentInChildren<PlayerController>().updateCharacter();
        player.GetComponentInChildren<SoulLauncher>().updateCharacter();

        characterEnemy.GetComponentInChildren<AbstractCharacter>().updateController();

        Destroy(enemy);
    } 

    public void SetDirection(Vector3 direction)
    {
        Vector3 directionNormalized = direction.normalized;
        float launchSpeed = Mathf.Lerp(minMoveSpeedSoul, maxMoveSpeedSoul, timeCharged / chargingtime);
        Rb.velocity = directionNormalized * launchSpeed;
        Debug.Log("launchSpeed" + launchSpeed);
    }

    public void SetTimeCharged(float timeCharged)
    {
        this.timeCharged = timeCharged;
    }
}
