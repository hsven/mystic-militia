using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMetrics : MonoBehaviour, IData
{
    public static GameMetrics Instance;
    public float playedTime = 0.0f;
    public int squadCount;
    public int formationCount;
    public int commandCount;
    public int mapPercentage;

    void Awake() {
        GameMetrics.Instance = this;    
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update(){
        playedTime += Time.deltaTime;
    }

    public void SaveData(ref GameData data){
        data.gameDuration = playedTime;
    }
}
