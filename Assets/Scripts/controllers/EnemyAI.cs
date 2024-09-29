using UnityEngine;

public abstract class EnemyAI : AbstractController
{
    [SerializeField] bool aggressive = true;

    protected float SightDistance = 5.0f;
    private float m_currentSightDistance = 5.0f;

    public AbstractCharacter SeenPlayer { get; private set; }

    protected float WalkingDirection {  get; set; }
    float AlertCountdown = 0;

    public GameObject Lightnings;

    public float alertTime = 1.0f;

    public bool IsOnAlert => AlertCountdown > 0;

    private float m_timeBeforeJump;

    public void AttachToBody(AbstractCharacter character) => Character = character;

    protected override void Awake()
    {
        base.Awake();
        
        // get first child
        if  (Character != null)
            Lightnings = transform.GetChild(0).gameObject.transform.Find("Lightnings").gameObject;
    }

    protected override void Start()
    {
        base.Start();

        WalkingDirection = 0.5f;
        m_timeBeforeJump = Random.Range(1.0f, 30.0f);
    }

    protected override void Update()
    {
        base.Update();

        if (Character == null) return;
        if(IsDead) return;
        if (SeenPlayer)
        {
            OnAlert();
        }
        else 
        {
            if (Character.Grounded)
            {
                if (CheckForWall() || CheckForCliff())
                {
                    WalkingDirection = -WalkingDirection;
                }
            }

            /*if (m_timeBeforeJump > 0.0f)
            {
                m_timeBeforeJump -= Time.deltaTime;
            }
            else
            {
                Character.Jump();
                m_timeBeforeJump = Random.Range(2.0f, 10.0f);
            }*/
        }

        Character.Move(WalkingDirection);


    }

    void FixedUpdate()
    {
        if (Character == null) return;
        if (Lightnings == null)
        {
            Lightnings = transform.GetChild(0).gameObject.transform.Find("Lightnings").gameObject;
        }

        AbstractCharacter player = CheckForPlayer();
        if (player)
        {
            AlertCountdown = alertTime;
            SeenPlayer = player;
            Lightnings.SetActive(true);
        }
        else if (AlertCountdown > 0)
        {
            AlertCountdown -= Time.deltaTime;
            if (AlertCountdown <= 0)
            {
                AlertCountdown = 0;
                SeenPlayer = null;
                WalkingDirection = 0.5f;
                m_currentSightDistance = SightDistance;
                Lightnings.SetActive(false);
            }
        }
    }

    public override bool IsAlly(AbstractController other) => other is EnemyAI;

    protected virtual void OnAlert()
    {
        if (SeenPlayer.CurrentHP <= 0)
        {
            SeenPlayer = null;
            return;
        }
    }
    protected override void OnDeath()
    {
        Skills.GainPoints(1);
        FindObjectOfType<Game>().SkillPoints = Skills.Points;
        FindObjectOfType<Game>().CheckForVictory();
    }

    AbstractCharacter CheckForPlayer()
    {
        if (!aggressive) return null;

        int layerMask = LayerMask.GetMask("Character") | LayerMask.GetMask("Floor");
        float[] angles = { -30, -15, 0, 15, 30, 60 };

        AbstractCharacter seenPlayer = null;

        foreach (float angle in angles)
        {
            float dist = 0;
            GameObject obstacle = GetRaycast(layerMask, angle, ref dist, Character.EyePosition);
            if (obstacle == null) continue;

            PlayerController controller = obstacle.GetComponentInParent<PlayerController>();
            if (controller != null) seenPlayer = controller.Character;
        }

        return seenPlayer;
    }

    bool CheckForCliff()
    {
        bool cliff = false;

        int layerMask = LayerMask.GetMask("Floor") | LayerMask.GetMask("Default");
        float dist = 0;
        GameObject obstacle = GetRaycast(layerMask, 90, ref dist, Character.EyePosition + 1.0f * Character.ForwardDirection);

        if (obstacle == null || dist > 1f) cliff = true;

        return cliff;
    }

    bool CheckForWall()
    {
        bool wall = false;

        int layerMask = LayerMask.GetMask("Floor") | LayerMask.GetMask("Default") | LayerMask.GetMask("Chest");
        float dist = 0;
        GameObject obstacle = GetRaycast(layerMask, 0, ref dist, Character.EyePosition+0.5f*Vector3.down);

        if (obstacle != null && dist < 0.5f) wall = true;
        return wall;
    }   
    
    public override void OnHit(Vector3 direction)
    {
        // turn to face the direction of the hit with walking direction
        if (Vector3.Dot(Character.ForwardDirection, direction) > 0)
        {
            WalkingDirection = -WalkingDirection;
            m_currentSightDistance = 2*SightDistance;
        }
        
    }

    GameObject GetRaycast(int layerMask, float angleX, ref float dist, Vector3 startPosition)
    {
        if (Character.Flipped) angleX *= -1;
        Vector3 direction = Quaternion.Euler(angleX, 0, 0) * Character.ForwardDirection;

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(startPosition, direction, out hit, m_currentSightDistance, layerMask))
        {
            dist = hit.distance;
            Color color = hit.transform.gameObject.GetComponentInParent<PlayerController>() ? Color.red : Color.yellow;
            Debug.DrawRay(startPosition, direction * hit.distance, color);
            return hit.transform.gameObject;
        }
        else
        {
            Debug.DrawRay(startPosition, direction * m_currentSightDistance, Color.white);
            return null;
        }
    }

    public new void updateCharacter()
    {
        base.updateCharacter();
        if (Character == null) return;

        Lightnings = transform.GetChild(0).gameObject.transform.Find("Lightnings").gameObject;
    }
}