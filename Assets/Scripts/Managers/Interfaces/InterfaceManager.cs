
using UnityEngine;


public interface IInteractable
{
    void Interact();
    string GetInteractionText();
}

public interface INPComponent
{
    BaseNPC Owner { get; }

    void Activate();
}

public interface IDamageable
{
    void TakeDamage(float damage);
}


public interface IEnemy
{
    void DetectPlayer(Vector3 playerPosition);
    void Attack();
}
