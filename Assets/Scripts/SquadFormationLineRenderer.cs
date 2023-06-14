using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadFormationLineRenderer : MonoBehaviour
{
    public LineRenderer lineRenderer;
    private int centerIndex;
    private Transform followTarget;
    private List<Vector3> originalPositions = new List<Vector3>();

    private void Update()
    {
        if (originalPositions.Count == 0) return;
        if (followTarget == null) return;

        var positionDif = originalPositions[centerIndex] - followTarget.position;
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            var curPos = originalPositions[i];
            lineRenderer.SetPosition(i, curPos - positionDif);
        }
    }

    public void SetFollowTarget(Transform target)
    {
        followTarget = target;
    }

    public void SetNewLine(int newPositionCount)
    {
        lineRenderer.positionCount = 0;
        originalPositions.Clear();
        lineRenderer.positionCount = newPositionCount;

        centerIndex = lineRenderer.positionCount / 2;
    }

    public void SetPosition(int index, Vector3 position)
    {
        lineRenderer.SetPosition(index, Vector3.zero);
        originalPositions.Insert(index, position);
    }

    public void SetPositions(List<Vector3> positions)
    {
        originalPositions.AddRange(positions);
    }
}
