using UnityEngine;

public abstract class AbstractController : MonoBehaviour
{
    public AbstractCharacter Character { get; protected set; }

    public bool IsDead { get; protected set; }

    protected virtual void Awake()
    {
        Character = GetComponentInChildren<AbstractCharacter>();

    }

    protected virtual void Start() { }

    protected virtual void Update() { }

    public abstract bool IsAlly(AbstractController other);

    public virtual void Kill()
    {
        if (IsDead) return;

        IsDead = true;
        OnDeath();
        Destroy(gameObject, 1.0f);
    }

    public virtual void OnFlip(Vector3 flipRotation, bool isReversed) { }
    protected virtual void OnDeath() { }

    public abstract void OnHit(Vector3 direction);
    public void updateCharacter()
    {
        Character = GetComponentInChildren<AbstractCharacter>();
    }

   
}