using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIUnitCard : MonoBehaviour {
    public PlayerInventory.UnitEntry entry;
    public bool isAddUnit;

    // public int unitCount;
    public TMP_Text counterText;
    public TMP_Text unitNameText;
    public TMP_Text buttonText;

    public void CreateUICard(PlayerInventory.UnitEntry entryData, bool add) {
        entry = entryData;
        isAddUnit = add;

        UpdateCardData();
    }

    public void UpdateCardData() {
        unitNameText.text = entry.unitData.unitName;
        counterText.text = "x" + entry.quantity.ToString();

        if(isAddUnit) buttonText.text = "Add";
        else buttonText.text = "Rmv";
    }

    public void OnAddClick() {
        if(isAddUnit) UISquadManager.Instance.RegisterUnitToSquad(this);
        else UISquadManager.Instance.RemoveUnitFromSquad(this);
    }
}