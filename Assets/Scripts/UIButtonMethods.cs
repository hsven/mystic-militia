using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIButtonMethods : MonoBehaviour
{
public GameObject playerInventory;
public GameObject gameMetrics;

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void NewGame(){
        if(PlayerInventory.Instance != null){
            Destroy(PlayerInventory.Instance.gameObject);
        }
        if(GameMetrics.Instance != null){
            Destroy(GameMetrics.Instance.gameObject);
        }
        var newInv = Instantiate(playerInventory);
        var metrics = Instantiate(gameMetrics);
        newInv.SetActive(true);
        metrics.SetActive(true);
    }

    public void addUnit(UnitData data){
        PlayerInventory.Instance.AddToInventory(data, 10);
    }

}
