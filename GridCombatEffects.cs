using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCombatEffects : MonoBehaviour {

    GridCombatant combatant;

    protected virtual void OnEnable () {
        combatant = GetComponentInParent<GridCombatant>();
        if (combatant == null) Destroy(this);
        combatant.onTakeDamage += OnDamageTaken;
    }
    protected virtual void OnDisable()
    {
        combatant.onTakeDamage -= OnDamageTaken;
    }

    protected virtual void OnDamageTaken(int damage)
    {
        
    }
}
