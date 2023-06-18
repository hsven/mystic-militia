using System.Collections;
using System.Collections.Generic;
using UnityEngine.Splines;
using UnityEngine.SceneManagement;
using UnityEngine;
using Unity.Mathematics;
using System.Linq;
using System;
using Map;
using UnityEditor;

#region Aux Classes

[Serializable]
public class PlayerSquad
{
    public String name = "Squad";

    public List<UnitController> units = new List<UnitController>();

    public Spline formation = new Spline();

    public bool isActive = false;
    public SquadFormationLineRenderer formationLineRenderer = null;

    public FormationRepr formationRepr = null;

    public Vector3 targetPos = Vector3.zero;
    public GameEnums.CommandTypes currentCommand;

    public PlayerSquad() {}

    public PlayerSquad(String name)
    {
        this.name = name;
    }
}

public class FormationRepr
{
    public Spline formation = new Spline();

    public SquadFormationLineRenderer formationLineRenderer = null;
    public List<PlayerSquad> playerSquads = new List<PlayerSquad>();

    public int activeSquadCount = 0;
    private int squadCount = 0;

    public void AttachSquad(PlayerSquad squad)
    {
        squadCount++;
        playerSquads.Add(squad);
    }

    public void AddActiveSquad()
    {
        activeSquadCount++;
        formationLineRenderer.lineRenderer.enabled = true;
    }

    public void DetachSquad(PlayerSquad squad) 
    { 
        squadCount--;
        playerSquads.Remove(squad);
        if (squadCount <= 0)
        {
            GameObject.Destroy(formationLineRenderer.gameObject);
        }
    }

    public void RemoveActiveSquad()
    {
        activeSquadCount--;
        if (activeSquadCount == 0) formationLineRenderer.lineRenderer.enabled = false;
    }

    public void SetNewFollow(Transform newTarget)
    {
        formationLineRenderer.SetFollowTarget(newTarget);
    }
}

#endregion

public class BattleManager : MonoBehaviour
{
    // public List<PlayerArmyInventory> armyInventory = new List<PlayerArmyInventory>();

    public static BattleManager Instance = null;
    public MapGenerator mapGenerator;
    public bool isPaused = false;
    public GameObject UIObj;

    public PlayerController player;
    public EnemyFormationManager enemyFormation;
    public GameObject unitPrefab;

    public List<UnitController> totalPlayerUnits = new List<UnitController>();
    public List<PlayerSquad> squads = new List<PlayerSquad>();
    public List<EnemyController> enemies = new List<EnemyController>();
    public Spline currentFormation;

    public int currentSquadSelection = -1;
    public int lastSquadCommanded = 0;

    //public LineRenderer repr;
    public SquadFormationLineRenderer squadFormationRender;


    void Awake() {
        BattleManager.Instance = this;

        if (currentFormation == null) currentFormation = GetComponentInChildren<Spline>();
    }

    private void Start()
    {
        PauseGame();
        if(mapGenerator != null){
            mapGenerator.GenerateMap();
             //TODO: Maybe redo how this initial spawns is configured
            var respawnLocs = GameObject.FindGameObjectsWithTag("Respawn");
            if (respawnLocs.Count() > 0)
            {
                player.transform.position = respawnLocs[0].transform.position;
            }

            var enemyRespawnLocs = GameObject.FindGameObjectsWithTag("EnemyRespawn");
            if(enemyRespawnLocs.Count() > 0)
            {
                enemyFormation.transform.position = enemyRespawnLocs[0].transform.position;
            }
        }
    }

