using System.Collections;
using UnityEngine;

public abstract class AbstractCharacter : Entity
{
    // Déclaration des constantes
    private static readonly Vector3 FlipRotation = new Vector3(0, 180, 0);

    // Déclaration des variables
    protected Vector3 _EyePosition { get; set; }

    public int JumpCount {  get; private set; }
    public Vector3 EyePosition { get => transform.position + _EyePosition; }
    public bool CanAttack { get => Time.time >= _LastAttackTime + 1f / AttackSpeed; }
    public int CurrentHP { get => _CurrentHP; private set => _CurrentHP = value; }
    public int MaxHP { get => _MaxHP; }

    public int StartHP = 100;
    int _CurrentHP { get; set; }
    float _LastAttackTime { get; set; }

    private AbstractController Controller { get; set; }

    private bool _isControlDisabled;

    private bool _isChangingColor = false;

    public bool IsControlDisabled
    {
        get { return _isControlDisabled; }
        protected set { _isControlDisabled = value; }
    }

    public Animator _Anim { get; set; }

    [SerializeField] protected float AttackSpeed = 1.2f;
    [SerializeField] float MoveSpeed = 5.0f;
    [SerializeField] float JumpForce = 10f;

    [SerializeField] int _MaxHP = 100;



    // Dash
    bool _IsDashing = false; 
    float _LastDashTime = -1;


    [SerializeField]
    float DashDistance = 5.0f;  // Distance du dash
    [SerializeField]
    float DashSpeed = 50.0f;    // Vitesse du dash

    [SerializeField]
    float _DashCooldown = 1.0f;  // Temps de recharge du dash

    Vector3 _DashDirection;  // Direction du dash (-z ou z)
    float _DistanceDashed = 0;  // Combien de distance a été parcourue pendant le dash actuel

    [SerializeField]
    LayerMask _DashLayer;  // Layer des objets sur lesquels le dash peut être effectué

    private TrailRenderer _trail;

    private SkinnedMeshRenderer skinnedRenderer;

    // Awake se produit avait le Start. Il peut être bien de régler les références dans cette section.
    protected override void Awake()
    {
        base.Awake();

        Controller = GetComponentInParent<AbstractController>();
        if (Controller == null) throw new MissingReferenceException("Character must be a child of AbstractController.");

        _trail = transform.Find("TrailOrigin")?.GetComponent<TrailRenderer>();
        if (_trail) _trail.enabled = false;

        _Anim = GetComponent<Animator>();
        try
        {
            skinnedRenderer = transform.Find("F01").transform.Find("f01").GetComponent<SkinnedMeshRenderer>();
        }
        catch
        {
            skinnedRenderer = null;
        }
    
    }

    protected override void Start()
    {
        JumpCount = 0;
        Grounded = false;
        Flipped = false;

        base.Start();

        _CurrentHP = StartHP;
        _EyePosition = Vector3.up * 0.8f;
        _LastAttackTime = Time.time;
    }

    // Vérifie les entrées de commandes du joueur
    protected override void Update() {
        base.Update();
        HandleDash();
    }



    public bool IsAlly(AbstractController other) => other.IsAlly(Controller);
    public bool IsAlly(AbstractCharacter other) => other.IsAlly(Controller);

    public Vector3 GetForward() { return transform.TransformDirection(Vector3.forward); }
    public Vector3 GetEyePosition() { return transform.position + _EyePosition; }
    public Vector3 GetPosition() { return transform.position; }

    public void Heal(int amount) {
        StartCoroutine(FlashColor(Color.green));
        if (amount > 0)
        {
            CurrentHP = Mathf.Min(CurrentHP + amount, MaxHP);
        }
    }


    public void TakeDamage(int amount)
    {
        if (_IsDashing) return;
        if (CurrentHP == 0) return;
        StartCoroutine(FlashColor(Color.red));
        if ((CurrentHP = Mathf.Max(0, CurrentHP - amount)) == 0)
        {
            PlayDeathAnim();
            Controller.Kill();
        }
        else
        {
            if (!((this is ShieldCharacter shieldCharacter && shieldCharacter.IsSwinging) ||
                (this is SwordCharacter swordCharacter && swordCharacter.IsSwinging)))
            {
                PlayPainAnim();
            }
        }
    }
    IEnumerator FlashColor(Color color)
    {
        if (!_isChangingColor)
        {
            if (skinnedRenderer != null)
            {
                _isChangingColor = true;
                Color originalColor = skinnedRenderer.material.color;
                skinnedRenderer.material.color = color;
                yield return new WaitForSeconds(0.2f);
                skinnedRenderer.material.color = originalColor;
                _isChangingColor = false;
            }
        }
    }


    public void Attack(Vector3 direction)
    {
        if (!CanAttack) return;

        OnAttack(direction);
        _LastAttackTime = Time.time;
    }

    public void PlayPainAnim()
    {
        _Anim.SetTrigger("Pain");
    }

    public void PlayShieldAnim()
    {
        _Anim.SetTrigger("Shield");
    }

