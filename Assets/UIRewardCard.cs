using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIRewardCard : MonoBehaviour
{
    [SerializeField]
    TMP_Text cardName;
    [SerializeField]
    Image img;
    [SerializeField]
    TMP_Text count;

    public RewardOptions.Reward unit;

    public void SetupCard(RewardOptions.Reward data)
    {
        unit = data;
        cardName.text = unit.data.name;
        if(unit.data.unitSprite) img.sprite = unit.data.unitSprite;
        count.text = "x" + unit.count;
    }
}
