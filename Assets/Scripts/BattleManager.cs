using System.Collections;
using System.Collections.Generic;
using UnityEngine.Splines;
using UnityEngine.SceneManagement;
using UnityEngine;
using Unity.Mathematics;
using System.Linq;
using System;

[Serializable]
public class PlayerSquad
{
    public String name = "Squad";

    public List<UnitController> units = new List<UnitController>();

    public Spline formation = new Spline();

    public bool isActive = false;
    public SquadFormationLineRenderer formationLineRenderer = null;

    public PlayerSquad() {}

    public PlayerSquad(String name)
    {
        this.name = name;
    }
}

public class BattleManager : MonoBehaviour
{
    // public List<PlayerArmyInventory> armyInventory = new List<PlayerArmyInventory>();

    public static BattleManager Instance = null;
    public MapGenerator mapGenerator;
    public bool isPaused = false;
    public GameObject UIObj;

    public PlayerController player;
    public GameObject unitPrefab;

    public List<UnitController> totalPlayerUnits = new List<UnitController>();
    public List<PlayerSquad> squads = new List<PlayerSquad>();

    public Spline currentFormation;
    public int currentSquadSelection = -1;
    public int lastSquadCommanded = 0;

    public LineRenderer repr;
    public SquadFormationLineRenderer squadFormationRender;

    void Awake() {
        BattleManager.Instance = this;

        if (currentFormation == null) currentFormation = GetComponentInChildren<Spline>();
    }

    private void Start()
    {
        PauseGame();
        mapGenerator.GenerateMap();

        //TODO: Maybe redo how this initial spawns is configured
        var respawnLocs = GameObject.FindGameObjectsWithTag("Respawn");
        player.transform.position = respawnLocs[0].transform.position;

    }

    //Currently more of a start game
    public void ResumeGame() {
        //TODO: Revisit UI being mentioned here
        UIObj.SetActive(false);

        int squadCount = 0;

        //Initializing the army as a squad
        var fullArmy = new PlayerSquad("Army");
        squads.Add(fullArmy);
        fullArmy.formationLineRenderer = squadFormationRender;

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
                    obj.transform.position = player.transform.position + UnityEngine.Random.insideUnitSphere * 2;
                    newPlayerSquad.units.Add(obj);
                    totalPlayerUnits.Add(obj);
                    obj.Setup(totalPlayerUnits.Count - 1, new Vector2Int(squadCount, i));
                }
            }
            newPlayerSquad.formationLineRenderer = Instantiate(squadFormationRender.gameObject, transform).GetComponent<SquadFormationLineRenderer>();
            squads.Add(newPlayerSquad);
        }

        fullArmy.units.AddRange(totalPlayerUnits);

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
                squad.units.RemoveAt(squadIndex.y);
            }
            return;
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

    public Vector2 GetUnitOffset(int unitIndex, Vector2Int squadIndex) {
        if (currentFormation.Count == 0) return Vector2.zero;


        var unitCount = squads[currentSquadSelection].units.Count;
        var formation = squads[currentSquadSelection].formation;


        float formationPosStep = formation.Count / unitCount;
        var length = formation.GetCurveLength(0);
        var interval = length / unitCount;
        
        float3 finalPos;
        if (currentSquadSelection == 0) finalPos = formation.EvaluatePosition(interval * unitIndex);
        else finalPos = formation.EvaluatePosition(interval * squadIndex.y);

        return new Vector3(finalPos.x, finalPos.y, 0);
    }

    public void SendCommandToUnits(GameEnums.CommandTypes selectedCommand, Vector3 mousePos)
    {
        foreach (UnitController unit in squads[currentSquadSelection].units)
        {
            unit.SetCommand(selectedCommand, new Vector2(mousePos.x, mousePos.y));
        }
        squads[currentSquadSelection].isActive = true;
        if (currentSquadSelection == 0)
        {
            for (int i = 1; i < squads.Count; i++)
            {
                squads[i].isActive = false;
                squads[i].formationLineRenderer.SetNewLine(0);
            }
        } else
        {
            squads[0].isActive = false;
        }

        UpdateSquadFormationRender();
    }

    public void SetFormation(List<Vector3> positions, int selectedSquad) {
        currentSquadSelection = selectedSquad;
        currentFormation.Clear();
        repr.SetPositions(new Vector3[]{});

        // If no formation was set, the units group up
        if (positions.Count == 0) {
            positions.Add(Vector3.zero);
        }
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        repr.positionCount = positions.Count;
        squads[currentSquadSelection].formationLineRenderer.SetNewLine(positions.Count);

        var centerPos = positions[positions.Count / 2];
        int i = 0;
        foreach (var pos in positions)
        {
            var adjustedPos = pos - centerPos;
            var worldReprPos = adjustedPos + mousePos;
            currentFormation.Add(new BezierKnot(adjustedPos));
            repr.SetPosition(i, adjustedPos);

            squads[currentSquadSelection].formationLineRenderer.SetPosition(i++, new Vector3(worldReprPos.x, worldReprPos.y, 0));

        }

        var centerIdx = squads[selectedSquad].units.Count / 2;
        squads[currentSquadSelection].formationLineRenderer.SetFollowTarget(squads[selectedSquad].units[centerIdx].transform);
        squads[selectedSquad].formation.Copy(currentFormation);
    }

    public int UpdateSelectedSquad(int newSquadValue)
    {
        currentSquadSelection = newSquadValue;
        if (currentSquadSelection < 0)
        {
            currentSquadSelection = 0;
        }
        else if(currentSquadSelection > squads.Count - 1)
        {
            currentSquadSelection = squads.Count - 1;
        }
        
        updateUnitBorders();
        UpdateSquadFormationRender();

        return currentSquadSelection;
    }

    private void UpdateSquadFormationRender()
    {
        if (squads.Count == 0) return;

        if (currentSquadSelection == 0)
        {
            if (!squads[currentSquadSelection].isActive)
            {
                squads[0].formationLineRenderer.lineRenderer.enabled = false;
                for (int i = 1; i < squads.Count; i++)
                {
                    squads[i].formationLineRenderer.lineRenderer.enabled = true;
                }
            }
            else
            {
                squads[0].formationLineRenderer.lineRenderer.enabled = true;
                for (int i = 1; i < squads.Count; i++)
                {
                    squads[i].formationLineRenderer.lineRenderer.enabled = false;
                }
            }
            return;
        }
        else
        {
            for (int i = 0; i < squads.Count; i++)
            {
                if (i == currentSquadSelection)
                    squads[i].formationLineRenderer.lineRenderer.enabled = true;
                else
                    squads[i].formationLineRenderer.lineRenderer.enabled = false;
            }
        }
    }

    private void updateUnitBorders()
    {
        foreach (UnitController unit in totalPlayerUnits)
        {
            unit.SetUnitBorders(currentSquadSelection);
        }
    }
}