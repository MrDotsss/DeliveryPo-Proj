using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Interfaces are like types that can detect or can find for other players to prevent mismatch and avoid confusion.
// Tagalog nalang, ito yung hahanapin ng ibang script para makapag connect yung magkaibang entitityy sa isa't isa
// Look for Hitbox and Hurtbox script for sample implementation ng pagdedetect ng "ables"
public interface IDamageable
{
    public void TakeDamage(float amount, Vector3 direction);
    //amount is kung ilan yung ibabawas sa target (positive dapat)
    //direction is kung saan galing yung damage "for effects and hit UI direction"
}

//if may knockback nilagay ko nalang for future purposes
public interface IKnockbackable
{
    public void ApplyKnockback(float force, Vector3 direction);
    //force is kung gaano kalakas ang pwersa sa target
    //direction is kung saan papunta yung force (you can negate the direction para pahigop)
}

public interface ITalkable
{
    // to be implemented (TBI)
    //public void Talk(//scriptable object here);
    //Depends sa dialogue system na maiimplement pero sure ang scriptable object para madali mainput nila writer yung lines ng NPC.
}

public interface IInteractable
{
    //TBI
    //public void Interact(//data here)
    //anything interactable from doors to buttons, physical object ket anu basta...
}

// as the name implies
public interface IPickable
{
    //TBI
    //public void PickUp(//data)
    //public void Throw(//data)
    //pubic void Drop()
}