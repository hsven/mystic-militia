using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class UISquadCard : MonoBehaviour {
    public PlayerInventory.SquadEntry entry;

    public TMP_Text squadName;

    public RectTransform scrollContent;
    public List<UIUnitCard> registeredUnits = new List<UIUnitCard>();
    private List<UIUnitCard> unitsInSquadRepr = new List<UIUnitCard>();

    public void CreateUICard(PlayerInventory.SquadEntry entryData) {
        entry = entryData;

        UpdateCardData();
    }

    public void SetName(string newName) {
        squadName.text = newName;
    }

    public void UpdateCardData() {
        for (int i = 0; i < scrollContent.childCount; i++)
        {
            Destroy(scrollContent.GetChild(i).gameObject);
        }

        foreach (var item in entry.unitEntries)
        {
            var unitCard = Instantiate(UISquadManager.Instance.unitUIEntry, scrollContent).GetComponent<UIUnitCard>();
            unitCard.CreateUICard(item, false);
        }
    }

} 