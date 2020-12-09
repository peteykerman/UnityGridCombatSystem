using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void VoidEvent();
public delegate void IntEvent(int value);
public enum Team { Team1, Team2}
[RequireComponent(typeof(GridAgent))]
public class GridCombatant : MonoBehaviour {


    public IntEvent onTakeDamage;
    public static List<GridCombatant> combatants = new List<GridCombatant>();
    public GridAgent gridAgent;
    public Character character;
    public Stats Stats { get { return character.FinalStats; } }
    public CharacterAbilityList Abilities { get { return character.abilities; } }
    public Team team;
    public int combatID;
    public int hp;
    public bool dead;
    
    public GridPoint Position { get { return gridAgent.position; } }

    #region Unity Functions
    private void OnEnable()
    {
        
        combatants.Add(this);
        combatID = combatants.IndexOf(this);
        gridAgent = GetComponent<GridAgent>();
        character = new Character();
        character.abilities = CharacterAbilityList.Basic;
        hp = Stats.maxHP;
    }
    private void OnDisable()
    {
        combatants.Remove(this);
    }
    private void Start()
    {
        

    }
    #endregion

    public void TakeDamage(int damage)
    {
        hp -= damage;
        if (onTakeDamage != null) onTakeDamage(damage);
        if(hp <= 0)
        {
            hp = 0;
            if (!dead)
            {
                dead = true;
                GridMaster.grid.gridIndex[Position.x, Position.y] = -1;
                GridMaster.grid.obstacleMap[Position.x, Position.y] = false;
                MeshRenderer[] mesh = GetComponentsInChildren<MeshRenderer>();
                for (int i = 0; i < mesh.Length; i++)
                {
                    mesh[i].enabled = false;
                }
            }
        }
    }
    public void TakeHealing(int healing)
    {
        hp += healing;
        if (hp > Stats.maxHP) hp = Stats.maxHP;
    }

    public void UseAbility(Ability toUse, GridCombatant target)
    {
        
        string y = " , ";
        Debug.Log(toUse.abilityName);
        Debug.Log(Stats.attack + y + target.Stats.defense + y + target.hp);
        GridCombat.UseAbilityOnCombatant(this, toUse, target);
    }

	void Initialize(Character character, Team team)
    {        
        this.character = character;
        hp = Stats.maxHP;
        gridAgent = GetComponent<GridAgent>();
    }
    public GridPoint[] AdjacentPoints()
    {
        return GridMaster.grid.CropToMap(gridAgent.position.GetCardinals());
    }
    public GridCombatant[] AdjacentCombatants()
    {
        GridPoint[] adjacent = AdjacentPoints();
        List<GridCombatant> adjacentCombatants = new List<GridCombatant>();
        for (int i = 0; i < adjacent.Length; i++)
        {
            int index = GridMaster.grid.GridIndex(adjacent[i]);
            if (index >= 0)
            {
                adjacentCombatants.Add(combatants[index]);
            }
        }
        return adjacentCombatants.ToArray();
    }
    public GridCombatant[] AdjacentEnemies()
    {
        GridPoint[] adjacent = AdjacentPoints();
        List<GridCombatant> adjacentCombatants = new List<GridCombatant>();
        for (int i = 0; i < adjacent.Length; i++)
        {
            int index = GridMaster.grid.GridIndex(adjacent[i]);
            if (index >= 0)
            {
                if(combatants[index].team != team)
                    adjacentCombatants.Add(combatants[index]);
            }
        }
        return adjacentCombatants.ToArray();
    }
    public static GridCombatant[] GetCombatants(Team team)
    {
        List<GridCombatant> toReturn = new List<GridCombatant>(combatants);
        for (int i = toReturn.Count - 1; i >= 0; i--)
        {
            if (toReturn[i].team != team || toReturn[i].dead) toReturn.RemoveAt(i);
        }
        return toReturn.ToArray();
    }
    public static GridCombatant[] GetAllCombatants()
    {
        return combatants.ToArray();
    }
}
