using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIButtonMethods : MonoBehaviour
{
    public GameObject playerInventory;

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void NewGame(){
        if(PlayerInventory.Instance != null){
            Destroy(PlayerInventory.Instance.gameObject);
        }
        var newInv = Instantiate(playerInventory);
        newInv.SetActive(true);
    }

    public void addUnit(UnitData data){
        PlayerInventory.Instance.AddToInventory(data, 10);
    }

    public void ResetToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
