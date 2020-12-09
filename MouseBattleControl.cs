using UnityEngine;
using System.Collections;

public class MouseBattleControl : MonoBehaviour
{
    enum ControlState { Waiting, SelectingAction, SelectingMoveTarget, SelectingAbilityTarget}
    ControlState state;
    public Team controllingTeam;
    static GridCombatant currentTurn;
    public LayerMask clickable;
    public float raycastDistance = 25f;
    bool movedThisTurn;
    bool actionTakenThisTurn;
    bool waiting;

    Ability abilityToUse;

    void EndTurn()
    {
        TurnManager.tm.NextTurn();
    }
    public void NextTurn()
    {
        movedThisTurn = false;
        actionTakenThisTurn = false;
        currentTurn = TurnManager.currentTurn.combatant;
        GridCombatantAI ai = currentTurn.GetComponent<GridCombatantAI>();
        if(!ai)
        {
            StateChange(ControlState.SelectingMoveTarget);
            waiting = false;
        }
        else
        {
            waiting = true;
        }
    }

    void StateChange(ControlState newState)
    {
        //Remove effects of old state
        switch (state)
        {
            case ControlState.SelectingAbilityTarget:
                GridMaster.GM.tileController.ClearHighlighted();
                break;

            case ControlState.SelectingAction:
                break;

            case ControlState.SelectingMoveTarget:
                GridMaster.GM.tileController.ClearHighlighted();
                break;

            case ControlState.Waiting:
                break;

        }

        //change state
        state = newState;

        //Initiate effects of new state
        switch (state)
        {
            case ControlState.SelectingAbilityTarget:
                GridMaster.GM.tileController.HighlightAttackable(abilityToUse.AppliedRange(currentTurn.Position));
                break;

            case ControlState.SelectingAction:
                break;

            case ControlState.SelectingMoveTarget:
                GridMaster.GM.tileController.HighlightMoveableTiles(currentTurn.gridAgent.MoveMap);
                break;

            case ControlState.Waiting:
                break;

        }
    }
    
    void OnLeftClick(GridTile tile)
    {
        if (currentTurn == null) return;
        switch (state)
        {
            case ControlState.SelectingAbilityTarget:
                GridPoint[] attackable = abilityToUse.AppliedRange(currentTurn.Position);
                for (int i = 0; i < attackable.Length; i++)
                {
                    if(tile.position == attackable[i])
                    {
                        UseAbilityOnTile(abilityToUse, tile);
                        GridMaster.GM.tileController.ClearHighlighted();
                        EndTurn();
                    }
                }
                break;

            case ControlState.SelectingAction:
                abilityToUse = Input.GetKey(KeyCode.LeftShift) ? currentTurn.Abilities[1] : currentTurn.Abilities[0];
                StateChange(ControlState.SelectingAbilityTarget);
                break;

            case ControlState.SelectingMoveTarget:
                Coroutine moving;
                if (currentTurn.gridAgent.MoveTo(tile.position, out moving))
                {
                    GridMaster.GM.tileController.ClearHighlighted();
                    waiting = true;
                    StartCoroutine(WaitForMove(moving));
                }
                break;

            case ControlState.Waiting:
                break;
        }        
    }
    void Attack(GridTile tile)
    {
        int agentIndex = GridMaster.grid.GridIndex(tile.position);
        if (agentIndex == -1) return;

        currentTurn.UseAbility(Ability.Attack, GridCombatant.combatants[agentIndex]);
    }
    void UseAbilityOnTile(Ability ability, GridTile target)
    {
        GridCombat.UseAbilityOnTile(currentTurn, ability, target.position);
    }
    void TargetAbility()
    {

    }
    IEnumerator WaitForMove(Coroutine waitFor)
    {
        GridMaster.GM.tileController.ClearHighlighted();
        waiting = true;
        yield return waitFor;
        waiting = false;
        movedThisTurn = true;
        StateChange(ControlState.SelectingAction);
    }

    private void OnEnable()
    {
        TurnManager.onNextTurn += NextTurn;
        
    }
    private void OnDisable()
    {
        TurnManager.onNextTurn -= NextTurn;
    }
    void Update()
    {
        if(currentTurn == null)
        {
            if (Input.GetKeyDown(KeyCode.Space) && TurnManager.battleStarted == false)
            {
                TurnManager.tm.StartBattle();
            }
            return;
        }
        if (waiting) return;
        if (Input.GetMouseButtonDown(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            Vector3 screen = Input.mousePosition;
            screen.z = Camera.main.nearClipPlane;
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(screen), out hit, raycastDistance, clickable))
            {
                GridTile tile = hit.collider.GetComponentInParent<GridTile>();
                if (tile)
                {
                    OnLeftClick(tile);
                }
            }
        }

    }
}
