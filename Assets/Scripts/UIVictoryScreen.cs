using Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIVictoryScreen : MonoBehaviour
{
    [SerializeField]
    Transform rewardOptionTransform;
    [SerializeField]
    GameObject rewardCardPrefab;

    private UIRewardCard currentSelectedCard;

    public RewardOptions options;
    public int rewardCount = 3;

    private void Start()
    {
        var rewardOptions = options.rewards.GetRandomElements(rewardCount);
        foreach (var opt in rewardOptions)
        {
            var card = Instantiate(rewardCardPrefab, rewardOptionTransform).GetComponent<UIRewardCard>();
            card.SetupCard(opt);
        }  
    }

    private void Update()
    {
        var selected = EventSystem.current.currentSelectedGameObject;
        if(selected != null)
        {
            var card = selected.GetComponent<UIRewardCard>();
            if (card != null )
            {
                currentSelectedCard = card;
            }
        }
    }

    public void OnRewardConfirm()
    {
        if (currentSelectedCard != null)
        {
            Debug.Log("Adding the following: " + currentSelectedCard.unit.data.name);
            PlayerInventory.Instance.AddToInventory(currentSelectedCard.unit.data, currentSelectedCard.unit.count);
            MapPlayerTracker.Instance.returnToTree();
        }
    }

    public void OnSkipReward()
    {
        MapPlayerTracker.Instance.returnToTree();
    }
}
