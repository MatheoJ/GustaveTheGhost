using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class PlayerController : AbstractController
{
    // Déclaration des constantes
    private static readonly Vector3 CameraPosition = new Vector3(10, 1, 0);
    private static readonly Vector3 InverseCameraPosition = new Vector3(-10, 1, 0);
    private GameObject Lightnings;
    Camera _MainCamera { get; set; }
    
    // Valeurs exposées
    [SerializeField]
    Transform Halo;

    [SerializeField]
    bool overrideSkills = false;

    protected override void Awake()
    {
        base.Awake();

        _MainCamera = Camera.main;

        if (Halo == null) throw new MissingReferenceException("Please set the Halo reference in PlayerController.");
        
    }

    protected override void Start() => base.Start();

    protected override void Update()
    {
        if (!isDead)
        {           

            if (Character != null)
            {
                CheckMove();
                CheckJump();
                CheckDash();
                CheckAttack();
                _MainCamera.transform.localPosition = Character.Position + CameraPosition;
                Halo.localPosition = Character.Position + 0.4f * Vector3.down;
            }
            else
            {

                GameObject soul = null;
                if (gameObject != null)
                {
                    Soul soulScript = gameObject.GetComponentInChildren<Soul>();
                    if (soulScript != null)
                    {
                        soul = soulScript.gameObject;
                    }
                }
                if (soul != null)
                {
                    _MainCamera.transform.localPosition = soul.transform.position + CameraPosition;
                }
            }

        }

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
        if (!isGrounded && !secondJumpAllowed) return;

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
            //dont display halo
            Halo.gameObject.SetActive(false);
        }
        else
        {
            Halo.gameObject.SetActive(true);
            Color resetColor = Color.white;
            resetColor.a = 0.23f;
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
}