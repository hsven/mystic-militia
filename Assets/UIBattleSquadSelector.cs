using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIBattleSquadSelector : MonoBehaviour
{
    public static UIBattleSquadSelector Instance;

    private List<UIToggleSquadCard> squadCards;

    [SerializeField]
    private GameObject scrollContent;
    [SerializeField]
    private GameObject UIToggleCardPrefab;

    private List<bool> toggleStates = new List<bool>();
    bool isPropagationOK = true;

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
}
