using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIUnitCard : MonoBehaviour {
    public int unitCount;
    public TMP_Text counter;

    void Start() {
        counter.text = "x" + unitCount.ToString();
    }

    public void UpdateCounter(int change) {
        unitCount += change;
        counter.text = "x" + unitCount.ToString();
    }

    public void OnAddClick() {
        if(UISquadManager.Instance.RegisterUnitToSquad(this)) {
            unitCount--;
            counter.text = "x" + unitCount.ToString();
        }
    }
}