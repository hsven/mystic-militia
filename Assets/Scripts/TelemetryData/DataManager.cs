using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataManager : MonoBehaviour
{

    public static DataManager Instance { get; private set; }

    private void Awake(){
        if(Instance != null){
            Debug.LogError("more than one manager");
        }
        Instance = this;
    }

    public void SaveData(){
        Debug.Log("time played: " + GameMetrics.Instance.playedTime);
    }
}
