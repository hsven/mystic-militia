using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

[CreateAssetMenu(fileName = "EnemyFormationData", menuName = "mystic-militia/EnemyFormation", order = 1)]

public class EnemyFormationData : ScriptableObject
{
    [Serializable]
    public class DataFormationPair
    {
        public UnitData unit;
        public int unitCount;
        public Spline formation;
    }

    public List<DataFormationPair> enemyFormations;
    public int difficultyLevel;
}