    //Currently more of a start game
    public void ResumeGame() {
        //TODO: Revisit UI being mentioned here
        UIObj.SetActive(false);

        if(enemyFormation != null)
        {
            enemyFormation.SpawnFormation(PlayerInventory.Instance.battlesFought);
        }


        int squadCount = 0;

        //Individual Squad generation
        int counter = 1;
        foreach (var squad in PlayerInventory.Instance.playerSquads)
        {
            if(squad.unitEntries.Count == 0) continue;

            var newPlayerSquad = new PlayerSquad("Squad #" + counter++);
            
            foreach (var unit in squad.unitEntries)
            {
                for (int i = 0; i < unit.quantity; i++)
                {
                    var obj = Instantiate(unitPrefab).GetComponent<UnitController>();
                    obj.SetupUnitData(unit.unitData);
                    obj.transform.position = player.transform.position + UnityEngine.Random.insideUnitSphere * 2;
                    
                    newPlayerSquad.units.Add(obj);
                    totalPlayerUnits.Add(obj);
                    newPlayerSquad.units.Add(obj);
                    obj.Setup(totalPlayerUnits.Count - 1, new Vector2Int(squadCount, i));
                }
            }
            newPlayerSquad.formationLineRenderer = Instantiate(squadFormationRender.gameObject, transform).GetComponent<SquadFormationLineRenderer>();
            squads.Add(newPlayerSquad);

            if(UIOffScreenIndicatorManager.Instance) UIOffScreenIndicatorManager.Instance.SpawnSquadIndicator(newPlayerSquad.units.Select(x => x.transform).ToList(), ++squadCount);
        }

        UIBattleSquadSelector.Instance.SetupBattleSquadUI();

        Time.timeScale = 1;
        isPaused = false;
    }
    
    
    public void PauseGame() {
        Time.timeScale = 0;
        isPaused = true;
    }

    public void ResetGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public int RegisterPlayerUnit(UnitController unit) {
        totalPlayerUnits.Add(unit);
        return totalPlayerUnits.Count - 1;
    }

    public void DeleteEntity(EntityController entity)
    {
        UnitController unit = entity as UnitController;
        if (unit != null)
        {
            if (totalPlayerUnits.Contains(unit)) totalPlayerUnits.Remove(unit);
            
            Vector2Int squadIndex = GetSquadIndex(unit);
            if (squadIndex.x != -1 && squadIndex.y != -1)
            {
                PlayerSquad squad = squads[squadIndex.x];
                //squad.
                squad.units.RemoveAt(squadIndex.y);
                DynamicFormationAdjust(squad);
            }

            if(totalPlayerUnits.Count == 0)
            {
                Debug.Log("Defeat");
            }
            return;
        }
        EnemyController enemy = entity as EnemyController;
        if (enemy != null)
        {
            enemies.Remove(enemy);
            if(enemies.Count == 0)
            {
                Debug.Log("Victory!");
                if(MapPlayerTracker.Instance) MapPlayerTracker.Instance.returnToTree();
            }
        }
    }

    public Vector2Int GetSquadIndex(UnitController unit) {
        for (int i = 0; i < squads.Count; i++)
        {
            int pos = squads[i].units.FindIndex(0, x => x == unit);
            if (pos == -1) continue;

            return new Vector2Int(i, pos);
        }
        return new Vector2Int(-1, -1);
    }

    public List<Vector2> GetPlayerUnitPositions() {
        return totalPlayerUnits.Select(unit => new Vector2(unit.transform.position.x, unit.transform.position.y)).ToList();
    }

    public Vector2 GetUnitOffset(int unitCount, int unitIndex, Spline formationToApply)
    {
        if (formationToApply.Count == 0) return Vector2.zero;

        float formationPosStep = formationToApply.Count / unitCount;
        var length = formationToApply.GetCurveLength(0);
        var interval = length / unitCount;

        float3 finalPos = formationToApply.EvaluatePosition(interval * unitIndex);

        return new Vector3(finalPos.x, finalPos.y, 0);
    }

    public void DynamicFormationAdjust(PlayerSquad squad)
    {
        List<UnitController> affectedUnits = new List<UnitController>();
        foreach (var sqd in squad.formationRepr.playerSquads)
        {
            affectedUnits.AddRange(sqd.units);
        }

        //Filter dead units
        affectedUnits = affectedUnits.FindAll(x => x != null).ToList();

        if (affectedUnits.Count == 0) return;

        int counter = 0;
        foreach (var unit in affectedUnits)
        {
            unit.SetCommand(squad.currentCommand, squad.targetPos, GetUnitOffset(affectedUnits.Count, counter++, squad.formation));
        }

        squad.formationRepr.SetNewFollow(affectedUnits[affectedUnits.Count / 2].transform);
    }

