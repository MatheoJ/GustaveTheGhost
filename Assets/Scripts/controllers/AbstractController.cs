using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractController : MonoBehaviour
{
    public AbstractCharacter Character { get; protected set; }

    public bool isDead = false;

    protected virtual void Awake()
    {
        Character = GetComponentInChildren<AbstractCharacter>();

    }

    protected virtual void Start() { }

    protected virtual void Update() { }

    public virtual void OnFlip(Vector3 flipRotation, bool isReversed) { }

    public void updateCharacter()
    {
        Character = GetComponentInChildren<AbstractCharacter>();
    }
}