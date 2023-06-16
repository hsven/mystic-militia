using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIOffScreenIndicatorManager : MonoBehaviour
{
    public static UIOffScreenIndicatorManager Instance;

    [SerializeField]
    private GameObject indicator;


    private void Awake()
    {
        Instance = this;
    }

    public void SpawnSquadIndicator(List<Transform> targets, int squadCount)
    {
        var squadIndicator = Instantiate(indicator, transform).GetComponent<UISquadIndicator>();
        squadIndicator.gameObject.SetActive(true);
        squadIndicator.StartIndicator(targets, squadCount);
    }
}
