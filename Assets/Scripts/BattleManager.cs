using System.Collections;
using System.Collections.Generic;
using UnityEngine.Splines;
using UnityEngine;
using Unity.Mathematics;
using System.Linq;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance = null;

    public PlayerController player;
    public List<UnitController> playerUnits = new List<UnitController>();
    
    public Spline unitFormation;
    public LineRenderer repr;
    
    void Awake() {
        BattleManager.Instance = this;

        if (unitFormation == null) unitFormation = GetComponentInChildren<Spline>();
    }

    public int RegisterPlayerUnit(UnitController unit) {
        playerUnits.Add(unit);
        return playerUnits.Count - 1;
    }

    public List<Vector2> GetPlayerUnitPositions() {
        return playerUnits.Select(unit => new Vector2(unit.transform.position.x, unit.transform.position.y)).ToList();
    }

    public Vector2 GetUnitOffset(int unitIndex) {
        if (unitFormation.Count == 0) return Vector2.zero;

        float formationPosStep = unitFormation.Count / playerUnits.Count;
        var length = unitFormation.GetCurveLength(0);
        var interval = length / playerUnits.Count;
        
        float3 finalPos = unitFormation.EvaluatePosition(interval * unitIndex);
        return new Vector3(finalPos.x, finalPos.y, 0);
    }

    public void SetFormation(List<Vector3> positions) {
        unitFormation.Clear();
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
            unitFormation.Add(new BezierKnot(adjustedPos));
            repr.SetPosition(i++, adjustedPos);
        }
    }
}