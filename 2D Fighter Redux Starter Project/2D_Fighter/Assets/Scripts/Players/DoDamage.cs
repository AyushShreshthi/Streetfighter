using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoDamage : MonoBehaviour
{
    StateManager states;

    public HandleDamageColliders.DamageType damageType;

    private void Start()
    {
        states = GetComponentInParent<StateManager>();

    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponentInParent<StateManager>())
        {
            StateManager oState = other.GetComponentInParent<StateManager>();

            if (oState != states)
            {
                if (!oState.currentlyAttacking)
                {
                    int damage =2;
                    if (damageType == HandleDamageColliders.DamageType.heavy)
                    {
                        damage = 5;
                    }
                    oState.TakeDamage(damage, damageType);
                }
            }
        }
    }
    
}