    public void SendCommandToUnits(GameEnums.CommandTypes selectedCommand, Vector3 mousePos, List<Vector3> formationPositions)
    {
        SetFormation(mousePos, formationPositions);

        List<UnitController> affectedUnits = new List<UnitController>();
        foreach (var sqd in squads)
        {
            if(sqd.isActive)
            {
                affectedUnits.AddRange(sqd.units);
                if(sqd.formationRepr != null)
                {
                    sqd.formationRepr.RemoveActiveSquad();
                    sqd.formationRepr.DetachSquad(sqd);
                    sqd.formationRepr = null;
                }
            }
        }
        //Filter dead units
        affectedUnits = affectedUnits.FindAll(x => x != null).ToList();

        if (affectedUnits.Count == 0) return;

        int counter = 0;
        foreach (var unit in affectedUnits)
        {
            unit.SetCommand(selectedCommand, mousePos, GetUnitOffset(affectedUnits.Count, counter++, currentFormation));
        }

        var repr = SetFormationLineRenderer(mousePos, formationPositions, affectedUnits[affectedUnits.Count / 2].transform);

        foreach (var sqd in squads)
        {
            if (sqd.isActive)
            {
                sqd.currentCommand = selectedCommand;
                sqd.targetPos = mousePos;
                sqd.formation.Clear();
                sqd.formation.Copy(currentFormation);

                sqd.formationRepr = repr;
                sqd.formationRepr.AttachSquad(sqd);
                sqd.formationRepr.AddActiveSquad();
            }
        }

        if(UIBattleSquadSelector.Instance != null)
        {
            UIBattleSquadSelector.Instance.ApplyCommandType(selectedCommand);
        }
    }

    public void SetFormation(Vector3 mousePos, List<Vector3> positions)
    {
        currentFormation.Clear();

        // If no formation was set, the units group up
        if (positions.Count == 0)
        {
            positions.Add(Vector3.zero);
        }

        var centerPos = positions[positions.Count / 2];
        int i = 0;
        foreach (var pos in positions)
        {
            var adjustedPos = pos - centerPos;
            var worldReprPos = adjustedPos + mousePos;
            currentFormation.Add(new BezierKnot(adjustedPos));
        }

    }

    public FormationRepr SetFormationLineRenderer(Vector3 mousePos, List<Vector3> positions, Transform centerTransform)
    {
        var renderer = Instantiate(squadFormationRender.gameObject, this.transform).GetComponent<SquadFormationLineRenderer>();

        // If no formation was set, the units group up
        if (positions.Count == 0)
        {
            //positions.Add(Vector3.zero);
            renderer.SetNewLine(0);
            return null;
        }
        renderer.SetNewLine(positions.Count);

        var centerPos = positions[positions.Count / 2];
        int i = 0;
        foreach (var pos in positions)
        {
            var adjustedPos = pos - centerPos;
            var worldReprPos = adjustedPos + mousePos;

            renderer.SetPosition(i++, new Vector3(worldReprPos.x, worldReprPos.y, 0));
        }

        renderer.SetFollowTarget(centerTransform);

        FormationRepr fullRepr = new FormationRepr();
        fullRepr.formationLineRenderer = renderer;
        fullRepr.formation.Copy(currentFormation);

        return fullRepr;
    }

    public void UpdateActiveSquad(int squadIndex, bool activeVal)
    {
        squads[squadIndex].isActive = activeVal;
        if (squads[squadIndex].formationRepr != null)
        {
            if(activeVal) squads[squadIndex].formationRepr.AddActiveSquad();
            else squads[squadIndex].formationRepr.RemoveActiveSquad();
        }
        UpdateSelectedSquad();
    }

    public void UpdateSelectedSquad()
    {
        updateUnitBorders();
    }

    private void UpdateSquadFormationRender()
    {
        if (squads.Count == 0) return;

        foreach (var sqd in squads)
        {
            if (sqd.formationRepr != null)
            {
                if (sqd.isActive)
                {
                    sqd.formationRepr.formationLineRenderer.lineRenderer.enabled = true;
                }
                else if(sqd.formationRepr.activeSquadCount == 0)
                {
                    sqd.formationRepr.formationLineRenderer.lineRenderer.enabled = false;
                }
            }
        }
    }

    private void updateUnitBorders()
    {
        foreach (var sqd in squads)
        {
            foreach (var unit in sqd.units)
            {
                if(unit != null)
                {
                    unit.SetUnitBorders(sqd.isActive);
                }
            }

        }
    }
}