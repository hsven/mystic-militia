using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBattleSquadSelector : MonoBehaviour
{
    public static UIBattleSquadSelector Instance;

    private List<UIToggleSquadCard> squadCards;

    [SerializeField]
    private GameObject scrollContent;
    [SerializeField]
    private GameObject UIToggleCardPrefab;

    private List<bool> toggleStates = new List<bool>();

    [SerializeField]
    private List<Sprite> commandTypeSprites = new List<Sprite>();

    [SerializeField]
    private Image currentCommandImage;
    [SerializeField]
    TMP_Text currentCommandText;

    void Awake()
    {
        UIBattleSquadSelector.Instance = this;
    }

    public void SetupBattleSquadUI()
    {
        int counter = 0;
        foreach (var sqd in BattleManager.Instance.squads)
        {
            var card = Instantiate(UIToggleCardPrefab, scrollContent.transform).GetComponent<UIToggleSquadCard>();

            card.SetProperties(sqd.name, counter++);
            toggleStates.Add(card.GetIsOn());
        }
        

        squadCards = scrollContent.GetComponentsInChildren<UIToggleSquadCard>().ToList();
        
    }

    public void OnToggleChanges(int cardIndex)
    {
        toggleStates[cardIndex] = squadCards[cardIndex].GetIsOn();
        BattleManager.Instance.UpdateActiveSquad(cardIndex, toggleStates[cardIndex]);
    }

    public void SelectedCommandType(GameEnums.CommandTypes selectedCommand)
    {
        currentCommandImage.sprite = commandTypeSprites[(int)selectedCommand];
        currentCommandText.text = selectedCommand.ToString();
    }

    public void ApplyCommandType(GameEnums.CommandTypes selectedCommand)
    {
        for (int i = 0; i < toggleStates.Count; i++)
        {
            if (toggleStates[i])
            {
                var squadCard = squadCards[i];
                squadCard.SetCommandTypeImage(commandTypeSprites[(int)selectedCommand]);
            }
        }
    }
}
