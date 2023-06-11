using UnityEngine;
using System.Collections;

public static class GameEnums {
    public enum CommandTypes
    {
        FOLLOW,
        ATTACK,
        DEFEND,
        PATROL,
    }
    public enum EntityRangeType
    {
        PLAYER,
        UNIT,
    }
}