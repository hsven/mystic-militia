using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class GameMetrics : MonoBehaviour
{
    [Serializable]
    public class SquadMetric
    {
        public List<string> units = new List<string>();
        public List<int> counts = new List<int>();
    }

    [Serializable]
    public class PerBattleMetrics
    {
        public int battleNumber;
        public int squadCount;
        public List<SquadMetric> units = new List<SquadMetric>();
        public int formationCount;
        public int commandCount;
    }

    public static GameMetrics Instance;
    public string startTime;
    public string endTime;

    public float playedTime = 0.0f;
    public int mapPercentage;
    public List<PerBattleMetrics> battleMetrics;

    public string fileNam="data.game";


    private DataHandler dataHandler;
    void Awake() {
        GameMetrics.Instance = this;    
        DontDestroyOnLoad(this.gameObject);
        startTime = System.DateTime.Now.ToLongTimeString();
    }

    void Start() {
        this.dataHandler=new DataHandler(Application.persistentDataPath, fileNam);
    }

    // Update is called once per frame
    void Update(){
        playedTime += Time.deltaTime;
    }

    public void SaveData(GameMetrics data){
        data.playedTime = playedTime;
        endTime = System.DateTime.Now.ToLongTimeString();

        dataHandler.Save(this);
    }

    public void RegisterBattleData(int battleIdx, int squadCount, int formationCount, int commandCount, List<PlayerInventory.SquadEntry> squads)
    {
        var metric = new PerBattleMetrics();

        metric.battleNumber = battleIdx;
        metric.squadCount = squadCount;
        metric.formationCount = formationCount;
        metric.commandCount = commandCount;

        foreach (var item in squads)
        {
            var pair = new SquadMetric();
            pair.units = item.unitEntries.Select(x => x.unitData.unitName).ToList();
            pair.counts = item.unitEntries.Select(x => x.quantity).ToList();
            metric.units.Add(pair);
        }

        battleMetrics.Add(metric);
    }
}
