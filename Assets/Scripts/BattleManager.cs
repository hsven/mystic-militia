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
    public List<UnitController> units = new List<UnitController>();

    public Spline formation = new Spline();

}

public class BattleManager : MonoBehaviour
{
    // public List<PlayerArmyInventory> armyInventory = new List<PlayerArmyInventory>();

    public static BattleManager Instance = null;
    public bool isPaused = false;
    public GameObject UIObj;

    public PlayerController player;
    public GameObject unitPrefab;

    public List<UnitController> totalPlayerUnits = new List<UnitController>();
    public List<PlayerSquad> squads = new List<PlayerSquad>();

    public Spline currentFormation;
    public int currentSquadSelection = -1;
    public LineRenderer repr;
    
    void Awake() {
        BattleManager.Instance = this;

        if (currentFormation == null) currentFormation = GetComponentInChildren<Spline>();
    }

    //Currently more of a start game
    public void ResumeGame() {
        //TODO: Revisit UI being mentioned here
        UIObj.SetActive(false);

        int squadCount = 0;
        foreach (var squad in PlayerInventory.Instance.playerSquads)
        {
            if(squad.unitEntries.Count == 0) continue;

            var newPlayerSquad = new PlayerSquad();
            
            foreach (var unit in squad.unitEntries)
            {
                for (int i = 0; i < unit.quantity; i++)
                {
                    var obj = Instantiate(unitPrefab).GetComponent<UnitController>();
                    obj.transform.position = UnityEngine.Random.insideUnitSphere * 2;
                    newPlayerSquad.units.Add(obj);
                    totalPlayerUnits.Add(obj);
                    obj.Setup(totalPlayerUnits.Count - 1, new Vector2Int(squadCount, i));
                }
            }

            squads.Add(newPlayerSquad);
        }

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

        var unitCount = currentSquadSelection == -1 ? totalPlayerUnits.Count : squads[currentSquadSelection].units.Count;
        var formation = currentSquadSelection == -1 ? currentFormation : squads[currentSquadSelection].formation;


        float formationPosStep = formation.Count / unitCount;
        var length = formation.GetCurveLength(0);
        var interval = length / unitCount;
        
        float3 finalPos;
        if (currentSquadSelection == -1) finalPos = formation.EvaluatePosition(interval * unitIndex);
        else finalPos = formation.EvaluatePosition(interval * squadIndex.y);

        return new Vector3(finalPos.x, finalPos.y, 0);
    }

    public void SetFormation(List<Vector3> positions, int selectedSquad) {
        currentSquadSelection = selectedSquad;
        currentFormation.Clear();
        repr.SetPositions(new Vector3[]{});

        // If no formation was set, the units group up
        if (positions.Count == 0) {
            positions.Add(Vector3.zero);
        }

        repr.positionCount = positions.Count;

        var centerPos = positions[positions.Count / 2];
        int i = 0;
        foreach (var pos in positions)
        {
            var adjustedPos = pos - centerPos;
            currentFormation.Add(new BezierKnot(adjustedPos));
            repr.SetPosition(i++, adjustedPos);
        }

        if(selectedSquad == -1) {
            foreach (var sqd in squads)
            {
                Debug.Log(sqd.formation);
                sqd.formation.Copy(currentFormation);
            }
        }
        else {
            squads[selectedSquad].formation.Copy(currentFormation);
        }


    }
}