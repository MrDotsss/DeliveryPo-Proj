using System;
using UnityEngine;

public abstract class BaseNPCComponent : MonoBehaviour
{
    protected BaseNPC Owner { get; private set; }

    public event Action<BaseNPCComponent> OnComponentFinished;

    public bool loop = false;

    public abstract void Activate();
    protected void FinishComponent()
    {
        if (loop) return;

        OnComponentFinished?.Invoke(this);
    }

    public void SetOwner(BaseNPC owner)
    {
        Owner = owner;
    } 
}