    public abstract void PlayDeathAnim();
    public abstract void OnAttack(Vector3 direction);
    public virtual void HandleHit(Vector3 direction, int damage, string weapon="gun")
    {
        if (Controller is PlayerController) return;
        Controller.OnHit(direction);
    }
    public void Move(float direction)
    {

        HorizontalMove(direction);
        Flip(direction);
    }

    // Gère le saut du personnage, ainsi que son animation de saut
    public void Jump()
    {
        Rb.velocity = new Vector3(Rb.velocity.x, 0, 0);
        Rb.AddForce(new Vector3(0, JumpForce, 0), ForceMode.Impulse);

        // If not grounded, the first jump is skipped.
        //   Without this line, the character could jump twice in the air if falling from a ledge.
        if (!Grounded && JumpCount == 0) JumpCount++;
        JumpCount++;

        _Anim.SetBool("Jump", true);
    }

    public void Dash()
    {
        if (Time.time - _LastDashTime < _DashCooldown || _IsDashing) return;

        Vector3 dashDirection = Flipped ? Vector3.back : Vector3.forward;
        BoxCollider box = GetComponent<BoxCollider>();
        Vector3 halfExtents = box.bounds.extents;

        // Vérifier les collisions
        bool isBlocked = Physics.CheckBox(box.bounds.center, halfExtents, transform.rotation, ~_DashLayer);

        if (isBlocked)
        {
            return;
        }

        _IsDashing = true;
        if (_trail)
        {
            _trail.enabled = true;
        }
        _DashDirection = dashDirection;
        _DistanceDashed = 0;
        _LastDashTime = Time.time;
        AudioManager.Instance.PlaySound("dash", 0.5f);
    }

    private void HandleDash()
    {
        if (_IsDashing)
        {
            float dashStep = DashSpeed * Time.deltaTime;

            // Check for collision 
            BoxCollider box = GetComponent<BoxCollider>();
            Vector3 halfExtents = box.bounds.extents;  // Demi-dimensions du collider
            RaycastHit hit;
            Vector3 adjustedOrigin = box.bounds.center - (_DashDirection * 0.1f);  // recule légèrement l'origine
            bool hasHitSomething = Physics.BoxCast(adjustedOrigin, halfExtents, _DashDirection, out hit, transform.rotation, dashStep + 0.1f);


            if (hasHitSomething)
            {
                if ((_DashLayer.value & (1 << hit.collider.gameObject.layer)) == 0)
                {
                    //Debug.Log("Not a dashable object");
                    _IsDashing = false;
                    if (_trail)
                    {
                        _trail.enabled = false;
                    }
                    return;
                }
            }

            transform.Translate(Vector3.forward * dashStep);
            _DistanceDashed += dashStep;

            if (_DistanceDashed >= DashDistance)
            {
                _IsDashing = false;
                if (_trail)
                {
                    _trail.enabled = false;
                }
                return;
            }
        }
    }


    // PRIVATE

    // Gère le mouvement horizontal
    void HorizontalMove(float direction)
    {
        if (Rb == null) return;

        if (direction < -1) direction = -1;
        if (direction > 1) direction = 1;

        float horizontal = MoveSpeed * direction;

        Rb.velocity = new Vector3(Rb.velocity.x, Rb.velocity.y, horizontal);
        _Anim.SetFloat("MoveSpeed", Mathf.Abs(horizontal));

        // if it is the player, play the walk sound
        if (Controller is PlayerController)
        {
            if (Grounded && Mathf.Abs(horizontal) > 0)
            {
                AudioManager.Instance.PlayWalk(0.3f);
            }
            else
            {
                AudioManager.Instance.StopWalk();
            }
        }
    }

    // Gère l'orientation du joueur et les ajustements de la camera
    void FlipCharacter(float horizontal)
    {
        if (horizontal < 0 && !Flipped)
        {
            Flipped = true;
            transform.Rotate(FlipRotation);
        }
        else if (horizontal > 0 && Flipped)
        {
            Flipped = false;
            transform.Rotate(-FlipRotation);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Weapon")){ 
            return;
        }

        // Insta death
        if (other.gameObject.layer == LayerMask.NameToLayer("Death"))
        {
            Debug.Log("Out of bounds");
            Controller.Kill();
            return;
        }

        // On s'assure de bien être en contact avec le sol
        if ((WhatIsGround & (1 << other.gameObject.layer)) == 0)
            return;

        print("OnTriggerEnter");

        if(!Grounded)
        {
            AudioManager.Instance.PlaySound("land", 0.1f);
        }
        // Évite une collision avec le plafond
        Rigidbody rb = GetComponent<Rigidbody>();
        Grounded = true;
        JumpCount = 0;
        _Anim.SetBool("Grounded", Grounded);

    }
    private void OnTriggerExit(Collider other)
    {
        // On s'assure de bien être en contact avec le sol
        if ((WhatIsGround & (1 << other.gameObject.layer)) == 0)
            return;

        if(Grounded)
        {
            AudioManager.Instance.PlaySound("jump", 0.1f);
        }

        Grounded = false;

        _Anim.SetBool("Grounded", false);
    }

    public void updateController()
    {
        Controller = GetComponentInParent<AbstractController>();
        if (Controller == null) throw new MissingReferenceException("Character must be a child of AbstractController.");
    }
}