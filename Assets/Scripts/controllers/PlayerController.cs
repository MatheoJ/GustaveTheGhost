using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerController : AbstractController
{
    // Déclaration des constantes
    private static readonly Vector3 CameraPosition = new Vector3(10, 1, 0);
    private static readonly Vector3 InverseCameraPosition = new Vector3(-10, 1, 0);
    private GameObject Lightnings;


    public bool initialCameraTour = true;
    private int currentTourPointIndex = 0;
    private EnemyAI[] enemies;

    Camera _MainCamera { get; set; }
    
    // Valeurs exposées
    [SerializeField]
    Transform Halo;

    [SerializeField]
    bool overrideSkills = false;

    [SerializeField]
    GameObject aimingDirectionArrow;

    protected override void Awake()
    {
        base.Awake();

        _MainCamera = Camera.main;
        Debug.Assert(_MainCamera != null, "Please set the MainCamera reference in PlayerController.");
        Debug.Assert(Halo != null, "Please set the Halo reference in PlayerController.");
        
    }

    protected override void Start() => base.Start();

    protected override void Update()
    {
        if (initialCameraTour)
        {
            PerformInitialCameraTour();
        }
        else
        {
            FollowPlayer();
            if (FindObjectOfType<SoulLauncher>().ShouldDrawArrow)
            {
                DrawAimingArrow();
            } else if(aimingDirectionArrow.activeSelf)
            {
                aimingDirectionArrow.SetActive(false);
            }
        }
    }
    private void PerformInitialCameraTour()
    {
        if (enemies == null)
        {
            // get all Enemy ai 
            enemies = FindObjectsOfType<EnemyAI>();
        }

        if (currentTourPointIndex < enemies.Length)
        {
            Vector3 targetPosition = enemies[currentTourPointIndex].transform.position + new Vector3(7,1,0);
            _MainCamera.transform.localPosition = Vector3.Lerp(_MainCamera.transform.localPosition, targetPosition, Time.deltaTime * 3f);
            if (Vector3.Distance(_MainCamera.transform.localPosition, targetPosition) < 0.1f)
            {
                currentTourPointIndex++;
            }
        }
        else
        {
            initialCameraTour = false;
        }

    }

    private void FollowPlayer() { 
        if (IsDead) return;
        if (Character != null)
        {
            if (!Character.IsControlDisabled)
            {
                CheckMove();
                CheckJump();
                CheckDash();
                CheckAttack();
            }

            Vector3 targetPosition = Character.transform.position + CameraPosition;
            _MainCamera.transform.localPosition = Vector3.Lerp(_MainCamera.transform.localPosition, targetPosition, Time.deltaTime * 3f);
            //Halo.localPosition = Character.Position + 0.4f * Vector3.down;
            Halo.position = Character.Position - 0.6f * Vector3.down;
        }
        else
        {
            Soul soul = gameObject.GetComponentInChildren<Soul>();
            if (soul)
            {
                Vector3 targetPosition = soul.transform.position + CameraPosition;
                _MainCamera.transform.localPosition = Vector3.Lerp(_MainCamera.transform.localPosition, targetPosition, Time.deltaTime * 3f);
            }
        }
    }

    public override bool IsAlly(AbstractController other) => other is PlayerController;

    public override void OnHit(Vector3 direction)
    {
        throw new System.NotImplementedException();
    }

    public void DetachBody() => Character = null;

    protected override void OnDeath()
    {
        Halo.gameObject.SetActive(false);

        FindObjectOfType<Game>().GameOver = true;
    }

    void CheckMove()
    {
        float horizontal = Input.GetAxis("Horizontal");

        Character.Move(horizontal);
    }

    void CheckJump()
    {
        if (!Input.GetButtonDown("Jump")) return;

        bool isGrounded = Character.Grounded;
        bool secondJumpAllowed = Skills.CanDoubleJump && Character.JumpCount <= 1;
        bool thirdJumpAllowed = Skills.CanTripleJump && Character.JumpCount <= 2;
        if (!isGrounded && !secondJumpAllowed && !thirdJumpAllowed) return;
        Character.Jump();
    }

    void CheckDash()
    {
        if (!overrideSkills)
        {
            if (!Skills.CanDash) return;
        }


        if (!Input.GetButtonDown("Dash")) return;

        Character.Dash();
    }

    void CheckAttack()
    {
        if (!Input.GetButtonDown("Fire1")) return;

        Vector3 position = Character.EyePosition + 0.5f * Character.ForwardDirection + 0.5f * Vector3.down;
        Vector3 direction = Utils.GetMouseWorldDirection(position);
        Character.Attack(direction);
    }

    public new void updateCharacter()
    {
        Character = GetComponentInChildren<AbstractCharacter>();
        
        if (Character == null)
        {
            Debug.Log("Character is null");
            //dont display halo
            Halo.gameObject.SetActive(false);
        }
        else
        {
            Halo.gameObject.SetActive(true);
            Color resetColor = Color.white;
            resetColor.a = 0.23f;
            Halo.transform.localScale = new Vector3(1, 1, 1);
            Halo.GetComponent<MeshRenderer>().material.SetColor("_TintColor", resetColor);

            Lightnings = FindChild(Character.transform, "Lightnings").gameObject;
            Lightnings.SetActive(false);
        }
    }

    public Transform GetHalo()
    {
        return Halo;
    }

    Transform FindChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name.Contains(name))
            {
                return child;
            }
        }
        return null;
    }

    public override void Kill()
    {
        if (IsDead) return;
        IsDead = true;
        OnDeath();
    }

    private void DrawAimingArrow()
    {
        if (Character != null && aimingDirectionArrow != null)
        {
            aimingDirectionArrow.SetActive(true);
            Vector3 direction = Utils.GetMouseWorldDirection(Character.transform.position + Vector3.up * 0.5f);
            aimingDirectionArrow.transform.position = Character.transform.position + 0.5f * direction + Vector3.up * 0.5f;
            aimingDirectionArrow.transform.rotation = Quaternion.LookRotation(direction);
            aimingDirectionArrow.transform.Rotate(00, 90, 0);
        }
        else if (Character == null && aimingDirectionArrow != null)
        {
            aimingDirectionArrow.SetActive(false);
        }
    }
}