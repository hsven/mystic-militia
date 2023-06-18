using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataManager : MonoBehaviour
{
    public GameMetrics gameMetrics;

    public static DataManager instance { get; private set; }

    private void Awake(){
        if(instance != null){
            Debug.LogError("more than one manager");
        }
        instance = this;
    }

    public void SaveData(){
        Debug.Log("time played: " + gameMetrics.playedTime);
    }

    private void OnApplicationQuit(){
        SaveData();
    }
}
