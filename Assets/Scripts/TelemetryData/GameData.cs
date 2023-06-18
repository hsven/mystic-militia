using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class GameData
{
   public float gameDuration;
   public int squadCount;
   public int formationCount;
   public int commandCount;
   public int mapPercentage;

   public GameData(){
    gameDuration=0;
    squadCount=0;
    formationCount=0;
    commandCount=0;
    mapPercentage=0;
   }
}
