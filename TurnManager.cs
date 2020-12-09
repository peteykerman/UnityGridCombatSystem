using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurnAgent
{    
    public int combatID;
    public int turnPoints;
    public int turnSpeed;
    public GridCombatant combatant;
    public TurnAgent(int combatID)
    {
        this.combatID = combatID;
        combatant = GridCombatant.combatants[combatID];
        turnSpeed = 100 - combatant.Stats.speed;
        turnPoints = turnSpeed;
       
    }
}

public class TurnManager : MonoBehaviour
{
    public delegate void OnNextTurn();
    public static OnNextTurn onNextTurn;
    public static TurnAgent currentTurn;
    public static TurnManager tm;
    public static bool battleStarted = false;
    List<TurnAgent> agentTurnCounters;
    //List<TurnAgent> sortedTurnOrder;
    
    public List<KeyValuePair<TurnAgent, int>> turnList;

    public void OnCombatantListChanged()
    {
        CreateTurnCounters();
        turnList = BuildTurnList();        
    }

    void CreateTurnCounters()
    {
        int count = GridCombatant.combatants.Count;
        agentTurnCounters = new List<TurnAgent>(count);
        for (int i = 0; i < count; i++)
        {
            agentTurnCounters.Add(new TurnAgent(i));
        }
    }
    //private void OnGUI()
    //{
    //    if(currentTurn != null)
    //    {
    //        GUI.Label(new Rect(100, 10, 100, 100), currentTurn.combatant.combatantName);
    //    }
    //    if (turnList != null)
    //    {
    //        int size = 20;
    //        int count = turnList.Count;
    //        for (int i = 0; i < count; i++)
    //        {
    //            GUI.Label(new Rect(10, (size * i), 100, (size)), turnList[i].Key.combatant.combatantName);

    //        }

    //    }
    //    else turnList = BuildTurnList();

        
    //}

    private void Start()
    {
        tm = this;        
    }

    public void StartBattle()
    {
        battleStarted = true;
        NextTurn();
    }

    //public static int CompareAgentsForTurn(TurnAgent a, TurnAgent b)
    //{
    //    int difference = a.turnPoints.CompareTo(b.turnPoints);
    //    if (difference == 0 && a.combatID != b.combatID)
    //    {
            
    //        difference = a.turnSpeed.CompareTo(b.turnSpeed);
    //        if (difference == 0)
    //        {
    //            difference = a.combatant.character.characterName.GetHashCode().CompareTo(b.combatant.character.characterName.GetHashCode());
    //        }
    //    }
        
    //    return difference;
    //}
    
    private void Initialize()
    {
        
    }
    public static int CompareTurnKeyValues(KeyValuePair<TurnAgent, int> first, KeyValuePair<TurnAgent, int> next)
    {
        int difference = first.Value.CompareTo(next.Value);
        if(difference == 0)
        {
            difference = first.Key.turnSpeed.CompareTo(next.Key.turnSpeed);
        }
        if (difference == 0)
        {
            difference = first.Key.combatID.CompareTo(next.Key.combatID);
            if (difference != 0)
            {
                difference = first.Key.combatant.character.characterName.GetHashCode().CompareTo(next.Key.combatant.character.characterName.GetHashCode());
            }
        }
        
        return difference;
    }
    List<KeyValuePair<TurnAgent, int>> BuildTurnList()
    {
        if (agentTurnCounters == null) CreateTurnCounters();

        List<KeyValuePair<TurnAgent, int>> turnList = new List<KeyValuePair<TurnAgent, int>>();
        int max = 200;
        for (int i = 0; i < agentTurnCounters.Count; i++)
        {
            if (agentTurnCounters[i].combatant.dead) continue;
            int w = 0;
            int turnW = agentTurnCounters[i].turnPoints + (agentTurnCounters[i].turnSpeed * w);
            while (turnW < max)
            {

                turnList.Add(new KeyValuePair<TurnAgent, int>(agentTurnCounters[i], turnW));
                w++;
                turnW = agentTurnCounters[i].turnPoints + (agentTurnCounters[i].turnSpeed * w);
            }
        }
        turnList.Sort(CompareTurnKeyValues);
        return turnList;
    }
    List<KeyValuePair<TurnAgent, int>> BuildTurnList(List<TurnAgent> agents)
    {
        List<KeyValuePair<TurnAgent, int>> turnList = new List<KeyValuePair<TurnAgent, int>>();
        int max = 100;
        for (int i = 0; i < agents.Count; i++)
        {
            int w = 0;
            int turnW = agents[i].turnPoints + (agents[i].turnSpeed * w);
            while (turnW < max)
            {

                turnList.Add(new KeyValuePair<TurnAgent, int>(agents[i], turnW));
                w++;
                turnW = agents[i].turnPoints + (agents[i].turnSpeed * w);
                if(w > 100)
                {
                    Debug.Log("w > 100, breaking");
                    break;
                }
            }
        }
        turnList.Sort(CompareTurnKeyValues);
        return turnList;
    }
    public void NextTurn()
    {
        
        if (currentTurn != null) currentTurn.turnPoints = currentTurn.turnSpeed;


        turnList = BuildTurnList();
        int min = turnList[0].Value;

        int count = agentTurnCounters.Count;
        for (int i = 0; i < count; i++)
        {
            agentTurnCounters[i].turnPoints -= min;
        }
        currentTurn = turnList[0].Key;
        turnList = BuildTurnList();

        if (onNextTurn != null) onNextTurn();
        ////original method
        //GridAgent[] agents = GridMaster.agents.ToArray();
        //if(currentTurn != null)currentTurn.turnPoints = currentTurn.turnSpeed;
        //int min = int.MaxValue;
        //int next = -1;
        //for (int i = 0; i < agents.Length; i++)
        //{
        //    if(agents[i].turnPoints <= min)
        //    {
        //        if (agents[i].turnPoints == min)
        //        {
        //            //do something for the tie
        //        }
        //        next = i;
        //        min = agents[i].turnPoints;
        //    }

        //}
        //for (int u = 0; u < agents.Length; u++)
        //{
        //    agents[u].turnPoints -= min;
        //}
        //currentTurn = agents[next];
    }



    /// Anonymous method delegate
    ///turns.Sort(delegate (KeyValuePair<string, int> first, KeyValuePair<string, int> next) { return first.Value.CompareTo(next.Value); });


}
