using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RewardOptions", menuName = "mystic-militia/RewardOptions", order = 3)]
public class RewardOptions : ScriptableObject
{
    [Serializable]
    public class Reward
    {
        public UnitData data;
        public int count;
    }

    public List<Reward> rewards;
}
