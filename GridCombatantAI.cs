using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(GridCombatant))]
public class GridCombatantAI : MonoBehaviour
{
    GridCombatant combatant;
    private void OnEnable()
    {
        combatant = GetComponent<GridCombatant>();
        TurnManager.onNextTurn += MyTurnCheck;
    }
    private void OnDisable()
    {
        TurnManager.onNextTurn -= MyTurnCheck;
    }
    void MyTurnCheck()
    {
        if(TurnManager.currentTurn.combatant == combatant)
        {
            StartCoroutine(ExecuteTurn());
        }
    }
    
    GridPoint SelectMoveTile()
    {
        GridPoint[] moveable = combatant.gridAgent.GetMoveable();

        Team other = combatant.team == Team.Team1 ? Team.Team2 : Team.Team1;
        GridCombatant[] otherTeam = GridCombatant.GetCombatants(other);
        GridCombatant[] sameTeam = GridCombatant.GetCombatants(combatant.team);

        List<TilePriority> priorites = TilePriority.ListFromGridPoints(moveable);
        //priorites.Add(new TilePriority(combatant.Position));

        for (int i = 0; i < priorites.Count; i++)
        {
            priorites[i].AssessTile(otherTeam, sameTeam);
            if (priorites[i].tile == combatant.Position) priorites[i].inversePriority -= 1;
        }
        priorites.Sort(TilePriority.SortLowToHigh);
        if (otherTeam.Length == 0) return priorites[priorites.Count - 1].tile;
        return priorites[0].tile;
    }
    IEnumerator ExecuteTurn()
    {
        GridPoint toMove = SelectMoveTile();
        Coroutine movement;
        combatant.gridAgent.MoveTo(toMove, out movement);
        yield return movement;

        GridCombatant[] attackable = combatant.AdjacentEnemies();
        if(attackable.Length == 0)
        {
            TurnManager.tm.NextTurn();
            yield break;
        }

        int toAttack = 0;
        int maxHealh = int.MinValue;
        for (int i = 0; i < attackable.Length; i++)
        {
            if(attackable[i].hp < maxHealh)
            {
                toAttack = i;
                maxHealh = attackable[i].hp;
            }
        }
        combatant.UseAbility(Ability.Attack, attackable[toAttack]);
        TurnManager.tm.NextTurn();
    }
}
public class TilePriority
{
    public GridPoint tile;
    public int distanceToNearestEnemy;
    public int distanceToNearestAlly;
    public int distanceToAllEnemies;
    public int distanceToAllAllies;
    public int inversePriority;

    public TilePriority(GridPoint point)
    {
        tile = point;
    }



    public void AssessTile(GridCombatant[] otherTeam, GridCombatant[] sameTeam)
    {
        bool enemyAdjacent = false;
        bool currentTile;
        //Distance to enemies
        int minEnemyDis = int.MaxValue;
        int totalEnemyDis = 0;
        for (int i = 0; i < otherTeam.Length; i++)
        {
            int dis = tile.DistanceTo(otherTeam[i].Position);
            totalEnemyDis += dis;
            if(dis < minEnemyDis)
            {
                minEnemyDis = dis;
                if (dis == 1) enemyAdjacent = true;
            }
        }
        

        //Distance to allies
        int minAllyDis = int.MaxValue;
        int totalAllyDis = 0;
        for (int i = 0; i < sameTeam.Length; i++)
        {
            int dis = tile.DistanceTo(sameTeam[i].Position);
            totalAllyDis += dis;
            if (dis < minAllyDis)
            {
                minAllyDis = dis;
            }
        }

        distanceToNearestEnemy = minEnemyDis;
        distanceToAllEnemies = totalEnemyDis;
        distanceToNearestAlly = minAllyDis;
        distanceToAllAllies = totalAllyDis;

        inversePriority = (int)(
                          (distanceToNearestAlly / 5)
                        + (distanceToAllAllies  / 10)
                        + (distanceToAllEnemies / 2)
                        + (distanceToNearestEnemy * 10)
                        );


        if (enemyAdjacent) inversePriority -= 20;
    }

    public static int SortHighToLow(TilePriority first, TilePriority next)
    {
        return next.inversePriority.CompareTo(first.inversePriority);
    }
    public static int SortLowToHigh(TilePriority first, TilePriority next)
    {
        return first.inversePriority.CompareTo(next.inversePriority);
    }
    public static TilePriority[] CreateFromGridPoints(GridPoint[] source)
    {
        TilePriority[] toReturn = new TilePriority[source.Length];
        for (int i = 0; i < source.Length; i++)
        {
            toReturn[i] = new TilePriority(source[i]);
        }
        return toReturn;
    }
    public static List<TilePriority> ListFromGridPoints(GridPoint[] source)
    {
        List<TilePriority> toReturn = new List<TilePriority>(source.Length);
        for (int i = 0; i < source.Length; i++)
        {
            toReturn.Add(new TilePriority(source[i]));
        }
        return toReturn;
    }
}
