using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using System.Linq;
using Map;

public class EnemyFormationManager : MonoBehaviour
{
    [SerializeField]
    private SplineContainer formationSpline;

    public GameObject enemyPrefab;
    public List<EnemyFormationData> enemyFormationList;
    public List<EnemyFormationData> bossFormationList;

    [SerializeField]
    bool isBossStage;

    public void SpawnFormation(int progressionStage)
    {
        for (int i = formationSpline.Splines.Count - 1; i >= 0 ; i--)
        {
            formationSpline.RemoveSpline(formationSpline.Splines[i]);
        }
        EnemyFormationData selectedFormation = null;
        if (!isBossStage )
        {
            //Either same progression stage or one bellow
            var enemyOptions = enemyFormationList.FindAll(x => x.difficultyLevel == progressionStage || x.difficultyLevel == (progressionStage - 1)).ToList();
        
            selectedFormation = enemyOptions.Random();
            Debug.Log("Battle against: " + selectedFormation.ToString());
        }
        else
        {
            selectedFormation = bossFormationList[0];
        }
        if (selectedFormation == null) return;

        int index = 0;
        foreach (var form in selectedFormation.enemyFormations)
        {
            formationSpline.AddSpline(new Spline());
            for (int i = 0; i < form.formation.Count(); i++)
            {
                formationSpline.Splines[index].Insert(i, form.formation[i]);
            }

            var interval = formationSpline.Splines[index].GetCurveLength(0) / form.unitCount;

            for (int i = 0; i < form.unitCount; i++) 
            {
                var newEnemy = Instantiate(enemyPrefab).GetComponent<EntityController>();
                newEnemy.unitData = form.unit;

                float3 pos = formationSpline.Splines[index].EvaluatePosition(interval * i);
                newEnemy.transform.position = new Vector3(pos.x, pos.y, 0) + transform.position;
            }

            index++;
        }

    }
}
