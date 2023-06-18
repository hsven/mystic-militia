using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIOptionButton : MonoBehaviour
{
    [SerializeField]
    TMP_Text optionDescription;

    public RewardOptions.Reward unit;

    public void SetupOption(RewardOptions.Reward data)
    {
        unit = data;
        optionDescription.text = "Add " + unit.data.unitName + " x" + unit.count;

        var but = GetComponent<Button>();
        but.onClick.AddListener(delegate { UIEventManager.Instance.OnSelectOption(unit.data, unit.count); });;
    }
}
