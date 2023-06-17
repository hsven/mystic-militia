using UnityEngine;
using System.Collections;

public static class GameEnums {
    public enum CommandTypes
    {
        FOLLOW,
        ATTACK,
        DEFEND,
    }
    public enum EnemyTarget
    {
        PLAYER,
        UNIT,
    }
}