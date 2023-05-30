using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class UISquad : MonoBehaviour {
    public GameObject scrollContent;
    public List<UIUnitCard> registeredUnits = new List<UIUnitCard>();
    private List<UIUnitCard> unitsInSquadRepr = new List<UIUnitCard>();
    public void AddUnit(UIUnitCard unit) {
        var pos = registeredUnits.FindIndex(0, x => x == unit);
        if (pos != -1) {
            unitsInSquadRepr[pos].UpdateCounter(1);
        }
        else {
            var newCard = Instantiate(UISquadManager.Instance.unitUIEntry, scrollContent.transform).GetComponent<UIUnitCard>();
            unitsInSquadRepr.Add(newCard);
            newCard.unitCount = 1;
            newCard.UpdateCounter(0);
        }
    }
} 