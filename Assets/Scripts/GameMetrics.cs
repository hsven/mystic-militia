using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameMetrics : MonoBehaviour
{
    public static GameMetrics Instance;
    public float playedTime = 0.0f;
    public int squadCount;
    public int formationCount;
    public int commandCount;
    public int mapPercentage;
    public string fileNam="data.game";

    private DataHandler dataHandler;

    void Awake() {
        GameMetrics.Instance = this;    
        DontDestroyOnLoad(this.gameObject);
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
        dataHandler.Save(this);
    }
}
