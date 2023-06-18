using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBattleResultManager : MonoBehaviour
{
    public static UIBattleResultManager Instance;

    [SerializeField]
    private GameObject victoryScreen;
    [SerializeField]
    private GameObject gameOverScreen;

    private void Awake()
    {
        UIBattleResultManager.Instance = this;
    }

    public void OpenResultScreen(bool isVictory)
    {
        if (isVictory)
        {
            Instantiate(victoryScreen, transform);
        }
        else
        {
            Instantiate(gameOverScreen, transform);
        }
    }
}
