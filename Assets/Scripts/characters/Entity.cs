using UnityEngine;


public abstract class Entity : MonoBehaviour
{
    private static readonly Vector3 FlipRotation = new Vector3(0, 180, 0);

    public Vector3 ForwardDirection { get => transform.TransformDirection(Vector3.forward); }
    public Vector3 Position { get => transform.position; }
    public bool Grounded { get; protected set; }
    public bool Flipped { get; protected set; }
    protected LayerMask WhatIsGround { get; private set; }
    protected Rigidbody Rb { get; set; }



    protected virtual void Awake()
    {
        Rb = GetComponent<Rigidbody>();
    }

    protected virtual void Start()
    {
        Grounded = false;
        Flipped = false;

        WhatIsGround = LayerMask.GetMask("Floor");
    }

    protected virtual void Update() { }

    

    protected void Flip(float horizontal)
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
}