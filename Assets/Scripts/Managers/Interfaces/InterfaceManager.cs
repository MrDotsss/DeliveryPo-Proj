
using UnityEngine;


public interface IInteractable
{
    void Interact();
    string GetInteractionText { get; } // Returns text like "Talk", "Pick Up", "Deliver"
}

public interface INPComponent
{
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
