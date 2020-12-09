using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TargetMode { Enemy, Friendly, Self, All }
public enum AbilityType { Damage, Healing, Status }

public static class GridCombat
{
    static Grid grid = GridMaster.grid;
    static List<GridCombatant> combatants = GridCombatant.combatants;

    public static void UseAbilityOnTile(GridCombatant attacker, Ability ability, GridPoint target)
    {
        //show hit effects on all tiles hit

        GridCombatant[] combatantsHit = CombatantsInHitZone(ability, target);
        combatantsHit = FilterTargetsByAbilityMode(attacker, ability, combatantsHit);
        for (int i = 0; i < combatantsHit.Length; i++)
        {
            UseAbilityOnCombatant(attacker, ability, combatantsHit[i]);
        }

    }
    public static void UseAbilityOnCombatant(GridCombatant attacker, Ability ability, GridCombatant target)
    {
        ability.UseOnCombatant(attacker, target);
    }
    public static void UseAbilityOnCombatants(GridCombatant attacker, Ability ability, GridCombatant[] targets)
    {
        for (int i = 0; i < targets.Length; i++)
        {
            ability.UseOnCombatant(attacker, targets[i]);
        }
    }

    public static GridCombatant[] FilterTargetsByAbilityMode(GridCombatant attacker, Ability ability, GridCombatant[] targets)
    {
        Team teamToTarget;
        switch (ability.abilityTargetMode)
        {
            case TargetMode.Enemy:
                teamToTarget = attacker.team == Team.Team1 ? Team.Team2 : Team.Team1;
                break;

            case TargetMode.Friendly:
                teamToTarget = attacker.team == Team.Team1 ? Team.Team1 : Team.Team2;
                break;

            case TargetMode.Self:
                return new GridCombatant[] { attacker };

            case TargetMode.All:
                return targets;

            default: return targets;
        }
        List<GridCombatant> toReturn = new List<GridCombatant>(targets);
        
        for (int i = toReturn.Count - 1; i >= 0; i--)
        {
            if (toReturn[i].team != teamToTarget) toReturn.RemoveAt(i);
        }
        return toReturn.ToArray();
    }
    
    public static GridCombatant[] CombatantsInHitZone(Ability ability, GridPoint target)
    {
        if(ability.areaOfEffect == 0)
        {
            return CombatantsFromGridPoint(target);
        }
        else
        {
            GridPoint[] tilesHit = target.ExtendedToSize(ability.areaOfEffect);
            return CombatantsFromGridPoints(tilesHit);
        }
    }
    public static GridCombatant[] CombatantsInRange(Ability ability, GridPoint position)
    {
        GridPoint[] range = ability.AppliedRange(position);
        List<GridCombatant> inRange = new List<GridCombatant>();
        for (int i = 0; i < range.Length; i++)
        {
            int gridIndex = grid.GridIndex(range[i]);
            if (gridIndex > -1)
            {
                inRange.Add(combatants[gridIndex]);
            }
        }
        return inRange.ToArray();
    }

    public static GridCombatant CombatantFromGridPoint(GridPoint gridPoint)
    {
        int index = grid.GridIndex(gridPoint);
        if (index == -1) return null;
        else return combatants[index];
    }
    public static GridCombatant[] CombatantsFromGridPoints(GridPoint[] gridPoints)
    {
        List<GridCombatant> toReturn = new List<GridCombatant>();

        for (int i = 0; i < gridPoints.Length; i++)
        {
            int index = grid.GridIndex(gridPoints[i]);
            if (index >= 0) toReturn.Add(combatants[index]);
        }
        return toReturn.ToArray();
    }
    public static GridCombatant[] CombatantsFromGridPoint(GridPoint gridPoint)
    {
        int index = grid.GridIndex(gridPoint);
        if (index == -1) return new GridCombatant[0];
        else return new GridCombatant[] { combatants[index] };
        
    }
}
